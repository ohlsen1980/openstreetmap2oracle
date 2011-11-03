//OpenStreetMap2Oracle - A windows application to export OpenStreetMap Data 
//               (*.osm - files) to an oracle database
//-------------------------------------------------------------------------------
//Copyright (C) 2011  Oliver Schöne
//-------------------------------------------------------------------------------
//This program is free software; you can redistribute it and/or
//modify it under the terms of the GNU General Public License
//as published by the Free Software Foundation; either version 2
//of the License, or (at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program; if not, write to the Free Software
//Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.


using System;
using System.Windows;
using OpenStreetMap2Oracle.tools;
using System.Windows.Forms;
using System.IO;
using OpenStreetMap2Oracle.businesslogic;
using System.ComponentModel;
using OpenStreetMap2Oracle.gui;
using OpenStreetMap2Oracle.oracle;
using System.Data.OracleClient;
using System.Windows.Threading;
using OpenStreetMap2Oracle.businesslogic.Transaction;

namespace OpenStreetMap2Oracle
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BackgroundWorker parseXMLWorker;
        string xmlPath      = String.Empty;
        private String sql  = String.Empty;    

        // some longs to count the elements, report only every 1000 points lines and polygons progress, this is much faster
        private long    _elementCount   = 0, 
                        _failedCount    = 0, 
                        _refreshCount   = 0, 
                        lineCount       = 0, 
                        failedLines     = 0, 
                        polygonCount    = 0, 
                        failedPolygons  = 0, 
                        displayPointCount   = 1000, 
                        displayLineCount    = 1000, 
                        displayPolygonCount = 1000, 
                        multipolygonCount   = 0;

        private const int DISPATCHER_FLUSH_THRESHOLD = 1000;

        private TransactionDispatcher _mTransactionDisp;
        private TransactionQueue _mTransactionQueue;

        public MainWindow()
        {
            InitializeComponent();
            this._mTransactionDisp = new TransactionDispatcher();
            this._mTransactionQueue = new TransactionQueue();
            this.Closed += new EventHandler(MainWindow_Closed);          
        }

        /// <summary>
        /// Disconnects all connections from the current pool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MainWindow_Closed(object sender, EventArgs e)
        {
            OracleConnectionFactory.DisconnectAll();
        }

        /// <summary>
        /// The file open menu was clicked
        /// </summary>
        private void FileOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "OpenStreetMap Dateien (*.osm)|*.OSM|" +
                            "Alle Dateien (*.*)|*.*";
            dialog.InitialDirectory = PathProvider.Path;

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                parseXMLWorker = new BackgroundWorker();
                parseXMLWorker.WorkerReportsProgress = true;
                parseXMLWorker.WorkerSupportsCancellation = true;
                parseXMLWorker.DoWork += new DoWorkEventHandler(parseXMLWorker_DoWork);                             
                parseXMLWorker.RunWorkerAsync();               
                xmlPath = dialog.FileName;
                PathProvider.Path = new FileInfo(dialog.FileName).DirectoryName;                
            }
        }
     
        /// <summary>
        /// Async Work has to be done
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void parseXMLWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;
            if ((bw.CancellationPending == true))
            {
                e.Cancel = true;
            }
            else
            {
                // Perform a time consuming operation and report progress.
                ApplicationManager.Instance().OnOSMElementAdded += new ApplicationManager.OnOSMElementAddedHandler(MainWindow_OnOSMElementAdded);
                ApplicationManager.Instance().OnXMLFinished += new ApplicationManager.XMLFinishedHandler(MainWindow_OnXMLFinished);
                ApplicationManager.Instance().ParseXML(xmlPath);
            }
        }

        void MainWindow_OnXMLFinished(object sender, eventArgs.XMLFinishedEventArgs e)
        {
            //OracleConnectionFactory.Transaction.Commit();           
            System.GC.Collect();
            this.NodesCount.Text = _elementCount.ToString();
            this.LinesCount.Text = lineCount.ToString();
            this.PolygonsCount.Text = polygonCount.ToString();
            this.MultiPolCount.Text = multipolygonCount.ToString();
            System.Windows.MessageBox.Show("OSM Daten in Datenbank übertragen\nPunkte: "+ _elementCount +"\nLinien: "+ lineCount + "\nPolygone: "+polygonCount + "\nMultipolygone: "+multipolygonCount);
        }
     
        /// <summary>
        /// An Element is analyzed an was added to the export queue
        /// </summary>       
        void MainWindow_OnOSMElementAdded(object sender, eventArgs.OSMAddedEventArg e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Send, new DispatcherOperationCallback(delegate
            {
                OSMElement element = e.Element;
                String SQL = element.ToSQL();
                if(!String.IsNullOrEmpty(SQL))
                {
                    //using (OracleCommand dbSqlCmd = OracleConnectionFactory.Connection.DbConnection.CreateCommand())
                    {

                        //dbSqlCmd.Transaction = OracleConnectionFactory.Transaction;
                        //dbSqlCmd.UpdatedRowSource = System.Data.UpdateRowSource.None;
                        try
                        {

                            this._mTransactionQueue.Add(new OSMTransactionObject(SQL));

                            if (this._mTransactionQueue.Data.Count >= DISPATCHER_FLUSH_THRESHOLD)
                            {
                                this._mTransactionDisp.Queue = this._mTransactionQueue;
                                this._mTransactionDisp.ProcessQueue();
                                this._mTransactionQueue.Clear();
                            }

                           
                            //OracleConnectionFactory.Connection.execSqlCmd(SQL, dbSqlCmd);


                            if (element.GetType() == typeof(Node))
                            {
                                _elementCount++;
                                //report only every 1000 objects progress, this is much faster!
                                if (_elementCount == displayPointCount)
                                {
                                    this.NodesCount.Text = _elementCount.ToString();
                                    displayPointCount = displayPointCount + 1000;
                                }
                            }
                            if (element.GetType() == typeof(Way))
                            {
                                Way way = element as Way;
                                if (way.Line.IsPolygon() == false)
                                {
                                    lineCount++;
                                    if (lineCount == displayLineCount)
                                    {
                                        this.LinesCount.Text = lineCount.ToString();
                                        displayLineCount = 1000 + displayLineCount;
                                    }
                                }
                                else
                                {
                                    polygonCount++;
                                    if (polygonCount == displayPolygonCount)
                                    {
                                        this.PolygonsCount.Text = polygonCount.ToString();
                                        displayPolygonCount = displayPolygonCount + 1000;
                                    }
                                }
                            }
                            if (element.GetType() == typeof(Relation))
                            {
                                multipolygonCount++;
                                this.MultiPolCount.Text = multipolygonCount.ToString();

                            }
                            _refreshCount++;
                            if (_refreshCount == 10000)
                            {
                                System.GC.Collect();
                                _refreshCount = 0;
                                //OracleConnectionFactory.Transaction.Commit();
                                //OracleConnectionFactory.Transaction = OracleConnectionFactory.Connection.DbConnection.BeginTransaction();
                            }
                        }
                        catch (Exception ex)
                        {                    
                            //NOTICE: If you export multiple osm extracts in 1 schema, there can be errors in primary key OSM_ID because
                            //the extracts in boundary regions are never exact, there can be double elements, for this case there is
                            //the DisplayMessagesChkBox, because the application crashes when there are about 10000 error messages in the SQLTextBox!!!
                            if (element.GetType() == typeof(Node))
                            {
                                _failedCount++;                                
                                if (this.DisplayMessagesChckBox.IsChecked == true)
                                {
                                    this.SQLTextBox.Text = SQLTextBox.Text + "\n" + SQL + "\n" + "Fehler Knoten: " + _failedCount.ToString();
                                    this.SQLTextBox.UpdateLayout();
                                }
                            }
                            if (element.GetType() == typeof(Way))
                            {
                                Way way = element as Way;
                                if (way.Line.IsPolygon() == false)
                                {
                                    failedLines++;                                    
                                    if (this.DisplayMessagesChckBox.IsChecked == true)
                                    {
                                        this.SQLTextBox.Text = SQLTextBox.Text + "\n" + SQL + "\n" + "Fehler Linien: " + failedLines.ToString();
                                        this.SQLTextBox.UpdateLayout();
                                    }
                                }
                                else
                                {
                                    failedPolygons++;                                    
                                    if (this.DisplayMessagesChckBox.IsChecked == true)
                                    {
                                        this.SQLTextBox.Text = SQLTextBox.Text + "\n" + SQL + "\n" + "Fehler Polygone: " + failedLines.ToString();
                                        this.SQLTextBox.UpdateLayout();
                                    }
                                }
                            }
                            if (element.GetType() == typeof(Relation))
                            {
                                _failedCount++;                               
                                if (this.DisplayMessagesChckBox.IsChecked == true)
                                {
                                    this.SQLTextBox.Text = SQLTextBox.Text + "\n" + SQL + "\n" + "Fehler Relation: " + _failedCount.ToString();
                                    this.SQLTextBox.UpdateLayout();
                                }
                            }
                        }
                    }
                }
                return null;
            }), null);

        }

        private void ConnOpen_Click(object sender, RoutedEventArgs e)
        {
            DbLoginWindow window = new DbLoginWindow();
            window.ShowDialog();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

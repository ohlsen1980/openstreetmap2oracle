using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using OpenStreetMap2Oracle.businesslogic.Transaction;
using OpenStreetMap2Oracle.tools;
using OpenStreetMap2Oracle.businesslogic;
using System.Windows.Threading;
using OpenStreetMap2Oracle.gui;

namespace OpenStreetMap2Oracle.controller
{
    public class AppManagerController
    {
        private BackgroundWorker parseXMLWorker;
        static string xmlPath = String.Empty;

        public MainWindow2 OwnerWindow
        {
            get;
            set;
        }

        public static string XmlPath
        {
            get { return xmlPath; }
            set { xmlPath = value; }
        }
        private String sql = String.Empty;
        private ProgressWindow _mProgressWindow;

        // some longs to count the elements, report only every 1000 points lines and polygons progress, this is much faster
        private long _elementCount = 0,
                        _failedCount = 0,
                        _refreshCount = 0,
                        lineCount = 0,
                        failedLines = 0,
                        polygonCount = 0,
                        failedPolygons = 0,
                        displayPointCount = 1000,
                        displayLineCount = 1000,
                        displayPolygonCount = 1000,
                        multipolygonCount = 0;

        private const int DISPATCHER_FLUSH_THRESHOLD = 1000;

        private TransactionDispatcher _mTransactionDisp;
        private TransactionQueue _mTransactionQueue;


        public AppManagerController()
        {
            this._mTransactionDisp = new TransactionDispatcher();
            this._mTransactionQueue = new TransactionQueue();
            this._mProgressWindow = new ProgressWindow();
        }

        public void Start()
        {
            if (OwnerWindow != null)
            {
                this._mProgressWindow.Owner = OwnerWindow;
            }
            this._mProgressWindow.Show();
            parseXMLWorker = new BackgroundWorker();
            parseXMLWorker.WorkerReportsProgress = true;
            parseXMLWorker.WorkerSupportsCancellation = true;
            parseXMLWorker.DoWork += new DoWorkEventHandler(parseXMLWorker_DoWork);
            parseXMLWorker.RunWorkerAsync();  
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
                ApplicationManager.Instance().OnOSMElementAdded += new ApplicationManager.OnOSMElementAddedHandler(OnOSMElementAdded);
                ApplicationManager.Instance().OnXMLFinished += new ApplicationManager.XMLFinishedHandler(OnXMLFinished);
                ApplicationManager.Instance().ParseXML(xmlPath);
            }
        }

        void OnXMLFinished(object sender, eventArgs.XMLFinishedEventArgs e)
        {
            //OracleConnectionFactory.Transaction.Commit();           
            System.GC.Collect();
            System.Windows.MessageBox.Show("OSM Daten in Datenbank übertragen\nPunkte: " + _elementCount + "\nLinien: " + lineCount + "\nPolygone: " + polygonCount + "\nMultipolygone: " + multipolygonCount);
        }

        /// <summary>
        /// An Element is analyzed an was added to the export queue
        /// </summary>       
        void OnOSMElementAdded(object sender, eventArgs.OSMAddedEventArg e)
        {
            this._mProgressWindow.Dispatcher.Invoke(DispatcherPriority.Send, new DispatcherOperationCallback(delegate
            {
                OSMElement element = e.Element;
                String SQL = element.ToSQL();
                if (!String.IsNullOrEmpty(SQL))
                {
                     {
                        try
                        {

                            this._mTransactionQueue.Add(new OSMTransactionObject(SQL));

                            if (this._mTransactionQueue.Data.Count >= DISPATCHER_FLUSH_THRESHOLD)
                            {
                                this._mTransactionDisp.Queue = this._mTransactionQueue;
                                this._mTransactionDisp.ProcessQueue();
                                this._mTransactionQueue.Clear();
                            }
         
                            if (element.GetType() == typeof(Node))
                            {
                                _elementCount++;
                                //report only every 1000 objects progress, this is much faster!
                                if (_elementCount == displayPointCount)
                                {
                                    this._mProgressWindow.CurrentNodes = _elementCount;
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
                                        this._mProgressWindow.CurrentLines = lineCount;
                                        displayLineCount = 1000 + displayLineCount;
                                    }
                                }
                                else
                                {
                                    polygonCount++;
                                    if (polygonCount == displayPolygonCount)
                                    {
                                        this._mProgressWindow.CurrentPolygons = polygonCount;
                                        displayPolygonCount = displayPolygonCount + 1000;
                                    }
                                }
                            }
                            if (element.GetType() == typeof(Relation))
                            {
                                multipolygonCount++;
                                this._mProgressWindow.CurrentMultiPolygons = multipolygonCount;

                            }
                            _refreshCount++;
                            if (_refreshCount == 10000)
                            {
                                System.GC.Collect();
                                _refreshCount = 0;
                            }
                        }
                        catch (Exception ex)
                        {
                            this._mProgressWindow.CurrentErrors++;
                            //NOTICE: If you export multiple osm extracts in 1 schema, there can be errors in primary key OSM_ID because
                            //the extracts in boundary regions are never exact, there can be double elements, for this case there is
                            //the DisplayMessagesChkBox, because the application crashes when there are about 10000 error messages in the SQLTextBox!!!
                            /*if (element.GetType() == typeof(Node))
                            {
                                _failedCount++;
                                if (this.DisplayMessagesChckBox.IsChecked == true)
                                {
                                    this.SQLTextBox.Text = SQLTextBox.Text + ex.Message + "\n";
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
                            }*/
                        }
                    }
                }
                return null;
            }), null);

        }
    }
}

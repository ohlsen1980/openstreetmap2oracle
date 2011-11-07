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
using System.Windows;

namespace OpenStreetMap2Oracle.controller
{
    public class AppManagerController
    {
        
        private BackgroundWorker xml_worker;
 
        static string xmlPath = String.Empty;

        /// <summary>
        /// Gets or sets the owner window
        /// </summary>
        public MainWindow2 OwnerWindow
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the XML Path
        /// </summary>
        public static string XmlPath
        {
            get { return xmlPath; }
            set { xmlPath = value; }
        }

        private String sql = String.Empty;
        private ProgressWindow _mProgressWindow;

        // some longs to count the elements, report only every 1000 points lines and polygons progress, this is much faster
        private long node_count = 0,
                        _failedCount = 0,
                        _refreshCount = 0,
                        line_count = 0,
                        failedLines = 0,
                        polygon_count = 0,
                        failedPolygons = 0,
                        displayPointCount = 1000,
                        displayLineCount = 1000,
                        displayPolygonCount = 1000,
                        mpolygon_count = 0;

        private long dispInc = 0;

        private DateTime _mLastDispatchItem;

        public const int DISPATCHER_FLUSH_THRESHOLD = 100;
        public const int GUI_REFRESH_ITEMS = 1000;

        private TransactionDispatcher _mTransactionDisp;
        private TransactionQueue _mTransactionQueue;

        /// <summary>
        /// Initializes the Controller
        /// </summary>
        public AppManagerController()
        {
            this._mTransactionDisp = new TransactionDispatcher();
            this._mTransactionQueue = new TransactionQueue();
            this._mProgressWindow = new ProgressWindow();
            this._mLastDispatchItem = DateTime.Now;
        }

        /// <summary>
        /// Starts the Controlling Process
        /// </summary>
        public void Start()
        {
            if (OwnerWindow != null)
            {
                this._mProgressWindow.Owner = OwnerWindow;
                this.OwnerWindow.IsBackgrounded = true;  
            }

            this._mTransactionDisp.Start();
            this._mProgressWindow.Show();
            xml_worker = new BackgroundWorker();
            xml_worker.WorkerReportsProgress = true;
            xml_worker.WorkerSupportsCancellation = true;
            xml_worker.DoWork += new DoWorkEventHandler(parseXMLWorker_DoWork);
            xml_worker.RunWorkerAsync();  
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
            System.Windows.MessageBox.Show("OSM Daten in Datenbank übertragen\nPunkte: " + node_count + "\nLinien: " + line_count + "\nPolygone: " + polygon_count + "\nMultipolygone: " + mpolygon_count);
        }

        /// <summary>
        /// An Element is analyzed an was added to the export queue
        /// </summary>       
        void OnOSMElementAdded(object sender, eventArgs.OSMAddedEventArg e)
        {
            this._mProgressWindow.Dispatcher.Invoke(DispatcherPriority.Send, new DispatcherOperationCallback(delegate
            {
                OSMElement element = e.Element;
                String sql_query = element.ToSQL();

                if (!String.IsNullOrEmpty(sql_query))
                {
                    try
                    {
                        // add the element to the dispatcher
                        this._mTransactionDisp.Add(new OSMTransactionObject(sql_query));

                        // refresh the UI data
                        if (element.GetType() == typeof(Node))
                        {
                            if (((++node_count) % GUI_REFRESH_ITEMS) == 0)
                            {
                                this._mProgressWindow.CurrentNodes = node_count;
                            }
                        }
                        else if (element.GetType() == typeof(Way))
                        {
                            if (!((Way)element).Line.IsPolygon())
                            {
                                if (((++line_count) % GUI_REFRESH_ITEMS) == 0)
                                {
                                    this._mProgressWindow.CurrentLines = line_count;
                                }
                            }
                            else
                            {
                                if (((++polygon_count) % GUI_REFRESH_ITEMS) == 0)
                                {
                                    this._mProgressWindow.CurrentPolygons = polygon_count;
                                }
                            }
                        }
                        else if (element.GetType() == typeof(Relation))
                        {
                            mpolygon_count++;
                            this._mProgressWindow.CurrentMultiPolygons = mpolygon_count;
                        }

                        if ((++dispInc) >= GUI_REFRESH_ITEMS)
                        {
                            dispInc = 0;

                            float millis = (new TimeSpan(DateTime.Now.Ticks - _mLastDispatchItem.Ticks)).Milliseconds;

                            if (millis > 0)
                            {
                                this._mProgressWindow.CurrentItemsPerSecond = (long)(((float)dispInc) / millis) * 1000;
                            }

                            _mLastDispatchItem = DateTime.Now;
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
                return null;
            }), null);

        }
    }
}

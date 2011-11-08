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
using OpenStreetMap2Oracle.oracle;

namespace OpenStreetMap2Oracle.controller
{
    public class AppManagerController
    {
        #region Properties 

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

        #endregion     

        #region Constants

        public const int DISPATCHER_FLUSH_THRESHOLD = 100;
        public const int GUI_REFRESH_ITEMS = 100;
        
        #endregion

        #region Types

        static string xmlPath = String.Empty;
        private String sql = String.Empty;
        private ProgressWindow _mProgressWindow;
        private DateTime _mLastDispatchItem;
        private DateTime _mDispatchStarted;
        private TransactionDispatcher _mTransactionDisp;
        private TransactionQueue _mTransactionQueue;
        private BackgroundWorker xml_worker;
        private TransactionStats trans_stats;

        private long node_count = 0,
                       failed_count = 0,
                       failed_polygons = 0,
                       failed_lines = 0,
                       line_count = 0,
                       polygon_count = 0,
                       mpolygon_count = 0,
                       disp_count = 0;

        private bool nodes_finished = false;

        #endregion

        #region .ctor
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
        #endregion

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
            this.trans_stats = new TransactionStats();
            this._mProgressWindow.Show();
            this._mDispatchStarted = DateTime.Now;
            xml_worker = new BackgroundWorker();
            xml_worker.WorkerReportsProgress = true;
            xml_worker.WorkerSupportsCancellation = true;
            xml_worker.DoWork += new DoWorkEventHandler(parseXMLWorker_DoWork);
            xml_worker.RunWorkerAsync();  
        }

        #region Event Methods

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
                ApplicationManager.Instance().OSMElementDelegate = OnOSMElementAdded; // OnOSMElementAdded += new ApplicationManager.OnOSMElementAddedHandler(OnOSMElementAdded);
                ApplicationManager.Instance().OnXMLFinished += new ApplicationManager.XMLFinishedHandler(OnXMLFinished);
                ApplicationManager.Instance().ParseXML(xmlPath);
            }
        }

        void OnXMLFinished(object sender, eventArgs.XMLFinishedEventArgs e)
        {
            //OracleConnectionFactory.Transaction.Commit();           
            System.GC.Collect();

            this._mProgressWindow.Dispatcher.Invoke(DispatcherPriority.Send, new DispatcherOperationCallback(
                delegate
                {
                    this._mProgressWindow.Close();
                    return null;
                }), null);

            OwnerWindow.Dispatcher.Invoke(DispatcherPriority.Send, new DispatcherOperationCallback(
                delegate
                {
                    TransactionSummary summary = new TransactionSummary(trans_stats, this.OwnerWindow);
                    summary.Show();
                    return null;
                }), null);
        }

        #endregion

        #region Main Event Method

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
                        this._mTransactionQueue.Add(new OSMTransactionObject(sql_query));

                        if (this._mTransactionQueue.Data.Count >= DISPATCHER_FLUSH_THRESHOLD)
                        {
                            TransactionDispatcher tmpDisp = new TransactionDispatcher();
                            TransactionQueue tmpQueue = (TransactionQueue)this._mTransactionQueue.Clone();
                            this._mTransactionQueue.Clear();
                            tmpDisp.ProcessQueue(tmpQueue);
                        }

                        // refresh the UI data
                        if (element.GetType() == typeof(Node))
                        {
                            if (((++node_count) % GUI_REFRESH_ITEMS) == 0)
                            {
                                this._mProgressWindow.CurrentNodes = node_count;
                            }
                            this.trans_stats.Nodes = node_count;
                        }
                        else
                        {
                            if (!nodes_finished)
                            {
                                nodes_finished = true;
                                OracleConnectionFactory.CommitAll();
                            }
                            if (element.GetType() == typeof(Way))
                            {
                                if (!((Way)element).Line.IsPolygon())
                                {
                                    if (((++line_count) % GUI_REFRESH_ITEMS) == 0)
                                    {
                                        this._mProgressWindow.CurrentLines = line_count;
                                    }
                                    this.trans_stats.Lines = line_count;
                                }
                                else
                                {
                                    if (((++polygon_count) % GUI_REFRESH_ITEMS) == 0)
                                    {
                                        this._mProgressWindow.CurrentPolygons = polygon_count;
                                    }
                                    this.trans_stats.Polygones = polygon_count;
                                }
                            }
                            else if (element.GetType() == typeof(Relation))
                            {
                                mpolygon_count++;
                                this._mProgressWindow.CurrentMultiPolygons = mpolygon_count;
                            }
                            this.trans_stats.Multipolygons = mpolygon_count;
                        }

                        

                        if ((++disp_count) >= GUI_REFRESH_ITEMS)
                        {
                            float millis = (float)(new TimeSpan(DateTime.Now.Ticks - _mLastDispatchItem.Ticks)).Milliseconds;
                            long itemsPerSecond = (long)((((float)disp_count) / (float)millis) * (float)1000);

                            if (millis > 0)
                            {
                                this._mProgressWindow.CurrentItemsPerSecond = itemsPerSecond;
                            }

                            this._mProgressWindow.CurrentTimeElapsed = new TimeSpan(DateTime.Now.Ticks - _mDispatchStarted.Ticks);

                            if (this.trans_stats.AverageIps == 0)
                            {
                                this.trans_stats.AverageIps = itemsPerSecond;
                            }
                            else
                            {
                                this.trans_stats.AverageIps = (this.trans_stats.AverageIps + itemsPerSecond) / 2;
                            }

                            _mLastDispatchItem = DateTime.Now;
                            disp_count = 0;
                        }
                    }
                    catch (Exception ex)
                    {
                        this._mProgressWindow.CurrentErrors++;
                        this.trans_stats.Errors++;

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

        #endregion
    }
}

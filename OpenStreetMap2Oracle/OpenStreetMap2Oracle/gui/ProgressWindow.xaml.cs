using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OpenStreetMap2Oracle.gui
{
    /// <summary>
    /// Interaktionslogik für ProgressWindow.xaml
    /// </summary>
    public partial class ProgressWindow : Window
    {
        public ProgressWindow()
        {
            InitializeComponent();
        }
		
		public long CurrentNodes {
			set {
                lblNodes.Content = value.ToString();
			}
            get
            {
                return long.Parse(lblNodes.Content.ToString());
            }
		}

        public TimeSpan CurrentTimeElapsed
        {
            set
            {
                lblTimeSpan.Content = String.Format("{0:D2}:{1:D2}:{2:D2}", value.Hours, value.Minutes, value.Seconds);
            }
        }

        public long CurrentLines
        {
            set
            {
                lblLines.Content = value.ToString();
            }
            get
            {
                return long.Parse(lblLines.Content.ToString());
            }
        }

        public long CurrentPolygons
        {
            set
            {
                lblPolygones.Content = value.ToString();
            }
            get
            {
                return long.Parse(lblPolygones.Content.ToString());
            }
        }

        public long CurrentMultiPolygons
        {
            set
            {
                lblMultiPolygones.Content = value.ToString();
            }
            get
            {
                return long.Parse(lblMultiPolygones.Content.ToString());
            }
        }

        public long CurrentErrors
        {
            set
            {
                lblErrors.Content = value.ToString();
            }
            get
            {
                return long.Parse(lblErrors.Content.ToString());
            }
        }

        public long CurrentItemsPerSecond
        {
            set
            {
                lblItemsPS.Content = value.ToString();
            }
            get
            {
                return long.Parse(lblItemsPS.Content.ToString());
            }
        }
    }
}

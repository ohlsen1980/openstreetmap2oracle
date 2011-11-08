using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using OpenStreetMap2Oracle.gui;
using Microsoft.Win32;
using OpenStreetMap2Oracle.tools;
using System.IO;
using OpenStreetMap2Oracle.controller;
using OpenStreetMap2Oracle.Properties;
using System.Windows.Media.Effects;

namespace OpenStreetMap2Oracle
{
	/// <summary>
	/// Interaktionslogik für MainWindow2.xaml
	/// </summary>
	public partial class MainWindow2 : Window
	{
        BlurEffect blur;

		public MainWindow2()
		{
			this.InitializeComponent();
            blur = new BlurEffect();
            blur.Radius = 5;
            blur.KernelType = KernelType.Gaussian;
			// Fügen Sie Code, der bei der Objekterstellung erforderlich ist, unter diesem Punkt ein.
            this.gridMoveHeader.MouseLeftButtonDown +=new System.Windows.Input.MouseButtonEventHandler(gridMoveHeader_MouseLeftButtonDown);
            this.btnClose.MouseLeftButtonDown += new MouseButtonEventHandler(btnClose_MouseLeftButtonDown);
            this.btnClose.MouseEnter += new MouseEventHandler(btnClose_MouseEnter);
		}

        void btnClose_MouseEnter(object sender, MouseEventArgs e)
        {
           
        }

        void btnClose_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
		
		private void btnCreateDBConnection(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
            this.IsBackgrounded = true;
            DbLoginWindow dbDialog = new DbLoginWindow(this);
            if (dbDialog.ShowDialog() == true)
            {
                btnSelectFile.Disabled = false;
            }
            this.IsBackgrounded = false;
		}

        public bool IsBackgrounded
        {
            get
            {
                return (overlayDark.Visibility == System.Windows.Visibility.Hidden);
            }
            set
            {
                overlayDark.Visibility = (value) ? Visibility.Visible : Visibility.Hidden;
                LayoutRoot.Effect = (value) ? blur : null;
            }
        }

		private void btnSelectFile_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
            this.IsBackgrounded = true;
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "OpenStreetMap Dateien (*.osm)|*.OSM|" +
                            "Alle Dateien (*.*)|*.*";
            if (!String.IsNullOrEmpty(Settings.Default.last_path))
            {
                dialog.InitialDirectory = Settings.Default.last_path;
            }
            else
            {
                dialog.InitialDirectory = PathProvider.Path;
            }

            if (dialog.ShowDialog() == true)
            {
                Settings.Default.last_path = PathProvider.Path = new FileInfo(dialog.FileName).DirectoryName;
                AppManagerController.XmlPath = dialog.FileName;
                btnStartMigration.Disabled = false;

                Settings.Default.Save();
            }

            this.IsBackgrounded = false;
			
		}

        private void btnStartMigration_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.IsEnabled = false;

            AppManagerController controller = new AppManagerController();
            controller.OwnerWindow = this;
            controller.Start();
        }

        private void gridMoveHeader_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch (Exception ex)
            {
            }
        }
	}
}
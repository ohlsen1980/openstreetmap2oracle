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

namespace OpenStreetMap2Oracle
{
	/// <summary>
	/// Interaktionslogik für MainWindow2.xaml
	/// </summary>
	public partial class MainWindow2 : Window
	{
		public MainWindow2()
		{
			this.InitializeComponent();
			
			// Fügen Sie Code, der bei der Objekterstellung erforderlich ist, unter diesem Punkt ein.
		}

		private void btnCreateDBConnection(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
            DbLoginWindow dbDialog = new DbLoginWindow(this);
            if (dbDialog.ShowDialog() == true)
            {
                btnSelectFile.Disabled = false;
            }
		}

		private void btnSelectFile_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "OpenStreetMap Dateien (*.osm)|*.OSM|" +
                            "Alle Dateien (*.*)|*.*";
            dialog.InitialDirectory = PathProvider.Path;

            if (dialog.ShowDialog() == true)
            {
                PathProvider.Path = new FileInfo(dialog.FileName).DirectoryName;
                AppManagerController.XmlPath = dialog.FileName;
                btnStartMigration.Disabled = false;
            }
			
		}

        private void btnStartMigration_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.IsEnabled = false;

            AppManagerController controller = new AppManagerController();
            controller.Start();
        }
	}
}
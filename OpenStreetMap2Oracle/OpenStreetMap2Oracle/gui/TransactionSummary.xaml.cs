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
using OpenStreetMap2Oracle.businesslogic.Transaction;

namespace OpenStreetMap2Oracle
{
	/// <summary>
	/// Interaktionslogik für TransactionSummary.xaml
	/// </summary>
	public partial class TransactionSummary : Window
	{
		public TransactionSummary(TransactionStats stats, Window owner)
		{
			this.InitializeComponent();
            this.Owner = owner;
            this.lblNodes.Content = stats.Nodes;
            this.lblLines.Content = stats.Lines;
            this.lblPolygones.Content = stats.Polygones;
            this.lblMultiPolygones.Content = stats.Multipolygons;
            this.lblErrors.Content = stats.Errors;
            this.lblItemsPS.Content = stats.AverageIps;
			// Fügen Sie Code, der bei der Objekterstellung erforderlich ist, unter diesem Punkt ein.
		}

		private void btnDone_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
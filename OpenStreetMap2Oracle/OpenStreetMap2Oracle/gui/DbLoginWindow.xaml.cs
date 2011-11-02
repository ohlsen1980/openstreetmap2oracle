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
using OpenStreetMap2Oracle.oracle;

namespace OpenStreetMap2Oracle.gui
{
    /// <summary>
    /// Interaktionslogik für DbLoginWindow.xaml
    /// </summary>
    public partial class DbLoginWindow : Window
    {
        private String _user = String.Empty;
        /// <summary>
        /// The oracle user
        /// </summary>
        public String User
        {
            get { return _user; }            
        }

        private String _passwort = String.Empty;
        /// <summary>
        /// The password
        /// </summary>
        public String Passwort
        {
            get { return _passwort; }            
        }

        private String _service = String.Empty;
        /// <summary>
        /// The TNS name of the oracle sid
        /// </summary>
        public String Service
        {
            get { return _service; }            
        }


        public DbLoginWindow()
        {
            InitializeComponent();
        }

        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {
            this._user = this.UserTxtBox.Text;
            this._passwort = this.PassTxtBox.Password;
            this._service = this.ServTxtBox.Text;
            DbExport conn = OpenStreetMap2Oracle.oracle.OracleConnectionFactory.CreateConnection(User, Passwort, Service);
            try
            {
                conn.openDbConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            this.Close();
            MessageBox.Show(conn.DbConnection.State.ToString());
        }

        private void AbbrBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

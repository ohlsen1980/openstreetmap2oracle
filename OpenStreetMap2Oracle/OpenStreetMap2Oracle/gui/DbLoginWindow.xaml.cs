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
using System.Windows.Media;

namespace OpenStreetMap2Oracle.gui
{
    /// <summary>
    /// Interaktionslogik für DbLoginWindow.xaml
    /// </summary>
    public partial class DbLoginWindow : Window
    {
        private String _errorMessage = String.Empty;

        private bool IsValidCon = false;

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



        public DbLoginWindow(Window owner)
        {
            InitializeComponent();            
            this.Owner = owner;
        }

        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {
            TestIt();
            if (IsValidCon)
            {
                //TODO Set User Credentials in global Class and get poolsize from user
                OracleConnectionFactory.Init(User, Passwort, Service, 75);
                OracleConnectionFactory.CreateConnection();
                this.Close();
            }           
        }

        private void AbbrBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TestConnection()
        {
            DbExport conn = new DbExport(User, Passwort, Service);
            try
            {
                conn.openDbConnection();
                if (conn.DbConnection.State == System.Data.ConnectionState.Open)
                    IsValidCon = true;
                else
                    IsValidCon = false;
                conn.closeDbConnection();
                conn.DbConnection.Dispose();
                conn.Dispose();
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
            }
           

            
        }

        private void TestIt()
        {
            IsValidCon = false;
            this._user = this.UserTxtBox.Text;
            this._passwort = this.PassTxtBox.Password;
            this._service = this.ServTxtBox.Text;
            
            if (User != String.Empty && Passwort != String.Empty && Service != String.Empty)
            {
                TestConnection();
            }

            if (IsValidCon == true)
            {
                this.TestConLabel.Text = "OK";
                this.TestConLabel.Foreground = Brushes.Green;
                this.TestConLabel.Visibility = Visibility.Visible;
            }
            else
            {
                if (_errorMessage != String.Empty)
                    this.TestConLabel.Text = _errorMessage;
                else
                    this.TestConLabel.Text = "FAILED";
                this.TestConLabel.Foreground = Brushes.Red;
                this.TestConLabel.Visibility = Visibility.Visible;
            }
        }

        private void TestBtn_Click(object sender, RoutedEventArgs e)
        {
            TestIt();
        }
    }
}

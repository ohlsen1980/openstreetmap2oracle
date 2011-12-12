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
            UserCredentials credentials = new UserCredentials(this._user, this._passwort, this._service);
            OracleConnectionFactory.Instance.Credentials = credentials;
            //DbExport conn = OpenStreetMap2Oracle.oracle.OracleConnectionFactory.CreateConnection(User, Passwort, Service);
            //try
            //{
            //    conn.openDbConnection();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
            this.Close();
            //MessageBox.Show(conn.DbConnection.State.ToString());
        }

        private void AbbrBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

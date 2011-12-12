using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenStreetMap2Oracle.oracle
{
    public class UserCredentials
    {

        private String _user = String.Empty;

        public String User
        {
            get { return _user; }           
        }

        private String _password = String.Empty;

        public String Password
        {
            get { return _password; }            
        }

        private String _service = String.Empty;

        public String Service
        {
            get { return _service; }            
        }

        public UserCredentials(String user, String password, String service)
        {
            this._user = user;
            this._password = password;
            this._service = service;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strata.DB.Drivers;
namespace Strata.DB {
    public sealed class DatabaseConfig {
        private string _server;
        private string _dbname;
        private int _port = 0;
        private string _username = null;
        private string _password = null;
        //private Func<AbstractDriver> _factory = null;
        public DatabaseConfig(string uri) {
            this.Uri = uri;
        }
        public DatabaseConfig(string server, string dbname) {
            this._server = server;
            this._dbname = dbname;
        }
        public DatabaseConfig(string server, int port, string dbname) {
            this._server = server;
            this._port = port;
            this._dbname = dbname;
        }
        public DatabaseConfig(string server, string dbname, string username, string password) : this(server, 0, dbname, username, password){}
        public DatabaseConfig(string server, int port, string dbname, string username, string password) {
            this._server = server;
            this._port = port;
            this._dbname = dbname;
            this._username = username;
            this._password = password;
        }


        //internal DatabaseConfig BindDriver(AbstractDriver driver) {
        //    this._driver = driver;
        //    return this;
        //}

        //internal AbstractDriver Spawn() {
        //    this._driver
        //}  


        #region -------- PROPERTIES --------
        internal string Label {
            get;
            set;
        }

        internal string Uri {
            get;
            set;
        }

        //internal AbstractDriver DriverClass{
        //    get;
        //    set;
        //}

        internal string ConnectionString {
            get;
            set;
        }

        internal int HashCode {
            get;
            set;
        }

        public bool Authenticated {
            get {
                if (this._username == null && this._password == null)
                    return false;
                return true;
            }
        }

        public string Server {
            get { return this._server; }
        }

        public string DBname {
            get { return this._dbname; }
        }

        public int Port {
            get { return this._port; }
            set { this._port = value; }
        }

        internal string Username {
            get { return this._username; }
        }

        internal string Password {
            get { return this._password; }
        }
        #endregion

        public override string ToString() {
            return this._dbname + "@" + this._server;
        }
    }
}

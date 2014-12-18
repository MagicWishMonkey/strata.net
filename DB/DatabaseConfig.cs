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

        public static DatabaseConfig Parse(string label, Dictionary<string, dynamic> settings) {
            var config = DatabaseConfig.Parse(settings);
            config.Label = label;
            return config;
        }
        public static DatabaseConfig Parse(Dictionary<string, dynamic> settings) {
            var driver = settings["driver"];
            var server = (string)settings["server"];
            var database = settings["database"];
            var config = new DatabaseConfig(server, database);
            config.Driver = (string)driver;

            var ix = server.IndexOf(":");
            if (ix > -1) {
                var port = server.Substring(ix + 1);
                server = server.Substring(0, ix);
                config._port = Int32.Parse(port);
                config._server = server;
            }

            try {
                var dsn = settings["dsn"];
                config.DSN = dsn;
            } catch (Exception ex) { }
            
            try{
                var username = settings["username"];
                config._username = username;
            }catch(Exception ex){}

            try{
                var password = settings["password"];
                config._password = password;
            }catch(Exception ex){}

            try {
                var port = (int)settings["port"];
                config._port = port;
            } catch (Exception ex) { }

            return config;
        }


        //internal DatabaseConfig BindDriver(AbstractDriver driver) {
        //    this._driver = driver;
        //    return this;
        //}

        //internal AbstractDriver Spawn() {
        //    this._driver
        //}  


        #region -------- PROPERTIES --------
        public string Driver {
            get;
            internal set;
        }

        public string Label {
            get;
            internal set;
        }

        internal string Uri {
            get;
            set;
        }

        internal string ConnectionString {
            get;
            set;
        }

        internal int HashCode {
            get;
            set;
        }

        internal string DSN {
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

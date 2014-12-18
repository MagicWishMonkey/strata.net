using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Transactions;
using Strata.DB.Drivers;

namespace Strata.DB {
    public class Database {
        private AbstractDriver _driver;
        internal Database(AbstractDriver driver) {
            this._driver = driver;
        }

        public Database AttachLabel(string label) {
            this.Label = label;
            return this;
        }

        public Query Query(string sql, params KeyValuePair<string, object>[] parameters) {
            var query = QueryFactory.Parse(sql);
            query.Link(this.ID);
            if (parameters != null && parameters.Length > 0)
                query.Bind(parameters);
            return query;
        }

        public Query Query(string sql, object obj) {
            var query = QueryFactory.Parse(sql);
            query.Link(this.ID);
            query.Bind(obj);
            return query;
        }

        public DataTable SelectTable(string sql) {
            return this.SelectTable(new Query(sql));
        }

        public DataTable SelectTable(Query query) {
            using (var txn = new DatabaseTransaction(this._driver)) {
                return txn.SelectTable(query);
            }
        }


        public List<Dictionary<string, dynamic>> Select(string sql) {
            return this.Select(new Query(sql));
        }

        public List<Dictionary<string, dynamic>> Select(Query query) {
            using (var txn = new DatabaseTransaction(this._driver)) {
                return txn.SelectTable(query).ToRecords();
            }
        }

        //public List<T> Select<T>(Query query) {

        //    //Func<Dictionary<string, dynamic>, T> factory = Strata.Util.Toolkit.Curry(
        //    //    (Func<Type, Dictionary<string, dynamic>, T>)Model.Construct, this.ModelClass
        //    //);

        //    using (var txn = new DatabaseTransaction(this._driver)) {
        //        var records = txn.SelectTable(query).ToRecords();
        //    }
        //}

        public List<dynamic> Scalars(string sql) {
            return this.Scalars(new Query(sql));
        }

        public List<dynamic> Scalars(Query query) {
            using (var txn = new DatabaseTransaction(this._driver)) {
                return txn.SelectTable(query).ToScalars();
            }
        }

        public List<T> Scalars<T>(string sql) {
            return this.Scalars<T>(new Query(sql));
        }

        public List<T> Scalars<T>(Query query) {
            using (var txn = new DatabaseTransaction(this._driver)) {
                var scalars = txn.SelectTable(query).ToScalars();
                try { 
                    var converted = scalars.ConvertAll(
                        new Converter<dynamic, T>(
                             delegate(dynamic x) {
                                 var o = (T)x;
                                 return o;
                             }   
                        )
                     );
                    return converted;
                } catch (Exception ex) {
                    scalars = scalars.Where(x => x != null).ToList();
                    var converted = scalars.ConvertAll(
                        new Converter<dynamic, T>(
                             delegate(dynamic x) {
                                 var o = (T)x;
                                 return o;
                             }
                        )
                     );
                    return converted;
                }
            }
        }

        #region -------- PUBLIC - Update/Insert/Execute --------
        public int Update(string sql) {
            if (String.IsNullOrEmpty(sql))
                throw new ArgumentNullException("The sql parameter is null/empty!");

            var o = this.Execute(new Query(sql));
            return (o is int) ? (int)o : 0;
        }
        public int Update(Query query) {
            if (query == null)
                throw new ArgumentNullException("The query parameter is null!");

            var o = this.Execute(query);
            return (o is int) ? (int)o : 0;
        }

        public int Insert(string sql, params KeyValuePair<string, object>[] parameters) {
            if (String.IsNullOrEmpty(sql))
                throw new ArgumentNullException("The sql parameter is null/empty!");
            
            var o = this.Execute(this.Query(sql, parameters));
            return (o is int) ? (int)o : 0;
        }

        public int Insert(Query query) {
            if (query == null)
                throw new ArgumentNullException("The query parameter is null!");

            var o = this.Execute(query);
            return (o is int) ? (int)o : 0;
        }

        public object Execute(string sql) {
            if (String.IsNullOrEmpty(sql))
                throw new ArgumentNullException("The sql parameter is null/empty!");
            return this.Execute(new Query(sql));
        }

        public object Execute(Query query) {
            if (query == null)
                throw new ArgumentNullException("The query parameter is null!");


            if (this._driver.SupportsTransactions) {
                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required)) {
                    using (var txn = new DatabaseTransaction(this._driver)) {
                        try {
                            //if (query.Type == CommandType.StoredProcedure) {
                            //    var obj = txn.Execute(query);
                            //    ts.Complete();
                            //    return obj;
                            //} 
                            var obj = txn.Execute(query);
                            ts.Complete();
                            return obj;
                        } catch (Exception ex) {
                            throw ex;
                        }
                    }
                }
            }

            using (var txn = new DatabaseTransaction(this._driver)) {
                try {
                    var obj = txn.Execute(query);
                    return obj;
                } catch (Exception ex) {
                    throw ex;
                }
            }
        }

        //public object ExecuteStoredProcedure(String storedProcedureName, List<Dictionary<String, object>> parameters) {
        //    if (storedProcedureName == null)
        //        throw new ArgumentNullException("The storedProcedureName parameter is null!");

        //    if (this.Driver.SupportsTransactions) {
        //        using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required)) {
        //            try {
        //                var obj = this.Transactor.ExecuteStoredProcedure(storedProcedureName, parameters);
        //                ts.Complete();
        //                return obj;
        //            } catch (Exception ex) {
        //                throw ex;
        //            }

        //        }
        //    }

        //    return this.Transactor.ExecuteStoredProcedure(storedProcedureName, parameters);
        //}
        #endregion



        #region -------- PUBLIC STATIC - CONNECT METHODS --------
        public static Database Connect(string driver, string uri) {
            return Connect(driver, null, 0, null, null, null, uri);
        }
        public static Database Connect(string driver, string server, string dbname) {
            return Connect(driver, server, 0, dbname, null, null, null);
        }
        public static Database Connect(string driver, string server, int port, string dbname) {
            return Connect(driver, server, 0, dbname, null, null, null);
        }
        public static Database Connect(string driver, string server, string dbname, string username, string password) {
            return Connect(driver, server, 0, dbname, username, password, null);
        }
        public static Database Connect(string driver, string server, int port, string dbname, string username, string password) {
            return Connect(driver, server, port, dbname, username, password, null);
        }
        public static Database Connect(string driver, string server, int port, string dbname, string username, string password, string uri) {
            DatabaseConfig config = null;
            if (uri != null) {
                if (server == null) {
                    config = new DatabaseConfig(uri);
                }
            }
            if(config == null)
                config = new DatabaseConfig(server, port, dbname, username, password);
            config.Label = "default";
            config.Driver = driver;
            return Database.Connect(config);
        }
        public static Database Connect(DatabaseConfig config) {
            var dbDriver = BuildDriver(config.Driver, config);
            return Register(new Database(dbDriver));
        }

        private static AbstractDriver BuildDriver(string name, DatabaseConfig config) {
            var type = name.Trim().ToLower();
            if (type == "sqlserver")
                return new SqlServerDriver(config);
            else if (type == "postgres" || type == "postgresql")
                return new PostgresDriver(config);
            //else if (type == "sqlite" || type == "sqllite")
            //    return new SQLiteDriver(config);
            throw new Exception("The driver is not recognized: " + name);
        }
        #endregion


        #region -------- PROPERTIES --------
        internal DatabaseConfig Config {
            get { return this._driver.Config; }
        }

        public string Label {
            get { return this.Config.Label; }
            set {
                if (value != null) {
                    this.Config.Label = value;
                }
            }
        }

        public int ID {
            get { return this.Config.HashCode; }
        }
        #endregion


        public override string ToString() {
            return "Database: " + this.Label;
        }

        #region -------- SINGLETON STUFF --------
        private static Dictionary<string, AbstractDriver> _lookup = new Dictionary<string, AbstractDriver>(StringComparer.CurrentCultureIgnoreCase);
        private static AbstractDriver _default;

        //internal static void ClearSaved() {
        //    _lookup.Clear();
        //    _default = null;
        //}

        internal static Database Register(Database database) {
            var id = database.Config.HashCode.ToString();
        

            lock (_lookup) {

                if (!_lookup.ContainsKey(id))
                    _lookup.Add(id, database._driver);

                //if (!_lookup.ContainsKey(database.Label.Trim().ToLower()))
                //    _lookup.Add(database.Label.Trim().ToLower(), database._driver);
                if (_default == null)
                    _default = database._driver;
            }
            return database;
        }

        internal static Database Get(int id) {
            var key = id.ToString();
            if (_lookup.ContainsKey(key))
                return new Database(_lookup[key]);
            throw new ArgumentException("A database with that id has not been registered. id-> " + id);
        }

        internal static Database Get(string id) {
            var key = id.Trim().ToLower();
            if (_lookup.ContainsKey(key))
                return new Database(_lookup[key]);
            throw new ArgumentException("A database with that id has not been registered. id-> " + id);
        }


        internal static void SetDefault(Database database) {
            Register(database);
            _default = database._driver;
        }

        internal static void SetDefault(string id) {
            var key = id.Trim().ToLower();
            if (!_lookup.ContainsKey(key))
                throw new ArgumentException("A database with that id has not been registered. id-> " + id);
            _default = _lookup[key];
        }

        public static Database Default {
            get {
                if (_default == null)
                    throw new System.Data.DataException("The Default database has not been assigned!");
                return new Database(_default);
            }
        }
        #endregion
    }
}

//using System;
//using System.Linq;
//using System.Data;
//using System.Data.Common;
//using System.Data.SQLite;
//using System.Collections.Generic;
//using System.Diagnostics.Contracts;
//using Strata.DB;

//namespace Strata.DB.Drivers {
//    public class SQLiteDriver : AbstractDriver {
//        #region -------- CONSTRUCTOR/VARIABLES --------
//        internal SQLiteDriver(DatabaseConfig config)
//            : base("SQLite", config, SQLiteClientFactory.Instance) {
//        }
//        #endregion

//        #region -------- PUBLIC VIRTUAL - BuildConnectionString --------
//        public override string BuildConnectionString(DatabaseConfig config) {
//            //if (!Util.IO.FileUtil.Exists(config.Uri)) {
//            //    var data = "U1FMaXRlIGZvcm1hdCAzAAQAAQEAQCAgAAAAAQAAAAEAAAAAAAAAAAAAAAEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABAC3mAQ0AAAAABAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==";
//            //    var raw = Util.Toolkit.UnBase64(data);
//            //    var bytes = Util.Toolkit.ToBytes(raw);
//            //    Util.IO.FileUtil.Write(config.Uri, bytes);
//            //}

//            string connectionString = null;
//            if (!config.Authenticated) {
//                connectionString = string.Format(
//                    "Data Source={0};Version=3;UseUTF16Encoding=True;",
//                    config.Uri
//                );
//            } else {
//                connectionString = string.Format(
//                    "Data Source={0};Password={1};Version=3;UseUTF16Encoding=True;",
//                    config.Uri,
//                    config.Password
//                );
//            }

//            // check if db is in place, if not create a default sqlite db with the sequences table in place
//            if (!Util.IO.FileUtil.Exists(config.Uri)) {
//                var conn = new System.Data.SQLite.SQLiteConnection(connectionString);
//                var cmd = new System.Data.SQLite.SQLiteCommand("CREATE TABLE \"sequences\" (\"id\" INTEGER NOT NULL, PRIMARY KEY(\"id\"));");
//                cmd.Connection = conn;

//                try {
//                    conn.Open();
//                    cmd.ExecuteNonQuery();
//                } catch (Exception ex) {
//                    throw ex;
//                } finally {
//                    cmd.Dispose();
//                    conn.TryClose();
//                }
//            }
//            return connectionString;
//        }
//        #endregion

//        #region -------- PUBLIC OVERRIDE - CreateParameter --------
//        public override DbParameter CreateParameter(QueryParameter parameter) {
//            var param = base.CreateParameter(parameter) as System.Data.SQLite.SQLiteParameter;
//            param.ParameterName = "@" + param.ParameterName;
//            return param;
//        }
//        #endregion
//    }

//    #region -------- SQLiteClientFactory CLASS --------
//    public class SQLiteClientFactory : DbProviderFactory {
//        private static SQLiteClientFactory _instance = null;

//        public static SQLiteClientFactory Instance {
//            get {
//                if (SQLiteClientFactory._instance == null) {
//                    SQLiteClientFactory._instance = new SQLiteClientFactory();
//                }
//                return SQLiteClientFactory._instance;
//            }
//        }

//        public override DbCommand CreateCommand() {
//            return new System.Data.SQLite.SQLiteCommand();
//        }

//        public override DbCommandBuilder CreateCommandBuilder() {
//            return new System.Data.SQLite.SQLiteCommandBuilder();
//        }

//        public override DbConnection CreateConnection() {
//            return new System.Data.SQLite.SQLiteConnection();
//        }

//        public override DbConnectionStringBuilder CreateConnectionStringBuilder() {
//            return new System.Data.SQLite.SQLiteConnectionStringBuilder();
//        }

//        public override DbDataAdapter CreateDataAdapter() {
//            return new System.Data.SQLite.SQLiteDataAdapter();
//        }

//        public override DbParameter CreateParameter() {
//            return new System.Data.SQLite.SQLiteParameter();
//        }

//        public override bool CanCreateDataSourceEnumerator {
//            get {
//                return false;
//            }
//        }
//    }
//    #endregion
//}

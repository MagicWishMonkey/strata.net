using System;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Strata.DB;
namespace Strata.DB.Drivers {
    public class OleDbDriver : AbstractDriver {
        #region -------- CONSTRUCTOR/VARIABLES --------
        internal OleDbDriver(DatabaseConfig config)
            : base("OleDb", config, System.Data.OleDb.OleDbFactory.Instance) {
        }
        internal OleDbDriver(string name, DatabaseConfig config)
            : base(name, config, System.Data.OleDb.OleDbFactory.Instance) {
        }
        #endregion


        #region -------- PUBLIC VIRTUAL - BuildConnectionString --------
        public override string BuildConnectionString(DatabaseConfig config) {
            throw new NotImplementedException("The connection string builder function is not implemented for the OleDbDriver base class.");
        }
        #endregion


        #region -------- PUBLIC OVERRIDE - CreateConnection/CreateAdapter --------
        public override DbConnection CreateConnection(string connectionString) {
            return new System.Data.OleDb.OleDbConnection(connectionString);
        }
        public override DbConnection CreateConnection() {
            return new System.Data.OleDb.OleDbConnection(this.Config.ConnectionString);
        }
        public override DbDataAdapter CreateAdapter(DbCommand command) {
            return new System.Data.OleDb.OleDbDataAdapter((System.Data.OleDb.OleDbCommand)command);
        }
        #endregion


        #region -------- PUBLIC OVERRIDE - CreateParameter --------
        public override DbParameter CreateParameter(QueryParameter parameter) {
            var param = base.CreateParameter(parameter) as OleDbParameter;
            param.ParameterName = "@" + param.ParameterName;
            return param;
        }
        #endregion

    }
}

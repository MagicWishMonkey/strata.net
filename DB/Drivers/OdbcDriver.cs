using System;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Strata.DB;
namespace Strata.DB.Drivers {
    public class OdbcDriver : AbstractDriver {
        #region -------- CONSTRUCTOR/VARIABLES --------
        internal OdbcDriver(DatabaseConfig config)
            : base("Odbc", config, System.Data.Odbc.OdbcFactory.Instance) {
        }
        internal OdbcDriver(string name, DatabaseConfig config)
            : base(name, config, System.Data.Odbc.OdbcFactory.Instance) {
        }
        #endregion


        #region -------- PUBLIC VIRTUAL - BuildConnectionString --------
        public override string BuildConnectionString(DatabaseConfig config) {
            throw new NotImplementedException("The connection string builder function is not implemented for the OdbcDriver base class.");
        }
        #endregion


        #region -------- PUBLIC OVERRIDE - CreateConnection --------
        public override DbConnection CreateConnection(string connectionString) {
            return new OdbcConnection(connectionString);
        }
        public override DbConnection CreateConnection() {
            return new OdbcConnection(this.Config.ConnectionString);
        }
        #endregion

        #region -------- PUBLIC OVERRIDE - CreateParameter --------
        public override DbParameter CreateParameter(QueryParameter parameter) {
            var param = base.CreateParameter(parameter) as OdbcParameter;
            //param.ParameterName = "@" + param.ParameterName;
            return param;
        }
        #endregion
    }
}

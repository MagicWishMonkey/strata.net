using System;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Strata.DB;
namespace Strata.DB.Drivers {
    public class SqlServerDriver : AbstractDriver {
        #region -------- CONSTRUCTOR/VARIABLES --------
        internal SqlServerDriver(DatabaseConfig config)
            : base("SqlServer", config, System.Data.SqlClient.SqlClientFactory.Instance) {
                this.SupportsTransactions = true;
        }
        #endregion
        
        #region -------- PUBLIC VIRTUAL - BuildConnectionString --------
        public override string BuildConnectionString(DatabaseConfig config) {
            if (config.Authenticated) {
                return "Server=" + config.Server + ";Database=" + config.DBname + ";User Id=" + config.Username + ";Password=" + config.Password + ";";
            } else {
                return "Server=" + config.Server + ";Database=" + config.DBname + ";Integrated Security=SSPI";
            }
        }
        #endregion

        #region -------- PUBLIC OVERRIDE - CreateParameter --------
        public override DbParameter CreateParameter(QueryParameter parameter) {
            var param = base.CreateParameter(parameter) as SqlParameter;
            param.ParameterName = "@" + param.ParameterName;
            return param;
        }
        #endregion
    }
}

using System;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Strata.DB;

// DOWNLOAD DRIVER: http://www.postgresql.org/ftp/odbc/versions/msi/

namespace Strata.DB.Drivers {
    public class PostgresDriver : OdbcDriver {
        #region -------- CONSTRUCTOR/VARIABLES --------
        internal PostgresDriver(DatabaseConfig config)
            : base("Postgres", config) {
        }
        #endregion


        #region -------- PUBLIC VIRTUAL - BuildConnectionString --------
        public override string BuildConnectionString(DatabaseConfig config) {
            if(config.Port < 1)
                config.Port = 5432;

            if (config.Authenticated) {
                var str = string.Format(
                    "Server={0};Port={1};Database={2};Uid={3};Pwd={4};",
                    config.Server,
                    config.Port,
                    config.DBname,
                    config.Username,
                    config.Password
                );
                str = "Driver={PostgreSQL UNICODE(x64)};" + str;
                return str;
                //return "Driver={PostgreSQL UNICODE(x64)};Host=" + config.Server + ";Port=" + config.Port + ";Database=" + config.DBname + ";Uid=" + config.Username + ";Pwd=" + config.Password + ";";
                //return "Host=" + config.Server + ";Port=5432;Database=" + config.DBname + ";User ID=" + config.Username + ";Password=" + config.Password + ";Pooling=true;Min Pool Size=0;Max Pool Size=100;Connection Lifetime=0;";
            } else {
                return "Host=" + config.Server + ";Database=" + config.DBname + ";Integrated Security=SSPI";
            }
        }
        #endregion


       

    }
}

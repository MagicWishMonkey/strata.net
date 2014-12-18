using System;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Strata.DB;
namespace Strata.DB.Drivers {
    public abstract class AbstractDriver {
        #region -------- CONSTRUCTOR/VARIABLES --------
        public bool SupportsTransactions = false;
        internal AbstractDriver(string driverName, DatabaseConfig config, DbProviderFactory factory) {
            Contract.Requires(!String.IsNullOrEmpty(driverName));
            Contract.Requires(config != null);
            Contract.Requires(factory != null);

            if (config.ConnectionString == null) {
                config.ConnectionString = this.BuildConnectionString(config);
                config.HashCode = config.ConnectionString.Trim().ToLower().GetHashCode();
            }

            this.Config = config;
            this.Factory = factory;
            var name = factory.GetType().FullName;
            this.Type = name.Substring(0, name.LastIndexOf('.'));
            this.Name = driverName;
        }
        #endregion

        #region -------- PUBLIC VIRTUAL - BuildConnectionString --------
        public abstract string BuildConnectionString(DatabaseConfig config);
           // throw new NotImplementedException();
        //} 
        #endregion

        #region -------- PUBLIC VIRTUAL - CreateConnection/CreateAdapter/CreateParameter/CreateCommand --------
        public virtual DbConnection CreateConnection() {
            var conn = this.Factory.CreateConnection();
            conn.ConnectionString = this.Config.ConnectionString;
            return conn;
        }

        public virtual DbConnection CreateConnection(string connectionString) {
            var conn = this.Factory.CreateConnection();
            conn.ConnectionString = connectionString;
            return conn;
        }

        public virtual DbDataAdapter CreateAdapter(DbCommand command) {
            var adapter = this.Factory.CreateDataAdapter();
            adapter.SelectCommand = command;
            return adapter;
        }

        public virtual DbParameter CreateParameter(QueryParameter parameter) {
            var param = this.Factory.CreateParameter();
            param.ParameterName = parameter.Name;
            param.Direction = parameter.Direction;
            param.Value = (parameter.Value == null) ? DBNull.Value : parameter.Value;
            return param;
        }

        public virtual DbCommand CreateCommand(string connectionString) {
            var connection = this.CreateConnection(connectionString);
            return this.CreateCommand(connection);
        }

        public virtual DbCommand CreateCommand(DbConnection connection) {
            var command = this.Factory.CreateCommand();
            command.Connection = connection;
            return command;
        }

        public virtual DbCommand CreateCommand(DbConnection connection, Query query) {
            var command = this.Factory.CreateCommand();
            command.Connection = connection;
            this.Bind(command, query);
            return command;
        }
        #endregion

        #region -------- PUBLIC VIRTUAL - Bind --------
        public virtual void Bind(DbCommand cmd, Query query) {
            cmd.CommandType = query.Type;
            cmd.CommandText = query.Sql;
            cmd.CommandTimeout = 30;
            if (query.Parameters == null)
                return;

            foreach (var param in query.Parameters) {
                var p = this.CreateParameter(param);
                cmd.Parameters.Add(p);
            }
        }
        #endregion

        #region -------- PROPERTIES --------
        public DatabaseConfig Config {
            get;
            private set;
        }

        public string Type {
            get;
            private set;
        }

        public string Name {
            get;
            private set;
        }

        public DbProviderFactory Factory {
            get;
            private set;
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Common;
using Strata.DB.Drivers;

namespace Strata.DB {
    internal sealed class DatabaseTransaction : IDisposable {
        private AbstractDriver _driver = null;
        private DbConnection _conn = null;
        public DatabaseTransaction(AbstractDriver driver) {
            this._driver = driver;
            this._conn = driver.CreateConnection();
        }

        private DbCommand CreateCommand(Query query) {
            var cmd = this._driver.CreateCommand(this._conn, query);
            return cmd;
        }
        private IDbCommand CreateStoredProcedureCommand(string procedureName) {
            var cmd = this._driver.CreateCommand(this._conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = procedureName;
            return cmd;
        }

        private DbDataAdapter CreateAdapter(DbCommand cmd) {
            var adapter = this._driver.CreateAdapter(cmd);
            return adapter;
        }


        #region -------- PUBLIC - Select/SelectTable --------
        public DataSet Select(Query query) {
            var cmd = this.CreateCommand(query);
            var adapter = this.CreateAdapter(cmd);
            DataSet ds = new DataSet();
            try {
                this._conn.Open();
                adapter.Fill(ds);
            } catch (Exception ex) {
                throw ex;
            } finally {
                cmd.Dispose();
                this._conn.TryClose();
            }
            return ds;
        }

        public DataTable SelectTable(Query query) {
            DataSet ds = this.Select(query);
            DataTable table = (ds.Tables.Count > 0) ? ds.Tables[0] : null;
            return table;
        }
        #endregion


        #region -------- PUBLIC - Execute --------
        public object Execute(Query query) {
            object result = null;
            IDbCommand command = this.CreateCommand(query);
            IDbConnection connection = command.Connection;
            IDbTransaction transaction = null;
            try {
                connection.Open();
                transaction = connection.BeginTransaction();
                command.Transaction = transaction;
                command.ExecuteNonQuery();
                transaction.Commit();
                transaction = null;
                if (query.HasOutputParams)
                    result = command.GetCommandOutputs();
            } catch (Exception ex) {
                //this.Source.Driver.HandleException(ex);
                if (transaction != null)
                    transaction.Rollback();
                throw new System.Data.DataException("Database.Execute Error-> " + ex.Message, ex);
            } finally {
                command.Dispose();
                //command.Connection.TryClose();
            }

            return result;
        }
        #endregion


        #region -------- PUBLIC - ExecuteStoredProcedure --------
        public object ExecuteStoredProcedure(String storedProcedureName, List<Dictionary<String, object>> parameters) {
            object result = null;
            IDbCommand command = this.CreateStoredProcedureCommand(storedProcedureName);
            IDbConnection connection = command.Connection;
            try {
                connection.Open();

                // Add Parameters
                if (parameters != null) {
                    foreach (Dictionary<String, object> dict in parameters) {
                        IDataParameter parameter = command.CreateParameter();
                        if (dict.ContainsKey("parameterName"))
                            parameter.ParameterName = (String)dict["parameterName"];
                        else
                            throw new System.Data.DataException("Database.ExecuteStoredProcedure Error-> No parameterName in iDataParameter specified");

                        if (dict.ContainsKey("DBType"))
                            parameter.DbType = (DbType)dict["DBType"];
                        else
                            throw new System.Data.DataException("Database.ExecuteStoredProcedure Error-> No DBType in iDataParameter specified");

                        if (dict.ContainsKey("value"))
                            parameter.Value = (Object)dict["value"];
                        else
                            throw new System.Data.DataException("Database.ExecuteStoredProcedure Error-> No value in iDataParameter specified");
                        command.Parameters.Add(parameter);
                    }
                }

                command.ExecuteScalar();

            } catch (Exception ex) {
                //this.Source.Driver.HandleException(ex);
                throw new System.Data.DataException("Database.ExecuteStoredProcedure Error-> " + ex.Message, ex);
            } finally {
                command.Dispose();
                //command.Connection.TryClose();
            }

            return result;
        }
        #endregion

        #region -------- DISPOSE/CLEANUP --------
        public void Dispose() {
            if (this._driver == null)
                return;
            try {
                this._driver = null;
                var conn = this._conn;
                if (conn != null) {
                    this._conn = null;
                    conn.TryClose();
                    conn.TryDispose();
                }
            } catch { }

            GC.SuppressFinalize(this); 
        }
        #endregion
    }
}

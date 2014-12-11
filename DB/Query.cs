using System;
using System.Linq;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
namespace Strata.DB {
    [Serializable]
    public class Query {
        #region -------- VARIABLES & CONSTRUCTOR(S) --------
        private int _dbID;
        private CommandType _type;
        private string _sql;
        //private Dictionary<string, QueryParameter> _params;
        private bool _hasOutputParams = false;
        public Query(string sql, params KeyValuePair<string, object>[] parameters) {
            if (String.IsNullOrEmpty(sql)) throw new ArgumentNullException("The sql parameter is null/empty!");

            //this._sql = sql;
            this._sql = FilterSqlComments(sql);
            this.Parameters = new List<QueryParameter>();
            //this._params = new Dictionary<string, QueryParameter>();
            if (this._sql.Trim().IndexOf(" ") < 0)///NO SPACES IN QUERY = STORED PROCEDURE CALL
                this._type = CommandType.StoredProcedure;
            else
                this._type = CommandType.Text;

            if (parameters == null || parameters.Length == 0)
                return;

            foreach (KeyValuePair<string, object> pair in parameters) {
                this.Set(pair.Key, pair.Value);
            }
        }
        #endregion


        internal void Link(int dbID) {
            this._dbID = dbID;
        }

        public DataTable SelectTable() {
            return this.DB.SelectTable(this);
        }

        public List<Dictionary<string, dynamic>> Select() {
            return this.DB.Select(this);
        }

        public List<dynamic> Scalars() {
            return this.DB.Scalars(this);
        }


       

        #region -------- PRIVATE STATIC - FilterSqlComments --------
        private static string FilterSqlComments(string sql) {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"--.+$", System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.Multiline);
            sql = regex.Replace(sql, " ");
            return sql;
        }
        #endregion

        #region -------- PUBLIC STATIC - Create --------
        public static Query Create(string sql, params KeyValuePair<string, object>[] parameters) {
            return (parameters == null) ? new Query(sql) : new Query(sql, parameters);
        }
        #endregion

        //#region -------- PUBLIC - BindParams --------
        //public void BindParams(Record record) {
        //    foreach (string key in this._params.Keys) {
        //        string val = record.GetValue(key);
        //        this.AddParameter(key, val);
        //    }
        //}
        //#endregion

        //#region -------- PUBLIC - AddParameter/AddOutputParameter --------
        //public void AddParameter(string key, object val) {
        //    if (String.IsNullOrEmpty(key))
        //        throw new ArgumentNullException("The key parameter is null/empty!");

        //    string id = key.Trim().ToLower();
        //    if (this._params.ContainsKey(id))
        //        throw new System.Data.DataException("A parameter with that name has already been added to the query! Key: \"" + key + "\"");
        //    if (val == null)
        //        val = DBNull.Value;
        //    var p = new QueryParameter(key, val);
        //    this._params.Add(id, p);
        //}
        //public void AddOutputParameter(string key) {
        //    this.AddOutputParameter(key, DbType.Object);
        //}

        ////public void AddOutputParameter(string key, QueryParameterTypes type) {
        ////    Asserter.CheckNull(key, "key");
        ////    switch(type) {
        ////        case(QueryParameterTypes.Number):
        ////            this.AddOutputParameter(key,DbType.Int32);
        ////            break;
        ////        case(QueryParameterTypes.String):
        ////            this.AddOutputParameter(key,DbType.String);
        ////            break;
        ////        default:
        ////            throw new DatabaseException("The QueryParameterType \""+Convert.ToString(type)+"\" is not supported at this time.");
        ////    }
        ////}

        //private void AddOutputParameter(string key, System.Data.DbType type) {
        //    if (String.IsNullOrEmpty(key))
        //        throw new ArgumentNullException("The key parameter is null/empty!");
        //    string id = key.Trim().ToLower();
        //    if (this._params.ContainsKey(id))
        //        throw new System.Data.DataException("A parameter with that name has already been added to the query! Key: \"" + key + "\"");
        //    var p = new QueryParameter(key, type, true);
        //    this._params.Add(id, p);

        //    this._hasOutputParams = true;
        //}
        //#endregion


        #region -------- PUBLIC - Get --------
        public QueryParameter Get(string parameterName) {
            Contract.Requires(!String.IsNullOrEmpty(parameterName));
            return this.Parameters.Where(x => x.Name.Equals(parameterName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }
        #endregion

        #region -------- PUBLIC - Set/SetOut --------
        public Query Set(string parameterName) {
            return this.Set(new QueryParameter(parameterName));
        }
        public Query Set(string parameterName, ParameterDirection direction) {
            return this.Set(new QueryParameter(parameterName, direction));
        }
        public Query Set(string parameterName, object value) {
            return this.Set(new QueryParameter(parameterName, value));
        }
        public Query Set(string parameterName, object value, ParameterDirection direction) {
            return this.Set(new QueryParameter(parameterName, value, direction));
        }

        // /// <summary>
        // /// Set the parameter name/value, this override is necessary to keep the runtime from mistakenly
        // /// assuming that an integer value of 1/2/3/6 is a ParameterDirection type cast value. Grrrrrr.
        // /// </summary>
        //public Query Set(string parameterName, int value) {
        //    return this.Set(new QueryParameter(parameterName, value));
        //}

        public Query Set(QueryParameter parameter) {
            Contract.Requires(parameter != null);
            Contract.Requires(!this.Contains(parameter.Name));

            var existing = this.Get(parameter.Name);
            if (existing != null) {//duplicate, overwrite
                this.Parameters.Remove(existing);
            }

            this.Parameters.Add(parameter);
            return this;
        }

        public Query SetOutput(string parameterName) {
            return this.Set(parameterName, ParameterDirection.Output);
        }
        public Query SetOutput(string parameterName, object value) {
            return this.Set(new QueryParameter(parameterName, value, ParameterDirection.Output));
        }
        public Query SetOutput(QueryParameter parameter) {
            parameter.Direction = ParameterDirection.Output;
            return this.Set(parameter);
        }
        #endregion

        #region -------- PUBLIC - Replace --------
        public void Replace(string key, string val) {
            this._sql = this._sql.Replace(key, val);
        }
        #endregion

        #region -------- PUBLIC - CopyParameters --------
        public void CopyParameters(IDictionary data, params string[] filter) {
            foreach (string key in data.Keys) {
                if (filter != null) {
                    if ((from f in filter where f == key select f).FirstOrDefault() != null)
                        continue;
                }

                object val = data[key];
                if (val == null)
                    val = DBNull.Value;
                this.Set(key, val);
            }
        }
        #endregion

        #region -------- PUBLIC - Clone --------
        public Query Clone() {
            return new Query(this._sql);
        }
        #endregion


        #region -------- PUBLIC - Contains --------
        public bool Contains(string parameterName) {
            Contract.Requires(!String.IsNullOrEmpty(parameterName));
            return this.Parameters.Exists(x => x.Name.Equals(parameterName, StringComparison.OrdinalIgnoreCase));
        }
        #endregion

        #region -------- PUBLIC - AppendSql --------
        public void AppendSql(string sql) {
            this._sql += "\r" + sql;
        }
        #endregion

        #region -------- PUBLIC - ToSqlString --------
        public string ToSqlString() {
            System.Text.StringBuilder buffer = new System.Text.StringBuilder();
            buffer.Append(this._sql);

            var parameters = this.Parameters;
            foreach (QueryParameter param in parameters) {
                string val = Convert.ToString(param.Value);
                if (param.Value == DBNull.Value) {
                    val = "null";
                } else if (param.Value is string) {
                    //half-assed sql injection filter... shouldn't use this method unless 
                    //you know exactly what kind of input data is being provided
                    val = val.Replace("'", "");
                    val = "'" + val + "'";
                }

                buffer.Replace(param.Name, val);
            }

            return buffer.ToString();
        }
        #endregion

        #region -------- PUBLIC OVERRIDE - ToString --------
        public override string ToString() {
            return this._sql;
        }
        #endregion

        #region -------- PROPERTIES --------
        public string Sql {
            get { return this._sql; }
            set {
                if (String.IsNullOrEmpty(value))
                    throw new System.Data.DataException("The specified sql query is not valid! query: \"" + ((value == null || value.Trim().Length == 0) ? "null/empty" : value) + "\""); //DataTool.GetStringValue(value) + "\"");
                this._sql = value;

                if (this._sql.Trim().IndexOf(" ") < 0)///NO SPACES IN QUERY = STORED PROCEDURE CALL
                    this._type = CommandType.StoredProcedure;
                else
                    this._type = CommandType.Text;
            }
        }

        public List<QueryParameter> Parameters {
            get;
            private set;
        }


        //public Dictionary<string, QueryParameter> Parameters {
        //    get { return this._params; }
        //}
        public CommandType Type {
            get { return this._type; }
        }

        //public bool IsSelect { 
        //    get { return (this._type == CommandType.Text && this._sql.Trim().ToLower().StartsWith("select")) ? true : false; } 
        //}


        internal bool HasOutputParams {
            get { return this._hasOutputParams; }
        }

        public string ID {
            get;
            internal set;
        }


        internal Database DB {
            get {
                var id = this._dbID;
                if (id == null)
                    throw new Exception("The database instance is not linked to this query!");
                return Database.Get(id);
            }
        }
        #endregion

        // #region -------- PUBLIC - Dispose --------
        // private bool _disposed = false;
        // public void Dispose() {
        //     if(this._disposed) return;
        //     if(this._params != null)
        //         this._params.Clear();
        //     this._params = null;
        //     this._sql = null;
        //     GC.SuppressFinalize(this);
        // }
        //~Query() { this.Dispose(); }
        // #endregion
    }


    //#region -------- STRUCT - Param --------
    //[Serializable]
    //public struct Param {
    //    #region -------- VARIABLES & CONSTRUCTOR(S) --------
    //    private string _key;
    //    private object _val;
    //    private bool _isOutput;
    //    internal Param(string key, object val) : this(key, val, false) { }
    //    internal Param(string key, object val, bool isOutput) {
    //        this._key = key;
    //        this._val = val;
    //        this._isOutput = isOutput;
    //    }
    //    #endregion


    //    #region -------- PROPERTIES --------
    //    public string Key { get { return this._key; } }
    //    public object Value { get { return this._val; } }
    //    public bool IsOutput { get { return this._isOutput; } }
    //    #endregion
    //#endregion
    //}
}

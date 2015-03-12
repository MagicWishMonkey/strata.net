using System;
using System.Linq;
using System.Data;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Strata.Util;

namespace Strata.DB {
    [Serializable]
    public class QueryInfo {
        private string _hash = null;
        internal string sql;
        internal CommandType type;
        internal List<QueryToken> tokens;
        public QueryInfo(string sql, List<QueryToken> tokens = null) {
            this.sql = sql;
            this.tokens = tokens;
            if (sql.Trim().IndexOf(" ") < 0)///NO SPACES IN QUERY = STORED PROCEDURE CALL
                this.type = CommandType.StoredProcedure;
            else
                this.type = CommandType.Text;
        }

        internal string hash {
            get {
                var hash = this._hash;
                if (hash == null) {
                    hash = Strata.Util.Toolkit.Hash(this.sql.Trim().ToLower());
                    this._hash = hash;
                }
                return hash;
            }
            set {
                this._hash = value;
            }
        }
    }

    [Serializable]
    public class Query {
        #region -------- VARIABLES & CONSTRUCTOR(S) --------
        
        private int _dbID;
        private QueryInfo _info = null;
        private string _sql;
        private bool _hasOutputParams = false;
        private Converter<object, object> _adapter = null;
        

        public Query(string sql) {
            this._sql = FilterSqlComments(sql);
        }
        public Query(string sql, QueryInfo info) {
            this._sql = sql;
            this._info = info;
        }
        public Query(string sql, List<QueryToken> tokens = null) {
            this._sql = sql;
            this._info = new QueryInfo(sql, tokens);
        }
        #endregion


        public Query AttachAdapter(Converter<object, object> fn) {
            this._adapter = fn;
            return this;
        }


        internal QueryInfo Info {
            get {
                var info = this._info;
                if (info != null)
                    return info;
                info = new QueryInfo(this._sql);
                this._info = info;
                return info;
            }
        }


        internal void Link(int dbID) {
            this._dbID = dbID;
        }

        public DataTable SelectTable() {
            return this.DB.SelectTable(this);
        }

        public T Record<T>() {
            var records = this.Records<T>();
            if (records.Count == 0)
                return default(T);
            return records[0];
        }

        public List<T> Records<T>() {
            System.Diagnostics.Debug.Assert((this._adapter != null), "The adapter is not defined!");

            var lst = this.DB.Select(this);
            var cpy = (from o in lst select (T)this._adapter(o)).ToList();
            return cpy;
        }

        public List<Dictionary<string, dynamic>> Select() {
            return this.DB.Select(this);
        }

        public List<dynamic> Scalars() {
            return this.DB.Scalars(this);
        }

        public dynamic Scalar() {
            return this.DB.Scalar(this);
        }

        public int Insert() {
            return this.DB.Insert(this);
        }

        public int Update() {
            return this.DB.Update(this);
        }



        #region -------- PUBLIC - Clone --------
        public Query Clone() {
            return new Query(this._sql, this.Info);
        }
        #endregion

        #region -------- PRIVATE STATIC - FilterSqlComments --------
        private static string FilterSqlComments(string sql) {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"--.+$", System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.Multiline);
            sql = regex.Replace(sql, " ");
            return sql;
        }
        #endregion

        #region -------- PUBLIC STATIC - Create --------
        public static Query Create(string sql, params KeyValuePair<string, object>[] parameters) {
            return (parameters == null) ? new Query(sql) : new Query(sql).Bind(parameters);
        }
        #endregion

        public Query Param(string name, object val) {
            this.Set(name, val);
            return this;
        }

        public Query Output(string name) {
            this.Set(name, ParameterDirection.Output);
            return this;
        }

        public Query Bind(object o) {
            var tokens = this.Tokens;
            if (tokens != null && tokens.Count > 0) {
                var wrap = new Reflect(o);
                for (var i = 0; i < tokens.Count; i++) {
                    var token = tokens[i];
                    var name = token.name;
                    var val = wrap[name];
                    this.Set(name, val);
                }
                return this;
            }


            var raw = Strata.Util.Reflect.Decompose(o);
            var keys = raw.Keys;
            //var lst = new List<KeyValuePair<string, object>>();
            foreach (var key in keys) {
                var val = raw[key];
                this.Set(key, val);
            }
            return this;
        }

        public Query Bind(KeyValuePair<string, object>[] parameters) {
            foreach (KeyValuePair<string, object> pair in parameters) {
                this.Set(pair.Key, pair.Value);
            }
            return this;
        }

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
            var parameters = this.Parameters;
            if (parameters == null)
                return null;
            return parameters.Where(x => x.Name.Equals(parameterName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }
        #endregion

        #region -------- PUBLIC - Set/SetOut --------
        public Query Set(string parameterName) {
            return this.Set(new QueryParameter(parameterName));
        }
        public Query Set(string parameterName, ParameterDirection direction) {
            if (direction == ParameterDirection.Output)
                this._hasOutputParams = true;
            return this.Set(new QueryParameter(parameterName, direction));
        }
        public Query Set(string parameterName, object value) {
            return this.Set(new QueryParameter(parameterName, value));
        }
        public Query Set(string parameterName, object value, ParameterDirection direction) {
            if (direction == ParameterDirection.Output)
                this._hasOutputParams = true;
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
            var parameters = this.Parameters;
            if (parameters != null) {
                var existing = this.Get(parameter.Name);
                if (existing != null) {//duplicate, overwrite
                    parameters.Remove(existing);
                }
            } else {
                parameters = new List<QueryParameter>();
                this.Parameters = parameters;
            }
            

            if (parameter.Direction == ParameterDirection.Output)
                this._hasOutputParams = true;

            parameters.Add(parameter);
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

        public Query Where(string column, List<string> values) {
            var buffer = new StringBuilder();
            buffer.Append("WHERE");
            for (var i = 0; i < values.Count; i++) {
                if (i > 0) {
                    buffer.Append(" OR ");
                }
                var value = values[i];
                buffer.Append(" " + column + "='" + value + "'");
            }
            buffer.Append(";");
            return this.Concat(buffer.ToString());
        }

        public Query Where(string column, List<int> values) {
            var buffer = new StringBuilder();
            buffer.Append("WHERE");
            for (var i = 0; i < values.Count; i++) {
                if (i > 0) {
                    buffer.Append(" OR ");
                }
                var value = values[i];
                buffer.Append(" " + column + "=" + value.ToString() + "");
            }
            buffer.Append(";");
            return this.Concat(buffer.ToString());
        }

        public Query Concat(string clause) {
            var sql = this._sql.TrimEnd();
            if (sql.EndsWith(";"))
                sql = sql.Substring(0, sql.Length - 1);

            var buffer = new StringBuilder();
            buffer.Append(sql);
            if (sql.ToLower().IndexOf(" where ") == -1){// && clause.ToLower().IndexOf(" where ") == -1) {
                var lc = clause.ToLower();
                if (lc.StartsWith("where") || lc.IndexOf(" where ") > -1) {
                    if (lc.StartsWith("where"))
                        buffer.Append(" ");
                    buffer.Append(clause);
                } else {
                    buffer.Append(" WHERE " + clause);
                }
            } else {
                buffer.Append(" AND (" + clause);
            }
            clause = clause.TrimEnd();
            if (!clause.EndsWith(";"))
                buffer.Append(";");

            this._sql = buffer.ToString();
            return this;
        }




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
                this._info = null;
                //if (this._sql.Trim().IndexOf(" ") < 0)///NO SPACES IN QUERY = STORED PROCEDURE CALL
                //    this._type = CommandType.StoredProcedure;
                //else
                //    this._type = CommandType.Text;
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
            get { return this.Info.type; }
        }

        //public bool IsSelect { 
        //    get { return (this._type == CommandType.Text && this._sql.Trim().ToLower().StartsWith("select")) ? true : false; } 
        //}

        public List<QueryToken> Tokens {
            get { return this.Info.tokens; }
        }

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



        //    ix = 0
        //    for c in sql:
        //        ix = (ix + 1)
        //        if c == ' ' or c == ',' or c == ';' or c == ')' or c == '\'' or c == '\r' or c == '\n':
        //            if capture:
        //                capture = False
        //                tokenEnd = ix
        //                tokens.append(Token(tokenBuffer, start_ix=tokenBegin, end_ix=tokenEnd))
        //                tokenBuffer = ""
        //                tokenBegin = 0
        //                tokenEnd = 0
        //            buffer.append(c)
        //        else:
        //            if capture:
        //                tokenBuffer += c
        //            else:
        //                if c == '@':
        //                    capture = True
        //                    tokenBuffer = ""
        //                    tokenBegin = ix
        //                    buffer.append("?")
        //                else:
        //                    buffer.append(c)

        //        prev = c

        //    if tokenBegin and len(tokenBuffer) > 0:
        //        tokens.append(Token(tokenBuffer, start_ix=tokenBegin, end_ix=ix))
        //        tokenBuffer = None

        //    sql = ''.join(buffer)

        //    try:
        //        buffer = []
        //        ix = 0
        //        for c in sql:
        //            if c == '?':
        //                buffer.append(':%s' % tokens[ix].name)
        //                ix = (ix + 1)
        //            else:
        //                buffer.append(c)

        //        sql = ''.join(buffer)
        //        query = Query(sql, params=params)
        //        query.tokens = tokens
        //    except Exception, ex:
        //        raise ex
        //    return query
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


    public class QueryFactory {
        public static Query Parse(string sql) {
            if (sql.IndexOf("@") == -1)
                return new Query(sql);

            var buffer = new StringBuilder();
            var ignore = false;
            var capture = false;
            List<QueryToken> tokens = new List<QueryToken>();

            //buffer = []
            //tokens = []
            //tokenBegin = 0
            //tokenEnd = 0
            //tokenIndex = {}
            //tokenBuffer = None

            int ix = 0;
            int startIX = 0;
            //int endIX = 0;
            StringBuilder token = null;
            //char prev = ' ';
            //char c = ' ';

            int max = sql.Length;
            for (ix = 0; ix < max; ix++) {
                char c = sql[ix];
                if (c == '\r' || c == '\n') {
                    if (ignore)
                        ignore = false;
                } else if (!ignore && c == '-' && ix < (max - 1) && sql[ix + 1] == '-') {
                    ix++;
                    ignore = true;
                }

                if (ignore)
                    continue;


                if(c == ' ' || c == ',' || c == ';' || c == ')' || c == '\'' || c == '\r' || c == '\n'){
                    if(capture){
                        capture = false;
                        tokens.Add(new QueryToken(token.ToString(), startIX, (ix + 1)));
                        token = null;
                        startIX = 0;
                    }
                    buffer.Append(c);
                }else {
                    if(capture)
                        token.Append(c);
                    else{
                        if(c == '@'){
                            capture = true;
                            startIX = (ix + 1);
                            token = new StringBuilder();
                            buffer.Append("?");
                        }else{
                            buffer.Append(c);
                        }
                    }
                }
            }

            if(token != null){
                tokens.Add(new QueryToken(token.ToString(), startIX, ix));
            }

            sql = buffer.ToString();
            var query = new Query(sql);
            var hash = Strata.Crypto.Hash.MD5(sql);
            query.Info.hash = hash;
            if (tokens.Count > 0)
                query.Info.tokens = tokens;
            return query;
        }
    }


    public class QueryToken {
        public string name;
        public int startIX;
        public int endIX;
        public QueryToken(string name, int startIX = 0, int endIX = 0) {
            this.name = name;
            this.startIX = startIX;
            this.endIX = endIX;
        }

        public int Length {
            get { return (this.endIX - this.startIX);}
        }


        public override string ToString() {
            return "Token -> " + this.name;
        }
    }
//class Token(object):
//    __slots__ = [
//        "name",
//        "start_ix",
//        "end_ix"
//    ]
//    def __init__(self, name, start_ix=0, end_ix=0):
//        self.name = name
//        self.start_ix = start_ix
//        self.end_ix = end_ix

//    @property
//    def length(self):
//        return (self.end_ix - self.start_ix)

//    def __repr__(self):
//        return "Token-> %s" % self.name
}

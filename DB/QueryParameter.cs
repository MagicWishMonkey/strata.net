using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
namespace Strata.DB {
    [Serializable]
    public class QueryParameter {
        #region -------- CONSTRUCTOR/VARIABLES --------
        public QueryParameter(string name) : this(name, null, ParameterDirection.Input) { }
        public QueryParameter(string name, ParameterDirection direction) : this(name, null, direction) { }
        public QueryParameter(string name, object value) : this(name, value, ParameterDirection.Input) { }
        public QueryParameter(string name, object value, ParameterDirection direction) : this(name, value, direction, DbType.Object) { }
        public QueryParameter(string name, DbType type) : this(name, null, ParameterDirection.Input, (int)type) { }
        public QueryParameter(string name, ParameterDirection direction, DbType type) : this(name, null, direction, (int)type) { }
        public QueryParameter(string name, object value, DbType type) : this(name, value, ParameterDirection.Input, (int)type) { }
        public QueryParameter(string name, object value, ParameterDirection direction, DbType type) : this(name, value, direction, (int)type) { }
        public QueryParameter(string name, int typeFlag) : this(name, null, ParameterDirection.Input, typeFlag) { }
        public QueryParameter(string name, ParameterDirection direction, int typeFlag) : this(name, null, direction, typeFlag) { }
        public QueryParameter(string name, object value, int typeFlag) : this(name, value, ParameterDirection.Input, typeFlag) { }
        public QueryParameter(string name, object value, ParameterDirection direction, int typeFlag) {
            Contract.Requires(!String.IsNullOrEmpty(name));
            this.Name = name;
            this.Value = value;
            this.Direction = direction;
            this.SetTypeFlag(typeFlag);
        }
        #endregion

        #region -------- PUBLIC - SetTypeFlag --------
        public QueryParameter SetTypeFlag(DbType type) {
            return this.SetTypeFlag((int)type);
        }
        public QueryParameter SetTypeFlag(int typeFlag) {
            Contract.Requires(typeFlag > -1);
            this.TypeFlag = typeFlag;
            return this;
        }
        #endregion


        #region -------- PUBLIC OVERRIDE - ToString --------
        public override string ToString() {
            if (this.Value == null)
                return this.Name;
            return this.Name + "->" + this.Value.ToString();
        }
        #endregion

        #region -------- PROPERTIES --------
        public string Name {
            get;
            private set;
        }

        public object Value {
            get;
            set;
        }

        public ParameterDirection Direction {
            get;
            set;
        }

        public int TypeFlag {
            get;
            private set;
        }

        public DbType DbType {
            get {
                var flag = this.TypeFlag;
                if (flag >= 0 && flag <= 27)
                    return (DbType)flag;
                return System.Data.DbType.Object;
            }
        }
        #endregion
    }
}

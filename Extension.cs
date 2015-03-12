using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strata.DB;
using Strata.Util;

namespace Strata {
    public partial class Extension : Strata.Context, IDisposable{
        public Query Query(string sql) {
            return Context.Database.Query(sql);
        }

        public Query Query(string sql, object obj) {
            return Context.Database.Query(sql, obj);
        }

        public List<T> Scalars<T>(string sql) {
            return Context.Database.Scalars<T>(sql);
        }

        #region -------- DISPOSE/CLEANUP --------
        private bool _disposed = false;
        public void Cleanup() {}
        public void Dispose() {
            if (this._disposed)
                return;
            this._disposed = true;
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}

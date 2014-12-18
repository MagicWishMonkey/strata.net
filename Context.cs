using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Security;
using System.Security.Principal;
using System.Runtime.Remoting.Contexts;
using Strata.Containers;
using Strata.DB;

namespace Strata {
    public partial class Context {
        #region -------- CONSTRUCTOR/VARIABLES --------
        [ThreadStatic]
        private static Core _core;

        //private User _user;
        //public Context(User user) {
        //    this._user = user;
        //}
        #endregion



        public static Database Database {
            get { return Context.Core.Database; }
        }

        //public Database Database(string name) {
        //    this.Context.Core.Databases[]
        //}


        public static User User {
            get { return User.Current; }
        }

        public static Core Core {
            get {
                var core = Context._core;
                if (core == null) {
                    core = new Core();
                    Context._core = core;
                }
                return core;
            }
        }

        //#region -------- PROPERTIES --------
        //public UserAccount User {
        //    get {
        //        return this._user;
        //    }
        //}
        //#endregion
    }
}
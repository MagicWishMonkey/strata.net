using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strata.DB;
using Strata.Util;
using Strata.Containers;
using Strata.Context;

namespace Strata {
    public class Core {
        private static Core _instance;

        private UserAccount _user;
        private Database[] _databases;
        internal Core(UserAccount user) {
            this._user = user;
        }

        public void Initialize(string workspace, string config = null) {
            if (Core._instance != null) {
                Console.WriteLine("The core instance has already been initialized!");
                return;
            }
        }


        public Database Database {
            get {
                if (this._databases == null) {
                    return null;
                }
                return this._databases[0];
            }
        }

        public Database[] Databases {
            get { return this._databases; }
        }
    }
}

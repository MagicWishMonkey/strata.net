//using System;
//using System.Linq;
//using System.Collections.Generic;
//using System.Security;
//using System.Security.Principal;
//using Strata.Containers;

//namespace Strata.Context {
//    [Serializable]
//    public class UserAccount : IPrincipal, IIdentity {
//        #region -------- CONSTRUCTOR/VARIABLES --------
//        private List<string> _permissions = new List<string>();
//        private HashSet<string> _permissionLookup = new HashSet<string>();
//        protected bool _isSystemAccount = false;
//        public UserAccount() {
//            this.Info = new Person {
//                Username = Current.Username
//            };
//        }

//        public UserAccount(string username, params string[] permissions) {
//            if (permissions == null)
//                permissions = new string[0];
//            this.Info = new Person {
//                Username = username,
//                Permissions = permissions
//            };

//            this.Bind(permissions);
//        }

//        public UserAccount(string username, string firstName, string lastName, params string[] permissions) {
//            if (permissions == null)
//                permissions = new string[0];

//            this.Info = new Person {
//                Username = username,
//                FirstName = firstName,
//                LastName = lastName,
//                Permissions = permissions
//            };

//            this.Bind(permissions);
//        }

//        public UserAccount(Person person) {
//            this.Info = person;
//            this.Bind(person.Permissions);
//        }
//        #endregion

//        private void Bind(string[] permissions) {
//            if (permissions.Length == 0)
//                return;

//            for (int i = 0; i < permissions.Length; i++) {
//                var permission = permissions[i];
//                var key = Squish(permission);
//                if (this._permissionLookup.Contains(key))
//                    continue;
//                this._permissionLookup.Add(key);
//                this._permissions.Add(permission);
//            }
//        }

//        #region -------- PUBLIC - IsInRole --------
//        public bool IsInRole(string permission) {
//            permission = Squish(permission);
//            return this._permissionLookup.Contains(permission);
//        }
//        #endregion


//        #region -------- PRIVATE STATIC HELPERS --------
//        private static string Squish(string txt) {
//            if (txt == null)
//                return "";
//            txt = txt.Replace(" ", "").Replace("\\", "/").ToLower();
//            return txt;
//        }
//        #endregion


//        #region -------- PUBLIC OVERRIDE - ToString --------
//        public override string ToString() {
//            if (String.IsNullOrEmpty(this.Username))
//                return this.GetType().Name;
//            return this.GetType().Name + " -> " + this.Username;
//        }
//        #endregion


//        #region -------- PROPERTIES --------
//        internal Person Info {
//            get;
//            private set;
//        }

//        public string Username {
//            get { return this.Info.Username; }
//        }

//        public string FirstName {
//            get { return this.Info.FirstName; }
//        }

//        public string LastName {
//            get { return this.Info.LastName; }
//        }

//        public string FullName {
//            get {
//                var fullName = "";
//                if (!String.IsNullOrEmpty(this.FirstName))
//                    fullName = this.FirstName;

//                if (!String.IsNullOrEmpty(this.LastName)) {
//                    if (!String.IsNullOrEmpty(this.FirstName))
//                        fullName += " ";
//                    fullName += this.LastName;
//                }
//                return fullName;
//            }
//        }

//        public string[] Permissions {
//            get { return this.Info.Permissions; }
//        }

//        public string AuthenticationType {
//            get { return "DEFAULT"; }
//        }

//        public bool IsAuthenticated {
//            get { return (String.IsNullOrEmpty(this.Username)) ? false : true; }
//        }

//        public string Name {
//            get { return this.Username; }
//        }

//        public IIdentity Identity {
//            get { return this; }
//        }

//        public bool IsSystemAccount {
//            get { return this._isSystemAccount; }
//        }
//        #endregion
//    }

//}

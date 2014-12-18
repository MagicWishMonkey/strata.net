using System;
using System.Linq;
using System.Collections.Generic;
using System.Security;
using System.Security.Principal;

namespace Strata.Containers {
    [Serializable]
    public class User : Person, IPrincipal, IIdentity {
        private delegate User Getter();
        private delegate void Setter(User account);
        private static Getter _get;
        private static Setter _set;

        #region -------- CONSTRUCTOR/VARIABLES --------
        private string[] _permissions = null;
        private HashSet<string> _permissionLookup = new HashSet<string>();
        protected bool _isSystemAccount = false;
        public User() : base(ExtractUsername()) { }

        //    this.Info = new Person {
        //        Username = Current.Username
        //    };
        //}

        public User(string username, params string[] permissions) : base(username, permissions){
            this.Bind(this.Permissions);
        }

        public User(string username, string firstName, string lastName, params string[] permissions) : base(username, firstName, lastName, permissions){
            this.Bind(this.Permissions);
        }

        //public User(Person person) {
        //    this.Info = person;
        //    this.Bind(person.Permissions);
        //}
        #endregion

        private void Bind(string[] permissions) {
            if (permissions == null || permissions.Length == 0)
                return;

            var permList = (this._permissions == null) ? new List<string>() : this._permissions.ToList();
            for (int i = 0; i < permissions.Length; i++) {
                var permission = permissions[i];
                var key = Squish(permission);
                if (this._permissionLookup.Contains(key))
                    continue;
                this._permissionLookup.Add(key);
                permList.Add(permission);
            }
            this._permissions = permList.ToArray();
        }

        #region -------- PUBLIC - IsInRole --------
        public bool IsInRole(string permission) {
            permission = Squish(permission);
            return this._permissionLookup.Contains(permission);
        }
        #endregion


        public void LinkToThread() {
            //_set.Invoke(this);
            System.Threading.Thread.CurrentPrincipal = this;
        }

        #region -------- PRIVATE - ExtractPrincipal/ExtractUsername --------
        private static User ExtractPrincipal(bool ignoreErrors = false) {
            IPrincipal principal = null;
            try {
                principal = System.Threading.Thread.CurrentPrincipal;
            } catch (Exception ex) {
                if (!ignoreErrors)
                    throw new Exception("The thread principal could not be accessed.", ex);
                return null;
            }

            User user = null;
            var identity = (principal != null) ? principal.Identity : null;
            if (identity != null && (identity is User)) {
                user = (User)identity;
            }
            if(user != null)
                return user;

            try {
                if (_get != null) {
                    user = _get.Invoke();
                    System.Threading.Thread.CurrentPrincipal = user;
                }
                if(user != null)
                    return user;
            } catch (Exception ex) {
                if (!ignoreErrors) {
                    if (ex is System.Reflection.TargetInvocationException)
                        ex = ex.InnerException;

                    throw new Exception("UserContext.ExtractUserAccount Error-> " + ex.Message, ex);
                }
                //Tracer.Write("UserContext.ExtractUserAccount Error-> " + ex.Message);
            }

            if (user == null)
                user = new SystemUser(WindowsIdentity.GetCurrent().Name);
            return user;
        }

        private static string ExtractUsername() {
            User user = ExtractPrincipal();
            if (user != null) {
                return user.Username;
            }
            return WindowsIdentity.GetCurrent().Name;
        }
        #endregion


        #region -------- PRIVATE STATIC HELPERS --------
        private static string Squish(string txt) {
            if (txt == null)
                return "";
            txt = txt.Replace(" ", "").Replace("\\", "/").ToLower();
            return txt;
        }
        #endregion


        #region -------- PUBLIC OVERRIDE - ToString --------
        public override string ToString() {
            if (String.IsNullOrEmpty(this.Username))
                return this.GetType().Name;
            return this.GetType().Name + " -> " + this.Username;
        }
        #endregion


        #region -------- PROPERTIES --------
        public static User Current {
            get { return ExtractPrincipal(); }
        }

        //internal Person Info {
        //    get;
        //    private set;
        //}

        //public string Username {
        //    get { return this._username; }
        //}

        //public string FirstName {
        //    get { return this.Info.FirstName; }
        //}

        //public string LastName {
        //    get { return this.Info.LastName; }
        //}

        //public string FullName {
        //    get {
        //        var fullName = "";
        //        if (!String.IsNullOrEmpty(this.FirstName))
        //            fullName = this.FirstName;

        //        if (!String.IsNullOrEmpty(this.LastName)) {
        //            if (!String.IsNullOrEmpty(this.FirstName))
        //                fullName += " ";
        //            fullName += this.LastName;
        //        }
        //        return fullName;
        //    }
        //}

        public string[] Permissions {
            get { return this._permissions; }
        }

        public string AuthenticationType {
            get { return "DEFAULT"; }
        }

        public bool IsAuthenticated {
            get { return (String.IsNullOrEmpty(this.Username)) ? false : true; }
        }

        public string Name {
            get { return this.Username; }
        }

        public IIdentity Identity {
            get { return this; }
        }

        public bool IsSystemAccount {
            get { return this._isSystemAccount; }
        }
        #endregion
    }


    [Serializable]
    public class SystemUser : User {
        #region -------- CONSTRUCTOR/VARIABLES --------
        public SystemUser(string username) : base(username){
            this._isSystemAccount = true;
        }
        #endregion
    }

}

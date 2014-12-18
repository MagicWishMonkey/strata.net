//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Diagnostics.Contracts;
//using System.Threading;
//using System.Security;
//using System.Security.Principal;
//using System.Runtime.Remoting.Contexts;
//using Strata;

//namespace Strata.Context {
//    public sealed class Current {
//        public delegate UserAccount Getter();
//        public delegate void Setter(UserAccount account);

//        #region -------- CONSTRUCTOR/VARIABLES --------
//        private static Getter _get;
//        private static Setter _set;
//        private static SystemAccount _defaultAccount;
//        static Current() {
//            _defaultAccount = new SystemAccount();
//        }
//        #endregion


//        #region -------- PRIVATE - ExtractUserAccount --------
//        private static UserAccount ExtractUserAccount(bool ignoreErrors = false) {
//            IPrincipal principal = null;
//            try {
//                principal = System.Threading.Thread.CurrentPrincipal;
//            } catch (Exception ex) {
//                if (!ignoreErrors)
//                    throw new Exception("The thread principal could not be accessed.", ex);
//                //else
//                //    Tracer.Write("The thread principal could not be accessed.");

//                return _defaultAccount;
//            }

//            UserAccount account = null;
//            var identity = (principal != null) ? principal.Identity : null;
//            if (identity != null && (identity is UserAccount)) {
//                account = (UserAccount)identity;
//            }
//            if (account != null)
//                return account;

//            try {
//                if (_get != null) {
//                    account = _get.Invoke();
//                    System.Threading.Thread.CurrentPrincipal = account;
//                }

//                if (account == null)
//                    account = _defaultAccount;
//            } catch (Exception ex) {
//                if (!ignoreErrors) {
//                    if (ex is System.Reflection.TargetInvocationException)
//                        ex = ex.InnerException;

//                    throw new Exception("UserContext.ExtractUserAccount Error-> " + ex.Message, ex);
//                }

//                //Tracer.Write("UserContext.ExtractUserAccount Error-> " + ex.Message);
//                return _defaultAccount;
//            }

//            return account;
//        }
//        #endregion

//        #region -------- PROPERTIES --------
//        public static Thread Thread {
//            get { return System.Threading.Thread.CurrentThread; }
//        }

//        public static string Username {
//            get {
//                string username = "";
//                try {
//                    var principal = System.Threading.Thread.CurrentPrincipal;
//                    var identity = (principal != null) ? principal.Identity : null;
//                    username = (identity != null) ? identity.Name : "";
//                } catch (Exception ex) {
//                    //Tracer.Write("Self.Username Error-> " + ex.Message);
//                    throw ex;
//                }
//                if (String.IsNullOrEmpty(username)) {
//                    username = WindowsIdentity.GetCurrent().Name;
//                }
//                if (!String.IsNullOrEmpty(username)) {
//                    if (username.IndexOf("\\") > -1) {
//                        username = username.Substring(username.IndexOf('\\') + 1);
//                    }
//                }
//                return username;
//            }
//        }

//        public static Core Context {
//            get {
//                return new Core(ExtractUserAccount());
//            }
//        }
//        #endregion
//    }
//}

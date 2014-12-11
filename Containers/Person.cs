using System;
using System.Linq;
using System.Collections.Generic;

namespace Strata.Containers {
    [Serializable]
    public class Person {
        #region -------- CONSTRUCTOR/VARIABLES --------
        private int _id = -1;
        private string _username = null;
        private string _firstName = null;
        private string _lastName = null;
        private string _label = null;
        private string[] _permissions = null;
        public Person() { }
        #endregion


        #region -------- PROPERTIES --------
        public int ID {
            get { return this._id; }
            set { this._id = value; }
        }

        public string Username {
            get { return this._username; }
            set { this._username = value; }
        }

        public string FirstName {
            get { return this._firstName; }
            set { this._firstName = value; }
        }

        public string LastName {
            get { return this._lastName; }
            set { this._lastName = value; }
        }

        public string[] Permissions {
            get { return this._permissions; }
            set { this._permissions = value; }
        }
        #endregion
    }
}

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
        private string[] _permissions = null;
        public Person() { }
        public Person(string username) {
            this._username = username;
        }
        public Person(string username, string[] permissions) {
            this._username = username;

            if (permissions == null)
                permissions = new string[0];
            this._permissions = permissions;
        }

        public Person(string username, string first, string last) {
            this._username = username;
            this._firstName = first;
            this._lastName = last;
        }

        public Person(string username, string first, string last, string[] permissions) {
            this._username = username;
            this._firstName = first;
            this._lastName = last;

            if (permissions == null)
                permissions = new string[0];
            this._permissions = permissions;
        }

        public Person(int id) {
            this.ID = id;
        }
        public Person(int id, string username) {
            this.ID = id;
            this._username = username;
        }
        public Person(int id, string username, string[] permissions) {
            this.ID = id;
            this._username = username;

            if (permissions == null)
                permissions = new string[0];
            this._permissions = permissions;
        }

        public Person(int id, string username, string first, string last) {
            this.ID = id;
            this._username = username;
            this._firstName = first;
            this._lastName = last;
        }

        public Person(int id, string username, string first, string last, string[] permissions) {
            this.ID = id;
            this._username = username;
            this._firstName = first;
            this._lastName = last;

            if (permissions == null)
                permissions = new string[0];
            this._permissions = permissions;
        }
        #endregion


        #region -------- PROPERTIES --------

        public string Label {
            get {
                if (this._firstName != null)
                    if (this._lastName != null)
                        return this._firstName + " " + this._lastName;
                    else
                        return this._firstName;
                return null;
            }
        }

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

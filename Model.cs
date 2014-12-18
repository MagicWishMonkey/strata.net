using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strata.Util;

namespace Strata {
    public class Model {
        private static Dictionary<string, string[]> _modelFields = new Dictionary<string, string[]>();
        private string _type = null;
        public Model() {
            this.id = -1;
        }

        public Repository Repository {
            get {
                return new Repository();
            }
        }

        public void Save() {
            this.Repository.Save(this);
        }

        public static Model Construct(Type modelType, Dictionary<string, dynamic> obj) {
            var model = (Model)Activator.CreateInstance(modelType);
            model.Bind(obj);
            return model;
        }

        public Model Bind(Dictionary<string, dynamic> obj) {
            var reflector = new Reflect(this);
            foreach (var key in obj.Keys) {
                var val = obj[key];
                reflector.TrySet(key, val);
            }
            return this;
        }

        public Dictionary<string, dynamic> Objectify() {
            var o = new Dictionary<string, dynamic>();
            var fields = Model.Fields(this);
            var wrap = new Reflect(this);
            for (var i = 0; i < fields.Length; i++) {
                var field = fields[i];
                var value = wrap[field];
                o[field] = value;
            }
            return o;
        }

        private static string[] Fields(Model model) {
            var type = model.GetType().FullName;
            //var type = model.Type;
            try {
                return _modelFields[type];
            } catch (Exception ex) {
                var properties = model.GetType().GetProperties();//System.Reflection.BindingFlags.Instance);
                var fields = new List<string>();
                foreach (var property in properties) {
                    var name = property.Name;
                    if (name == "Repository" || name == "Type")
                        continue;

                    fields.Add(name);
                }
                _modelFields[type] = fields.ToArray();
                return fields.ToArray();
            }
        }

        public int id {
            get;
            set;
        }


        public string Type {
            get {
                var type = this._type;
                if (type != null)
                    return type;
                var t = this.GetType();
                type = t.Name;
                this._type = type;
                return type;
            }
        }


        public override string ToString() {
            var type = this.Type;
            var id = this.id;
            return type;
        }
    }
    

    public class Member : Model {
        public string label {
            get;
            set;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Strata.Util {
    public class Reflect {
        private object _instance;
        private Type _type;
        public Reflect(object instance) {
            this._instance = instance;
            this._type = instance.GetType();
        }

        public void TrySet(string propertyName, object value){
            this.Set(propertyName, value, true);
        }
        public void Set(string propertyName, object value, bool ignoreErrors = false) {
            var type = this._type;
            var instance = this._instance;
            var field = type.GetField(propertyName);
            if (field != null) {
                field.SetValue(instance, value);
                return;
            }

            var property = type.GetProperty(propertyName);
            if (property != null) {
                property.SetValue(instance, value);
                return;
            }
                
            if(ignoreErrors)
                return;
            throw new Exception("The property could not be found.");
        }

        public object Get(string propertyName) {
            var type = this._type;
            var instance = this._instance;
            var field = type.GetField(propertyName);
            if (field != null) {
                return field.GetValue(instance);
            }

            var property = type.GetProperty(propertyName);
            if (property == null)
                throw new Exception("The property could not be found.");
            return property.GetValue(instance);
        }

        public object this[string property] {
            get { return this.Get(property); }
            set { this.Set(property, value); }
        }

        //public static Dictionary<string, dynamic> Objectify(object target) {
        //    var bucket = new Dictionary<string, dynamic>();

        //    var type = target.GetType();
        //    FieldInfo[] fields = type.GetFields();
        //    foreach (var field in fields) {
        //        var name = field.Name;
        //        var value = field.GetValue(target);
        //        if (value == null) {
        //            bucket[name] = value;
        //            continue;
        //        }
        //        if (Types.IsPrimitive(value)) {
        //            if (value == null) {
        //                if (Types.IsNumeric(value)) {
        //                    continue;
        //                }
        //            }
        //            bucket[name] = value;
        //            continue;
        //        }
        //}

        public static Dictionary<string, dynamic> Decompose(object target) {
            var bucket = new Dictionary<string, dynamic>();
            var type = target.GetType();

            var attributes = new List<KeyValuePair<string, dynamic>>();
            PropertyInfo[] properties = type.GetProperties();
            foreach (var field in properties) {
                var name = field.Name;
                var value = field.GetValue(target);
                attributes.Add(new KeyValuePair<string, dynamic>(name, value));
            }

            FieldInfo[] fields = type.GetFields();
            foreach (var field in properties) {
                var name = field.Name;
                var value = field.GetValue(target);
                attributes.Add(new KeyValuePair<string, dynamic>(name, value));
            }

            foreach (var attribute in attributes) {
                var name = attribute.Key;
                var value = attribute.Value;
                if (value == null) {
                    bucket[name] = value;
                    continue;
                }
                if (Types.IsPrimitive(value)) {
                    if (value == null) {
                        if (Types.IsNumeric(value)) {
                            continue;
                        }
                    }
                    bucket[name] = value;
                    continue;
                }

                if (value is IList) {
                    var lst = (IList)value;
                    var cnt = lst.Count;
                    if (cnt > 0) {
                        var cpy = (IList)Activator.CreateInstance(lst.GetType());
                        for (int i = 0; i < cnt; i++) {
                            var e = lst[i];
                            if (Types.IsPrimitive(e)) {
                                cpy.Add(e);
                            } else {
                                cpy.Add(Reflect.Decompose(e));
                            }
                        }
                        bucket[name] = cpy;
                    } else {
                        var cpy = Activator.CreateInstance(lst.GetType());
                        bucket[name] = cpy;
                    }
                } else {
                    bucket[name] = Reflect.Decompose(value);
                }
            }
            return bucket;
        }
    }
}

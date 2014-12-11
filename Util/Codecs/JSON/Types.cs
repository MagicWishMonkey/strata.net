using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;


namespace Strata.Util.Codecs.JSON {
    internal delegate object GenericGetter(object obj);
    internal delegate void GenericSetter(object target, object value);

    internal class Getters {
        public string Name;
        public GenericGetter Getter;
        public Type propertyType;
    }

    internal class DatasetSchema {
        public List<string> Info { get; set; }
        public string Name { get; set; }
    }


    internal class SafeDictionary<TKey, TValue> {
        private readonly object _Padlock = new object();
        private readonly Dictionary<TKey, TValue> _Dictionary = new Dictionary<TKey, TValue>();


        public bool TryGetValue(TKey key, out TValue value) {
            return _Dictionary.TryGetValue(key, out value);
        }

        public TValue this[TKey key] {
            get {
                return _Dictionary[key];
            }
        }
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            return ((ICollection<KeyValuePair<TKey, TValue>>)_Dictionary).GetEnumerator();
        }

        public void Add(TKey key, TValue value) {
            lock (_Padlock) {
                if (_Dictionary.ContainsKey(key) == false)
                    _Dictionary.Add(key, value);
            }
        }
    }

    internal class Types {
        readonly SafeDictionary<Type, List<Getters>> _getterscache = new SafeDictionary<Type, List<Getters>>();
        // private static SafeDictionary<Type, string> _locals = new SafeDictionary<Type, string>();
        public Types() { }

        internal List<Getters> GetGetters(Type type) {
            List<Getters> val = null;
            if (_getterscache.TryGetValue(type, out val))
                return val;

            PropertyInfo[] props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            List<Getters> getters = new List<Getters>();
            foreach (PropertyInfo p in props) {
                if (!p.CanWrite) continue;

                object[] att = p.GetCustomAttributes(typeof(System.Xml.Serialization.XmlIgnoreAttribute), false);
                if (att != null && att.Length > 0)
                    continue;

                JSON.GenericGetter g = CreateGetMethod(p);
                if (g != null) {
                    Getters gg = new Getters();
                    gg.Name = p.Name;
                    gg.Getter = g;
                    gg.propertyType = p.PropertyType;
                    getters.Add(gg);
                }
            }

            FieldInfo[] fi = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (var f in fi) {
                object[] att = f.GetCustomAttributes(typeof(System.Xml.Serialization.XmlIgnoreAttribute), false);
                if (att != null && att.Length > 0)
                    continue;

                JSON.GenericGetter g = CreateGetField(type, f);
                if (g != null) {
                    Getters gg = new Getters();
                    gg.Name = f.Name;
                    gg.Getter = g;
                    gg.propertyType = f.FieldType;
                    getters.Add(gg);
                }
            }

            _getterscache.Add(type, getters);
            return getters;
        }

        internal static string GetTypeAssemblyName(Type t) {
            return t.AssemblyQualifiedName;

            //typename = 
            //string val = "";
            //if (_tyname.TryGetValue(t, out val))
            //    return val;
            //else {
            //    string s = t.AssemblyQualifiedName;
            //    _tyname.Add(t, s);
            //    return s;
            //}
        }
    

        private GenericGetter CreateGetMethod(PropertyInfo propertyInfo) {
            MethodInfo getMethod = propertyInfo.GetGetMethod();
            if (getMethod == null)
                return null;

            Type[] arguments = new Type[1];
            arguments[0] = typeof(object);

            DynamicMethod getter = new DynamicMethod("_", typeof(object), arguments);
            ILGenerator il = getter.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Castclass, propertyInfo.DeclaringType);
            il.EmitCall(OpCodes.Callvirt, getMethod, null);

            if (!propertyInfo.PropertyType.IsClass)
                il.Emit(OpCodes.Box, propertyInfo.PropertyType);

            il.Emit(OpCodes.Ret);

            return (GenericGetter)getter.CreateDelegate(typeof(GenericGetter));
        }

        private static GenericGetter CreateGetField(Type type, FieldInfo fieldInfo) {
            DynamicMethod dynamicGet = new DynamicMethod("_", typeof(object), new Type[] { typeof(object) }, type, true);
            ILGenerator il = dynamicGet.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, fieldInfo);
            if (fieldInfo.FieldType.IsValueType)
                il.Emit(OpCodes.Box, fieldInfo.FieldType);
            il.Emit(OpCodes.Ret);

            return (GenericGetter)dynamicGet.CreateDelegate(typeof(GenericGetter));
        }

        private static GenericSetter CreateSetField(Type type, FieldInfo fieldInfo) {
            Type[] arguments = new Type[2];
            arguments[0] = arguments[1] = typeof(object);

            DynamicMethod dynamicSet = new DynamicMethod("_", typeof(void), arguments, type, true);
            ILGenerator il = dynamicSet.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            if (fieldInfo.FieldType.IsValueType)
                il.Emit(OpCodes.Unbox_Any, fieldInfo.FieldType);
            il.Emit(OpCodes.Stfld, fieldInfo);
            il.Emit(OpCodes.Ret);

            return (GenericSetter)dynamicSet.CreateDelegate(typeof(GenericSetter));
        }
    }
}

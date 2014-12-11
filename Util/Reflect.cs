using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Strata.Util {
    public static class Reflect {
        public static Dictionary<string, dynamic> Decompose(object target) {
            var bucket = new Dictionary<string, dynamic>();
            var type = target.GetType();
            FieldInfo[] fields = type.GetFields();
            foreach (var field in fields) {
                var name = field.Name;
                var value = field.GetValue(target);
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

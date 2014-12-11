using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strata.Util {
    public static class Types {
        public static bool IsPrimitive(bool o) {
            return true;
        }
        public static bool IsPrimitive(byte o) {
            return true;
        }
        public static bool IsPrimitive(char o) {
            return true;
        }
        public static bool IsPrimitive(DBNull o) {
            return true;
        }
        public static bool IsPrimitive(DateTime o) {
            return true;
        }
        public static bool IsPrimitive(decimal o) {
            return true;
        }
        public static bool IsPrimitive(double o) {
            return true;
        }
        public static bool IsPrimitive(Int16 o) {
            return true;
        }
        public static bool IsPrimitive(Int32 o) {
            return true;
        }
        public static bool IsPrimitive(Int64 o) {
            return true;
        }
        public static bool IsPrimitive(sbyte o) {
            return true;
        }
        public static bool IsPrimitive(string o) {
            return true;
        }
        public static bool IsPrimitive(UInt16 o) {
            return true;
        }
        public static bool IsPrimitive(UInt32 o) {
            return true;
        }
        public static bool IsPrimitive(UInt64 o) {
            return true;
        }

        public static bool IsPrimitive(object o) {
            Type t = o.GetType();
            switch (Type.GetTypeCode(t)) {
                case TypeCode.Boolean:
                    return true;
                case TypeCode.Byte:
                    return true;
                case TypeCode.Char:
                    return true;
                case TypeCode.DBNull:
                    return true;
                case TypeCode.DateTime:
                    return true;
                case TypeCode.Decimal:
                    return true;
                case TypeCode.Double:
                    return true;
                case TypeCode.Empty:
                    return true;
                case TypeCode.Int16:
                    return true;
                case TypeCode.Int32:
                    return true;
                case TypeCode.Int64:
                    return true;
                case TypeCode.SByte:
                    return true;
                case TypeCode.Single:
                    return true;
                case TypeCode.String:
                    return true;
                case TypeCode.UInt16:
                    return true;
                case TypeCode.UInt32:
                    return true;
                case TypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }



        public static bool IsNumeric(object o) {
            Type t = o.GetType();
            switch (Type.GetTypeCode(t)) {
                case TypeCode.Byte:
                    return true;
                case TypeCode.Decimal:
                    return true;
                case TypeCode.Double:
                    return true;
                case TypeCode.Int16:
                    return true;
                case TypeCode.Int32:
                    return true;
                case TypeCode.Int64:
                    return true;
                case TypeCode.SByte:
                    return true;
                case TypeCode.Single:
                    return true;
                case TypeCode.UInt16:
                    return true;
                case TypeCode.UInt32:
                    return true;
                case TypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }

    }
}

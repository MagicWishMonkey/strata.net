using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strata.Util.Codecs;
using Strata.Util.Codecs.JSON;
using Strata.Util.IO;
namespace Strata.Util {
    public static class Toolkit {
        public static Func<TResult> Curry<T1, TResult>(Func<T1, TResult> function, T1 arg) {
            return () => function(arg);
        }

        public static Func<T2, TResult> Curry<T1, T2, TResult>(Func<T1, T2, TResult> function, T1 arg) {
            return (b) => function(arg, b);
        }

        public static Func<T2, T3, TResult> Curry<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> function, T1 arg) {
            return (b, c) => function(arg, b, c);
        }

        public static IEnumerable<U> Map<T, U>(this IEnumerable<T> s, Func<T, U> f) {
            var results = new List<U>();
            foreach (var item in s) {
                //yield return f(item);
                var o = f(item);
                results.Add(o);
            }
            return results;
        }

        public static string Base36(int num) {
            return Base36Formatter.Encode(num);
        }

        public static int UnBase36(string data) {
            return Base36Formatter.Decode(data);
        }


        public static string Base64(byte[] bytes) {
            return System.Convert.ToBase64String(bytes);
        }

        public static string Base64(string txt) {
            var bytes = Text.ReadBytes(txt);
            return System.Convert.ToBase64String(bytes);
        }

        public static string UnBase64(byte[] bytes) {
            return System.Text.Encoding.Unicode.GetString(bytes);
        }

        public static string UnBase64(string txt) {
            var bytes = System.Convert.FromBase64String(txt);
            return System.Text.Encoding.Unicode.GetString(bytes);
        }


        public static byte[] ToBytes(string txt) {
            var bytes = Text.ReadBytes(txt);
            return bytes;
        }


        public static string Json(object obj) {
            return JSONFormatter.Write(obj);
        }

        public static string Json(object obj, bool indent) {
            return JSONFormatter.Write(obj, indent);
        }

        public static object UnJson(string data) {
            return JSONFormatter.Read(data);
        }

        public static T UnJson<T>(string data) {
            return JSONFormatter.Read<T>(data);
        }


        public static Directory Directory(string path) {
            return new Directory(path);
        }

        public static File File(string path) {
            return new File(path);
        }

        #region -------- URI METHODS --------
        /// <summary>
        /// Create a uri instance for the specified path.
        /// </summary>
        /// <param name="path">The path to create the uri from.</param>
        /// <returns>A uri instance.</returns>
        public static Uri ToUri(string path) {
            if (String.IsNullOrEmpty(path))
                return null;
            Uri rez;
            if (Uri.TryCreate(path, UriKind.Absolute, out rez))
                return null;
            return rez;
        }

        /// <summary>
        /// Check and see if the file is a valid uri, return 
        /// true if it is, false otherwise
        /// </summary>
        /// <param name="path">The path to check</param>
        /// <returns>A boolean flag specifying whether or not the path is a valid uri</returns>
        public static bool IsValidUri(string path) {
            if (String.IsNullOrEmpty(path))
                return false;
            Uri rez;
            if (Uri.TryCreate(path, UriKind.Absolute, out rez))
                return true;
            return false;
        }

        /// <summary>
        /// Check and see if the uri points to a remote file.Return 
        /// true if it is, false otherwise
        /// </summary>
        /// <param name="uri">The uri to check</param>
        /// <returns>A boolean flag specifying whether or not the path is a valid web uri</returns>
        public static bool IsWebUri(Uri uri) {
            return IsWebUri(uri.AbsoluteUri);
        }

        /// <summary>
        /// Check and see if the path is a valid web uri, return 
        /// true if it is, false otherwise
        /// </summary>
        /// <param name="path">The path to check</param>
        /// <returns>A boolean flag specifying whether or not the path is a valid web uri</returns>
        public static bool IsWebUri(string path) {
            if (!IsValidUri(path))
                return false;
            path = path.ToLower();
            if (path.StartsWith("http:") || path.StartsWith("https:"))// || path.StartsWith("www."))
                return true;
            return false;
        }
        #endregion

        public static void Override(IDictionary<string, dynamic> a, IDictionary<string, dynamic> b) {
            foreach (KeyValuePair<string, dynamic> entry in b) {
                var key = entry.Key;
                var val = entry.Value;

                dynamic existing = null;
                if (a.TryGetValue(key, out existing)) {
                    if (val is IDictionary)
                        if (existing is IDictionary)
                            Override(existing, val);
                        else
                            a[key] = val;
                    else
                        a[key] = val;
                } else {
                    a[key] = val;
                }
            }
        }
    }
}

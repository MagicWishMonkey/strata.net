using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using Strata.Util.Codecs;
using Strata.Util.Codecs.JSON;
using Strata.Util.IO;

namespace Strata.Util {
    public static class Toolkit {
        //public delegate void FN();
        //public delegate void Callback(object o);

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

        public static Worker Dispatch(FN fn) {
            var w = new Worker(fn);
            return w.Launch();
        }


        //public static string Base64(byte[] bytes) {
        //    return System.Convert.ToBase64String(bytes);
        //}

        //public static string Base64(string txt) {
        //    var bytes = Text.ReadBytes(txt);
        //    return System.Convert.ToBase64String(bytes);
        //}

        //public static string UnBase64(byte[] bytes) {
        //    return System.Text.Encoding.Unicode.GetString(bytes);
        //}

        //public static string UnBase64(string txt) {
        //    var bytes = System.Convert.FromBase64String(txt);
        //    return System.Text.Encoding.Unicode.GetString(bytes);
        //}


        public static string Base64(string txt) {
            var bytes = Text.ReadBytes(txt);
            return Base64(bytes);
        }
        public static string Base64(byte[] bytes) {
            var txt = System.Convert.ToBase64String(bytes);
            return txt;
        }

        public static string UnBase64(byte[] bytes) {
            var txt = System.Text.Encoding.Unicode.GetString(bytes);
            return UnBase64(txt);
        }
        public static string UnBase64(string txt) {
            var bytes = System.Convert.FromBase64String(txt);
            return Text.ReadString(bytes);
        }

        public static byte[] FromBase64String(byte[] bytes) {
            var txt = Text.ReadString(bytes);
            return System.Convert.FromBase64String(txt);
        }
        public static byte[] FromBase64String(string txt) {
            var bytes = System.Convert.FromBase64String(txt);
            return bytes;
        }


        public static byte[] ToBytes(string txt) {
            var bytes = Text.ReadBytes(txt);
            return bytes;
        }


        public static string Json(object obj, bool indent = false) {
            if (obj is Model) {
                obj = ((Model)obj).Objectify();
            } else if (obj is Array || obj is IList || obj is ICollection){
                var isModel = false;
                foreach(var o in ((IEnumerable)obj)){
                    if(o is Model)
                        isModel = true;
                    break;
                }
                if(isModel){
                    var models = ((IEnumerable)obj);
                    var objects = new List<Dictionary<string, dynamic>>();
                    foreach (var model in models) {
                        var o = ((Model)model).Objectify();
                        objects.Add(o);
                    }
                    obj = objects;
                }
            }
                
            if(!indent)
                return JSONFormatter.Write(obj);
            return JSONFormatter.Write(obj, indent);
        }

        //public static string Json(object obj, bool indent) {
        //    return JSONFormatter.Write(obj, indent);
        //}

        public static object UnJson(string data) {
            return JSONFormatter.Read(data);
        }

        public static T UnJson<T>(string data) {
            return JSONFormatter.Read<T>(data);
        }
        
        public static Directory Directory() {
            var path = System.IO.Directory.GetCurrentDirectory();
            return new Directory(path);
        }
        public static Directory Directory(string path) {
            return new Directory(path);
        }

        public static File File(string path) {
            return new File(path);
        }

        public static string Hash(string data) {
            return Strata.Crypto.Hash.MD5(data);
        }

        public static string GUID(int length = 32){
            if (length <= 32) { 
                var uuid = System.Guid.NewGuid().ToString();
                var txt = Strata.Crypto.Hash.MD5(uuid);
                if (length < 32)
                    txt = txt.Substring(0, length);
                return txt;
            }

            var buffer = new System.Text.StringBuilder();
            while (buffer.Length < length) {
                var uuid = System.Guid.NewGuid().ToString();
                var txt = Strata.Crypto.Hash.MD5(uuid);
                buffer.Append(txt);
            }

            var str = buffer.ToString();
            if (str.Length > length)
                str = str.Substring(0, length);
            return str;
        }



        //public static void CopySection(Stream input, string targetFile,
        //                       int length, byte[] buffer) {
        //    using (Stream output = File.OpenWrite(targetFile)) {
        //        int bytesRead = 1;
        //        // This will finish silently if we couldn't read "length" bytes.
        //        // An alternative would be to throw an exception
        //        while (length > 0 && bytesRead > 0) {
        //            bytesRead = input.Read(buffer, 0, Math.Min(length, buffer.Length));
        //            output.Write(buffer, 0, bytesRead);
        //            length -= bytesRead;
        //        }
        //    }
        //}


        public static byte[] DumpStream(System.IO.Stream stream) {
            stream.Seek(0, System.IO.SeekOrigin.Begin);
            //var ix = 0;
            //var chunk_size = 1024;
            //List<byte> bytes = new List<byte>();
            //while (true) {
            //    var buffer = new byte[chunk_size];
            //    int cnt = stream.Read(buffer, ix, chunk_size);
            //    if (cnt > 0) {
            //        bytes.AddRange(buffer);   
            //        ix += cnt;
            //        continue;
            //    }
            //    return bytes.ToArray();
            //}

            using (System.IO.BinaryReader br = new System.IO.BinaryReader(stream)) {
                byte[] bytes = br.ReadBytes((int)stream.Length);
                return bytes;
            }
        }


        public static byte[] Pickle(object o) {
            var stream = new System.IO.MemoryStream();
            var formatter = new BinaryFormatter();
            
            try { 
                formatter.Serialize(stream, o);
                return DumpStream(stream);
            } catch (Exception ex) {
                throw ex;
            } finally {
                stream.Close();
                stream.Dispose();
            }
        }

        public static T Unpickle<T>(byte[] data) {
            var value = Unpickle(data);
            return (T)value;
        }
        public static object Unpickle(byte[] data) {
            var stream = new System.IO.MemoryStream(data);
            var formatter = new BinaryFormatter();
            try {
                var o = formatter.Deserialize(stream);
                return o;
            } catch (Exception ex) {
                throw ex;
            } finally {
                stream.Close();
                stream.Dispose();
            }
        }

        //public static T Deserialize<T>(byte[] bytes) {
        //    var obj = Deserialize(bytes);
        //    return (T)obj;
        //}

        //public static object Deserialize(byte[] bytes) {
        //    if (bytes == null || bytes.Length == 0)
        //        throw new ArgumentNullException("The bytes parameter is null/empty!");

        //    var stream = new MemoryStream(bytes);
        //    var formatter = new BinaryFormatter();
        //    object obj = null;
        //    try {
        //        obj = formatter.Deserialize(stream);
        //    } catch (Exception ex) {
        //        throw new FoundationException("An error ocurred while serializing the object. " + ex.Message, ex);
        //    } finally {
        //        stream.Close();
        //        stream.Dispose();
        //    }

        //    return obj;
        //}

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





        public static Directory CurrentDirectory {
            get {
                var path = System.IO.Directory.GetCurrentDirectory();
                return new Directory(path);
            }
        }


    }
}

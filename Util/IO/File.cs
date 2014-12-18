using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Strata.Util;
namespace Strata.Util.IO {
    public sealed class File : IComparable, IFileSystemEntry {
        #region -------- VARIABLES & CONSTRUCTOR(S) --------
        //private static Uri _blankURI = new Uri("http://localhost/");
        public File(string path) {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException("The path parameter is null/empty!");

            if (!Toolkit.IsValidUri(path)) {
                //perhaps it is a web url with a missing http prefix?
                if (path.ToLower().StartsWith("www.") && Toolkit.IsValidUri("http://" + path)) {
                    path = "http://" + path;
                } else {
                    throw new IOException("The specified path is not a valid Uri. path-> " + path);
                }
            }


            if (Toolkit.IsWebUri(path)) {
                //path = path.Replace("%20", " ");
                path = Http.UrlDecode(path);
                path = path.Replace(@"\", "/");
            } else {
                path = path.Replace("/", @"\");
            }


            try {
                this.Uri = new Uri(path);
                this.FullPath = path;
            } catch (Exception ex) {
                throw new IOException("The specified path is not a valid Uri. path-> " + path + "\r" + ex.Message, ex);
            }
        }
        #endregion


        #region -------- PUBLIC - Delete/TryDelete --------
        /// <summary>
        /// Delete the file if it doesnt exist
        /// </summary>
        public void Delete() {
            FileUtil.Delete(this.FullPath);
        }
        /// <summary>
        /// Attempt to delete the file, return true if successful false otherwise
        /// </summary>
        public bool TryDelete() {
            try {
                FileUtil.Delete(this.FullPath);
                return true;
            } catch (Exception ex) {
                Log.Trace(ex);
                return false;
            }
        }


        /// <summary>
        /// Delete the file. If the file is currently in use by a different process, try multiple times.
        /// </summary>
        /// <param name="timeout">The maximum amount of time to keep trying</param>
        public void DeleteLocked(int timeout = 100) {
            if (!this.Exists)
                return;

            try {
                this.Delete();
                return;
            } catch { }

            int maxTS = System.Environment.TickCount + timeout;
            while (System.Environment.TickCount < maxTS) {
                System.Threading.Thread.Sleep(1);

                try {
                    this.Delete();
                    return;
                } catch { }
            }

            this.Delete();
        }

        /// <summary>
        /// Delete the file. If the file is currently in use by a different process, try multiple times.
        /// </summary>
        /// <param name="timeout">The maximum amount of time to keep trying</param>
        /// <returns>True if the file is successfull deleted, false otherwise</returns>
        public bool TryDeleteLocked(int timeout = 100) {
            try {
                this.TryDelete();
                return true;
            } catch (Exception ex) {
                Log.Trace(ex);
                return false;
            }
        }
        #endregion

        #region -------- PUBLIC - ReadText/ReadBytes --------
        /// <summary>
        /// Open the specified file and return it as text
        /// </summary>
        /// <returns>The file contents</returns>
        public string ReadText() {
            return FileUtil.ReadText(this.FullPath);
        }

        public T ReadText<T>(Func<string, T> adapter) {
            string data = FileUtil.ReadText(this.FullPath);
            return adapter(data);
        }

        /// <summary>
        /// Open the specified file and return it as an array of strings
        /// </summary>
        /// <returns>The file contents</returns>
        public List<string> ReadLines() {
            string txt = this.ReadText();

            StringReader reader = new StringReader(txt);
            List<string> lines = new List<string>();
            string line = reader.ReadLine();
            while (line != null) {
                lines.Add(line);
                line = reader.ReadLine();
            }
            return lines;
        }


        /// <summary>
        /// Open the specified file and return the binary data
        /// </summary>
        /// <returns>The file contents</returns>
        public byte[] Read() {
            return FileUtil.ReadBytes(this.FullPath);
        }


        /// <summary>
        /// Open the specified file and return it as text
        /// </summary>
        /// <param name="path">The path of the file to read from</param>
        /// <returns>The file contents</returns>
        public static string ReadText(string path) {
            return FileUtil.ReadText(path);
        }


        /// <summary>
        /// Open the specified file and return it as text
        /// </summary>
        /// <param name="path">The path of the file to read from</param>
        /// <returns>The file contents</returns>
        public static List<string> ReadLines(string path) {
            string txt = FileUtil.ReadText(path);
            return Text.Explode(txt);
        }

        /// <summary>
        /// Open the specified file and return the binary data
        /// </summary>
        /// <param name="path">The path of the file to read from</param>
        /// <returns>The file contents</returns>
        public static byte[] Read(string path) {
            return FileUtil.ReadBytes(path);
        }
        #endregion

        #region -------- PUBLIC - Hide --------
        /// <summary>
        /// Make this directory hidden
        /// </summary>
        public void Hide() {
            FileInfo info = this.Info;
            info.Attributes = info.Attributes | FileAttributes.Hidden;
        }

        /// <summary>
        /// Make this directory read-only
        /// </summary>
        public void ReadOnly() {
            FileInfo info = this.Info;
            info.Attributes = info.Attributes | FileAttributes.ReadOnly;
        }

        /// <summary>
        /// Add an attribute to the file
        /// </summary>
        public void SetAttribute(FileAttributes attribute) {
            FileInfo info = this.Info;
            info.Attributes = info.Attributes | attribute;
        }

        public bool HasAttribute(FileAttributes attribute) {
            return (this.Info.Attributes.ToString().IndexOf(attribute.ToString()) == -1) ? false : true;
        }
        #endregion

        #region -------- PUBLIC - Create --------
        /// <summary>
        /// Creates a file entry on disk
        /// </summary>
        public void Create() {
            throw new NotImplementedException();
            //if(this.Exists) return;
            //FileUtil.Create(this.FullPath);
        }
        /// <summary>
        /// Attempt to create the file, return true if successful false otherwise
        /// </summary>
        public bool TryCreate() {
            throw new NotImplementedException();

            //if(this.Exists) return true;
            //try {
            //    FileUtil.Create(this.FullPath);
            //    return true;
            //} catch(UndertowIOException ex) {
            //    Tracer.Write(ex.Message);
            //    return false;
            //}
        }
        #endregion

        #region -------- PUBLIC - Write --------
        /// <summary>
        /// Writes the file data to the specified location
        /// </summary>
        /// <param name="data">The data to write to the file</param>
        public void Write(string data) {
            this.Write(Text.ToBytes(data), false);
        }

        /// <summary>
        /// Writes the file data to the specified location
        /// </summary>
        /// <param name="data">The data to write to the file</param>
        /// <param name="overwrite">If true, the existing file data will be overwritten (if the file already exists). If the file exists and overwrite is false, an exception will be thrown.</param>
        public void Write(string data, bool overwrite) {
            this.Write(Text.ToBytes(data), overwrite);
        }

        /// <summary>
        /// Writes the file data to the specified location
        /// </summary>
        /// <param name="data">The data to write to the file</param>
        public void Write(byte[] data) {
            this.Write(data, false);
        }

        /// <summary>
        /// Writes the file data to the specified location
        /// </summary>
        /// <param name="data">The data to write to the file</param>
        /// <param name="overwrite">If true, the existing file data will be overwritten (if the file already exists). If the file exists and overwrite is false, an exception will be thrown.</param>
        public void Write(byte[] data, bool overwrite) {
            FileUtil.Write(this.FullPath, data);
        }


        /// <summary>
        /// Writes the xml file data to the specified location
        /// </summary>
        /// <param name="data">The data to write to the file</param>
        public void Write(System.Xml.XmlNode xml) {
            this.Write(Text.ToBytes(xml.InnerXml), false);
        }

        /// <summary>
        /// Writes the file data to the specified location
        /// </summary>
        /// <param name="stream">The stream to write to the file</param>
        public void Write(System.IO.Stream stream) {
            FileUtil.Write(this.FullPath, stream);
        }

        /// <summary>
        /// Writes the file data to the specified location
        /// </summary>
        /// <param name="path">The path of the file to write to</param>
        /// <param name="data">The data to write to the file</param>
        public static void WriteText(string path, string data) {
            FileUtil.Write(path, data);
        }

        /// <summary>
        /// Writes the file data to the specified location
        /// </summary>
        /// <param name="path">The path of the file to write to</param>
        /// <param name="data">The data to write to the file</param>
        public static void WriteBinary(string path, byte[] data) {
            FileUtil.Write(path, data);
        }
        #endregion

        #region -------- PUBLIC - Append --------
        /// <summary>
        /// Appends the file data to the file in the specified location
        /// </summary>
        /// <param name="data">The data to append to the file</param>
        public void Append(string data) {
            throw new NotImplementedException();

            //lock(this) {
            //    FileUtil.Append(this.FullPath, data);
            //}
        }
        /// <summary>
        /// Appends the file data to the file in the specified location followed by a carriage return
        /// </summary>
        /// <param name="data">The data to append to the file</param>
        public void AppendLine(string data) {
            throw new NotImplementedException();

            //lock(this) {
            //    FileUtil.Append(this.FullPath, data + "\r\n");
            //}
        }
        /// <summary>
        /// Appends the file data to the file in the specified location
        /// </summary>
        /// <param name="data">The data to append to the file</param>
        public void Append(byte[] data) {
            throw new NotImplementedException();

            //lock(this) {
            //    FileUtil.Append(this.FullPath, data);
            //}
        }
        /// <summary>
        /// Appends the data in the specified file to the end of this file
        /// </summary>
        /// <param name="data">The path of the file to append to this one</param>
        public void Append(File file) {
            throw new NotImplementedException();

            //lock(this) {
            //    FileUtil.Merge(file.FullPath, this.FullPath);
            //}
        }
        #endregion

        #region -------- PUBLIC - Copy/Move --------
        /// <summary>
        /// Copy the file from one location to another
        /// </summary>
        /// <param name="dest">The location to copy the file to</param>
        public void Copy(File dest) {
            FileUtil.Copy(this.FullPath, dest.FullPath, false);
        }

        /// <summary>
        /// Copy the file from one location to another, return true if successful, false otherwise
        /// </summary>
        /// <param name="dest">The location to copy the file to</param>
        public bool TryCopy(File dest) {
            try {
                FileUtil.Copy(this.FullPath, dest.FullPath, false);
                return true;
            } catch (Exception ex) {
                Log.Trace("TryCopy Failed! " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Copy the file from one location to another
        /// </summary>
        /// <param name="dest">The location to copy the file to</param>
        /// <param name="overwrite">A flag specifying whether or not to overwrite the file if the dest file already exists. Default is false.</param>
        public void Copy(File dest, bool overwrite) {
            FileUtil.Copy(this.FullPath, dest.FullPath, overwrite);
        }


        /// <summary>
        /// Move the file from one location to another
        /// </summary>
        /// <param name="dest">The location to move the file to</param>
        public void Move(File dest) {
            FileUtil.Move(this.FullPath, dest.FullPath, false);
        }

        /// <summary>
        /// Move the file from one location to another, return true if successful, false otherwise
        /// </summary>
        /// <param name="dest">The location to move the file to</param>
        public bool TryMove(File dest) {
            try {
                FileUtil.Move(this.FullPath, dest.FullPath, false);
                return true;
            } catch (Exception ex) {
                Log.Trace("TryMove Failed! " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Move the file from one location to another
        /// </summary>
        /// <param name="dest">The location to move the file to</param>
        /// <param name="overwrite">A flag specifying whether or not to overwrite the file if the dest file already exists. Default is false.</param>
        public void Move(File dest, bool overwrite) {
            FileUtil.Move(this.FullPath, dest.FullPath, overwrite);
        }
        #endregion

        #region -------- PUBLIC - OpenReadStream/OpenWriteStream --------
        public System.IO.FileStream OpenReadStream() {
            if (!this.Exists)
                throw new System.IO.IOException("The file does not exist! " + this.FullPath);

            return System.IO.File.OpenRead(this.FullPath);
        }

        public System.IO.FileStream OpenWriteStream() {
            if (!this.Exists)
                throw new System.IO.IOException("The file does not exist! " + this.FullPath);

            return System.IO.File.OpenWrite(this.FullPath);
        }
        #endregion

        #region -------- STATIC - Get --------
        /// <summary>
        /// Create a new File object instance
        /// </summary>
        public static File Get(string path) {
            return new File(path);
        }
        #endregion


        public File ChangeExtension(string extension) {
            string fileName = this.NameWithoutExtension + "." + extension;
            return this.Parent.GetFile(fileName);
        }

        //#region -------- STATIC - Merge --------
        ///// <summary>
        ///// Merge the data from a list of files
        ///// </summary>
        //public static ByteBuffer Merge(params File[] files) {
        //    ByteBuffer buffer = new ByteBuffer();
        //    foreach(File file in files) {
        //        if(file == null || !file.Exists) continue;
        //        buffer.Append(file.Read());
        //    }
        //    return buffer;
        //}

        ///// <summary>
        ///// Merge the data from a list of files
        ///// </summary>
        //public static ByteBuffer Merge(params string[] files) {
        //    ByteBuffer buffer = new ByteBuffer();
        //    foreach(string filePath in files) {
        //        File file = File.Get(filePath);
        //        if(!file.Exists) continue;
        //        buffer.Append(file.Read());
        //    }
        //    return buffer;
        //}
        //#endregion

        #region -------- PUBLIC - GetUniqueDerivative --------
        /// <summary>
        /// If a file with the same name already exists in the current directory, create a unique derivation of the file name
        /// </summary>
        public File GetUniqueDerivative() {
            if (!this.Exists)
                return this;

            string name = this.NameWithoutExtension;
            string ext = this.Extension;
            int counter = 0;
            Directory dir = this.Parent;

            string path = dir.GetFile(this.Name).FullPath;
            while (FileUtil.Exists(path)) {
                path = dir.GetFile(name + "[" + (++counter).ToString() + "]." + ext).FullPath;
            }

            File file = File.Get(path);
            return file;
        }
        #endregion

        #region -------- PUBLIC - CompareTo --------
        public int CompareTo(object o) {
            if (o == null || !(o is File)) return -1;
            if (this.Exists && ((File)o).Exists) return this.DateModified.CompareTo(((File)o).DateModified);///default sort by oldest first
            return 0;
        }
        #endregion

        #region -------- PUBLIC OVERRIDE - Equals/GetHashCode --------
        public override bool Equals(object obj) {
            File file = (obj as File);
            if (file == null)
                return false;

            return file.Info.Equals(this.Info);
        }

        public override int GetHashCode() {
            return this.FullPath.GetHashCode();
        }
        #endregion


        #region -------- PROPERTIES --------
        /// <summary>
        /// Returns the full path to this file
        /// </summary>
        public string FullPath {
            get;
            private set;
        }

        /// <summary>
        /// Returns the Uri for this file
        /// </summary>
        public Uri Uri {
            get;
            private set;
        }

        /// <summary>
        /// Return the name of this file
        /// </summary>
        public string Name {
            get {
                return System.IO.Path.GetFileName(this.FullPath);
            }
        }


        /// <summary>
        /// Return the name of this file sans extension
        /// </summary>
        public string NameWithoutExtension {
            get {
                return System.IO.Path.GetFileNameWithoutExtension(this.FullPath);
            }
        }


        /// <summary>
        /// Return the extension of this file (does not include the period)
        /// </summary>
        public string Extension {
            get {
                string ext = System.IO.Path.GetExtension(this.FullPath).ToLower();
                //string ext = System.IO.Path.GetFileName(this._fullPath).ToLower();
                if (ext.StartsWith("."))
                    ext = ext.Substring(1);
                return ext;
            }
        }


        /// <summary>
        /// Return the parent Directory for this file
        /// </summary>
        public Directory Parent {
            get {
                return Directory.Get(System.IO.Path.GetDirectoryName(this.FullPath).ToLower());
            }
        }


        /// <summary>
        /// The size of this file in bytes
        /// </summary>
        public long Size {
            get {
                if (!this.Exists)
                    return 0;
                return this.Info.Length;
            }
        }

        /// <summary>
        /// The size of this file in kilobytes
        /// </summary>
        public long SizeKB {
            get {
                if (!this.Exists)
                    return 0;
                return this.Info.Length / 1024;
            }
        }


        /// <summary>
        /// A boolean value specifying whether or not the file exists
        /// </summary>
        public bool Exists {
            get {
                return FileUtil.Exists(this.FullPath);
            }
        }

        /// <summary>
        /// Returns the DateTime representing the date this file was created
        /// </summary>
        public DateTime DateCreated {
            get {
                return this.Info.CreationTime;
            }
        }

        /// <summary>
        /// Returns the DateTime representing the date this file was last written/modified
        /// </summary>
        public DateTime DateModified {
            get {
                return this.Info.LastWriteTime;
            }
        }

        /// <summary>
        /// Returns the DateTime representing the date this file was last read
        /// </summary>
        public DateTime DateLastRead {
            get {
                return this.Info.LastAccessTime;
            }
        }

        /// <summary>
        /// Returns the age of the file as a TimeSpan object
        /// </summary>
        public TimeSpan Age {
            get {
                return Dates.Delta(this.Info.CreationTime, DateTime.Now);
            }
        }


        /// <summary>
        /// Returns a timespan object represneting the last time elapsed since the file was last accessed
        /// </summary>
        public TimeSpan LastRead {
            get {
                return Dates.Delta(this.Info.LastAccessTime, DateTime.Now);
            }
        }

        /// <summary>
        /// Returns true if the file is read only
        /// </summary>
        public bool IsReadOnly {
            get {
                return this.Info.IsReadOnly;
            }
        }

        /// <summary>
        /// Returns true if the file points to a location on the web
        /// </summary>
        public bool IsWebUri {
            get { return Toolkit.IsWebUri(this.FullPath); }
        }

        /// <summary>
        /// Returns true/false specfiying whether or not the file is hidden
        /// </summary>
        public bool IsHidden {
            get {
                return (this.Info.Attributes.ToString().IndexOf("Hidden") == -1) ? false : true;
            }
        }

        /// <summary>
        /// Returns the FileInfo object for this file
        /// </summary>
        public FileInfo Info {
            get { return new System.IO.FileInfo(this.FullPath); }
        }
        #endregion

        #region -------- PUBLIC OVERRIDE - ToString --------
        /// <summary>
        /// Returns the full path of this file
        /// </summary>
        /// <returns>The full path of the file</returns>
        public override string ToString() {
            //return ((this.Age.Minutes*60) + this.Age.Seconds) + "  " + this.FullPath;
            return this.FullPath;
        }
        #endregion
    }

    internal static class FileUtil {
        #region -------- PUBLIC - Delete --------
        /// <summary>
        /// If the file exists, try to delete it
        /// </summary>
        /// <param name="path">The absolute path/uri to check</param>
        public static void Delete(string path) {
            if (!Exists(path))
                return;

            try {
                System.IO.File.Delete(path);
            } catch (Exception ex) {
                throw new IOException("An error occurred while deleting the file. path-> " + path + "\r" + ex.Message, ex);
            }
        }
        #endregion

        #region -------- PUBLIC - Create --------
        /// <summary>
        /// Create a file in the specified location
        /// </summary>
        /// <param name="path">The absolute path/uri to create</param>
        public static File Create(string path) {
            //if (path == null || path.Length == 0) throw new Undertow.NullEmptyException("path");
            if (Exists(path))
                throw new IOException("That file already exists. path-> " + path);

            try {
                //System.IO.File.Create(path);
                using (System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.CreateNew)) { }
                return new File(path);
            } catch (Exception ex) {
                throw new IOException("An error occurred while creating the file. path-> " + path + "\r" + ex.Message, ex);
            }
        }
        #endregion

        #region -------- PUBLIC - Exists --------
        /// <summary>
        /// Verifies whether or not the specified file exists
        /// </summary>
        /// <param name="path">The absolute path/uri to check</param>
        public static bool Exists(string path) {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException("The path parameter is null/empty!");

            try {
                return System.IO.File.Exists(path);
            } catch (Exception ex) {
                throw new IOException("An error occurred while checking if the file exists. path-> " + path + "\r" + ex.Message, ex);
            }
        }
        #endregion

        #region -------- PUBLIC - ReadText/ReadBytes --------
        /// <summary>
        /// Open the specified file and return it as text
        /// </summary>
        /// <param name="path">The absolute path/uri of the file to open</param>
        /// <returns>The file contents</returns>
        public static string ReadText(string path) {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException("The path parameter is null/empty!");

            byte[] bytes = ReadBytes(path);
            return System.Text.Encoding.UTF8.GetString(bytes);
            //return System.Text.Encoding.Default.GetString(bytes);
        }

        /// <summary>
        /// Open the specified file and return it as a byte array
        /// </summary>
        /// <param name="path">The absolute path/uri of the file to open</param>
        /// <returns>The file contents</returns>
        public static byte[] ReadBytes(string path) {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException("The path parameter is null/empty!");

            //if (UriUtil.IsWebUri(path))
            //    return DownloadBytes(path);

            if (!Exists(path))
                throw new IOException("The file does not exist. path-> " + path);

            try {
                byte[] data = System.IO.File.ReadAllBytes(path);
                return data;
            } catch (Exception ex) {
                throw new IOException("File read error-> " + path + "\r" + ex.Message, ex);
            }
        }
        #endregion

        //#region -------- PUBLIC - DownloadText/DownloadBytes --------
        ///// <summary>
        ///// Download the remote file and return it as text data
        ///// </summary>
        ///// <param name="path">The url pointing to the file to download</param>
        ///// <returns>The binary file data</returns>
        //public static string DownloadText(string path) {
        //    byte[] data = DownloadBytes(path);
        //    return System.Text.Encoding.Default.GetString(data);
        //}
        ///// <summary>
        ///// Download the remote file and return it as a byte array
        ///// </summary>
        ///// <param name="path">The url pointing to the file to download</param>
        ///// <returns>The binary file data</returns>
        //public static byte[] DownloadBytes(string path) {
        //    if (String.IsNullOrEmpty(path))
        //        throw new ArgumentNullException("The path parameter is null/empty!");

        //    try {
        //        System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(path);
        //        request.Timeout = Constants.DefaultHttpTimeout;
        //        request.UserAgent = Constants.DefaultUserAgent;
        //        request.Method = "GET";

        //        System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();
        //        byte[] data = StreamUtil.ReadData(response.GetResponseStream());
        //        response.Close();
        //        return data;
        //    } catch (Exception ex) {
        //        throw new IOException("File download error-> " + path + "\r" + ex.Message, ex);
        //    }
        //}
        //#endregion

        #region -------- PUBLIC - Write --------
        /// <summary>
        /// Writes the file data to the specified location
        /// </summary>
        /// <param name="path">The absolute path/uri of the file to write to</param>
        /// <param name="data">The data to write to the file</param>
        public static void Write(string path, string data) {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException("The path parameter is null/empty!");
            if (data == null) data = "";
            Write(path, System.Text.Encoding.Default.GetBytes(data), true);
        }

        /// <summary>
        /// Writes the file data to the specified location
        /// </summary>
        /// <param name="path">The absolute path/uri of the file to write to</param>
        /// <param name="data">The data to write to the file</param>
        public static void Write(string path, byte[] data) {
            Write(path, data, true);
        }

        /// <summary>
        /// Writes the file data to the specified location
        /// </summary>
        /// <param name="path">The absolute path/uri of the file to write to</param>
        /// <param name="data">The data to write to the file</param>
        /// <param name="overwrite">If true, the existing file data will be overwritten (if the file already exists). If the file exists and overwrite is false, an exception will be thrown.</param>
        public static void Write(string path, byte[] data, bool overwrite) {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException("The path parameter is null/empty!");

            try {
                string dir = System.IO.Path.GetDirectoryName(path);
                if (!System.IO.Directory.Exists(dir))
                    System.IO.Directory.CreateDirectory(dir);

                System.IO.FileMode mode = (overwrite) ? System.IO.FileMode.Create : FileMode.CreateNew;
                using (System.IO.FileStream fs = new FileStream(path, mode)) {
                    fs.Write(data, 0, data.Length);
                }
            } catch (Exception ex) {
                throw new IOException("File write error-> " + path + "\r" + ex.Message, ex);
            }
        }

        /// <summary>
        /// Writes the data from the stream to the file location
        /// </summary>
        /// <param name="path">The absolute path/uri of the file to write to</param>
        /// <param name="stream">The stream to write from</param>
        public static void Write(string path, System.IO.Stream stream) {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException("The path parameter is null/empty!");
            if (stream == null)
                throw new ArgumentNullException("The stream parameter is null!");

            try {
                using (System.IO.FileStream toStream = new FileStream(path, FileMode.OpenOrCreate)) {
                    Redirect(stream, toStream);
                }
            } catch (Exception ex) {
                throw new IOException("File write error-> " + path + "\r" + ex.Message, ex);
            }

            //StreamUtil.Redirect(
        }
        #endregion

        //#region -------- PUBLIC - Append --------
        ///// <summary>
        ///// Writes the file data to the specified location. Appends the data to the end of the file if it already exists
        ///// </summary>
        ///// <param name="path">The absolute path/uri of the file to write to</param>
        ///// <param name="data">The data to write to the file</param>
        //public static void Append(string path, string data) {
        //    if (path == null || path.Length == 0) throw new Undertow.NullEmptyException("path");
        //    if (data == null) data = "";
        //    Append(path, System.Text.Encoding.Default.GetBytes(data));
        //}
        ///// <summary>
        ///// Writes the file data to the specified location. Appends the data to the end of the file if it already exists
        ///// </summary>
        ///// <param name="path">The absolute path/uri of the file to write to</param>
        ///// <param name="data">The data to write to the file</param>
        //public static void Append(string path, byte[] data) {
        //    if (path == null || path.Length == 0) throw new Undertow.NullEmptyException("path");
        //    try {
        //        string dir = System.IO.Path.GetDirectoryName(path);
        //        if (!System.IO.Directory.Exists(dir))
        //            System.IO.Directory.CreateDirectory(dir);
        //        using (System.IO.FileStream fs = new FileStream(path, System.IO.FileMode.Append)) {
        //            fs.Write(data, 0, data.Length);
        //        }
        //    } catch (Exception ex) {
        //        throw new UndertowIOException(path, IOActions.Appending, ex);
        //    }
        //}
        //#endregion

        //#region -------- PUBLIC - Merge --------
        ///// <summary>
        ///// Appends the data from one file to another file
        ///// </summary>
        ///// <param name="fromFile">The absolute path/uri of the file to append from</param>
        ///// <param name="toFile">The absolute path/uri of the file to append to</param>
        //public static void Merge(string fromFile, string toFile) {
        //    if (fromFile == null || fromFile.Length == 0) throw new Undertow.NullEmptyException("fromFile");
        //    if (toFile == null || toFile.Length == 0) throw new Undertow.NullEmptyException("toFile");
        //    if (!Exists(fromFile))
        //        throw new UndertowIOException(fromFile, "That fromFile path specifies a file that does not exist.");
        //    try {
        //        System.IO.FileStream reader = new FileStream(fromFile, FileMode.Open);
        //        System.IO.FileStream writer = new FileStream(toFile, FileMode.Append);
        //        while (true) {
        //            byte[] block = new byte[10240];
        //            int count = reader.Read(block, 0, block.Length);
        //            writer.Write(block, 0, count);
        //            if (count < block.Length || count == 0)
        //                break;
        //        }
        //        writer.Flush();
        //        reader.Close();
        //        writer.Close();
        //    } catch (Exception ex) {
        //        throw new UndertowIOException("fromFile: \"" + fromFile + "\" toFile: \"" + toFile + "\"", IOActions.Appending, ex);
        //    }
        //}
        //#endregion

        #region -------- PUBLIC - Copy/Move --------
        /// <summary>
        /// Copy the file from one location to another
        /// </summary>
        /// <param name="sourceFilePath">The absolute path/uri of the source file</param>
        /// <param name="destFilePath">The absolute path/uri of the destination file</param>
        public static void Copy(string sourceFilePath, string destFilePath) {
            Copy(sourceFilePath, destFilePath, false);
        }
        /// <summary>
        /// Copy the file from one location to another
        /// </summary>
        /// <param name="sourceFilePath">The absolute path/uri of the source file</param>
        /// <param name="destFilePath">The absolute path/uri of the destination file</param>
        /// <param name="overwrite">A flag specifying whether or not to overwrite the file if the dest file already exists. Default is false.</param>
        public static void Copy(string sourceFilePath, string destFilePath, bool overwrite) {
            if (String.IsNullOrEmpty(sourceFilePath))
                throw new ArgumentNullException("The sourceFilePath parameter is null/empty!");
            if (String.IsNullOrEmpty(destFilePath))
                throw new ArgumentNullException("The destFilePath parameter is null/empty!");

            if (!Exists(sourceFilePath))
                throw new IOException("The source file could not be found. path-> " + sourceFilePath);
            if (!overwrite && Exists(destFilePath))
                throw new IOException("A file already exists at the destination path. Specify overwrite = true in order to overwrite the existing file. path-> " + destFilePath);

            try {
                System.IO.File.Copy(sourceFilePath, destFilePath, overwrite);
            } catch (Exception ex) {
                throw new IOException("File copy error, sourceFilePath-> " + sourceFilePath + " destFilePath-> " + destFilePath + "\r" + ex.Message, ex);
            }
        }


        /// <summary>
        /// Move the file from one location to another
        /// </summary>
        /// <param name="sourceFilePath">The absolute path/uri of the source file</param>
        /// <param name="destFilePath">The absolute path/uri of the destination file</param>
        public static void Move(string sourceFilePath, string destFilePath) {
            Move(sourceFilePath, destFilePath, false);
        }

        /// <summary>
        /// Move the file from one location to another
        /// </summary>
        /// <param name="sourceFilePath">The absolute path/uri of the source file</param>
        /// <param name="destFilePath">The absolute path/uri of the destination file</param>
        /// <param name="overwrite">A flag specifying whether or not to overwrite the file if the dest file already exists. Default is false.</param>
        public static void Move(string sourceFilePath, string destFilePath, bool overwrite) {
            if (String.IsNullOrEmpty(sourceFilePath))
                throw new ArgumentNullException("The sourceFilePath parameter is null/empty!");
            if (String.IsNullOrEmpty(destFilePath))
                throw new ArgumentNullException("The destFilePath parameter is null/empty!");

            if (!Exists(sourceFilePath))
                throw new IOException("The source file could not be found. path-> " + sourceFilePath);
            if (!overwrite && Exists(destFilePath))
                throw new IOException("A file already exists at the destination path. Specify overwrite = true in order to overwrite the existing file. path-> " + destFilePath);


            if (overwrite && Exists(destFilePath))
                Delete(destFilePath);

            try {
                System.IO.File.Move(sourceFilePath, destFilePath);
            } catch (Exception ex) {
                throw new IOException("File move error, sourceFilePath-> " + sourceFilePath + " destFilePath-> " + destFilePath + "\r" + ex.Message, ex);
            }
        }
        #endregion

        #region -------- PUBLIC - ReadData --------
        public static byte[] ReadData(System.IO.Stream stream) {
            byte[] data = new byte[10240];

            System.IO.MemoryStream buffer = new MemoryStream();
            //int offset = 0;
            int size = stream.Read(data, 0, data.Length);
            while (size > 0) {
                int count = size;

                buffer.Write(data, 0, count);
                //offset += count;
                size = stream.Read(data, 0, data.Length);
            }
            data = buffer.ToArray();
            buffer.Dispose();
            return data;
        }
        #endregion

        //#region -------- PUBLIC - OpenReadStream/OpenWriteStream --------
        //public static System.IO.FileStream OpenReadStream(string path) {
        //    System.IO.FileStream stream = new System.IO.FileStream(path, System.IO.FileMode.Open, FileAccess.Read);
        //    return stream;
        //}

        //public static System.IO.FileStream OpenWriteStream(string path) {
        //    System.IO.FileStream stream = new System.IO.FileStream(path, System.IO.FileMode.OpenOrCreate, FileAccess.Write);
        //    return stream;
        //}
        //#endregion

        #region -------- PUBLIC - Redirect --------
        public static void Redirect(Stream input, Stream output) {
            byte[] block = new byte[2048];
            int offset = 0;
            int count = input.Read(block, 0, block.Length);
            while (count > 0) {
                output.Write(block, 0, count);
                offset += count;
                block = new byte[1024];
                count = input.Read(block, 0, block.Length);
                if (count < block.Length)
                    Array.Resize(ref block, count);
            }
            output.Flush();
        }
        #endregion
    }
}

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Strata.Util;
namespace Strata.Util.IO {
    public sealed class Directory : IComparable, IFileSystemEntry {
        #region -------- VARIABLES & CONSTRUCTOR(S) --------
        public Directory(string path) {
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

            if (this.IsWebUri) {
                path = Http.UrlDecode(path);
                path = path.Replace(@"\", "/");
                if (!path.EndsWith("/"))
                    path += "/";
            } else {
                path = path.Replace("/", @"\");
                if (!path.EndsWith(@"\"))
                    path += @"\";
            }

            try {
                this.Uri = new Uri(path);
                this.FullPath = path;
            } catch (Exception ex) {
                throw new IOException("The specified path is not a valid Uri. path-> " + path + "\r" + ex.Message, ex);
            }
        }
        #endregion

        #region -------- PUBLIC - FindPreceding --------
        /// <summary>
        /// Walk backwards through the directory tree until
        /// the specified directory name is located.
        /// </summary>
        public Directory FindPreceding(string name) {
            Directory current = this;
            while (!current.IsRoot) {
                if (current.Contains(name))
                    return current.GetDirectory(name);
                current = current.Parent;
            }
            return null;
        }

        public File FindFile(string name) {
            return FindFile(name, false);
        }
        public File FindFile(string name, bool lookInParents) {
            Directory current = this;
            while (!current.IsRoot) {
                File file = current.GetFile(name);
                if (file.Exists)
                    return file;

                current = current.Parent;
            }
            return null;
        }
        #endregion

        #region -------- PUBLIC - Hide --------
        /// <summary>
        /// Make this file hidden
        /// </summary>
        public void Hide() {
            DirectoryInfo info = this.Info;
            info.Attributes = info.Attributes | FileAttributes.Hidden;
        }
        #endregion


        #region -------- PUBLIC - SetAttribute --------
        /// <summary>
        /// Add an attribute to the directory
        /// </summary>
        public void SetAttribute(FileAttributes attribute) {
            DirectoryInfo info = this.Info;
            info.Attributes = info.Attributes | attribute;
        }
        #endregion


        #region -------- PUBLIC - Create --------
        /// <summary>
        /// Create the directory if it doesnt exist
        /// </summary>
        public void Create() {
            if (this.Exists)
                return;

            DirectoryUtil.Create(this.FullPath);
        }


        /// <summary>
        /// Attempt to create the directory, return true if successful false otherwise
        /// </summary>
        public bool TryCreate() {
            if (this.Exists)
                return true;

            try {
                DirectoryUtil.Create(this.FullPath);
                return true;
            } catch (Exception ex) {
                //TODO: replace!
                Console.WriteLine("Directory.TryCreate Error-> " + ex.Message);
                return false;
            }
        }
        #endregion

        #region -------- PUBLIC - Delete/TryDelete --------
        /// <summary>
        /// Delete the directory
        /// </summary>
        public void Delete() {
            DirectoryUtil.Delete(this.FullPath);
        }

        /// <summary>
        /// Attempt to delete the directory, return true if successful false otherwise
        /// </summary>
        public bool TryDelete() {
            if (!this.Exists)
                return false;

            try {
                DirectoryUtil.Delete(this.FullPath);
                return true;
            } catch (Exception ex) {
                Log.Trace(ex);
                return false;
            }
        }
        #endregion

        #region -------- PUBLIC - Clear --------
        /// <summary>
        /// Clear the directory of any files
        /// </summary>
        public void Clear() {
            this.Clear(false);
        }

        /// <summary>
        /// Clear the directory of any files
        /// </summary>
        /// <param name="removeSubdirectories">A boolean value specifying whether or not to try and delete the subdirectories as well. Default is false.</param>
        public void Clear(bool removeSubdirectories) {
            DirectoryUtil.Clear(this.FullPath, removeSubdirectories);
        }
        #endregion

        #region -------- PUBLIC - GetFiles --------
        /// <summary>
        /// Return a list of the files in the directory
        /// </summary>
        public List<File> GetFiles() {
            return DirectoryUtil.GetFiles(this.FullPath);
        }
        /// <summary>
        /// Return a list of the files in the directory
        /// </summary>
        /// <param name="pattern">The pattern to filter results by (e.g. *.jpg to return all jpg files. *.jpg,*.bmp would return all jpg and bmp files)</param>
        public List<File> GetFiles(string pattern) {
            if(String.IsNullOrEmpty(pattern))
                return DirectoryUtil.GetFiles(this.FullPath);

            return DirectoryUtil.GetFiles(this.FullPath, pattern);
        }


        /// <summary>
        /// Return a list of the files in the directory
        /// </summary>
        /// <param name="includeSubDirectories">Flag specifying whether or not to include subdirectories in the search. Default is false.</param>
        public List<File> GetFiles(bool includeSubDirectories) {
            return DirectoryUtil.GetFiles(this.FullPath, includeSubDirectories);
        }

        /// <summary>
        /// Return a list of the files in the directory
        /// </summary>
        /// <param name="pattern">The pattern to filter results by (e.g. *.jpg to return all jpg files. *.jpg,*.bmp would return all jpg and bmp files)</param>
        /// <param name="includeSubDirectories">Flag specifying whether or not to include subdirectories in the search. Default is false.</param>
        public List<File> GetFiles(string pattern, bool includeSubDirectories) {
            if (String.IsNullOrEmpty(pattern))
                return DirectoryUtil.GetFiles(this.FullPath, includeSubDirectories);

            return DirectoryUtil.GetFiles(this.FullPath, pattern, includeSubDirectories);
        }
        #endregion

        #region -------- PUBLIC - GetDirectories --------
        /// <summary>
        /// Return a list of subdirectories
        /// </summary>
        public List<Directory> GetDirectories() {
            return this.GetDirectories(false);
        }
        #endregion

        #region -------- PUBLIC - GetDirectories --------
        /// <summary>
        /// Return a list of subdirectories. If deepSearch is specified as true this will return
        /// an array of ALL nested subdirectories
        /// </summary>
        /// <param name="deepSearch">Flag dictating whether or not to include all the subdirectories in a search operation. Default is false.</param>
        public List<Directory> GetDirectories(bool deepSearch) {
            return DirectoryUtil.GetSubdirectories(this.FullPath, deepSearch);

        }
        #endregion

        #region -------- PUBLIC - GetDirectory --------
        /// <summary>
        /// Return a Directory child from the directory name or relative path provided
        /// </summary>
        public Directory GetDirectory(string path) {
            return this.GetDirectory(path, false);
        }

        /// <summary>
        /// Return a Directory child from the directory name or relative path provided
        /// </summary>
        public Directory GetDirectory(string path, bool createIfNew) {
            Uri subUri;
            if (!Uri.TryCreate(this.Uri, path, out subUri))
                throw new IOException("The specified path is not a valid Uri. path-> " + path);


            Directory sub = Directory.Get(subUri);
            if (!sub.Exists && createIfNew)
                sub.Create();

            return sub;
        }
        #endregion

        #region -------- PUBLIC - NewUniqueDirectory --------
        /// <summary>
        /// Return a uniquely named directory
        /// </summary>
        public Directory NewUniqueDirectory() { return NewUniqueDirectory("", false); }
        public Directory NewUniqueDirectory(bool create) {
            return NewUniqueDirectory("", create);
        }

        /// <summary>
        /// Return a uniquely named directory
        /// </summary>
        public Directory NewUniqueDirectory(string prefix) { return NewUniqueDirectory(prefix, false); }
        public Directory NewUniqueDirectory(string prefix, bool create) {
            string name = prefix;
            Directory sub = this.GetDirectory(name);
            //while (sub.Exists) {
            //    string guid = Guid.NewGuid().ToString("D").Substring(0, 6);
            //    name = prefix + "." + guid;
            //    sub = this.GetDirectory(name);
            //}


            int counter = 0;
            while (sub.Exists) {
                name = prefix + "[" + (counter++).ToString() + "]";
                sub = this.GetDirectory(name);
            }

            if (create)
                sub.Create();
            return sub;
        }
        #endregion

        #region -------- PUBLIC - GetFile --------
        public File File(string name) {
            Uri newUri = new Uri(this.Uri, name);
            if (this.IsWebUri)
                return new File(newUri.AbsoluteUri);

            return new File(newUri.LocalPath);
        }


        /// <summary>
        /// Return a File from the name provided
        /// </summary>
        public File GetFile(string name) {
            Uri newUri = new Uri(this.Uri, name);
            if (this.IsWebUri)
                return new File(newUri.AbsoluteUri);

            return new File(newUri.LocalPath);

            //if (this._webURL)
            //    return new File(newUri.AbsoluteUri);
            //else if(newUri.IsUnc)
            //    return new File(newUri.LocalPath);
            //return new File(newUri.LocalPath);
        }
        #endregion

        #region -------- PUBLIC - NewUniqueFile --------
        /// <summary>
        /// Return a File from the name provided
        /// </summary>
        public File NewUniqueFile(string name) {
            string ext = System.IO.Path.GetExtension(name);
            if (!String.IsNullOrEmpty(ext))
                name = System.IO.Path.GetFileNameWithoutExtension(name);


            string path = "";
            lock (this) {
                path = this.FullPath + name + ext;
                while (System.IO.File.Exists(path)) {
                    path = this.FullPath + name + "[" + RNG.Random(999999).ToString() + "]" + ext;
                }
                FileUtil.Create(path);
            }
   
            return new File(path);

            //string ext = System.IO.Path.GetExtension(name);
            //string path = this.FullPath + name;
            //while (System.IO.File.Exists(path)) {
            //    path = this.FullPath + Counter.RandomString + "_" + name;
            //}
            //return new File(path);
        }
        #endregion

        #region -------- PUBLIC - GetUniqueDerivative --------
        /// <summary>
        /// If a directory with the same name already exists in the current parent directory, create a unique derivation of the directory name
        /// </summary>
        public Directory GetUniqueDerivative() {
            if (!this.Exists)
                return this;

            string name = this.Name;
            int counter = 0;
            Directory dir = this.Parent;

            string path = dir.GetDirectory(name + "[" + (++counter).ToString() + "]\\").FullPath; ;
            while (DirectoryUtil.Exists(path)) {
                path = dir.GetDirectory(name + "[" + (++counter).ToString() + "]\\").FullPath;
            }

            Directory sub = Directory.Get(path);
            return sub;
        }
        #endregion

        #region -------- PUBLIC - Reset/TryReset --------
        /// <summary>
        /// Delete this directory and all subdirectories/files contained therein and create a new directory for the same path.
        /// </summary>
        public void Reset() {
            throw new NotImplementedException();

            //if (!this.Exists)
            //    this.Create();
            //else {
            //    this.Delete();
            //    this.Create();
            //}
        }
        /// <summary>
        /// Attempt to delete the directory and all subdirectories/files contained therein and create a new directory for the same path.
        /// 
        /// Return true if successful false otherwise
        /// </summary>
        public bool TryReset() {
            throw new NotImplementedException();

            //try {
            //    if (!this.Exists) {
            //        this.Create();
            //        return true;
            //    } else {
            //        this.Delete();
            //        this.Create();
            //        return true;

            //    }
            //} catch (UndertowIOException ex) {
            //    Tracer.Write(ex.Message);
            //    return false;
            //}
        }
        #endregion

        #region -------- PUBLIC - Contains --------
        /// <summary>
        /// Check the for the specified directory/file, return true if it exists, false otherwise
        /// </summary>
        /// <param name="name">The name of the directory/file to check for</param>
        /// <returns>True if the directory/file exists, false otherwise</returns>
        public bool Contains(string name) {
            string path = this.FullPath + name;
            if (DirectoryUtil.Exists(path))
                return true;

            return FileUtil.Exists(path);
        }
        /// <summary>
        /// Check the for the specified file, return true if it exists, false otherwise
        /// </summary>
        /// <param name="name">The name of the file to check for</param>
        /// <returns>True if the file exists, false otherwise</returns>
        public bool ContainsFile(string name) {
            return FileUtil.Exists(this.FullPath + name);
        }

        /// <summary>
        /// Check the for the specified directory, return true if it exists, false otherwise
        /// </summary>
        /// <param name="name">The name of the directory to check for</param>
        /// <returns>True if the directory exists, false otherwise</returns>
        public bool ContainsDirectory(string name) {
            return DirectoryUtil.Exists(this.FullPath + name);
        }
        #endregion

        #region -------- PUBLIC - GetAncestor --------
        /// <summary>
        /// Return the ancestor of this directory at the specified depth
        /// </summary>
        /// <param name="depth">The distance of the ancestor from this directory (e.g. if this directory is c:\inetpub\wwwroot\YourApp\ , the 2nd level ancestor would be c:\inetpub\)</param>
        public Directory GetAncestor(int depth) {
            int count = 1;
            Directory ancestor = this.Parent;
            string root = this.Info.Root.FullName;
            while (count < depth && !ancestor.FullPath.Equals(root)) {
                ancestor = ancestor.Parent;
                count++;
            }
            return ancestor;
        }
        #endregion

        #region -------- STATIC - Get --------
        /// <summary>
        /// Create a new Directory object instance
        /// </summary>
        public static Directory Get(string path) {
            return new Directory(path);
        }

        /// <summary>
        /// Create a new Directory object instance
        /// </summary>
        public static Directory Get(Uri uri) {
            if (uri == null)
                throw new ArgumentNullException("The uri parameter is null!");

            string path = (Toolkit.IsWebUri(uri)) ? uri.AbsoluteUri : uri.LocalPath;
            return new Directory(path);
        }
        #endregion

        #region -------- PUBLIC - CalculateSize/CalculateSizeKB --------
        /// <summary>
        /// Calculate the total size of this directory in bytes
        /// </summary>
        public long CalculateSize() {
            if (!this.Exists)
                return 0;
           
            List<File> files = this.GetFiles(true);
            long size = 0;
            foreach (File file in files) {
                size += file.Size;
            }
            return size;
        }
        /// <summary>
        /// Calculate the total size of this directory in kilobytes
        /// </summary>
        public long CalculateSizeKB() {
            if (!this.Exists)
                return 0;
            long size = this.CalculateSize();
            return size / 1024;
        }
        #endregion


        #region -------- PUBLIC - CompareTo --------
        public int CompareTo(object o) {
            if (o == null || !(o is Directory)) return -1;
            if (this.Exists && ((Directory)o).Exists)
                return this.DateModified.CompareTo(((Directory)o).DateModified);///default sort by oldest first
            return 0;
        }
        #endregion

        #region -------- PUBLIC OVERRIDE - Equals/GetHashCode --------
        public override bool Equals(object obj) {
            Directory dir = (obj as Directory);
            if (dir == null)
                return false;

            return dir.Info.Equals(this.Info);
        }

        public override int GetHashCode() {
            return this.FullPath.GetHashCode();
        }
        #endregion



        #region -------- PROPERTIES --------
        /// <summary>
        /// Returns the full path to this directory
        /// </summary>
        public string FullPath {
            get;
            private set;
        }

        /// <summary>
        /// Returns the Uri for this directory
        /// </summary>
        public Uri Uri {
            get;
            private set;
        }

        

        /// <summary>
        /// Return the name of this directory
        /// </summary>
        public string Name {
            get {
                return this.Info.Name;
            }
        }

        /// <summary>
        /// The number of files in the directory
        /// </summary>
        public int TotalFiles {
            get { return (this.Exists) ? this.Info.GetFiles().Length : 0; }
        }

        /// <summary>
        /// The number of subdirectories in the directory
        /// </summary>
        public int TotalDirectories {
            get { return (this.Exists) ? this.Info.GetDirectories().Length : 0; }
        }

        /// <summary>
        /// A boolean value specifying whether or not the file exists
        /// </summary>
        public bool Exists {
            get { return DirectoryUtil.Exists(this.FullPath); }
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
        /// Returns the age of the directory as a TimeSpan object
        /// </summary>
        public TimeSpan Age {
            get {
                return Dates.Delta(this.Info.CreationTime, DateTime.Now);
            }
        }


        /// <summary>
        /// Returns true if the file points to a location on the web
        /// </summary>
        public bool IsWebUri {
            get { return Toolkit.IsWebUri(this.FullPath); }
        }


        /// <summary>
        /// Return the parent Directory for this directory
        /// </summary>
        public Directory Parent {
            get {
                return (this.Info.Parent != null) ? new Directory(this.Info.Parent.FullName) : null;
            }
        }

        /// <summary>
        /// Returns true if this directory is located inside a parent directory, false otherwise
        /// </summary>
        public bool HasParent {
            get {
                if (this.Info.Parent.FullName == this.Info.FullName) return false;
                return true;
            }
        }

        /// <summary>
        /// Returns true if this is a top-level directory
        /// </summary>
        public bool IsRoot {
            get {
                if (this.Info.Parent == this.Info)
                    return true;
                return false;
            }
        }

        /// <summary>
        /// Returns true/false specfiying whether or not the directory is read only
        /// </summary>
        public bool IsReadOnly {
            get {
                return (this.Info.Attributes.ToString().IndexOf("ReadOnly") == -1) ? false : true;
            }
        }

        /// <summary>
        /// Returns true/false specfiying whether or not the directory is hidden
        /// </summary>
        public bool IsHidden {
            get {
                return (this.Info.Attributes.ToString().IndexOf("Hidden") == -1) ? false : true;
            }
        }


        /// <summary>
        /// Returns the DirectoryInfo object for this directory
        /// </summary>
        public DirectoryInfo Info {
            get { return new DirectoryInfo(this.FullPath); }
        }

        public static Directory Current {
            get {
                return Directory.Get(System.Environment.CurrentDirectory + "\\");
            }
        }
        #endregion

        #region -------- PUBLIC OVERRIDE - ToString --------
        /// <summary>
        /// Returns the full path of this directory
        /// </summary>
        /// <returns>The full path of the directory</returns>
        public override string ToString() {
            return this.FullPath;
        }
        #endregion
    }
    
    
    internal static class DirectoryUtil {
        #region -------- PUBLIC - Create --------
        /// <summary>
        /// Create a directory in the specified location. If the directory already exists no action is taken.
        /// </summary>
        /// <param name="path">The absolute path/uri for the directory to create</param>
        public static Directory Create(string path) {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException("The path argument is null/empty!");

            //if (Exists(path))
            //    return new Directory(path);

            try {
                path = System.IO.Path.GetDirectoryName(path);
                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);
                return new Directory(path);
            } catch (Exception ex) {
                throw new IOException("An error occurred while creating the directory. path-> " + path + "\r" + ex.Message, ex);
            }
        }
        #endregion


        #region -------- PUBLIC - Delete --------
        /// <summary>
        /// Delete the specified directory
        /// </summary>
        /// <param name="path">The absolute path/uri for the directory to delete</param>
        public static void Delete(string path) {
            if (!Exists(path))
                return;

            try {
                System.IO.Directory.Delete(path, true);
            } catch (Exception ex) {
                throw new IOException("An error occurred while deleting the directory. path-> " + path + "\r" + ex.Message, ex);
            }
        }
        #endregion

        #region -------- PUBLIC - Exists --------
        /// <summary>
        /// Verifies whether or not the specified directory exists
        /// </summary>
        /// <param name="path">The absolute path/uri to check</param>
        public static bool Exists(string path) {
            if (String.IsNullOrEmpty(path)) throw new ArgumentNullException("The path argument is null/empty!");
            //if (path == null || path.Length == 0) throw new Undertow.NullEmptyException("path");

            try {
                return System.IO.Directory.Exists(path);
            } catch (Exception ex) {
                throw new IOException("An error occurred while checking to see if the directory exists. path->" + path, ex);
            }
        }
        #endregion


        #region -------- PUBLIC - Clear --------
        /// <summary>
        /// If the directory exists remove all of the files it contains
        /// </summary>
        /// <param name="path">The absolute path/uri for the directory to Clear</param>
        public static void Clear(string path) {
            Clear(path, false);
        }

        /// <summary>
        /// If the directory exists remove all of the files it contains
        /// </summary>
        /// <param name="path">The absolute path/uri for the directory to Clear</param>
        /// <param name="recursiveClear">Flag specifying whether or not to delete all of the subdirectories as well. Default is false.</param>
        public static void Clear(string path, bool recursiveClear) {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException("The path argument is null/empty!");

            if (!System.IO.Directory.Exists(path))
                throw new IOException("The directory does not exist! path-> " + path);

            try {
                string[] files = System.IO.Directory.GetFiles(path);
                foreach (string f in files) {
                    FileUtil.Delete(f);
                }
                if (!recursiveClear)
                    return;

                string[] subs = System.IO.Directory.GetDirectories(path);
                foreach (string d in subs) {
                    Delete(d);
                }
            } catch (Exception ex) {
                throw new IOException("An error occurred while deleting the directory contents. path-> " + path + "\r" + ex.Message, ex);
            }
        }
        #endregion



        #region -------- PUBLIC - GetSubdirectories --------
        /// <summary>
        /// Return the subdirectories of the specified directory
        /// </summary>
        /// <param name="path">The absolute path/uri to check</param>
        public static List<Directory> GetSubdirectories(string path) {
            return GetSubdirectories(path, false);
        }
        /// <summary>
        /// Return the subdirectories of the specified directory
        /// </summary>
        /// <param name="path">The absolute path/uri to check</param>
        /// <param name="deepSearch">Flag dictating whether or not to include all the subdirectories in a search operation. Default is false.</param>
        public static List<Directory> GetSubdirectories(string path, bool deepSearch) {
            if (!Exists(path))
                return new List<Directory>(0);

            try {
                SearchOption option = SearchOption.TopDirectoryOnly;
                if (deepSearch)
                    option = SearchOption.AllDirectories;

                string[] subs = System.IO.Directory.GetDirectories(path, "*.*", option);
                List<Directory> directories = new List<Directory>(subs.Length);
                foreach (string sub in subs)
                    directories.Add(new Directory(sub));

                return directories;
            } catch (Exception ex) {
                throw new IOException("An error occurred while fetching subdirectories. path-> " + path + "\r" + ex.Message, ex);
            }
        }
        #endregion



        #region -------- PUBLIC - GetFiles --------
        /// <summary>
        /// Return the files of the specified directory
        /// </summary>
        /// <param name="path">The absolute path/uri to check</param>
        public static List<File> GetFiles(string path) {
            return GetFiles(path, "*.*", false);
        }
        /// <summary>
        /// Return the files of the specified directory
        /// </summary>
        /// <param name="path">The absolute path/uri to check</param>
        /// <param name="includeSubDirectories">Flag specifying whether or not to include subdirectories in the search. Default is false.</param>
        public static List<File> GetFiles(string path, bool includeSubDirectories) {
            return GetFiles(path, "*.*", includeSubDirectories);
        }

        /// <summary>
        /// Return the files of the specified directory. This method can be used to handle multiple patterns.
        /// </summary>
        /// <param name="path">The absolute path/uri to check</param>
        /// <param name="paterns">An array of patterns to match the returning files by</param>
        public static List<File> GetFiles(string path, string[] patterns) {
            return GetFiles(path, patterns, false);
        }
        /// <summary>
        /// Return the files of the specified directory. This method can be used to handle multiple patterns.
        /// </summary>
        /// <param name="path">The absolute path/uri to check</param>
        /// <param name="paterns">An array of patterns to match the returning files by</param>
        /// <param name="includeSubDirectories">Flag specifying whether or not to include subdirectories in the search. Default is false.</param>
        public static List<File> GetFiles(string path, string[] patterns, bool includeSubDirectories) {
            if (!Exists(path))
                return new List<File>(0);

            List<File> list = new List<File>();
            foreach (string pattern in patterns) {
                List<File> group = GetFiles(path, pattern, includeSubDirectories);
                foreach (File file in group) {
                    list.Add(file);
                }
            }

            return list;
        }


        /// <summary>
        /// Return the files of the specified directory
        /// </summary>
        /// <param name="path">The absolute path/uri to check</param>
        /// <param name="pattern">The pattern to filter results by (e.g. *.jpg to return all jpg files. *.jpg,*.bmp would return all jpg and bmp files)</param>
        public static List<File> GetFiles(string path, string pattern) {
            return GetFiles(path, pattern, false);
        }

        /// <summary>
        /// Return the files of the specified directory
        /// </summary>
        /// <param name="path">The absolute path/uri to check</param>
        /// <param name="pattern">The pattern to filter results by (e.g. *.jpg to return all jpg files. *.jpg,*.bmp would return all jpg and bmp files)</param>
        /// <param name="includeSubDirectories">Flag specifying whether or not to include subdirectories in the search. Default is false.</param>
        public static List<File> GetFiles(string path, string pattern, bool includeSubDirectories) {
            if (!Exists(path))
                return new List<File>(0);
            if (pattern == null || pattern.Trim().Length == 0)
                pattern = "*.*";

            if (pattern.IndexOf(',') > -1) {
                string[] patterns = pattern.Replace(" ", "").Split(',');
                return GetFiles(path, patterns, includeSubDirectories);
            }

            try {
                SearchOption option = (includeSubDirectories) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                string[] filePathList = System.IO.Directory.GetFiles(path, pattern, option);
                List<File> files = new List<File>(filePathList.Length);
                foreach (string filePath in filePathList) {
                    File file = new File(filePath);
                    files.Add(file);
                }

                return files;
            } catch (Exception ex) {
                throw new IOException("An error occurred while retrieving a list of the files in the directory.\r\tPath-> " + path + "\r" + ex.Message, ex);
            }
        }
        #endregion



        #region -------- PUBLIC - GetDirectoryContents --------
        /// <summary>
        /// Return a list of all files/directories nested inside the specified directory
        /// </summary>
        /// <param name="path">The absolute path/uri to check</param>
        public static List<IFileSystemEntry> GetDirectoryContents(string path) {
            return GetDirectoryContents(path, false);
        }


        /// <summary>
        /// Return a list of all files/directories nested inside the specified directory
        /// </summary>
        /// <param name="path">The absolute path/uri to check</param>
        /// <param name="recursive">Flag dictating whether or not to include all the subdirectories in a search operation. Default is false.</param>
        public static List<IFileSystemEntry> GetDirectoryContents(string path, bool recursive) {
            if (!Exists(path))
                return new List<IFileSystemEntry>(0);

            List<IFileSystemEntry> entries = new List<IFileSystemEntry>();
            Directory root = Directory.Get(path);
            SearchOption option = (recursive) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            try {
                List<File> files = root.GetFiles();
                foreach (File file in files) {
                    entries.Add(file);
                }

                string[] subs = System.IO.Directory.GetDirectories(path, "*.*", option);
                foreach (string sub in subs) {
                    Directory dir = Directory.Get(Path.Combine(path, sub));
                    entries.Add(dir);
                    files = dir.GetFiles();
                    foreach (File file in files) {
                        entries.Add(file);
                    }
                }
            } catch (Exception ex) {
                throw new IOException("An error occurred while fetching directory contents. path-> " + path + "\r" + ex.Message, ex);
            }
            return entries;
        }
        #endregion


        //        #region -------- PUBLIC - Copy --------
        //        /// <summary>
        //        /// Recursively copy the directory/subdirectories and all files to the specfied location
        //        /// </summary>
        //        /// <param name="source">The absolute path of the directory to copy</param>
        //        /// <param name="destination">The absolute path of target destination of the copied directory</param>
        //        public static void Copy(string sourcePath, string destinationPath, bool overwrite) {
        //            if(String.IsNullOrEmpty(sourcePath)) throw new ArgumentNullException("The sourcePath argument is null/empty!");
        //            if (String.IsNullOrEmpty(destinationPath)) throw new ArgumentNullException("The destinationPath argument is null/empty!");

        //            Directory source = Directory.Get(sourcePath);
        //            Directory destination = Directory.Get(destinationPath);
        //            if(!source.Exists)
        //                throw new IOException("The source directory does not exist!");
        //            try {
        //                destination.Create();

        //                File[] files = source.GetFiles();
        //                foreach (File file in files) {
        //                    if (overwrite) {
        //                        file.Copy(destination.GetFile(file.Name), true);
        //                    } else if (!destination.GetFile(file.Name).Exists) {
        //                        file.Copy(destination.GetFile(file.Name));
        //                    }
        //                }

        //                Directory[] subs = source.GetDirectories();
        //                foreach(Directory sub in subs) {
        //                    Directory subDest = destination.GetDirectory(sub.Name);
        //                    Copy(sub.FullPath, subDest.FullPath, overwrite);
        //                }
        //            } catch(Exception ex) {
        //                throw new IOException("An error occurred while copying the direcory. from \"" + sourcePath + "\" to \"" + destinationPath+"\"", ex);
        //            }
        //        }
        //////		/// <summary>
        //////		/// Copy the directory to the specfiied location
        //////		/// </summary>
        //////		/// <param name="source">The absolute path of the directory to copy</param>
        //////		/// <param name="destination">The absolute path of target destination of the copied directory</param>
        //////		public static void Copy(string source, string destination){
        //////			if(!Exists(source))
        //////				throw new JetStreamException("The specified path does not exist. Source: ",source);
        ////////			if(!Exists(destination))
        ////////				throw new JetStreamException("The specified path does not exist. Destination: ",destination);
        //////			try{
        //////				
        //////			}
        //////			catch(Exception ex){
        //////				throw new JetStreamException("An error occurred while trying to delete the directory. Path: "+path+", Message: "+ex.Message,ex.InnerException);
        //////			}
        //////
        ////////			if(source == null || source.Length == 0 || destination == null || destination.Length == 0)
        ////////				throw new System.IO.IOException(Errors.NullEmptyStringParameter+" Source: "+source+", Destination: "+destination);			
        ////////			if(!System.IO.Directory.Exists(source))
        ////////				throw new System.IO.IOException(Errors.CannotCopyDirectory+" Source: "+source);
        ////////			if(!System.IO.Directory.Exists(destination))
        ////////				Create(destination);
        ////////
        ////////			string[] files = GetFiles(source);
        ////////			int count = files.Length;
        ////////			for(int i = 0; i<count; i++){
        ////////				string target = destination+"\\"+System.IO.Path.GetFileName(files[i]);
        ////////				FileUtil.Copy(files[i],target);
        ////////			}
        //        ////		}
        //        #endregion
    }
}

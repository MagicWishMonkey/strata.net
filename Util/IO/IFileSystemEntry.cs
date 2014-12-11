using System;
using System.Collections;
using System.Collections.Generic;
namespace Strata.Util.IO {
    public interface IFileSystemEntry {
        /// <summary>
        /// Make this file/directory hidden
        /// </summary>
        void Hide();
        /// <summary>
        /// Creates a file/directory entry on disk
        /// </summary>
        void Create();
        /// <summary>
        /// Attempt to create the file/directory, return true if successful false otherwise
        /// </summary>
        bool TryCreate();
        /// <summary>
        /// Delete the file/directory
        /// </summary>
        void Delete();
        /// <summary>
        /// Attempt to delete the file/directory, return true if successful false otherwise
        /// </summary>
        bool TryDelete();
        /// <summary>
        /// Returns the full path to the file/directory. This is an http URL for 
        /// files/directories located on the internet.
        /// </summary>
        string FullPath { get; }
        /// <summary>
        /// Returns the Uri to the file/directory
        /// </summary>
        Uri Uri { get; }
        /// <summary>
        /// Return the name of the file/directory
        /// </summary>
        string Name { get; }
        /// <summary>
        /// A boolean value specifying whether or not the file/directory exists
        /// </summary>
        bool Exists { get; }
        /// <summary>
        /// Returns the DateTime representing the date the file/directory was created
        /// </summary>
        DateTime DateCreated { get; }
        /// <summary>
        /// Returns the DateTime representing the date the file/directory was last written/modified
        /// </summary>
        DateTime DateModified { get; }
        /// <summary>
        /// Returns the DateTime representing the date the file/directory was last read
        /// </summary>
        DateTime DateLastRead { get; }
        /// <summary>
        /// Returns true if the file/directory is read only
        /// </summary>
        bool IsReadOnly { get; }
        /// <summary>
        /// Returns true if the file/directory points to a location on the web
        /// </summary>
        bool IsWebUri { get; }
        /// <summary>
        /// Returns true/false specfiying whether or not the file/directory is hidden
        /// </summary>
        bool IsHidden { get; }
        /// <summary>
        /// Returns the FileInfo object for this file
        /// </summary>
        //System.IO.FileSystemInfo Info { get; }
        
    }
}

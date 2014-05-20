//-----------------------------------------------------------------------------------------
// <copyright file="FileManager.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using CloudPact.MowblyFramework.Core.Log;
using Microsoft.Phone.Storage;
using Newtonsoft.Json;
using SharpGIS;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace CloudPact.MowblyFramework.Core.Managers
{
    public enum StorageType
    {
        Internal = 0,
        External,
        Cache        // cache directory
    };

    public enum FileLevel
    {        
        App = 1,
        Storage
    };

    public enum FileType
    {
        File = 0,
        Directory
    };

    class FileManager
    {
        static ExternalStorageDevice sdCard;

        /// <summary>
        /// Free memory available in SD card. Not supported.
        /// </summary>
        internal static long AvailableExternalMemory { get { return -1; } }

        /// <summary>
        /// Free memory available in internal storage
        /// </summary>
        internal static long AvailableInternalMemory
        { 
            get 
            {
                using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    return file.AvailableFreeSpace;
                }
            } 
        }

        /// <summary>
        /// Total memory available in SD card. Not supported.
        /// </summary>
        internal static long TotalExternalMemory { get { return -1; } }

        /// <summary>
        /// Total memory available in internal storage. Not supported.
        /// </summary>
        internal static long TotalInternalMemory { get { return -1; } }

        /// <summary>
        /// Copies a source directory to specified destination directory.
        /// </summary>
        /// <param name="src">Path of the source directory</param>
        /// <param name="dest">Path of the destination directory</param>
        internal static void CopyDirectory(string src, string dest)
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!storage.DirectoryExists(dest))
                {
                    storage.CreateDirectory(dest);
                }

                // Copy all files.
                string[] files = storage.GetFileNames(src + "\\*.*");
                foreach (string file in files)
                {
                    string srcfile = Path.Combine(src, file);
                    string destfile = Path.Combine(dest, file);
                    // Delete file if exists
                    if (storage.FileExists(destfile))
                    {
                        storage.DeleteFile(destfile);
                    }
                    storage.CopyFile(srcfile, destfile);
                }

                // Process subdirectories.
                string[] dirs = storage.GetDirectoryNames(src + "\\*");
                foreach (string dir in dirs)
                {
                    string destinationDir = Path.Combine(dest, dir);
                    string srcDir = Path.Combine(src, dir);
                    CopyDirectory(srcDir, destinationDir);
                }
            }
        }

        /// <summary>
        /// Creates the directory in the specified path. Parent directories in the path are created if not found.
        /// </summary>
        /// <param name="path">The path of the directory to create.</param>
        internal static void CreateDirectory(string path)
        {
            using (IsolatedStorageFile Storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!Storage.DirectoryExists(path))
                {
                    Storage.CreateDirectory(path);
                }
            }
        }

        /// <summary>
        /// Creates the file in the specified path. Parent directories in the path are created if not found.
        /// </summary>
        /// <param name="path">The path of the file to create.</param>
        internal static void CreateFile(string path)
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!storage.FileExists(path))
                {
                    string parentDir = Path.GetDirectoryName(path);
                    if (parentDir != string.Empty && !storage.DirectoryExists(parentDir))
                    {
                        storage.CreateDirectory(parentDir);
                    }
                    IsolatedStorageFileStream isf = storage.CreateFile(path);
                    isf.Flush();
                    isf.Close();
                    isf.Dispose();
                }
            }
        }

        /// <summary>
        /// Deletes the specified file from storage. 
        /// throws IsolatedStorageException
        /// </summary>
        /// <param name="file"></param>
        internal static void DeleteFile(string file)
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (storage.FileExists(file))
                {
                    storage.DeleteFile(file);
                }
            }
        }

        /// <summary>
        /// Deletes all sub directories and files of the specified directory recursively.
        /// </summary>
        /// <param name="path">Path of the directory to delete</param>
        internal static void DeleteDirectory(string path)
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (storage.DirectoryExists(path))
                {
                    // Get the directories list
                    string[] dirs = storage.GetDirectoryNames(path + "\\*");
                    foreach (string dir in dirs)
                    {
                        DeleteDirectory(Path.Combine(path, dir));
                    }

                    // Get the files list
                    string[] files = storage.GetFileNames(path + "\\*");
                    foreach (string file in files)
                    {
                        storage.DeleteFile(Path.Combine(path, file));
                    }

                    // Delete the directory
                    storage.DeleteDirectory(path);
                }
            }
        }

        /// <summary>
        /// Determines whether the specified path points to an existing directory
        /// </summary>
        /// <param name="path">Path of the directory</param>
        /// <returns>True, if the specified directory exists; False, otherwise</returns>
        internal static bool DirectoryExists(string path)
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                return storage.DirectoryExists(path);
            }
        }

        /// <summary>
        /// Determines whether the specified path points to an existing file
        /// </summary>
        /// <param name="path">Path of the file</param>
        /// <returns>True, if the specified file exists; False, otherwise</returns>
        internal static bool FileExists(string path)
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                return storage.FileExists(path);
            }
        }

        /// <summary>
        /// Creates the file, if not exists.
        /// </summary>
        /// <param name="filePath">
        /// <see cref="CloudPact.MowblyFramework.Core.Managers.FilePath">FilePath</see> object
        /// </param>
        /// <param name="storageType">
        /// </param>
        /// <returns></returns>
        internal static string GetAbsolutePath(FilePath filePath, bool shouldCreateFile = true)
        {
            string rootDir = String.Empty;
            FileLevel level = filePath.Level;
            StorageType storageType = filePath.StorageType;
            string path = filePath.Path;
            if (level != FileLevel.Storage && 
                !path.StartsWith("/") && 
                !path.StartsWith("file://"))
            {
                rootDir = (storageType == StorageType.Cache) ?
                    Mowbly.CacheDirectory : Mowbly.DocumentsDirectory;
            }

            // Get the full path
            path = Path.Combine(rootDir, filePath.Path);

            // Create the file
            if(shouldCreateFile)
            {
                CreateFile(path);
            }
            return path;
        }

        /// <summary>
        /// Checks if there is an external storage and is ready for use.
        /// </summary>
        /// <returns>True, if there is an external device ready for use</returns>
        internal static async Task<ExternalStorageDevice> GetExternalStorageAsync()
        {
            if (sdCard == null)
            {
                IEnumerable<ExternalStorageDevice> devices = 
                    await ExternalStorage.GetExternalStorageDevicesAsync();
                sdCard = devices.FirstOrDefault();
            }

            return sdCard;
        }

        /// <summary>
        /// Returns the list of folders and files in the specified path
        /// </summary>
        /// <param name="path">Path of the directory</param>
        /// <param name="storageType">
        /// <see cref="CloudPact.MowblyFramework.Core.Managers.StorageType">StorageType</see>
        ///  of the directory
        /// </param>
        /// <returns>
        /// <see cref="System.Tuple">Tuple</see> containing the status, list of folders and files
        ///  and error message
        /// </returns>
        internal static async Task<Tuple<bool, List<Dictionary<string, object>>, string>> 
            GetFoldersAndFilesAsync(string path, StorageType storageType)
        {
            bool status = true;
            List<Dictionary<string, object>> fileList = new List<Dictionary<string, object>>();
            string error = String.Empty;
            try
            {
                if (storageType == StorageType.External)
                {
                    ExternalStorageDevice sdCard =
                        await FileManager.GetExternalStorageAsync();
                    if (sdCard != null)
                    {
                        ExternalStorageFolder root = await sdCard.GetFolderAsync(path);
                        IEnumerable<ExternalStorageFolder> folders = await root.GetFoldersAsync();

                        // Get the directories list
                        folders.ToList().ForEach(dir =>
                        {
                            Dictionary<string, object> fileInfo = new Dictionary<string, object>();
                            fileInfo.Add("name", dir.Path);
                            fileInfo.Add("type", FileType.Directory);
                            fileList.Add(fileInfo);
                        });

                        // Get the files list
                        IEnumerable<ExternalStorageFile> files = await root.GetFilesAsync();
                        files.ToList().ForEach(file =>
                        {
                            Dictionary<string, object> fileInfo = new Dictionary<string, object>();
                            fileInfo.Add("name", file.Path);
                            fileInfo.Add("type", FileType.File);
                            fileList.Add(fileInfo);
                        });
                    }
                    else
                    {
                        status = false;
                        error = Constants.STRING_EXTERNAL_STORAGE_ERROR;
                    }
                }
                else if (FileManager.DirectoryExists(path))
                {
                    using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        // Get the directories list
                        string[] files = storage.GetDirectoryNames(path + "\\*");
                        foreach (string file in files)
                        {
                            Dictionary<string, object> fileInfo = new Dictionary<string, object>();
                            fileInfo.Add("name", file);
                            fileInfo.Add("type", FileType.Directory);
                            fileList.Add(fileInfo);
                        }

                        // Get the files list
                        files = storage.GetFileNames(path + "\\*");
                        foreach (string file in files)
                        {
                            Dictionary<string, object> fileInfo = new Dictionary<string, object>();
                            fileInfo.Add("name", file);
                            fileInfo.Add("type", FileType.File);
                            fileList.Add(fileInfo);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                status = false;
                error = e.Message;
                Logger.Error("Error retrieving file list as JSON. Reason - " + error);
            }

            return Tuple.Create(status, fileList, error);
        }

        /// <summary>
        /// Returns the size of the specified file
        /// </summary>
        /// <param name="path">Path of the file</param>
        /// <returns>Size of the file if found; 0 otherwise</returns>
        internal static long GetFileSize(string path)
        {
            long size = 0;
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                try
                {
                    using (IsolatedStorageFileStream stream = storage.OpenFile(path, FileMode.Open, FileAccess.Read))
                    {
                        size = stream.Length;

                        stream.Close();
                        stream.Dispose();
                    }
                }
                catch { }
            }

            return size;
        }

        /// <summary>
        /// Moves the specified directory to the provided destination directory
        /// </summary>
        /// <param name="srcDir"></param>
        /// <param name="destDir"></param>
        internal static void MoveDirectory(string srcDir, string destDir)
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                // Delete destination directory if exists
                if (storage.DirectoryExists(destDir))
                {
                    DeleteDirectory(destDir);
                }

                // Move the directory
                storage.MoveDirectory(srcDir, destDir);
            }
        }

        /// <summary>
        /// Moves the specified file to the provided destination path
        /// </summary>
        /// <param name="srcFile">Path to the source file</param>
        /// <param name="destFile">Path to destination file</param>
        internal static void MoveFile(string srcFile, string destFile)
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                // Delete destination file if exists
                if (storage.FileExists(destFile))
                {
                    storage.DeleteFile(destFile);
                }

                // Move the file
                storage.MoveFile(srcFile, destFile);
            }
        }

        /// <summary>
        /// Reads the binary data of file.
        /// </summary>
        /// <param name="path">Path of the file to read</param>
        /// <returns>File content as a byte array</returns>
        internal static byte[] ReadAsBytes(string path)
        {
            using (MemoryStream mstream = new MemoryStream())
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (storage.FileExists(path))
                    {
                        using (BinaryReader stream = new BinaryReader(storage.OpenFile(path, FileMode.Open, FileAccess.Read)))
                        {
                            byte[] buf = new byte[1024];
                            int numRead = 0;
                            do
                            {
                                numRead = stream.Read(buf, 0, buf.Length);
                                if (numRead > 0) mstream.Write(buf, 0, numRead);
                            } while (numRead > 0);

                            // Close stream
                            stream.Close();
                            stream.Dispose();
                        }
                    }
                }
                return mstream.ToArray();
            }
        }

        /// <summary>
        /// Returns input stream of the specified file
        /// </summary>
        /// <param name="path">Path of the file to read</param>
        /// <returns>Input stream of the file</returns>
        internal static IInputStream ReadAsStream(string path)
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                var stream = storage.OpenFile(path, FileMode.Open);
                stream.Seek(0, SeekOrigin.Begin);
                return stream.AsInputStream();
            }
        }

        /// <summary>
        /// Reads text file data.
        /// </summary>
        /// <param name="path">Path of the file to read</param>
        /// <returns>Contents of the file as string</returns>
        internal static string ReadAsString(string path)
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (StreamReader reader = new StreamReader(storage.OpenFile(path, FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Overwrites and saves the specified data to isolated storage
        /// </summary>
        /// <param name="filePath">The path of the file to write the data.</param>
        /// <param name="data">The byte data to write into the file.</param>
        internal static void WriteDataToFile(string filePath, byte[] data, bool isAppend)
        {
            if (data != null)
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    FileMode mode = (isAppend) ? FileMode.Append : FileMode.Create;
                    using (BinaryWriter bw = new BinaryWriter(storage.OpenFile(filePath, mode, FileAccess.Write)))
                    {
                        bw.Write(data, 0, data.Length);
                        bw.Close();
                        bw.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// Overwrite and saves the specified stream to isolated storage
        /// </summary>
        /// <param name="filePath">The path of the file to write the data.</param>
        /// <param name="data">The byte data to write into the file.</param>
        internal static void WriteDataToFile(string filePath, Stream stream, bool isAppend)
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                FileMode mode = (isAppend) ? FileMode.Append : FileMode.Create;
                byte[] data = new byte[1024];
                int numRead = 0;
                using (BinaryWriter bw = new BinaryWriter(storage.OpenFile(filePath, mode, FileAccess.Write)))
                {
                    if (stream.CanSeek && stream.CanRead)
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        using (BinaryReader br = new BinaryReader(stream))
                        {
                            do
                            {
                                numRead = br.Read(data, 0, data.Length);
                                if (numRead > 0)
                                {
                                    bw.Write(data, 0, numRead);
                                    bw.Flush();
                                }
                            } while (numRead > 0);
                            bw.Close();
                            br.Close();
                            bw.Dispose();
                            br.Dispose();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// OverWrites string data to file opening it with specified mode
        /// </summary>
        /// <param name="filePath">Path of the file to write content in</param>
        /// <param name="data">String data to write in the file</param>
        /// <param name="mode">Mode in which the file to be opened</param>
        internal static void WriteStringToFile(string filePath, string data, bool isAppend)
        {
            FileMode mode = (isAppend) ? FileMode.Append : FileMode.Create;
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (StreamWriter stream = new StreamWriter(storage.OpenFile(filePath, mode, FileAccess.Write, FileShare.Read)))
                {
                    stream.Write(data);

                    // Close stream
                    stream.Close();
                    stream.Dispose();
                }
            }
        }

        /// <summary>
        /// Unzips a given zip file into the path specified.Path can be directory
        /// or file.
        /// throws Exception
        /// </summary>
        /// <param name="ZipFilePath"></param>
        /// <param name="PathToExtract"></param>
        internal static void unzip(string ZipFilePath, string PathToExtract)
        {
            IsolatedStorageFile storage = null;
            IsolatedStorageFileStream stream = null;
            string oldfilePath = null;
            string newfilePath = null;
            try
            {
                using (storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (storage.FileExists(ZipFilePath))
                    {
                        // Create target directory, if not exists
                        CreateDirectory(PathToExtract);

                        using (stream = new IsolatedStorageFileStream(ZipFilePath, FileMode.Open,
                            FileAccess.Read, storage))
                        {
                            if (stream != null)
                            {
                                UnZipper unzip = new UnZipper(stream);
                                foreach (string filename in unzip.FileNamesInZip)
                                {
                                    newfilePath = PathToExtract + "\\" + filename;
                                    oldfilePath = ZipFilePath + "\\" + filename;

                                    // Logger.Info("Unzipping  " + newfilePath);

                                    if (filename.EndsWith("/"))
                                    {
                                        newfilePath = newfilePath.Substring(0, newfilePath.LastIndexOf("/"));
                                        storage.CreateDirectory(newfilePath);
                                    }
                                    else
                                    {
                                        //it is a file
                                        if (filename.IndexOf("/") > -1)
                                        {
                                            string dirPath = newfilePath.Substring(0, newfilePath.LastIndexOf("/"));
                                            if (!storage.DirectoryExists(dirPath))
                                            {
                                                storage.CreateDirectory(dirPath);
                                            }
                                        }

                                        byte[] data = null;
                                        Stream fileStream = unzip.GetFileStream(filename);
                                        if (fileStream != null)
                                        {
                                            using (BinaryReader br = new BinaryReader(fileStream))
                                            {
                                                data = br.ReadBytes((int)fileStream.Length);
                                            }

                                            //Remove if already exists
                                            if (storage.FileExists(newfilePath))
                                            {
                                                storage.DeleteFile(newfilePath);
                                            }

                                            using (BinaryWriter bw = new BinaryWriter(storage.CreateFile(newfilePath)))
                                            {
                                                bw.Write(data);
                                                bw.Close();
                                                bw.Dispose();
                                            };
                                            try
                                            {
                                                fileStream.Close();
                                                fileStream.Dispose();
                                            }
                                            catch { }
                                        }
                                        else
                                        {
                                            Logger.Warn("File is empty: " + filename);
                                            storage.CreateFile(newfilePath);
                                        }
                                    }
                                    // Logger.Info("Done");
                                }
                                try
                                {
                                    stream.Close();
                                    stream.Dispose();

                                    unzip.Dispose();
                                }
                                catch { }
                            }
                            else
                            {
                                string msg = "Empty zip file stream. Reason - Stream is empty";
                                throw new Exception(msg);
                            }
                        }
                    }
                    else
                    {
                        string msg = String.Format("Cannot unzip file. Invalid path : {0}", ZipFilePath);
                        throw new Exception(msg);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("Unzip failed. Reason - " + e.Message);
                throw e;
            }
            finally
            {
                try
                {
                    storage.Dispose();
                }
                catch { }
            }
        }

        #region App helpers

        /// <summary>
        /// Delete the app cache directory
        /// </summary>
        internal static void ClearAppCacheDir()
        {
            Logger.Debug("Clearing cache dir...");
            try
            {                
                DeleteDirectory(Mowbly.CacheDirectory);
            }
            catch (Exception e)
            {
                Logger.Error("Clearing cache dir failed. Reason - " + e.Message);
            }
        }

        /// <summary>
        /// Clear Mowbly directories
        /// </summary>
        internal static void Reset()
        {
            DeleteDirectory(Mowbly.AppDirectory);
            DeleteDirectory(Mowbly.CacheDirectory);
            DeleteDirectory(Mowbly.DocumentsDirectory);
            DeleteDirectory(Mowbly.TmpDirectory);
        }

        #endregion
    }

    #region FilePath

    [JsonObject(MemberSerialization.OptIn)]
    public sealed class FilePath
    {
        const string KEY_LEVEL = "level";
        const string KEY_PATH = "path";
        const string KEY_STORAGE_TYPE = "storageType";

        [JsonProperty(
            PropertyName = KEY_PATH,
            Required = Required.Default)]
        public string Path { get; set; }

        [JsonProperty(
            PropertyName = KEY_STORAGE_TYPE,
            Required = Required.Default)]
        public StorageType StorageType { get; set; }

        [JsonProperty(
            PropertyName = KEY_LEVEL,
            Required = Required.Default)]
        public FileLevel Level { get; set; }
    }

    #endregion
}
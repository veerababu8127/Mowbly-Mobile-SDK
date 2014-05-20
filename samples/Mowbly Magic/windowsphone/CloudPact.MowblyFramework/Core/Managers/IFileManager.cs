//-----------------------------------------------------------------------------------------
// <copyright file="IFileManager.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using Microsoft.Phone.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CloudPact.MowblyFramework.Core.Managers
{
    interface IFileManager
    {
        void CopyDirectory(string src, string dest);

        void CreateDirectory(string path);

        void CreateFile(string path);

        void DeleteFile(string file);

        void DeleteDirectory(string path);

        Task DeleteDirectoryAsync(string path);

        bool DirectoryExists(string path);

        bool FileExists(string path);

        string GetAbsolutePath(FilePath filePath);

        Task<ExternalStorageDevice> GetExternalStorageAsync();

        Task<Tuple<bool, List<Dictionary<string, object>>, string>>
            GetFoldersAndFiles(string path, StorageType storageType);

        void MoveDirectory(string srcDir, string destDir);

        void MoveFile(string srcFile, string destFile);

        byte[] ReadAsBytes(string filePath);

        string ReadAsString(string path);

        void Reset();

        void WriteDataToFile(string filePath, byte[] data, bool isAppend);

        void WriteDataToFile(string filePath, Stream stream, bool isAppend);

        void WriteStringToFile(string filePath, string data, bool isAppend);

        void unzip(string ZipFilePath, string PathToExtract);
    }
}

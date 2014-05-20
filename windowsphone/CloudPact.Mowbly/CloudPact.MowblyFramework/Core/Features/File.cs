//-----------------------------------------------------------------------------------------
// <copyright file="File.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using CloudPact.MowblyFramework.Core.Log;
using CloudPact.MowblyFramework.Core.Managers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace CloudPact.MowblyFramework.Core.Features
{
    class File : Feature
    {
        const long DEFAULT_INTERNAL_MEM = 0;

        const long DEFAULT_CACHE_MEM = 0;

        #region Feature

        /// <summary>
        /// Name of the feature
        /// </summary>
        internal override string Name { get { return Constants.FEATURE_FILE; } }

        /// <summary>
        /// Invoke the method specified by the 
        /// <see cref="CloudPact.MowblyFramework.Core.Features.JSMessage">JSMessage</see> object
        /// </summary>
        /// <param name="message">
        /// <see cref="CloudPact.MowblyFramework.Core.Features.JSMessage">JSMessage</see> object
        /// </param>
        internal async override void InvokeAsync(JSMessage message)
        {
            string callbackId = message.CallbackId;
            
            FilePath options = 
                JsonConvert.DeserializeObject<FilePath>(message.Args[message.Args.Length - 1].ToString());
            
            // Set the file path in FilePath options
            // Normal file operations get path as first parameter and options as second
            // Unzip method receives a file object with path in it already.
            if (!message.Method.Equals("unzip"))
            {
                options.Path = message.Args[0] as string;
            }
            
            
            string path;

            try
            {
                switch (message.Method)
                {
                    case "deleteDirectory":
                        // Get the absolute path 
                        path = FileManager.GetAbsolutePath(options, false);

                        // Delete directory
                        try
                        {
                            FileManager.DeleteDirectory(path);
                            InvokeCallbackJavascript(callbackId, new MethodResult { Result = true });
                        }
                        catch (Exception e)
                        {
                            InvokeCallbackJavascript(callbackId, new MethodResult
                            {
                                Code = MethodResult.FAILURE_CODE,
                                Result = false,
                                Error = new MethodError
                                {
                                    Message = e.Message
                                }
                            });
                        }
                        break;

                    case "deleteFile":
                        
                        // Get the absolute path 
                        //Open : Ask
                        path = FileManager.GetAbsolutePath(options, false);
                        
                        // Delete file
                        try
                        {
                            FileManager.DeleteFile(path);
                            InvokeCallbackJavascript(callbackId, new MethodResult { Result = true });
                        }
                        catch (Exception e)
                        {
                            InvokeCallbackJavascript(callbackId, new MethodResult
                            {
                                Code = MethodResult.FAILURE_CODE,
                                Result = false,
                                Error = new MethodError
                                {
                                    Message = e.Message
                                }
                            });
                        }
                        break;

                    case "getDirectory":
                        
                        // Get the absolute path 
                        path = FileManager.GetAbsolutePath(options);

                        // Create directory
                        try
                        {
                            FileManager.CreateDirectory(path);
                            InvokeCallbackJavascript(callbackId, new MethodResult { Result = true });
                        }
                        catch (Exception e)
                        {
                            InvokeCallbackJavascript(callbackId, new MethodResult
                            {
                                Code = MethodResult.FAILURE_CODE,
                                Result = false,
                                Error = new MethodError
                                {
                                    Message = e.Message
                                }
                            });
                        }
                        break;

                    case "getFile":

                        // Get the absolute path 
                        path = FileManager.GetAbsolutePath(options);

                        // Create file
                        try
                        {
                            FileManager.CreateFile(path);
                            InvokeCallbackJavascript(callbackId, new MethodResult { Result = true });
                        }
                        catch (Exception e)
                        {
                            InvokeCallbackJavascript(callbackId, new MethodResult
                            {
                                Code = MethodResult.FAILURE_CODE,
                                Result = false,
                                Error = new MethodError
                                {
                                    Message = e.Message
                                }
                            });
                        }
                        break;

                    case "getFilesJSONString":
                        
                        // Get the absolute path 
                        path = FileManager.GetAbsolutePath(options, false);

                        // Get files list
                        Tuple<bool, List<Dictionary<string, object>>, string> getFilesResult =
                            await FileManager.GetFoldersAndFilesAsync(path, options.StorageType);
                        if (getFilesResult.Item1)
                        {
                            InvokeCallbackJavascript(callbackId, new MethodResult
                            {
                                Result = getFilesResult.Item2
                            });
                        }
                        else
                        {
                            InvokeCallbackJavascript(callbackId, new MethodResult
                            {
                                Code = MethodResult.FAILURE_CODE,
                                Result = false,
                                Error = new MethodError
                                {
                                    Message = getFilesResult.Item3
                                }
                            });
                        }
                        break;

                    case "getRootDirectory":
                        
                        // Get the absolute path 
                        path = FileManager.GetAbsolutePath(options, false);

                        // Return root directory
                        InvokeCallbackJavascript(callbackId, new MethodResult { Result = path });
                        break;

                    case "read":
                        
                        // Get the absolute path 
                        path = FileManager.GetAbsolutePath(options,false);

                        // Read file
                        try
                        {
                            string fileContentStr = FileManager.ReadAsString(path);
                            InvokeCallbackJavascript(callbackId, new MethodResult { Result = fileContentStr });
                        }
                        catch (Exception e)
                        {
                            Logger.Error("File not found" + e.Message);
                            InvokeCallbackJavascript(callbackId, new MethodResult
                            {
                                Code = MethodResult.FAILURE_CODE,
                                Result = false,
                                Error = new MethodError
                                {
                                    Message = Mowbly.GetString(Constants.STRING_FILE_NOT_FOUND)
                                }
                            });
                        }
                        break;

                    case "readData":
                        
                        // Get the absolute path 
                        path = FileManager.GetAbsolutePath(options, false);

                        // Read file
                        try
                        {
                            string fileContentData = Convert.ToBase64String(FileManager.ReadAsBytes(path));
                            InvokeCallbackJavascript(callbackId, new MethodResult { Result = fileContentData });
                        }
                        catch (Exception e)
                        {
                            Logger.Error("File not found" + e.Message);
                            InvokeCallbackJavascript(callbackId, new MethodResult
                            {
                                Code = MethodResult.FAILURE_CODE,
                                Result = false,
                                Error = new MethodError
                                {
                                    Message = Mowbly.GetString(Constants.STRING_FILE_NOT_FOUND)
                                }
                            });
                        }
                        break;

                    case "testDirExists":
                        
                        // Get the absolute path 
                        path = FileManager.GetAbsolutePath(options,false);

                        // Test dir exists
                        try
                        {
                            bool isDirExists = FileManager.DirectoryExists(path);
                            InvokeCallbackJavascript(callbackId, new MethodResult { Result = isDirExists });
                        }
                        catch (Exception e)
                        {
                            InvokeCallbackJavascript(callbackId, new MethodResult
                            {
                                Code = MethodResult.FAILURE_CODE,
                                Result = false,
                                Error = new MethodError
                                {
                                    Message = e.Message
                                }
                            });
                        }
                        break;

                    case "testFileExists":
                        
                        // Get the absolute path 
                        path = FileManager.GetAbsolutePath(options, false);

                        // Test file exists
                        try
                        {
                            bool isFileExists = FileManager.FileExists(path);
                            InvokeCallbackJavascript(callbackId, new MethodResult { Result = isFileExists });
                        }
                        catch (Exception e)
                        {
                            InvokeCallbackJavascript(callbackId, new MethodResult
                            {
                                Code = MethodResult.FAILURE_CODE,
                                Result = false,
                                Error = new MethodError
                                {
                                    Message = e.Message
                                }
                            });
                        }
                        break;

                    case "unzip":
                        
                        // Get the absolute path 
                        path = FileManager.GetAbsolutePath(options, false);
                        
                        // Source file
                        FilePath srcFileOptions = JsonConvert.DeserializeObject<FilePath>(message.Args[0].ToString());
                        string srcFilePath = FileManager.GetAbsolutePath(srcFileOptions, false);

                        if (!FileManager.FileExists(srcFilePath))
                        {
                            throw new Exception(Mowbly.GetString(Constants.STRING_FILE_NOT_FOUND));
                        }   

                        // Unzip
                        try
                        {
                            FileManager.unzip(srcFilePath, path);
                            InvokeCallbackJavascript(callbackId, new MethodResult { Result = true });
                        }
                        catch (Exception e)
                        {
                            InvokeCallbackJavascript(callbackId, new MethodResult
                            {
                                Code = MethodResult.FAILURE_CODE,
                                Result = false,
                                Error = new MethodError
                                {
                                    Message = e.Message
                                }
                            });
                        }
                        break;

                    case "write":
                        
                        // Get the absolute path 
                        path = FileManager.GetAbsolutePath(options);

                        // Write content to file
                        try
                        {
                            string fileContentStr = message.Args[1] as string;
                            bool shouldAppendContent = (bool)message.Args[2];
                            FileManager.WriteStringToFile(path, fileContentStr, shouldAppendContent);
                            InvokeCallbackJavascript(callbackId, new MethodResult { Result = true });
                        }
                        catch (Exception e)
                        {
                            InvokeCallbackJavascript(callbackId, new MethodResult
                            {
                                Code = MethodResult.FAILURE_CODE,
                                Result = false,
                                Error = new MethodError
                                {
                                    Message = e.Message
                                }
                            });
                        }
                        break;

                    case "writeData":
                        
                        // Get the absolute path 
                        path = FileManager.GetAbsolutePath(options);

                        // Write content to file
                        try
                        {
                            byte[] fileContentBytes = Convert.FromBase64String(message.Args[0] as string);
                            bool shouldAppendContent = (bool)message.Args[1];
                            FileManager.WriteDataToFile(path, fileContentBytes, shouldAppendContent);
                            InvokeCallbackJavascript(callbackId, new MethodResult { Result = true });
                        }
                        catch (Exception e)
                        {
                            InvokeCallbackJavascript(callbackId, new MethodResult
                            {
                                Code = MethodResult.FAILURE_CODE,
                                Result = false,
                                Error = new MethodError
                                {
                                    Message = e.Message
                                }
                            });
                        }
                        break;

                    default:
                        Logger.Error("Feature " + Name + " does not support method " + message.Method);
                        break;
                }
            }
            catch (Exception ce)
            {
                Logger.Error("Exception occured. Reason - " + ce.Message);
            }
        }

        #endregion
    }
}
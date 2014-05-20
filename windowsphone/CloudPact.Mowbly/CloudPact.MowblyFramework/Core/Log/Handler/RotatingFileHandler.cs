//-----------------------------------------------------------------------------------------
// <copyright file="RotatingFileHandler.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using CloudPact.MowblyFramework.Core.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPact.MowblyFramework.Core.Log.Handler
{
    class RotatingFileHandler : FileLogHandler
    {
        private int maxFilesCount;

        private long maxFileSize;

        public RotatingFileHandler(string filePath)
            : base(filePath)
        {
            this.maxFileSize = 10 * 1024 * 1024;
            this.maxFilesCount = 1;
        }

        public override void Handle(LogEvent e)
        {
            if (this.Writer != null)
            {
                base.Handle(e);
                Stream stream = this.Writer.BaseStream;
                if (stream.Length >= maxFileSize)
                {
                    Rotate();
                }
            }
        }

        public override void Reset()
        {
            if (maxFilesCount > 0)
            {
                string path;
                for (int i = 0; i < maxFilesCount; i++)
                {
                    path = this.FilePath + "." + i;
                    try
                    {
                        FileManager.DeleteFile(path);
                    }
                    catch (Exception e)
                    {
                        Logger.Error("Error resetting rotating log file. Reason - " + e.Message);
                    }
                }
            }

            base.Reset();
        }

        private void Rotate()
        {
            if (maxFilesCount > 0)
            {
                // Close writer
                Writer.Flush();
                Writer.Close();

                // Delete the oldest log file
                bool isRotateSuccess = true;
                string oldestLogFile = this.FilePath + "." + maxFilesCount;
                try
                {
                    FileManager.DeleteFile(oldestLogFile);
                }
                catch (Exception e)
                {
                    // Error deleting file. Truncate and continue logging
                    Logger.Error("Error deleting rotating log file. Reason - " + e.Message);
                    isRotateSuccess = false;
                }

                // Rotate the files
                string srcPath, destPath;
                for (int i=maxFilesCount-1; i>=0 && isRotateSuccess; --i)
                {
                    srcPath = (i > 0) ? (this.FilePath + "." + i) : (this.FilePath);
                    destPath = this.FilePath + "." + (i + 1);
                    try
                    {
                        FileManager.MoveFile(srcPath, destPath);
                        isRotateSuccess = true;
                    }
                    catch (Exception e)
                    {
                        Logger.Error("Error rotating log file. Reason - " + e.Message);
                        isRotateSuccess = false;
                    }
                }
            }

            // Truncate the file
            InitializeWriter(true);
        }
    }
}
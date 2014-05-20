//-----------------------------------------------------------------------------------------
// <copyright file="FileLogHandler.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using CloudPact.MowblyFramework.Core.Managers;
using System;
using System.IO;
using System.IO.IsolatedStorage;

namespace CloudPact.MowblyFramework.Core.Log.Handler
{
    class FileLogHandler : AbstractLogHandler
    {
        internal const string LOG_HANDLER_FILE_NAME = "__file_log_handler";

        string filePath;

        internal StreamWriter Writer;

        object writelock = new object();

        internal string FilePath
        {
            get
            {
                return filePath;
            }
            set
            {
                filePath = value;
                InitializeWriter();
            }
        }

        public FileLogHandler(string filePath)
            : base()
        {
            this.Name = LOG_HANDLER_FILE_NAME;
            this.FilePath = filePath;
        }

        internal void InitializeWriter(bool truncate = false)
        {
            try
            {
                IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
                if (!storage.FileExists(this.filePath))
                {
                    FileManager.CreateFile(filePath);
                }
                Stream stream = (truncate) ? 
                    storage.OpenFile(filePath, FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite)  : 
                    storage.OpenFile(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                if (Writer != null)
                {
                    Writer.Flush();
                    Writer.Close();
                    Writer.Dispose();
                }
                Writer = new StreamWriter(stream);

                isDisposed = false;
            }
            catch(Exception e)
            {
                Logger.Warn("Error initializing FileLogHandler. Reason " + e.Message);
            }
        }

        #region ILogHandler

        public override void Start()
        {
            // Setup - Initialize writer
            InitializeWriter();

            // Write header
            String header = Formatter.getHeader();
            if (header != null && Writer != null)
            {
                Writer.Write(header);
                Writer.Flush();
            }
        }

        public override void Handle(LogEvent e)
        {
            if (Writer != null)
            {
                lock (writelock)
                {
                    Writer.Write(this.Formatter.Format(e));
                    Writer.Flush();
                }
            }
        }

        public override void Shutdown()
        {
            if (Writer != null)
            {
                // Write footer
                String footer = Formatter.getFooter();
                if (footer != null)
                {
                    Writer.Write(footer);
                    Writer.Flush();
                }

                // Dispose
                try
                {
                    Writer.Dispose();
                    Writer = null;
                }
                catch { }
            }
        }

        public override void Reset()
        {
            if (Writer != null)
            {
                lock (writelock)
                {
                    // Clear the log file
                    try
                    {
                        // Dispose
                        Dispose();

                        // Delete the file
                        FileManager.DeleteFile(this.FilePath);
                    }
                    catch (Exception e)
                    {
                        Logger.Warn("Error clearing FileLog. Reason " + e.Message);
                    }
                }
            }

            InitializeWriter(true);
        }

        #endregion

        #region Server synchronization

        internal Stream GetLogFileContent()
        {
            lock (writelock)
            {
                try
                {
                    IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
                    return storage.OpenFile(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                }
                catch (Exception e) { throw e; }
            }
        }

        #endregion

        #region IDisposable

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Usage", 
            "CA2213:DisposableFieldsShouldBeDisposed", 
            MessageId = "Writer")]
        public override void Dispose()
        {
            try
            {
                if (!isDisposed)
                {
                    if (Writer != null)
                    {
                        Shutdown();
                    }
                    isDisposed = true;
                }
            }
            catch {}
        }

        #endregion
    }
}
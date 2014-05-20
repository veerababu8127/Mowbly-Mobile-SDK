//-----------------------------------------------------------------------------------------
// <copyright file="CameraTask.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using Microsoft.Phone.Tasks;
using System;

namespace CloudPact.MowblyFramework.Core.Features.CameraFeature
{
    internal enum CameraTaskType
    {
        CameraCapture = 0,
        PhotoChooser
    }

    /// <summary>
    /// Class that encapsulates the state of the camera getPicture operation
    /// </summary>
    class CameraTask
    {
        string callbackId;

        CameraOptions options;

        /// <summary>
        /// Name of the camera task
        /// </summary>
        internal virtual string Name { get { return String.Empty; } }

        /// <summary>
        /// Type of the camera task
        /// </summary>
        internal virtual CameraTaskType Type { get { return CameraTaskType.CameraCapture; } }

        /// <summary>
        /// Callback id of the getPicture call from Mowbly JS layer
        /// </summary>
        internal string CallbackId { get { return callbackId; }}

        /// <summary>
        /// User options of the getPicture call from Mowbly JS layer
        /// </summary>
        internal CameraOptions Options { get { return options; }}

        /// <summary>
        /// Event raised after the getPicture operation is completed
        /// </summary>
        internal event EventHandler<PhotoResult> Completed;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="callbackId">Callback id of the getPicture operation</param>
        /// <param name="options">User options of the getPicture operation</param>
        internal CameraTask(string callbackId, CameraOptions options)
        {
            this.callbackId = callbackId;
            this.options = options;
        }

        /// <summary>
        /// Shows the native phone task
        /// </summary>
        internal virtual void Show() { }

        /// <summary>
        /// Event handler for the native phone task
        /// </summary>
        /// <param name="sender">
        /// <see cref="CloudPact.MowblyFramework.Core.Features.CameraFeature.CameraTask">
        /// CameraTask</see> object that triggered the event
        /// </param>
        /// <param name="e">
        /// <see cref="Microsoft.Phone.Tasks.PhotoResult">PhotoResult</see> object resulted 
        ///  from the native phone task
        /// </param>
        internal void OnNativeTaskCompleted(object sender, PhotoResult e)
        {
            if (this.Completed != null)
            {
                Completed(this, e);
            }
        }
    }

    /// <summary>
    /// Camera capture task
    /// </summary>
    class MowblyCameraCaptureTask : CameraTask
    {
        CameraCaptureTask cameraCaptureTask;

        internal override string Name { get { return Constants.KEY_CAMERA_CAPTURE_TASK; } }

        internal override CameraTaskType Type { get { return CameraTaskType.CameraCapture; } }

        internal MowblyCameraCaptureTask(string callbackId, CameraOptions options)
            : base(callbackId, options)
        {
            cameraCaptureTask = new CameraCaptureTask();
            cameraCaptureTask.Completed += OnNativeTaskCompleted;
        }

        ~MowblyCameraCaptureTask()
        {
            cameraCaptureTask.Completed -= OnNativeTaskCompleted;
            cameraCaptureTask = null;
        }

        internal override void Show()
        {
            cameraCaptureTask.Show();
        }
    }

    /// <summary>
    /// Photo chooser task
    /// </summary>
    class MowblyPhotoChooserTask : CameraTask
    {
        PhotoChooserTask photoChooserTask;

        internal override string Name { get { return Constants.KEY_PHOTO_CHOOSER_TASK; } }

        internal override CameraTaskType Type { get { return CameraTaskType.PhotoChooser; } }

        internal MowblyPhotoChooserTask(string callbackId, CameraOptions options)
            : base(callbackId, options)
        {
            photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += OnNativeTaskCompleted;
        }

        ~MowblyPhotoChooserTask()
        {
            photoChooserTask.Completed -= OnNativeTaskCompleted;
            photoChooserTask = null;
        }

        internal override void Show()
        {
            photoChooserTask.Show();
        }
    }
}

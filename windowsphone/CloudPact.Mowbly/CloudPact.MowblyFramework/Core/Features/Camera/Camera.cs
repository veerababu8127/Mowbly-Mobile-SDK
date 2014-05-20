//-----------------------------------------------------------------------------------------
// <copyright file="Camera.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using CloudPact.MowblyFramework.Core.Features.CameraFeature;
using CloudPact.MowblyFramework.Core.Log;
using CloudPact.MowblyFramework.Core.Managers;
using CloudPact.MowblyFramework.Core.Ui;
using CloudPact.MowblyFramework.Core.Utils;
using ExifLib;
using Microsoft.Devices;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Windows.Phone.Media.Capture;

namespace CloudPact.MowblyFramework.Core.Features
{
    class Camera : Feature
    {
        #region Feature

        /// <summary>
        /// Name of the feature
        /// </summary>
        internal override string Name { get { return Constants.FEATURE_CAMERA; } }

        /// <summary>
        /// Invoke the method specified by the 
        /// <see cref="CloudPact.MowblyFramework.Core.Features.JSMessage">JSMessage</see> object
        /// </summary>
        /// <param name="message">
        /// <see cref="CloudPact.MowblyFramework.Core.Features.JSMessage">JSMessage</see> object
        /// </param>
        internal async override void InvokeAsync(JSMessage message)
        {
            switch (message.Method)
            {
                case "getPicture":
                    int source = Convert.ToInt32(message.Args[0]);
                    CameraOptions options = 
                        JsonConvert.DeserializeObject<CameraOptions>(message.Args[1].ToString());
                    string callbackId = message.CallbackId;
                    try
                    {
                        if (source == (int)Camera.Source.CAMERA)
                        {
                            FilePath filePath = options.FilePath;
                            bool isWriteToGallery = (filePath == null);
                        }

                        // Create the CameraTask
                        CameraTask cameraTask;
                        if (source == (int)Camera.Source.CAMERA)
                        {
                            Logger.Debug("Launching camera...");
                            cameraTask = new MowblyCameraCaptureTask(callbackId, options);
                        }
                        else
                        {
                            Logger.Debug("Launching photo chooser...");
                            cameraTask = new MowblyPhotoChooserTask(callbackId, options);
                        }

                        // Subscribe to CameraTask Completed event
                        cameraTask.Completed += OnCameraTaskCompleted;

                        // Show the CameraTask
                        UiDispatcher.BeginInvoke(() =>
                        {
                            cameraTask.Show();
                        });

                        // Make a note that app is navigated to external task
                        Mowbly.AppNavigatedToExternalPage = true;
                    }
                    catch (Exception ce)
                    {
                        Logger.Error("Exception has occured. Reason - " + ce.Message);
                    }

                    break;
                case "getConfiguration":
                    List<CameraConfig> cameraConfigList = await GetCameraConfigurations();
                    MethodResult result = cameraConfigList.Count > 0 ?
                        new MethodResult
                        {
                            Result = cameraConfigList
                        } :
                        new MethodResult
                        {
                            Code = MethodResult.FAILURE_CODE,
                            Error = new MethodError
                            {
                                Message = Mowbly.GetString(Constants.STRING_CAMERA_INITIALIZATION_ERROR)
                            }
                        };
                    InvokeCallbackJavascript(message.CallbackId, result);
                    break;
                default:
                    Logger.Error("Feature " + Name + " does not support method " + message.Method);
                    break;
            }
        }

        #endregion

        #region Camera

        // Constants
        internal const int DEFAULT_HEIGHT = 480;
        internal const int DEFAULT_WIDTH = 320;
        internal const string KEY_CAMERA_CONFIG = "camera_config";
        internal const string KEY_DATA = "data";
        internal const string KEY_FILE_PATH = "filePath";
        internal const string KEY_FLASH = "flash";
        internal const string KEY_HEIGHT = "height";
        internal const string KEY_PATH = "path";
        internal const string KEY_QUALITY = "quality";
        internal const string KEY_READ_DATA = "readData";
        internal const string KEY_RESOLUTIONS = "resolutions";
        internal const string KEY_TYPE = "type";
        internal const string KEY_WIDTH = "width";

        /// <summary>
        /// Source enumeration
        /// </summary>
        internal enum Source
        {
            PHOTO_LIBRARY = 0,
            CAMERA,
            CAMERA_ROLL
        };

        /// <summary>
        /// Output quality of the photo
        /// </summary>
        internal enum Quality
        {
            QUALITY_LOW = 25,
            QUALITY_MEDIUM = 75,
            QUALITY_HIGH = 100
        };

        /// <summary>
        /// Output format of the photo
        /// </summary>
        internal enum Format
        {
            TYPE_JPG = 0,
            TYPE_PNG
        }

        #region Camera configuration

        // Gets the configuration of the specified camera
        private async Task<CameraConfig> GetCameraConfiguration(
            CameraType cameraType, CameraSensorLocation sensorLocation)
        {
            CameraConfig cameraConfig = null;
            PhotoCaptureDevice photoCaptureDevice = null;
            FlashState flash = FlashState.Off;
            List<CameraResolution> resolutions = new List<CameraResolution>();
            try
            {
                // Read supported resolutions
                System.Collections.Generic.IReadOnlyList<Windows.Foundation.Size> SupportedResolutions =
                        PhotoCaptureDevice.GetAvailableCaptureResolutions(sensorLocation);
                resolutions = (from resolution in SupportedResolutions
                         select new CameraResolution
                         {
                             Width = resolution.Width,
                             Height = resolution.Height
                         }).ToList();

                // Read flash support.
                // Opening camera is required to read flash, request to open with min resolution
                var minResolution = SupportedResolutions.OrderBy(size => size.Width).First();
                photoCaptureDevice = await PhotoCaptureDevice.OpenAsync(sensorLocation, minResolution);
                Enum.TryParse<FlashState>(
                    photoCaptureDevice.GetProperty(KnownCameraPhotoProperties.FlashMode).ToString(),
                    true,
                    out flash);

                // Create the camera config
                cameraConfig = new CameraConfig
                {
                    Type = (int)cameraType,
                    Sizes = resolutions,
                    Flash = flash != FlashState.Off ? true : false
                };
            }
            catch (Exception e)
            {
                Logger.Error("Error in Camera get configuration. Reason - " + e.Message);
            }
            finally
            {
                if (photoCaptureDevice != null)
                {
                    try { photoCaptureDevice.Dispose(); }
                    catch { }
                }
            }
            return cameraConfig;
        }

        // Gets the configurations of all cameras in the device
        private async Task<List<CameraConfig>> GetCameraConfigurations()
        {
            // Try load the camera configurations from preferences
            List<CameraConfig> cameraConfigList = 
                (PreferencesManager.Instance.Contains(KEY_CAMERA_CONFIG)) ?
                PreferencesManager.Instance.Get<List<CameraConfig>>(KEY_CAMERA_CONFIG) :
                new List<CameraConfig>();

            if (cameraConfigList.Count != 2)
            {
                // Front camera config. Read configuration if not available.
                CameraConfig frontCameraConfig =
                    cameraConfigList.Where(config => config.Type.Equals(CameraType.FrontFacing)).
                    FirstOrDefault();
                if (frontCameraConfig == null)
                {
                    frontCameraConfig = await GetCameraConfiguration(
                        CameraType.FrontFacing, CameraSensorLocation.Front);
                    if (frontCameraConfig != null)
                    {
                        cameraConfigList.Add(frontCameraConfig);
                    }
                }

                // Back camera config. Read configuration if not available.
                CameraConfig backCameraConfig =
                    cameraConfigList.Where(config => config.Type.Equals(CameraType.Primary)).
                    FirstOrDefault();
                if (backCameraConfig == null)
                {
                    backCameraConfig = await GetCameraConfiguration(
                        CameraType.Primary, CameraSensorLocation.Back);
                    if (backCameraConfig != null)
                    {
                        cameraConfigList.Add(backCameraConfig);
                    }
                }

                // Update preferences
                if (cameraConfigList.Count > 0)
                {
                    PreferencesManager.Instance.Put<List<CameraConfig>>(
                        KEY_CAMERA_CONFIG, cameraConfigList);
                }
            }
            return cameraConfigList;
        }

        #endregion

        #region GetPicture

        // Event handler called when the camera capture task is completed
        private void OnCameraTaskCompleted(object sender, PhotoResult e)
        {
            Dictionary<string, string> result = new Dictionary<string,string>();
            bool status = false;
            string error = String.Empty;
            CameraTask cameraTask = sender as CameraTask;
            string callbackId = cameraTask.CallbackId;
            if (e.TaskResult == TaskResult.OK)
            {
                if (e.Error == null)
                {
                    CameraOptions options = cameraTask.Options;
                    string filePath = (options.FilePath != null) ?
                                FileManager.GetAbsolutePath(options.FilePath) :
                                FileManager.GetAbsolutePath(new FilePath
                                {
                                    Path = Path.GetFileName(e.OriginalFileName),
                                    Level = FileLevel.App,
                                    StorageType = StorageType.Cache
                                });
                    result.Add(KEY_PATH, filePath);


                    if (cameraTask.Type == CameraTaskType.PhotoChooser || options.ReadData)
                    {
                        // Load the image as bitmap to know dimensions
                        BitmapImage bi = new BitmapImage();
                        bi.SetSource(e.ChosenPhoto);
                        int imgWidth = bi.PixelWidth;
                        int imgHeight = bi.PixelHeight;

                        // Get the target dimensions with user requested width/height
                        int width, height;
                        if (options.Width == -1)
                        {
                            if (options.Height == -1)
                            {
                                // Auto width and height. Do nothing.
                                height = imgHeight;
                                width = imgWidth;
                            }
                            else
                            {
                                // Auto width, scale by height
                                float scale = imgHeight / options.Height;
                                width = (int)(imgWidth * scale);
                                height = options.Height;
                            }
                        }
                        else
                        {
                            if (options.Height == -1)
                            {
                                // Auto height, scale by width
                                float scale = imgWidth / options.Width;
                                height = (int)(imgHeight * scale);
                                width = options.Width;
                            }
                            else
                            {
                                // User provided required dimensions. Scale to them.
                                height = options.Height;
                                width = options.Width;
                            }
                        }

                        // Format the image as specified in options
                        // Though width and height can be same as the captured image,
                        // formatting is required as quality might be different
                        try
                        {
                            e.ChosenPhoto.Seek(0, SeekOrigin.Begin);
                            byte[] data = FormatImage(e.ChosenPhoto, width, height, options.Quality);
                            if (data != null)
                            {
                                try
                                {
                                    // Write to File
                                    FileManager.WriteDataToFile(filePath, data, false);

                                    // Set data in result
                                    if (options.ReadData)
                                    {
                                        result.Add(KEY_DATA, Convert.ToBase64String(data));
                                    }
                                    status = true;
                                }
                                catch (Exception ex)
                                {
                                    // Error writing picture
                                    error = String.Concat(Mowbly.GetString(Constants.STRING_CAMERA_WRITE_PICTURE_ERROR), ex.Message);
                                }
                            }
                            {
                                // Error in formatting picture
                                error = Mowbly.GetString(Constants.STRING_CAMERA_FORMAT_ERROR);
                            }
                        }
                        catch (Exception ex)
                        {
                            // Error formatting picture
                            error = String.Concat(Mowbly.GetString(Constants.STRING_CAMERA_PROCESS_PICTURE_ERROR), ex.Message);
                        }
                    }
                    else
                    {
                        try
                        {
                            // Choose pic with no read
                            FileManager.WriteDataToFile(filePath, e.ChosenPhoto, false);

                            status = true;
                        }
                        catch (Exception ex)
                        {
                            // Error writing picture
                            error = String.Concat(Mowbly.GetString(Constants.STRING_CAMERA_WRITE_PICTURE_ERROR), ex.Message);
                        }
                    }
                }
                else
                {
                    // Error in capturing picture
                    error = String.Concat(Mowbly.GetString(Constants.STRING_CAMERA_CAPTURE_ERROR), e.Error.Message);
                }
            }
            else
            {
                // User cancelled the task
                error = Mowbly.GetString(Constants.STRING_ACTIVITY_CANCELLED);
            }

            if (status)
            {
                InvokeCallbackJavascript(callbackId, new MethodResult
                    {
                        Result = result
                    });
            }
            else
            {
                Logger.Error(error);
                InvokeCallbackJavascript(callbackId, new MethodResult
                {
                    Code = MethodResult.FAILURE_CODE,
                    Error = new MethodError
                    {
                        Message = error
                    }
                });
            }
            Logger.Debug(cameraTask.Name + " completed");
        }

        /// <summary>
        /// Resizes/scales the image data to the specified size and quality
        /// Refer: http://www.mindscapehq.com/blog/index.php/2012/02/28/windows-phone-7-working-with-camera-tasks/
        /// </summary>
        /// <param name="stream">Image stream</param>
        /// <param name="width">Target width of the image</param>
        /// <param name="height">Target height of the image</param>
        /// <param name="quality">Target quality of the image</param>        
        /// <returns>Formatted image</returns>
        private byte[] FormatImage(Stream stream, int width, int height, int quality)
        {
            WriteableBitmap target = null;
            BitmapImage image = null;

            try
            {
                JpegInfo info = ExifReader.ReadJpeg(stream, string.Empty);
                byte[] data = null;
                int angle = 0;
                switch (info.Orientation)
                {
                    case ExifOrientation.TopLeft:
                    case ExifOrientation.Undefined:
                        angle = 0;
                        break;
                    case ExifOrientation.TopRight:
                        angle = 90;
                        break;
                    case ExifOrientation.BottomRight:
                        angle = 180;
                        break;
                    case ExifOrientation.BottomLeft:
                        angle = 270;
                        break;
                }

                if (angle > 0d)
                {
                    target = RotateStream(stream, angle);
                }
                if (target == null)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    image = new BitmapImage();
                    image.SetSource(stream);
                    target = new WriteableBitmap(image);
                }

                MemoryStream outStream = new MemoryStream();
                target.SaveJpeg(outStream, 
                    width, 
                    height, 
                    (int)((PhoneApplicationPage)Mowbly.ActivePhoneApplicationPage).Orientation, 
                    quality);
                outStream.Seek(0, SeekOrigin.Begin);
                data = ((MemoryStream)outStream).ToArray();

                outStream.Close();
                outStream.Dispose();
                outStream = null;

                Logger.Debug("Camera: Formatted image [JPG " + width + "x" + height + ";quality" + quality + "]");
                return data;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (image != null)
                {
                    image.UriSource = null;
                    image = null;
                }
                target = null;

                //outStream ,image,target are eligible for cleanup                
                GCUtils.DoGC();
            }
        }

        // Rotate the picture data to the specified angle
        private WriteableBitmap RotateStream(Stream stream, int angle)
        {
            stream.Position = 0;
            if (angle % 90 != 0 || angle < 0) throw new ArgumentException();
            if (angle % 360 == 0) return null;

            BitmapImage bitmap = new BitmapImage();
            bitmap.SetSource(stream);
            WriteableBitmap wbSource = new WriteableBitmap(bitmap);

            WriteableBitmap wbTarget = null;
            if (angle % 180 == 0)
            {
                wbTarget = new WriteableBitmap(wbSource.PixelWidth, wbSource.PixelHeight);
            }
            else
            {
                wbTarget = new WriteableBitmap(wbSource.PixelHeight, wbSource.PixelWidth);
            }

            for (int x = 0; x < wbSource.PixelWidth; x++)
            {
                for (int y = 0; y < wbSource.PixelHeight; y++)
                {
                    switch (angle % 360)
                    {
                        case 90:
                            wbTarget.Pixels[(wbSource.PixelHeight - y - 1) + x * wbTarget.PixelWidth] =
                                wbSource.Pixels[x + y * wbSource.PixelWidth];
                            break;
                        case 180:
                            wbTarget.Pixels[(wbSource.PixelWidth - x - 1) + (wbSource.PixelHeight - y - 1) *
                                wbSource.PixelWidth] = wbSource.Pixels[x + y * wbSource.PixelWidth];
                            break;
                        case 270:
                            wbTarget.Pixels[y + (wbSource.PixelWidth - x - 1) * wbTarget.PixelWidth] =
                                wbSource.Pixels[x + y * wbSource.PixelWidth];
                            break;
                    }
                }
            }

            return wbTarget;
        }

        #endregion

        #endregion        
    }

    #region Camera Json Objects

    [JsonObject(MemberSerialization.OptIn)]
    public sealed class CameraConfig
    {
        [DefaultValue(false)]
        [JsonProperty(
            PropertyName = Camera.KEY_FLASH,
            Required = Required.Default)]
        public bool Flash { get; set; }

        [JsonProperty(
            PropertyName = Camera.KEY_RESOLUTIONS,
            Required = Required.Always)]
        public List<CameraResolution> Sizes { get; set; }

        [JsonProperty(
            PropertyName = Camera.KEY_TYPE,
            Required = Required.Always)]
        public int Type { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public struct CameraOptions
    {
        [JsonProperty(
            PropertyName = Camera.KEY_WIDTH,
            Required = Required.Default)]
        [DefaultValue(Camera.DEFAULT_WIDTH)]
        public int Width { get; set; }

        [JsonProperty(
            PropertyName = Camera.KEY_HEIGHT,
            Required = Required.Default)]
        [DefaultValue(Camera.DEFAULT_HEIGHT)]
        public int Height { get; set; }

        [JsonProperty(
            PropertyName = Camera.KEY_QUALITY,
            Required = Required.Default)]
        [DefaultValue(Camera.Quality.QUALITY_MEDIUM)]
        public int Quality { get; set; }

        [JsonProperty(
            PropertyName = Camera.KEY_TYPE,
            Required = Required.Default)]
        [DefaultValue(Camera.Format.TYPE_JPG)]
        public int Type { get; set; }

        [JsonProperty(
            PropertyName = Camera.KEY_READ_DATA,
            Required = Required.Default)]
        [DefaultValue(false)]
        public bool ReadData { get; set; }

        [DefaultValue(null)]
        [JsonProperty(
            PropertyName = Camera.KEY_FILE_PATH,
            Required = Required.Default,
            DefaultValueHandling = DefaultValueHandling.Populate)]
        public FilePath FilePath { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public struct CameraResolution
    {
        [JsonProperty(
            PropertyName = Camera.KEY_WIDTH,
            Required = Required.Always)]
        public double Height { get; set; }

        [JsonProperty(
            PropertyName = Camera.KEY_HEIGHT,
            Required = Required.Always)]
        public double Width { get; set; }
    }

    #endregion
}

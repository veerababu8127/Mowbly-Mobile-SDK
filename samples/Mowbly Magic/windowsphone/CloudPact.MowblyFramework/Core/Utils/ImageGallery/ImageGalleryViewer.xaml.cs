
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using CloudPact.MowblyFramework.Core.Utils.ImageGallery;
using Newtonsoft.Json;
using System.IO;
using CloudPact.MowblyFramework.Core;
using CloudPact.MowblyFramework.Core.Managers;


namespace ImageGallery
{
    public partial class MainPage : PhoneApplicationPage
    {
        private List<string> images = new List<string>();

        private List<string> imageTitles = new List<string>();

        private int imageIndex = 0;

        private double actualImageHeight;

        private double actualImageWidth;        

        private double initialScale;

        private bool isOrizinal = true;

        GalleryInfo galleryInfo;


        // Constructor
        public MainPage()
        {
            InitializeComponent();            
            Loaded += new RoutedEventHandler(MainPage_Loaded);
            var gl = GestureService.GetGestureListener(LayoutRoot);
            gl.Flick += new EventHandler<FlickGestureEventArgs>(GestureListener_Flick);
            gl.DragDelta += new EventHandler<DragDeltaGestureEventArgs>(GestureListener_DragDelta);
            gl.DoubleTap += new EventHandler<Microsoft.Phone.Controls.GestureEventArgs>(GestureListener_Doubletap);
        }

        private void onDoneClick(object sender, RoutedEventArgs e)
        {
            //Close Gallery 
            NavigationService.GoBack();
        }

        void GestureListener_Doubletap(object sender, Microsoft.Phone.Controls.GestureEventArgs e)
        {            
            var transformNormal = (CompositeTransform)img.RenderTransform;
            transformNormal.ScaleX = 1.0;
            transformNormal.ScaleY = 1.0;
            img.RenderTransform = transformNormal;
            isOrizinal = true;
        }

        void GestureListener_DragDelta(object sender, DragDeltaGestureEventArgs e)
        {
            if (!isOrizinal)
            {
                Transform.CenterX = (Transform.CenterX - e.HorizontalChange);
                Transform.CenterY = (Transform.CenterY - e.VerticalChange);

                if (Transform.CenterX < 0)
                    Transform.CenterX = 0;
                else if (Transform.CenterX > ContentPanel.ActualWidth)
                    Transform.CenterX = ContentPanel.ActualWidth;
                else if (Transform.CenterX > (img.Height * Transform.ScaleX))
                    Transform.CenterX = img.Height * Transform.ScaleX;
                if (Transform.CenterY < 0)
                    Transform.CenterY = 0;
                else if (Transform.CenterY > ContentPanel.ActualHeight)
                    Transform.CenterY = ContentPanel.ActualHeight;
                else if (Transform.CenterY > (img.Height * Transform.ScaleY))
                    Transform.CenterY = img.Height * Transform.ScaleY;
            }
        }



        private void OnPinchStarted(object sender, PinchStartedGestureEventArgs e)
        {
            initialScale = Transform.ScaleX;
        }

        private void OnPinchDelta(object sender, PinchGestureEventArgs e)
        {

            double distanceRatio = e.DistanceRatio;
            var image = sender as System.Windows.Controls.Image;
            if (image == null) return;
            var transform = image.RenderTransform as CompositeTransform;
            if (transform == null) return;
            if (initialScale * e.DistanceRatio * image.ActualHeight > 8 * image.ActualHeight)
            {
                distanceRatio = 8.0;
            }

            if (initialScale * e.DistanceRatio * image.ActualHeight < image.ActualHeight / 2)
            {
                distanceRatio = 1.0;
            }

            transform.ScaleX = initialScale * distanceRatio;
            transform.ScaleY = initialScale * distanceRatio;
            if (e.DistanceRatio > 1.0 || e.DistanceRatio < 1.0)
            {
                isOrizinal = false;
            }
        }

        void GestureListener_Flick(object sender, FlickGestureEventArgs e)
        {
            if (e.Direction.ToString() == "Horizontal" && isOrizinal)
            {
                if (image_placeholder.Text == galleryInfo.ErrorMessage)
                {
                    image_placeholder.Text = galleryInfo.LoadMessage;
                }
                else
                {
                    image_placeholder.Text = galleryInfo.LoadMessage;
                }
                if (e.HorizontalVelocity < 0) // determine direction (Right > 0)
                {
                    if (imageIndex < images.Count)
                    {
                        if (imageIndex != images.Count - 1)
                        {
                            imageIndex = imageIndex + 1;
                            loadImage(imageIndex);
                        }
                    }

                }
                else
                {
                    if (imageIndex >= 0)
                    {
                        if (imageIndex != 0)
                        {
                            imageIndex = imageIndex - 1;
                            loadImage(imageIndex);
                        }
                    }
                }
            }
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var param = PhoneApplicationService.Current.State["param"];
            galleryInfo = JsonConvert.DeserializeObject<GalleryInfo>(param.ToString());
            page_name.Text = galleryInfo.Title;
            LoadImages(galleryInfo.Images); 
            loadImage(galleryInfo.Index);
            if (galleryInfo.LoadMessage != "")
            {
                image_placeholder.Text = galleryInfo.LoadMessage;
            }
            if (galleryInfo.ButtonLabel != "")
            {
                done.Content = galleryInfo.ButtonLabel;
            }
        }

        private void LoadImages(object[] galleryinfo)
        {
            ImageObject imageObject;
            foreach (var image in galleryinfo)
            {
                imageObject = JsonConvert.DeserializeObject<ImageObject>(image.ToString());
                if (imageObject.ImageUrl.StartsWith("http://") || imageObject.ImageUrl.StartsWith("https://"))
                {
                    images.Add(imageObject.ImageUrl);
                }
                else
                {
                    string localUrl = Path.Combine(Mowbly.AppDirectory, imageObject.ImageUrl.Substring(1));
                    images.Add(localUrl);
                }
                
                imageTitles.Add(imageObject.ImageTitle);

            }
        }

        private void onImageLoaded(object sender, RoutedEventArgs e)
        {
            actualImageHeight = img.ActualHeight;
            actualImageWidth = img.ActualWidth;
        }

        private void loadImage(int imageIndex)
        {
            var bi = new BitmapImage(new Uri(images[imageIndex], UriKind.RelativeOrAbsolute));
            var transformNormal = (CompositeTransform)img.RenderTransform;
            transformNormal.ScaleX = 1.0;
            transformNormal.ScaleY = 1.0;
            img.RenderTransform = transformNormal;
            img.Source = bi;
            image_title.Text = imageTitles[imageIndex];
        }


        private void loading_failed(object sender, ExceptionRoutedEventArgs e)
        {
            if (galleryInfo.ErrorMessage != "")
            {
                image_placeholder.Text = galleryInfo.ErrorMessage;
            }
            else
            {
                image_placeholder.Text = "Error loading image";
            }
        }

        private void loading_success(object sender, ExceptionRoutedEventArgs e)
        {
            if (galleryInfo.LoadMessage != "")
            {
                image_placeholder.Text = galleryInfo.LoadMessage;
            }
        }
    }

    #region GalleryInfo

    [JsonObject(MemberSerialization.OptIn)]
    public sealed class GalleryInfo
    {
        const string KEY_TITLE = "title";
        const string KEY_LOAD_MESSAGE = "loadingMsg";
        const string KEY_INDEX = "index";
        const string KEY_ERROR_MESSAGE = "errorMsg";
        const string KEY_IMAGES = "images";
        const string KEY_BUTTON_LABEL = "buttonLabel";

        [JsonProperty(
            PropertyName = KEY_TITLE)]
        public string Title { get; set; }

        [JsonProperty(
            PropertyName = KEY_BUTTON_LABEL)]
        public string ButtonLabel { get; set; }

        [JsonProperty(
            PropertyName = KEY_LOAD_MESSAGE)]
        public string LoadMessage { get; set; }

        [JsonProperty(
            PropertyName = KEY_INDEX)]
        public int Index { get; set; }

        [JsonProperty(
            PropertyName = KEY_ERROR_MESSAGE)]
        public string ErrorMessage { get; set; }

        [JsonProperty(
            PropertyName = KEY_IMAGES)]
        public object[] Images { get; set; }
    }

    #endregion

    #region ImageObject

    [JsonObject(MemberSerialization.OptIn)]
    public sealed class ImageObject
    {
        const string KEY_URL = "url";
        const string KEY_IMAGE_TITLE = "title";
        
        [JsonProperty(
            PropertyName = KEY_IMAGE_TITLE,
            Required = Required.Default)]
        public string ImageTitle { get; set; }

        [JsonProperty(
            PropertyName = KEY_URL,
            Required = Required.Default)]
        public string ImageUrl { get; set; }
    }

    #endregion
}

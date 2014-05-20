//-----------------------------------------------------------------------------------------
// <copyright file="ImageGallery.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CloudPact.MowblyFramework.Core.Utils;
using CloudPact.MowblyFramework.Core.Utils.ImageGallery;
using CloudPact.MowblyFramework.Core.Ui;
using CloudPact.MowblyFramework.Core.Log;

namespace CloudPact.MowblyFramework.Core.Features
{
    class ImageGallery : Feature
    {
        internal async override void InvokeAsync(JSMessage message)
        {
            switch (message.Method)
            {
                case "showAsGallery" :
                    object GalleryDetails = JsonConvert.DeserializeObject(message.Args[0].ToString());
                    ImageGalleryViewer gallery = new ImageGalleryViewer(GalleryDetails);
                    UiDispatcher.BeginInvoke(() =>
                    {
                        try
                        {
                            gallery.launchGallery();
                        }
                        catch (Exception e)
                        {
                            Logger.Error(e.Message);
                        }
                    });
                    break;
            }
        }
    }
}

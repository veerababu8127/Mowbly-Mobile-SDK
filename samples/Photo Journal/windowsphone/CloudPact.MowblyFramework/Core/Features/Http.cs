//-----------------------------------------------------------------------------------------
// <copyright file="Http.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using CloudPact.MowblyFramework.Core.Log;
using CloudPact.MowblyFramework.Core.Managers;
using Microsoft.Phone.Net.NetworkInformation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CloudPact.MowblyFramework.Core.Features
{
    public enum HttpMethod
    {
        Get = 0,
        Post,
        PostMutlipart,
        Put,
        PutMultipart,
        Delete,
        Head
    };

    class Http : Feature
    {
        const string KEY_FILE = "file";

        const string KEY_JSON = "json";

        const string KEY_STRING = "string";

        string UserAgent = MowblyClientManager.Instance.GetUserAgent();

        bool ReplaceInBody = false;
        
        String Username;
        
        String Password;

        #region Feature

        /// <summary>
        /// Name of the feature
        /// </summary>
        internal override string Name { get { return Constants.FEATURE_HTTP; } }

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
            switch (message.Method)
            {
                case "request":
                    try
                    {
                        // Read args
                        HttpOptions options =
                            JsonConvert.DeserializeObject<HttpOptions>(message.Args[0].ToString());
                        
                        // Check network available
                        if (NetworkInterface.GetIsNetworkAvailable())
                        {
                            await Request(options, callbackId);
                        }
                        else
                        {
                            InvokeCallbackJavascript(callbackId, new MethodResult
                                {
                                    Code = MethodResult.FAILURE_CODE,
                                    Error = new MethodError
                                    {
                                        Message = Mowbly.GetString(Constants.STRING_NO_CONNECTIVITY)
                                    }
                                });
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error("Error in Http request. Reason - " + e.Message);
                        InvokeCallbackJavascript(callbackId, new MethodResult
                        {
                            Code = MethodResult.FAILURE_CODE,
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

        #endregion

        /// <summary>
        /// Sends request as specified in Http options
        /// </summary>
        /// <param name="options">HttpOptions object</param>
        /// <param name="callbackId">Callback id of the response handler in Mowbly JS layer</param>
        async Task Request(HttpOptions options, string callbackId)
        {
            HttpResponseMessage responseMessage;            
            ReplaceInBody = options.With_Credentials && options.Replace_In_Body;
            if (options.With_Credentials)
            {
                if (options.Replace_In_Url)
                {
                    options.Url = ReplacePlaceHolders(options.Url);
                }
               
                if (options.Replace_In_Headers.Count != 0)
                {
                    foreach (var key in options.Replace_In_Headers)
                    {
                        options.Headers[key] = ReplacePlaceHolders(options.Headers[key]);
                    }
                }
                
                
            }
            if (options.Parts.Count > 0)
            {
                if (options.Method == HttpMethod.Post || options.Method == HttpMethod.Put)
                {
                    // Multipart - Post as multipart
                    responseMessage = await PostMultipartFormData(options);
                }
                else
                {
                    // Send request based on specified http method ignoring data
                    responseMessage = await SendRequestByHttpMethod(options);
                }
            }
            else
            {
                // Could be Json, File or string
                if (options.DataType.Equals(KEY_JSON))
                {
                    if (ReplaceInBody)
                    {
                        Dictionary<string, object> oData =
                            JsonConvert.DeserializeObject<Dictionary<string, object>>(options.Data.ToString());
                        oData.Keys.ToList<string>().ForEach((key) =>
                        {
                            oData[key] = ReplacePlaceHolders(oData[key]);
                        });
                     }
                    if (options.Method == HttpMethod.Post || options.Method == HttpMethod.Put)
                    {
                        // Json - Post as form url encoded content
                        responseMessage = await PostFormUrlEncodedData(options);
                    }
                    else if (options.Method != HttpMethod.Delete)
                    {
                        // Add the Json name value pairs to Url query parameters
                        // and send request based on http method specified
                        if (options.Parts.Count > 0)
                        {
                            UriBuilder uriBuilder = new UriBuilder(options.Url);
                            string queryStr = String.Join(
                                "&",
                                options.Parts.Select(p => String.Format("{0}={1}",
                                    p.Name, Uri.EscapeDataString(p.Value.ToString()))).ToArray());
                            uriBuilder.Query = queryStr;
                            options.Url = uriBuilder.ToString();
                        }
                        responseMessage = await SendRequestByHttpMethod(options);
                    }
                    else
                    {
                        // Send request based on specified http method ignoring data
                        responseMessage = await SendRequestByHttpMethod(options);
                    }
                }
                else if (options.DataType.Equals(KEY_FILE))
                {
                    if (options.Method == HttpMethod.Post || options.Method == HttpMethod.Put)
                    {
                        // Post file content as stream
                        responseMessage = await PostFile(options);
                    }
                    else
                    {
                        // Send request based on specified http method ignoring data
                        responseMessage = await SendRequestByHttpMethod(options);
                    }
                }
                else
                {
                    // String
                    if (ReplaceInBody)
                    {
                        options.Data = ReplacePlaceHolders(options.Data);
                    }
                    if (options.Method == HttpMethod.Post || options.Method == HttpMethod.Put)
                    {
                        // Post string
                        responseMessage = await PostString(options);
                    }
                    else
                    {
                        // Send request based on specified http method ignoring data
                        responseMessage = await SendRequestByHttpMethod(options);
                    }
                }
            }
            
            // Ensure success
            Dictionary<string, string> headers;
            Dictionary<string, object> response;
            // Download the content to file if requested
            if (options.DownloadFile != null)
            {
                await DownloadContent(responseMessage.Content, options.DownloadFile);
                headers = new Dictionary<string, string>();
                response = new Dictionary<string, object>
                {
                    {"headers", headers}
                };
            }
            else
            {
                // Create response
                headers = new Dictionary<string, string>();
                // TODO: Read headers
                response = new Dictionary<string, object>
                {
                    {"data", await responseMessage.Content.ReadAsStringAsync()},
                    {"headers", headers}
                };
            }
            InvokeCallbackJavascript(callbackId, new MethodResult
                {
                    Code = (int)responseMessage.StatusCode,
                    Result = response
                });
        }

        async Task DownloadContent(HttpContent content, FilePath filePath)
        {
            Stream contentStream = null, fileStream = null;
            try
            {
                // Read content as stream
                contentStream = await content.ReadAsStreamAsync();

                // Determine the download file path
                string path = FileManager.GetAbsolutePath(filePath, false);

                // Write the content from content stream to file stream
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    storage.CreateDirectory(Path.GetDirectoryName(path));
                    fileStream = storage.OpenFile(path, FileMode.Create, FileAccess.Write, FileShare.None);
                    await contentStream.CopyToAsync(fileStream,bufferSize: 4096);
                }
            }
            catch (Exception e) { throw e; }
            finally
            {
                // Close the streams
                if (contentStream != null)
                {
                    contentStream.Close();
                    contentStream.Dispose();
                }
                if (fileStream != null)
                {
                    fileStream.Close();
                    fileStream.Dispose();
                }
            }
        }

        async Task<HttpResponseMessage> PostString(HttpOptions options)
        {
            Uri uri = new Uri(options.Url);
            using (HttpClient httpClient = new HttpClient())
            {
                // Create the string content
                StringContent content = new StringContent(options.Data.ToString());
                
                // Set headers
                SetHeadersOnContent(content, options);
                
                // Set timeout
                httpClient.Timeout = TimeSpan.FromMilliseconds(options.Timeout);                

                // Post
                return await httpClient.PostAsync(uri, content);
            }
        }

        async Task<HttpResponseMessage> PostFormUrlEncodedData(HttpOptions options)
        {
            Uri uri = new Uri(options.Url);
            using (HttpClient httpClient = new HttpClient())
            {
                Dictionary<string, object> oData =
                                JsonConvert.DeserializeObject<Dictionary<string, object>>(options.Data.ToString());
                if (ReplaceInBody)
                {
                    oData.Keys.ToList<string>().ForEach((key) =>
                    {
                        oData[key] = ReplacePlaceHolders(oData[key]); 
                       
                    });
                }

                // Create the form url encoded content
                IEnumerable<KeyValuePair<string, string>> pairs = 
                    oData.Select(part => new KeyValuePair<string, string>(part.Key.ToString(), part.Value.ToString()));

                
                FormUrlEncodedContent content = new FormUrlEncodedContent(pairs);


                // Set headers
                SetHeadersOnContent(content, options);

                // Set timeout
                httpClient.Timeout = TimeSpan.FromMilliseconds(options.Timeout);                

                // Post
                return await httpClient.PostAsync(uri, content);
            }
        }

        async Task<HttpResponseMessage> PostFile(HttpOptions options)
        {
            Uri uri = new Uri(options.Url);
            Stream fileStream = null;
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    // Create the file stream content
                    Dictionary<string, FilePath> oData =
                                JsonConvert.DeserializeObject<Dictionary<string, FilePath>>(options.Data.ToString());
                    FilePath filePath = oData[KEY_FILE];
                    string path = FileManager.GetAbsolutePath(filePath);
                    IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
                    fileStream = storage.OpenFile(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                    StreamContent content = new StreamContent(fileStream);

                    // Set headers
                    SetHeadersOnContent(content, options);

                    // Set timeout
                    httpClient.Timeout = TimeSpan.FromMilliseconds(options.Timeout);                    

                    // Post
                    HttpResponseMessage responseMessage = await httpClient.PostAsync(uri, content);

                    return responseMessage;
                }
            }
            catch (Exception e) { throw e; }
            finally
            {
                // Close the file stream
                if (fileStream != null)
                {
                    fileStream.Close();
                    fileStream.Dispose();
                }
            }
        }

        async Task<HttpResponseMessage> PostMultipartFormData(HttpOptions options)
        {
            Uri uri = new Uri(options.Url);
            FileStream fileStream = null;
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    // Create the multipart form data content
                    MultipartFormDataContent content = new MultipartFormDataContent();
                    options.Parts.ForEach((part) =>
                    {
                        if (part.Type.Equals(KEY_FILE))
                        {
                            // File - Set the name of the file as filename, if filename not provided
                            FilePath filePath = JsonConvert.DeserializeObject<FilePath>(part.Value.ToString());
                            string path = FileManager.GetAbsolutePath(filePath, false);
                            string fileName = part.FileName ?? Path.GetFileNameWithoutExtension(path);

                            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
                            fileStream = storage.OpenFile(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                            content.Add(
                                new StreamContent(fileStream),
                                part.Name,
                                fileName);
                        }
                        else
                        {
                            // String
                            if (ReplaceInBody)
                            {
                                part.Value = ReplacePlaceHolders(part.Value);
                            }
                            content.Add(new StringContent(part.Value as string), part.Name);
                        }
                    });

                    // Set headers
                    SetHeadersOnContent(content, options);

                    // Set timeout
                    httpClient.Timeout = TimeSpan.FromMilliseconds(options.Timeout);
                    

                    // Post
                    return await httpClient.PostAsync(uri, content);
                }
            }
            catch (Exception e) { throw e; }
            finally
            {
                // Close the file stream
                if (fileStream != null)
                {
                    fileStream.Close();
                    fileStream.Dispose();
                }
            }
        }

        async Task<HttpResponseMessage> SendRequestByHttpMethod(HttpOptions options)
        {
            Uri uri = new Uri(options.Url);
            using (HttpClient httpClient = new HttpClient())
            {
                // Set timeout
                httpClient.Timeout = TimeSpan.FromMilliseconds(options.Timeout);
                if (options.Headers.Count > 0)
                {
                    foreach (var pair in options.Headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(pair.Key, pair.Value);
                    }
                }
                if (options.Auth_Mode == 1)
                {
                    httpClient.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue(
                                "Basic",
                                Convert.ToBase64String(
                                    Encoding.UTF8.GetBytes(
                                        string.Format("{0}:{1}", options.Username, options.Password))));
                }
                
                if (options.Method == HttpMethod.Delete)
                {
                    return await httpClient.DeleteAsync(uri);
                }
                else
                {
                    // Default is Get
                    return await httpClient.GetAsync(uri);
                }
            }
        }

        void SetHeadersOnContent(HttpContent content, HttpOptions options)
        {
            options.Headers.Keys.ToList<string>().ForEach((key) =>
            {
                if (key.ToLower().Equals("content-type"))
                {
                    content.Headers.ContentType =
                        new System.Net.Http.Headers.MediaTypeHeaderValue(options.Headers[key]);
                }
                else
                {
                    content.Headers.Add(key, options.Headers[key]);
                }
            });
        }

        string ReplacePlaceHolders(object Key)
        {
            return Key.ToString().Replace(Constants.USERNAME_PLACEHOLDER, Username)
                    .Replace(Constants.PASSWORD_PLACEHOLDER, Password);   
        }
    }

    #region Http helper classes

    [JsonObject(MemberSerialization.OptIn)]
    sealed class HttpOptions
    {
        private const string KEY_DATA = "data";
        private const string KEY_DATA_TYPE = "dataType";
        private const string KEY_DOWNLOAD_FILE = "downloadFile";
        private const string KEY_HEADERS = "headers";
        private const string KEY_PARTS = "parts";
        private const string KEY_STRING = "string";
        private const string KEY_TIMEOUT = "timeout";
        private const string KEY_TYPE = "type";
        private const string KEY_URL = "url";
        private const string KEY_VALUE = "value";
        private const string KEY_WITH_CREDENTIALS = "withCredentials";
        private const string KEY_REPLACE_IN_BODY = "replaceInBody";
        private const string KEY_REPLACE_IN_HEADERS = "replaceInHeaders";
        private const string KEY_REPLACE_IN_URL = "replaceInUrl";
        private const string KEY_AUTH_MODE = "authMode";
        private const string KEY_USERNAME = "username";
        private const string KEY_PASSWORD = "password";


        [JsonProperty(
            PropertyName = KEY_DATA,
            Required = Required.Default)]
        public object Data { get; set; }

        [JsonProperty(
            PropertyName = KEY_DATA_TYPE,
            Required = Required.Default)]
        [DefaultValue(KEY_STRING)]
        public string DataType { get; set; }

        [JsonProperty(
            PropertyName = KEY_DOWNLOAD_FILE,
            Required = Required.Default)]
        public FilePath DownloadFile { get; set; }

        [JsonProperty(
            PropertyName = KEY_HEADERS,
            Required = Required.Default)]
        public Dictionary<string, string> Headers { get; set; }

        [JsonProperty(
            PropertyName = KEY_PARTS,
            Required = Required.Default)]
        public List<HttpPart> Parts { get; set; }

        [JsonProperty(
            PropertyName = KEY_TIMEOUT,
            Required = Required.Default)]
        public int Timeout { get; set; }

        [JsonProperty(
            PropertyName = KEY_TYPE,
            Required = Required.Default)]
        public string Type { get; set; }

        [JsonProperty(
            PropertyName = KEY_WITH_CREDENTIALS,
            Required = Required.Default)]
        public bool With_Credentials { get; set; }

        [JsonProperty(
            PropertyName = KEY_REPLACE_IN_BODY,
            Required = Required.Default)]
        public bool Replace_In_Body { get; set; }

        [JsonProperty(
            PropertyName = KEY_REPLACE_IN_HEADERS,
            Required = Required.Default)]
        public List<String> Replace_In_Headers { get; set; }

        [JsonProperty(
            PropertyName = KEY_REPLACE_IN_URL,
            Required = Required.Default)]
        public bool Replace_In_Url { get; set; }

        [JsonProperty(
            PropertyName = KEY_AUTH_MODE,
            Required = Required.Default)]
        public int Auth_Mode { get; set; }

        [JsonProperty(
            PropertyName = KEY_USERNAME,
            Required = Required.Default)]
        public string Username { get; set; }

        [JsonProperty(
            PropertyName = KEY_PASSWORD,
            Required = Required.Default)]
        public string Password { get; set; }

        [JsonProperty(
            PropertyName = KEY_URL,
            Required = Required.Default)]
        public string Url { get; set; }

        public HttpMethod Method { get; set; }

        [OnDeserialized()]
        public void OnDeserialized(StreamingContext context)
        {
            // Set http method
            this.Method = GetHttpMethodByType(this.Type);
        }

        private HttpMethod GetHttpMethodByType(string type)
        {
            HttpMethod method;
            if (type.Equals("POST"))
            {
                method = HttpMethod.Post;
            }
            else if (type.Equals("PUT"))
            {
                method = HttpMethod.Put;
            }
            else if (type.Equals("DELETE"))
            {
                method = HttpMethod.Delete;
            }
            else if (type.Equals("HEAD"))
            {
                method = HttpMethod.Head;
            }
            else
            {
                method = HttpMethod.Get;
            }
            return method;
        }
    }

    sealed class HttpPart
    {
        private const string KEY_CONTENT_TYPE = "contentType";
        private const string KEY_FILE = "file";
        private const string KEY_FILE_NAME = "filename";
        private const string KEY_NAME = "name";
        private const string KEY_TYPE = "type";
        private const string KEY_VALUE = "value";

        [JsonProperty(
           PropertyName = KEY_CONTENT_TYPE,
           Required = Required.Default)]
        public string ContentType { get; set; }

        [JsonProperty(
            PropertyName = KEY_FILE_NAME,
            Required = Required.Default)]
        public string FileName { get; set; }

        [JsonProperty(
            PropertyName = KEY_NAME,
            Required = Required.Default)]
        public string Name { get; set; }

        [JsonProperty(
            PropertyName = KEY_TYPE,
            Required = Required.Default)]
        public string Type { get; set; }

        [JsonProperty(
            PropertyName = KEY_VALUE,
            Required = Required.Default)]
        public object Value { get; set; }

        public void OnDeerialized(StreamingContext context)
        {
            if(this.Type.Equals(KEY_FILE))
            {
                this.Value = JsonConvert.DeserializeObject<FilePath>(this.Value.ToString());
            }
        }
    }
    
    #endregion
}
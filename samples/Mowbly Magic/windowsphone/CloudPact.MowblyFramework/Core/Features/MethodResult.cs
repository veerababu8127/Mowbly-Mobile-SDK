//-----------------------------------------------------------------------------------------
// <copyright file="MethodResult.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using Newtonsoft.Json;
using System;

namespace CloudPact.MowblyFramework.Core.Features
{
    /// <summary>
    /// Class that encapsulates the result of a Feature method execution
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    sealed class MethodResult
    {
        public const int SUCCESS_CODE = 1;
        
        public const int FAILURE_CODE = 0;

        int code = SUCCESS_CODE;

        object result;

        MethodError error;

        /// <summary>
        /// Status code returned by the method indicating success or failure of its execution
        /// </summary>
        [JsonProperty(PropertyName = "code", Required = Required.Always)]
        public int Code
        {
            get { return code; }
            set { code = value; }
        }

        /// <summary>
        /// Result returned by the method execution
        /// </summary>
        [JsonProperty(PropertyName = "result", 
            Required = Required.Default, 
            NullValueHandling = NullValueHandling.Ignore)]
        public object Result
        {
            get { return result; }
            set { result = value; }
        }

        /// <summary>
        /// Error returned by the method execution
        /// </summary>
        [JsonProperty(PropertyName = "error", 
            Required = Required.Default, 
            NullValueHandling = NullValueHandling.Ignore)]
        public MethodError Error
        {
            get { return error; }
            set { error = value; }
        }
    }

    /// <summary>
    /// Class that encapsulates the error of a Feature method execution
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    sealed class MethodError
    {
        string message;

        string description;

        /// <summary>
        /// Short description of the error
        /// </summary>
        [JsonProperty(PropertyName = "message", Required = Required.Always)]
        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        /// <summary>
        /// Detailed description of the error
        /// </summary>
        [JsonProperty(PropertyName = "description", 
            Required = Required.Default,
            NullValueHandling = NullValueHandling.Ignore)]
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public override string ToString()
        {
            return String.Format("Error Message:{0},\nDescription:{1}", message, description ?? "");
        }
    }
}

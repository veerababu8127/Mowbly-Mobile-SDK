//-----------------------------------------------------------------------------------------
// <copyright file="JSBridge.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

using Newtonsoft.Json;

namespace CloudPact.MowblyFramework.Core.Features
{
    /// <summary>
    /// Javascript message to Mowbly JS layer from Native
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public struct WindowsPlug
    {
        [JsonProperty(PropertyName = "o", Required = Required.Always)]
        public string JSObject { get; set; }

        [JsonProperty(PropertyName = "m", Required = Required.Always)]
        public string Method { get; set; }

        [JsonProperty(PropertyName = "a", Required = Required.Default, 
            DefaultValueHandling = DefaultValueHandling.Ignore)]
        public object[] Args { get; set; }
    }

    /// <summary>
    /// Javascript message from Mowbly JS later to Native
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public struct JSMessage
    {
        [JsonProperty(PropertyName = "feature", Required = Required.Always)]
        public string Feature { get; set; }

        [JsonProperty(PropertyName = "method", Required = Required.Always)]
        public string Method { get; set; }

        [JsonProperty(PropertyName = "args", Required = Required.Default)]
        public object[] Args { get; set; }

        [JsonProperty(PropertyName = "callbackid", Required = Required.Default)]
        public string CallbackId { get; set; }
    }
}

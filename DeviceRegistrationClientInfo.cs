using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.Crm.Services.Utility
{
    /// <summary>Device registration client info</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [XmlRoot("ClientInfo")]
    public sealed class DeviceRegistrationClientInfo
    {
        /// <summary>
        /// Gets or sets the device registration client info's application id
        /// </summary>
        [XmlAttribute("name")]
        public Guid ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets the device registration client info version
        /// </summary>
        [XmlAttribute("version")]
        public string Version { get; set; }
    }
}
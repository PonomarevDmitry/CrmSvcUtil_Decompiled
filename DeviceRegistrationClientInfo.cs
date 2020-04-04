using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.Crm.Services.Utility
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    [XmlRoot("ClientInfo")]
    public sealed class DeviceRegistrationClientInfo
    {
        [XmlAttribute("name")]
        public Guid ApplicationId { get; set; }

        [XmlAttribute("version")]
        public string Version { get; set; }
    }
}
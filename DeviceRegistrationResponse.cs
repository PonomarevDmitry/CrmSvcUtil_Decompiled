using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.Crm.Services.Utility
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    [XmlRoot("DeviceAddResponse")]
    public sealed class DeviceRegistrationResponse
    {
        [XmlElement("success")]
        public bool IsSuccess { get; set; }

        [XmlElement("puid")]
        public string Puid { get; set; }

        [XmlElement("Error")]
        public DeviceRegistrationResponseError Error { get; set; }

        [XmlElement("ErrorSubcode")]
        public string ErrorSubCode { get; set; }
    }
}
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Crm.Services.Utility
{
    [XmlRoot("Data")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class LiveDevice
    {
        [XmlAttribute("version")]
        public int Version { get; set; }

        [XmlElement("User")]
        public DeviceUserName User { get; set; }

        [XmlElement("Token")]
        public XmlNode Token { get; set; }

        [XmlElement("Expiry")]
        public string Expiry { get; set; }

        [XmlElement("ClockSkew")]
        public string ClockSkew { get; set; }
    }
}
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Crm.Services.Utility
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    [XmlRoot("Data")]
    public sealed class LiveDevice
    {
        [XmlAttribute("version")]
        public int Version { get; set; }

        [XmlElement("User")]
        public DeviceUserName User { get; set; }

        [SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", Justification = "This is required for proper XML Serialization", MessageId = "System.Xml.XmlNode")]
        [XmlElement("Token")]
        public XmlNode Token { get; set; }

        [XmlElement("Expiry")]
        public string Expiry { get; set; }

        [XmlElement("ClockSkew")]
        public string ClockSkew { get; set; }
    }
}
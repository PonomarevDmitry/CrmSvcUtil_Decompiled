using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.Crm.Services.Utility
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    [XmlRoot("Authentication")]
    public sealed class DeviceRegistrationAuthentication
    {
        [XmlElement("Membername")]
        public string MemberName { get; set; }

        [XmlElement("Password")]
        public string Password { get; set; }
    }
}
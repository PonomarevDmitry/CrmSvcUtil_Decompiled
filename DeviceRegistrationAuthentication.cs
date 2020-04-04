using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.Crm.Services.Utility
{
    /// <summary>Device registration authentication</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [XmlRoot("Authentication")]
    public sealed class DeviceRegistrationAuthentication
    {
        /// <summary>
        /// Gets or sets the device registration authentication member name
        /// </summary>
        [XmlElement("Membername")]
        public string MemberName { get; set; }

        /// <summary>
        /// Gets or sets the device registration authentication password
        /// </summary>
        [XmlElement("Password")]
        public string Password { get; set; }
    }
}
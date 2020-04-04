using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.Crm.Services.Utility
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    [XmlRoot("DeviceAddRequest")]
    public sealed class DeviceRegistrationRequest
    {
        public DeviceRegistrationRequest()
        {
        }

        public DeviceRegistrationRequest(Guid applicationId, LiveDevice device)
          : this()
        {
            if (device == null)
                throw new ArgumentNullException(nameof(device));
            this.ClientInfo = new DeviceRegistrationClientInfo()
            {
                ApplicationId = applicationId,
                Version = "1.0"
            };
            this.Authentication = new DeviceRegistrationAuthentication()
            {
                MemberName = device.User.DeviceId,
                Password = device.User.DecryptedPassword
            };
        }

        [XmlElement("ClientInfo")]
        public DeviceRegistrationClientInfo ClientInfo { get; set; }

        [XmlElement("Authentication")]
        public DeviceRegistrationAuthentication Authentication { get; set; }
    }
}
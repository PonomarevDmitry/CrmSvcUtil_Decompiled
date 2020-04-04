using System;
using System.ComponentModel;
using System.Security.Cryptography;
using System.ServiceModel.Description;
using System.Text;
using System.Xml.Serialization;

namespace Microsoft.Crm.Services.Utility
{
    /// <summary>Device user name</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class DeviceUserName
    {
        /// <summary>Default constructor</summary>
        public DeviceUserName()
        {
            this.UserNameType = "Logical";
        }

        /// <summary>Gets or sets the device user name</summary>
        [XmlAttribute("username")]
        public string DeviceName { get; set; }

        /// <summary>Gets or sets the device user type</summary>
        [XmlAttribute("type")]
        public string UserNameType { get; set; }

        /// <summary>Gets or sets the device user password</summary>
        [XmlElement("Pwd")]
        public string EncryptedPassword { get; set; }

        /// <summary>Gets or sets the device id</summary>
        public string DeviceId
        {
            get
            {
                return "11" + this.DeviceName;
            }
        }

        /// <summary>Gets or sets the device user's decrypted password</summary>
        [XmlIgnore]
        public string DecryptedPassword
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.EncryptedPassword))
                    return this.EncryptedPassword;
                byte[] bytes = ProtectedData.Unprotect(Convert.FromBase64String(this.EncryptedPassword), (byte[])null, DataProtectionScope.LocalMachine);
                if (bytes == null || bytes.Length == 0)
                    return (string)null;
                return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    this.EncryptedPassword = value;
                else
                    this.EncryptedPassword = Convert.ToBase64String(ProtectedData.Protect(Encoding.UTF8.GetBytes(value), (byte[])null, DataProtectionScope.LocalMachine));
            }
        }

        /// <summary>Creates client credentials for the device user</summary>
        /// <returns>An instance of ClientCredentials</returns>
        public ClientCredentials ToClientCredentials()
        {
            return new ClientCredentials()
            {
                UserName = {
          UserName = this.DeviceId,
          Password = this.DecryptedPassword
        }
            };
        }
    }
}
using System;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.Crm.Services.Utility
{
    /// <summary>Device registration response error</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [XmlRoot("Error")]
    public sealed class DeviceRegistrationResponseError
    {
        private string _code;

        /// <summary>Gets or sets the device registration error code</summary>
        [XmlAttribute("Code")]
        public string Code
        {
            get
            {
                return this._code;
            }
            set
            {
                this._code = value;
                int result;
                if (string.IsNullOrEmpty(value) || !value.StartsWith("dc", StringComparison.Ordinal) || (!int.TryParse(value.Substring(2), NumberStyles.Integer, (IFormatProvider)CultureInfo.InvariantCulture, out result) || !Enum.IsDefined(typeof(DeviceRegistrationErrorCode), (object)result)))
                    return;
                this.RegistrationErrorCode = (DeviceRegistrationErrorCode)Enum.ToObject(typeof(DeviceRegistrationErrorCode), result);
            }
        }

        [XmlIgnore]
        internal DeviceRegistrationErrorCode RegistrationErrorCode { get; private set; }
    }
}
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.Crm.Services.Utility
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    [SuppressMessage("Microsoft.Design", "CA1064:ExceptionsShouldBePublic", Justification = "This exception is intended for internal use only.")]
    [Serializable]
    internal sealed class DeviceRegistrationFailedException : Exception
    {
        internal DeviceRegistrationFailedException()
        {
        }

        internal DeviceRegistrationFailedException(DeviceRegistrationErrorCode code, string subCode)
          : this(code, subCode, (Exception)null)
        {
        }

        internal DeviceRegistrationFailedException(
          DeviceRegistrationErrorCode code,
          string subCode,
          Exception innerException)
          : base(code.ToString() + ": " + subCode, innerException)
        {
        }

        private DeviceRegistrationFailedException(SerializationInfo si, StreamingContext sc)
          : base(si, sc)
        {
        }
    }
}
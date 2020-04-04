namespace Microsoft.Crm.Services.Utility
{
    /// <summary>Indicates an error during registration</summary>
    internal enum DeviceRegistrationErrorCode
    {
        /// <summary>Unspecified or Unknown Error occurred</summary>
        Unknown = 0,
        /// <summary>Interface Disabled</summary>
        InterfaceDisabled = 1,
        /// <summary>Invalid Request Format</summary>
        InvalidRequestFormat = 3,
        /// <summary>Unknown Client Version</summary>
        UnknownClientVersion = 4,
        /// <summary>Blank Password</summary>
        BlankPassword = 6,
        /// <summary>Missing Device User Name or Password</summary>
        MissingDeviceUserNameOrPassword = 7,
        /// <summary>Invalid Parameter Syntax</summary>
        InvalidParameterSyntax = 8,
        /// <summary>Internal Error</summary>
        InternalError = 11, // 0x0000000B
                            /// <summary>Device Already Exists</summary>
        DeviceAlreadyExists = 13, // 0x0000000D
    }
}
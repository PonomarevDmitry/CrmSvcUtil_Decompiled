namespace Microsoft.Crm.Services.Utility
{
    internal enum DeviceRegistrationErrorCode
    {
        Unknown = 0,
        InterfaceDisabled = 1,
        InvalidRequestFormat = 3,
        UnknownClientVersion = 4,
        BlankPassword = 6,
        MissingDeviceUserNameOrPassword = 7,
        InvalidParameterSyntax = 8,
        InternalError = 11, // 0x0000000B
        DeviceAlreadyExists = 13, // 0x0000000D
    }
}
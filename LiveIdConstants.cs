using System;
using System.IO;

namespace Microsoft.Crm.Services.Utility
{
    internal static class LiveIdConstants
    {
        public static readonly string LiveDeviceFileNameFormat = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "LiveDeviceID"), "LiveDevice{0}.xml");
        public const string RegistrationEndpointUriFormat = "https://login.live{0}.com/ppsecure/DeviceAddCredential.srf";
        public const string DevicePrefix = "11";
        public const string ValidDeviceNameCharacters = "0123456789abcdefghijklmnopqrstuvqxyz";
        public const int DeviceNameLength = 24;
        public const string ValidDevicePasswordCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^*()-_=+;,./?`~";
        public const int DevicePasswordLength = 24;
    }
}
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.ServiceModel.Description;
using System.Xml.Serialization;

namespace Microsoft.Crm.Services.Utility
{
    internal static class DeviceIdManager
    {
        private static readonly Random RandomInstance = new Random();

        public static ClientCredentials LoadOrRegisterDevice(
          Guid applicationId,
          Uri issuerUri)
        {
            return DeviceIdManager.LoadDeviceCredentials(issuerUri) ?? DeviceIdManager.RegisterDevice(applicationId, issuerUri);
        }

        public static ClientCredentials RegisterDevice(
          Guid applicationId,
          Uri issuerUri)
        {
            return DeviceIdManager.RegisterDevice(applicationId, issuerUri, (string)null, (string)null);
        }

        public static ClientCredentials RegisterDevice(
          Guid applicationId,
          Uri issuerUri,
          string deviceName,
          string devicePassword)
        {
            if (string.IsNullOrWhiteSpace(deviceName) != string.IsNullOrWhiteSpace(devicePassword))
                throw new ArgumentNullException(nameof(deviceName), "Either deviceName/devicePassword should both be specified or they should be null.");
            DeviceUserName userName;
            if (string.IsNullOrWhiteSpace(deviceName))
                userName = DeviceIdManager.GenerateDeviceUserName();
            else
                userName = new DeviceUserName()
                {
                    DeviceName = deviceName,
                    DecryptedPassword = devicePassword
                };
            return DeviceIdManager.RegisterDevice(applicationId, issuerUri, userName);
        }

        public static ClientCredentials LoadDeviceCredentials(Uri issuerUri)
        {
            LiveDevice liveDevice = DeviceIdManager.ReadExistingDevice(DeviceIdManager.DiscoverEnvironment(issuerUri));
            if (liveDevice == null || liveDevice.User == null)
                return (ClientCredentials)null;
            return liveDevice.User.ToClientCredentials();
        }

        private static void Serialize<T>(Stream stream, T value)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), string.Empty);
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            xmlSerializer.Serialize(stream, (object)value, namespaces);
        }

        private static T Deserialize<T>(Stream stream)
        {
            return (T)new XmlSerializer(typeof(T), string.Empty).Deserialize(stream);
        }

        private static FileInfo GetDeviceFile(string environment)
        {
            return new FileInfo(string.Format((IFormatProvider)CultureInfo.InvariantCulture, LiveIdConstants.LiveDeviceFileNameFormat, string.IsNullOrWhiteSpace(environment) ? (object)(string)null : (object)("-" + environment.ToUpperInvariant())));
        }

        private static ClientCredentials RegisterDevice(
          Guid applicationId,
          Uri issuerUri,
          DeviceUserName userName)
        {
            string environment = DeviceIdManager.DiscoverEnvironment(issuerUri);
            LiveDevice device = new LiveDevice()
            {
                User = userName,
                Version = 1
            };
            DeviceRegistrationRequest registrationRequest = new DeviceRegistrationRequest(applicationId, device);
            DeviceRegistrationResponse registrationResponse = DeviceIdManager.ExecuteRegistrationRequest(string.Format((IFormatProvider)CultureInfo.InvariantCulture, "https://login.live{0}.com/ppsecure/DeviceAddCredential.srf", string.IsNullOrWhiteSpace(environment) ? (object)(string)null : (object)("-" + environment)), registrationRequest);
            if (!registrationResponse.IsSuccess)
                throw new DeviceRegistrationFailedException(registrationResponse.Error.RegistrationErrorCode, registrationResponse.ErrorSubCode);
            DeviceIdManager.WriteDevice(environment, device);
            return device.User.ToClientCredentials();
        }

        private static LiveDevice ReadExistingDevice(string environment)
        {
            FileInfo deviceFile = DeviceIdManager.GetDeviceFile(environment);
            if (!deviceFile.Exists)
                return (LiveDevice)null;
            using (FileStream fileStream = deviceFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                return DeviceIdManager.Deserialize<LiveDevice>((Stream)fileStream);
        }

        private static void WriteDevice(string environment, LiveDevice device)
        {
            FileInfo deviceFile = DeviceIdManager.GetDeviceFile(environment);
            if (!deviceFile.Directory.Exists)
                deviceFile.Directory.Create();
            using (FileStream fileStream = deviceFile.Open(FileMode.CreateNew, FileAccess.Write, FileShare.None))
                DeviceIdManager.Serialize<LiveDevice>((Stream)fileStream, device);
        }

        private static DeviceRegistrationResponse ExecuteRegistrationRequest(
          string url,
          DeviceRegistrationRequest registrationRequest)
        {
            WebRequest webRequest = WebRequest.Create(url);
            webRequest.ContentType = "application/soap+xml; charset=UTF-8";
            webRequest.Method = "POST";
            webRequest.Timeout = 180000;
            using (Stream requestStream = webRequest.GetRequestStream())
                DeviceIdManager.Serialize<DeviceRegistrationRequest>(requestStream, registrationRequest);
            try
            {
                using (WebResponse response = webRequest.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                        return DeviceIdManager.Deserialize<DeviceRegistrationResponse>(responseStream);
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    using (Stream responseStream = ex.Response.GetResponseStream())
                        return DeviceIdManager.Deserialize<DeviceRegistrationResponse>(responseStream);
                }
                else
                    throw;
            }
        }

        private static DeviceUserName GenerateDeviceUserName()
        {
            return new DeviceUserName()
            {
                DeviceName = DeviceIdManager.GenerateRandomString("0123456789abcdefghijklmnopqrstuvqxyz", 24),
                DecryptedPassword = DeviceIdManager.GenerateRandomString("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^*()-_=+;,./?`~", 24)
            };
        }

        private static string GenerateRandomString(string characterSet, int count)
        {
            char[] chArray = new char[count];
            char[] charArray = characterSet.ToCharArray();
            lock (DeviceIdManager.RandomInstance)
            {
                for (int index = 0; index < count; ++index)
                    chArray[index] = charArray[DeviceIdManager.RandomInstance.Next(0, charArray.Length)];
            }
            return new string(chArray);
        }

        private static string DiscoverEnvironment(Uri issuerUri)
        {
            if ((Uri)null == issuerUri)
                return (string)null;
            if (issuerUri.Host.Length > "login.live".Length && issuerUri.Host.StartsWith("login.live", StringComparison.OrdinalIgnoreCase))
            {
                string str = issuerUri.Host.Substring("login.live".Length);
                if ('-' == str[0])
                {
                    int num = str.IndexOf('.', 1);
                    if (-1 != num)
                        return str.Substring(1, num - 1);
                }
            }
            return (string)null;
        }
    }
}
// Decompiled with JetBrains decompiler
// Type: Microsoft.Crm.Services.Utility.DeviceRegistrationRequest
// Assembly: CrmSvcUtil, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 756175FA-E97D-49FF-B443-B7A725C9F163
// Assembly location: C:\Users\dmitriy.ponomarev\AppData\Roaming\MscrmTools\XrmToolBox\NugetPlugins\DLaB.Xrm.EarlyBoundGenerator.1.2019.3.19\lib\net462\plugins\DLaB.EarlyBoundGenerator\CrmSvcUtil.exe

using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.Crm.Services.Utility
{
  [XmlRoot("DeviceAddRequest")]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DeviceRegistrationRequest
  {
    public DeviceRegistrationRequest()
    {
    }

    public DeviceRegistrationRequest(Guid applicationId, LiveDevice device)
      : this()
    {
      if (device == null)
        throw new ArgumentNullException(nameof (device));
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

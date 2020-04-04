// Decompiled with JetBrains decompiler
// Type: Microsoft.Crm.Services.Utility.DeviceUserName
// Assembly: CrmSvcUtil, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 756175FA-E97D-49FF-B443-B7A725C9F163
// Assembly location: C:\Users\dmitriy.ponomarev\AppData\Roaming\MscrmTools\XrmToolBox\NugetPlugins\DLaB.Xrm.EarlyBoundGenerator.1.2019.3.19\lib\net462\plugins\DLaB.EarlyBoundGenerator\CrmSvcUtil.exe

using System;
using System.ComponentModel;
using System.Security.Cryptography;
using System.ServiceModel.Description;
using System.Text;
using System.Xml.Serialization;

namespace Microsoft.Crm.Services.Utility
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DeviceUserName
  {
    public DeviceUserName()
    {
      this.UserNameType = "Logical";
    }

    [XmlAttribute("username")]
    public string DeviceName { get; set; }

    [XmlAttribute("type")]
    public string UserNameType { get; set; }

    [XmlElement("Pwd")]
    public string EncryptedPassword { get; set; }

    public string DeviceId
    {
      get
      {
        return "11" + this.DeviceName;
      }
    }

    [XmlIgnore]
    public string DecryptedPassword
    {
      get
      {
        if (string.IsNullOrWhiteSpace(this.EncryptedPassword))
          return this.EncryptedPassword;
        byte[] bytes = ProtectedData.Unprotect(Convert.FromBase64String(this.EncryptedPassword), (byte[]) null, DataProtectionScope.LocalMachine);
        if (bytes == null || bytes.Length == 0)
          return (string) null;
        return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
      }
      set
      {
        if (string.IsNullOrWhiteSpace(value))
          this.EncryptedPassword = value;
        else
          this.EncryptedPassword = Convert.ToBase64String(ProtectedData.Protect(Encoding.UTF8.GetBytes(value), (byte[]) null, DataProtectionScope.LocalMachine));
      }
    }

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

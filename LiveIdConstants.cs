// Decompiled with JetBrains decompiler
// Type: Microsoft.Crm.Services.Utility.LiveIdConstants
// Assembly: CrmSvcUtil, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 756175FA-E97D-49FF-B443-B7A725C9F163
// Assembly location: C:\Users\dmitriy.ponomarev\AppData\Roaming\MscrmTools\XrmToolBox\NugetPlugins\DLaB.Xrm.EarlyBoundGenerator.1.2019.3.19\lib\net462\plugins\DLaB.EarlyBoundGenerator\CrmSvcUtil.exe

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

// Decompiled with JetBrains decompiler
// Type: Microsoft.Crm.Services.Utility.DeviceRegistrationResponseError
// Assembly: CrmSvcUtil, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 756175FA-E97D-49FF-B443-B7A725C9F163
// Assembly location: C:\Users\dmitriy.ponomarev\AppData\Roaming\MscrmTools\XrmToolBox\NugetPlugins\DLaB.Xrm.EarlyBoundGenerator.1.2019.3.19\lib\net462\plugins\DLaB.EarlyBoundGenerator\CrmSvcUtil.exe

using System;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.Crm.Services.Utility
{
  [XmlRoot("Error")]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DeviceRegistrationResponseError
  {
    private string _code;

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
        if (string.IsNullOrEmpty(value) || !value.StartsWith("dc", StringComparison.Ordinal) || (!int.TryParse(value.Substring(2), NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result) || !Enum.IsDefined(typeof (DeviceRegistrationErrorCode), (object) result)))
          return;
        this.RegistrationErrorCode = (DeviceRegistrationErrorCode) Enum.ToObject(typeof (DeviceRegistrationErrorCode), result);
      }
    }

    [XmlIgnore]
    internal DeviceRegistrationErrorCode RegistrationErrorCode { get; private set; }
  }
}

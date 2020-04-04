// Decompiled with JetBrains decompiler
// Type: Microsoft.Crm.Services.Utility.LiveDevice
// Assembly: CrmSvcUtil, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 756175FA-E97D-49FF-B443-B7A725C9F163
// Assembly location: C:\Users\dmitriy.ponomarev\AppData\Roaming\MscrmTools\XrmToolBox\NugetPlugins\DLaB.Xrm.EarlyBoundGenerator.1.2019.3.19\lib\net462\plugins\DLaB.EarlyBoundGenerator\CrmSvcUtil.exe

using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Crm.Services.Utility
{
  [XmlRoot("Data")]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class LiveDevice
  {
    [XmlAttribute("version")]
    public int Version { get; set; }

    [XmlElement("User")]
    public DeviceUserName User { get; set; }

    [XmlElement("Token")]
    public XmlNode Token { get; set; }

    [XmlElement("Expiry")]
    public string Expiry { get; set; }

    [XmlElement("ClockSkew")]
    public string ClockSkew { get; set; }
  }
}

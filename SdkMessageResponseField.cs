// Decompiled with JetBrains decompiler
// Type: Microsoft.Crm.Services.Utility.SdkMessageResponseField
// Assembly: CrmSvcUtil, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 756175FA-E97D-49FF-B443-B7A725C9F163
// Assembly location: C:\Users\dmitriy.ponomarev\AppData\Roaming\MscrmTools\XrmToolBox\NugetPlugins\DLaB.Xrm.EarlyBoundGenerator.1.2019.3.19\lib\net462\plugins\DLaB.EarlyBoundGenerator\CrmSvcUtil.exe

namespace Microsoft.Crm.Services.Utility
{
  public sealed class SdkMessageResponseField
  {
    private int _index;
    private string _name;
    private string _clrFormatter;
    private string _value;

    public SdkMessageResponseField(int index, string name, string clrFormatter, string value)
    {
      this._clrFormatter = clrFormatter;
      this._index = index;
      this._name = name;
      this._value = value;
    }

    public int Index
    {
      get
      {
        return this._index;
      }
    }

    public string Name
    {
      get
      {
        return this._name;
      }
    }

    public string CLRFormatter
    {
      get
      {
        return this._clrFormatter;
      }
    }

    public string Value
    {
      get
      {
        return this._value;
      }
    }
  }
}

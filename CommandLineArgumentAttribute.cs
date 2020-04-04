// Decompiled with JetBrains decompiler
// Type: Microsoft.Crm.Services.Utility.CommandLineArgumentAttribute
// Assembly: CrmSvcUtil, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 756175FA-E97D-49FF-B443-B7A725C9F163
// Assembly location: C:\Users\dmitriy.ponomarev\AppData\Roaming\MscrmTools\XrmToolBox\NugetPlugins\DLaB.Xrm.EarlyBoundGenerator.1.2019.3.19\lib\net462\plugins\DLaB.EarlyBoundGenerator\CrmSvcUtil.exe

using System;

namespace Microsoft.Crm.Services.Utility
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
  internal sealed class CommandLineArgumentAttribute : Attribute
  {
    private ArgumentType _argumentType;
    private string _fullname;
    private string _shortname;
    private string _description;
    private string _parameterDescription;
    private string _sampleUsageValue;

    internal CommandLineArgumentAttribute(ArgumentType argType, string name)
    {
      this._argumentType = argType;
      this._fullname = name;
      this._shortname = string.Empty;
      this._description = string.Empty;
      this._parameterDescription = string.Empty;
    }

    public ArgumentType Type
    {
      get
      {
        return this._argumentType;
      }
    }

    public string Name
    {
      get
      {
        return this._fullname;
      }
      set
      {
        this._fullname = value;
      }
    }

    public string Shortcut
    {
      get
      {
        return this._shortname;
      }
      set
      {
        this._shortname = value;
      }
    }

    public string Description
    {
      get
      {
        return this._description;
      }
      set
      {
        this._description = value;
      }
    }

    public string ParameterDescription
    {
      get
      {
        return this._parameterDescription;
      }
      set
      {
        this._parameterDescription = value;
      }
    }

    public string SampleUsageValue
    {
      get
      {
        return this._sampleUsageValue;
      }
      set
      {
        this._sampleUsageValue = value;
      }
    }
  }
}

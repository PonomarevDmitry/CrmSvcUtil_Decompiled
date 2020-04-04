// Decompiled with JetBrains decompiler
// Type: Microsoft.Crm.Services.Utility.SdkMessageFilter
// Assembly: CrmSvcUtil, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 756175FA-E97D-49FF-B443-B7A725C9F163
// Assembly location: C:\Users\dmitriy.ponomarev\AppData\Roaming\MscrmTools\XrmToolBox\NugetPlugins\DLaB.Xrm.EarlyBoundGenerator.1.2019.3.19\lib\net462\plugins\DLaB.EarlyBoundGenerator\CrmSvcUtil.exe

using System;

namespace Microsoft.Crm.Services.Utility
{
  public sealed class SdkMessageFilter
  {
    private int _primaryObjectTypeCode;
    private int _secondaryObjectTypeCode;
    private Guid _id;
    private bool _isVisible;

    public SdkMessageFilter(Guid id)
    {
      this._id = id;
    }

    public SdkMessageFilter(
      Guid id,
      int primaryObjectTypeCode,
      int secondaryObjectTypeCode,
      bool isVisible)
    {
      this._id = id;
      this.PrimaryObjectTypeCode = primaryObjectTypeCode;
      this.SecondaryObjectTypeCode = secondaryObjectTypeCode;
      this.IsVisible = isVisible;
    }

    public Guid Id
    {
      get
      {
        return this._id;
      }
    }

    public int PrimaryObjectTypeCode
    {
      get
      {
        return this._primaryObjectTypeCode;
      }
      private set
      {
        this._primaryObjectTypeCode = value;
      }
    }

    public int SecondaryObjectTypeCode
    {
      get
      {
        return this._secondaryObjectTypeCode;
      }
      private set
      {
        this._secondaryObjectTypeCode = value;
      }
    }

    public bool IsVisible
    {
      get
      {
        return this._isVisible;
      }
      private set
      {
        this._isVisible = value;
      }
    }

    internal void Fill(Result result)
    {
      this.PrimaryObjectTypeCode = result.SdkMessagePrimaryOTCFilter;
      this.SecondaryObjectTypeCode = result.SdkMessageSecondaryOTCFilter;
      this.IsVisible = false;
    }
  }
}

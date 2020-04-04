// Decompiled with JetBrains decompiler
// Type: Microsoft.Crm.Services.Utility.SdkMessageResponse
// Assembly: CrmSvcUtil, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 756175FA-E97D-49FF-B443-B7A725C9F163
// Assembly location: C:\Users\dmitriy.ponomarev\AppData\Roaming\MscrmTools\XrmToolBox\NugetPlugins\DLaB.Xrm.EarlyBoundGenerator.1.2019.3.19\lib\net462\plugins\DLaB.EarlyBoundGenerator\CrmSvcUtil.exe

using System;
using System.Collections.Generic;

namespace Microsoft.Crm.Services.Utility
{
  public sealed class SdkMessageResponse
  {
    private Guid _id;
    private Dictionary<int, SdkMessageResponseField> _responseFields;

    public SdkMessageResponse(Guid id)
    {
      this._id = id;
      this._responseFields = new Dictionary<int, SdkMessageResponseField>();
    }

    public Guid Id
    {
      get
      {
        return this._id;
      }
    }

    public Dictionary<int, SdkMessageResponseField> ResponseFields
    {
      get
      {
        return this._responseFields;
      }
    }

    internal void Fill(Result result)
    {
      if (!result.SdkMessageResponseFieldPosition.HasValue)
        return;
      if (!this.ResponseFields.ContainsKey(result.SdkMessageResponseFieldPosition.Value))
      {
        SdkMessageResponseField messageResponseField = new SdkMessageResponseField(result.SdkMessageResponseFieldPosition.Value, result.SdkMessageResponseFieldName, result.SdkMessageResponseFieldClrFormatter, result.SdkMessageResponseFieldValue);
        this.ResponseFields.Add(result.SdkMessageResponseFieldPosition.Value, messageResponseField);
      }
      SdkMessageResponseField responseField = this.ResponseFields[result.SdkMessageResponseFieldPosition.Value];
    }
  }
}

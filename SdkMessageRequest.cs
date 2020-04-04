// Decompiled with JetBrains decompiler
// Type: Microsoft.Crm.Services.Utility.SdkMessageRequest
// Assembly: CrmSvcUtil, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 756175FA-E97D-49FF-B443-B7A725C9F163
// Assembly location: C:\Users\dmitriy.ponomarev\AppData\Roaming\MscrmTools\XrmToolBox\NugetPlugins\DLaB.Xrm.EarlyBoundGenerator.1.2019.3.19\lib\net462\plugins\DLaB.EarlyBoundGenerator\CrmSvcUtil.exe

using System;
using System.Collections.Generic;

namespace Microsoft.Crm.Services.Utility
{
  public sealed class SdkMessageRequest
  {
    private Guid _id;
    private SdkMessagePair _messagePair;
    private string _name;
    private Dictionary<int, SdkMessageRequestField> _requestFields;

    public SdkMessageRequest(SdkMessagePair message, Guid id, string name)
    {
      this._id = id;
      this._name = name;
      this._messagePair = message;
      this._requestFields = new Dictionary<int, SdkMessageRequestField>();
    }

    public Guid Id
    {
      get
      {
        return this._id;
      }
    }

    public SdkMessagePair MessagePair
    {
      get
      {
        return this._messagePair;
      }
    }

    public string Name
    {
      get
      {
        return this._name;
      }
    }

    public Dictionary<int, SdkMessageRequestField> RequestFields
    {
      get
      {
        return this._requestFields;
      }
    }

    internal void Fill(Result result)
    {
      if (!result.SdkMessageRequestFieldPosition.HasValue)
        return;
      if (!this.RequestFields.ContainsKey(result.SdkMessageRequestFieldPosition.Value))
      {
        SdkMessageRequestField messageRequestField = new SdkMessageRequestField(this, result.SdkMessageRequestFieldPosition.Value, result.SdkMessageRequestFieldName, result.SdkMessageRequestFieldClrParser, result.SdkMessageRequestFieldIsOptional);
        this.RequestFields.Add(result.SdkMessageRequestFieldPosition.Value, messageRequestField);
      }
      SdkMessageRequestField requestField = this.RequestFields[result.SdkMessageRequestFieldPosition.Value];
    }
  }
}

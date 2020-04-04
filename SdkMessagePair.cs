// Decompiled with JetBrains decompiler
// Type: Microsoft.Crm.Services.Utility.SdkMessagePair
// Assembly: CrmSvcUtil, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 756175FA-E97D-49FF-B443-B7A725C9F163
// Assembly location: C:\Users\dmitriy.ponomarev\AppData\Roaming\MscrmTools\XrmToolBox\NugetPlugins\DLaB.Xrm.EarlyBoundGenerator.1.2019.3.19\lib\net462\plugins\DLaB.EarlyBoundGenerator\CrmSvcUtil.exe

using System;

namespace Microsoft.Crm.Services.Utility
{
  public sealed class SdkMessagePair
  {
    private Guid _id;
    private string _messageNamespace;
    private SdkMessage _message;
    private SdkMessageRequest _sdkMessageRequest;
    private SdkMessageResponse _sdkMessageResponse;

    public SdkMessagePair(SdkMessage message, Guid id, string messageNamespace)
    {
      this._message = message;
      this._id = id;
      this._messageNamespace = messageNamespace;
    }

    public Guid Id
    {
      get
      {
        return this._id;
      }
    }

    public string MessageNamespace
    {
      get
      {
        return this._messageNamespace;
      }
    }

    public SdkMessage Message
    {
      get
      {
        return this._message;
      }
      set
      {
        this._message = value;
      }
    }

    public SdkMessageRequest Request
    {
      get
      {
        return this._sdkMessageRequest;
      }
      set
      {
        this._sdkMessageRequest = value;
      }
    }

    public SdkMessageResponse Response
    {
      get
      {
        return this._sdkMessageResponse;
      }
      set
      {
        this._sdkMessageResponse = value;
      }
    }

    internal void Fill(Result result)
    {
      if (result.SdkMessageRequestId != Guid.Empty)
      {
        if (this.Request == null)
          this.Request = new SdkMessageRequest(this, result.SdkMessageRequestId, result.SdkMessageRequestName);
        this.Request.Fill(result);
      }
      if (!(result.SdkMessageResponseId != Guid.Empty))
        return;
      if (this.Response == null)
        this.Response = new SdkMessageResponse(result.SdkMessageResponseId);
      this.Response.Fill(result);
    }
  }
}

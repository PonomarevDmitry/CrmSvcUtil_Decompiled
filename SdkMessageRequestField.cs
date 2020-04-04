// Decompiled with JetBrains decompiler
// Type: Microsoft.Crm.Services.Utility.SdkMessageRequestField
// Assembly: CrmSvcUtil, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 756175FA-E97D-49FF-B443-B7A725C9F163
// Assembly location: C:\Users\dmitriy.ponomarev\AppData\Roaming\MscrmTools\XrmToolBox\NugetPlugins\DLaB.Xrm.EarlyBoundGenerator.1.2019.3.19\lib\net462\plugins\DLaB.EarlyBoundGenerator\CrmSvcUtil.exe

using System;

namespace Microsoft.Crm.Services.Utility
{
    public sealed class SdkMessageRequestField
    {
        private const string EntityTypeName = "Microsoft.Xrm.Sdk.Entity,Microsoft.Xrm.Sdk";
        private SdkMessageRequest _request;
        private int _index;
        private string _name;
        private string _clrFormatter;
        private bool _isOptional;

        public SdkMessageRequestField(
          SdkMessageRequest request,
          int index,
          string name,
          string clrFormatter,
          bool isOptional)
        {
            this._request = request;
            this._clrFormatter = clrFormatter;
            this._name = name;
            this._index = index;
            this._isOptional = isOptional;
        }

        public SdkMessageRequest Request
        {
            get
            {
                return this._request;
            }
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

        public bool IsOptional
        {
            get
            {
                return this._isOptional;
            }
        }

        public bool IsGeneric
        {
            get
            {
                if (string.Equals(this.CLRFormatter, EntityTypeName, StringComparison.Ordinal))
                    return this.Request.MessagePair.Message.SdkMessageFilters.Count > 1;
                return false;
            }
        }
    }
}

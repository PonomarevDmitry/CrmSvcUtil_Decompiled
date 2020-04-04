using System;
using System.Diagnostics.CodeAnalysis;

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

        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "CLR")]
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
                if (string.Equals(this.CLRFormatter, "Microsoft.Xrm.Sdk.Entity,Microsoft.Xrm.Sdk", StringComparison.Ordinal))
                    return this.Request.MessagePair.Message.SdkMessageFilters.Count > 1;
                return false;
            }
        }
    }
}
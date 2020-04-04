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
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
            Dictionary<int, SdkMessageResponseField> responseFields1 = this.ResponseFields;
            int? responseFieldPosition = result.SdkMessageResponseFieldPosition;
            int key1 = responseFieldPosition.Value;
            if (!responseFields1.ContainsKey(key1))
            {
                responseFieldPosition = result.SdkMessageResponseFieldPosition;
                SdkMessageResponseField messageResponseField1 = new SdkMessageResponseField(responseFieldPosition.Value, result.SdkMessageResponseFieldName, result.SdkMessageResponseFieldClrFormatter, result.SdkMessageResponseFieldValue);
                Dictionary<int, SdkMessageResponseField> responseFields2 = this.ResponseFields;
                responseFieldPosition = result.SdkMessageResponseFieldPosition;
                int key2 = responseFieldPosition.Value;
                SdkMessageResponseField messageResponseField2 = messageResponseField1;
                responseFields2.Add(key2, messageResponseField2);
            }
            Dictionary<int, SdkMessageResponseField> responseFields3 = this.ResponseFields;
            responseFieldPosition = result.SdkMessageResponseFieldPosition;
            int index = responseFieldPosition.Value;
            SdkMessageResponseField messageResponseField = responseFields3[index];
        }
    }
}
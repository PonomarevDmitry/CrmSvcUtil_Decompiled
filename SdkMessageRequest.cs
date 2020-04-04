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
            Dictionary<int, SdkMessageRequestField> requestFields1 = this.RequestFields;
            int? requestFieldPosition = result.SdkMessageRequestFieldPosition;
            int key1 = requestFieldPosition.Value;
            if (!requestFields1.ContainsKey(key1))
            {
                requestFieldPosition = result.SdkMessageRequestFieldPosition;
                SdkMessageRequestField messageRequestField1 = new SdkMessageRequestField(this, requestFieldPosition.Value, result.SdkMessageRequestFieldName, result.SdkMessageRequestFieldClrParser, result.SdkMessageRequestFieldIsOptional);
                Dictionary<int, SdkMessageRequestField> requestFields2 = this.RequestFields;
                requestFieldPosition = result.SdkMessageRequestFieldPosition;
                int key2 = requestFieldPosition.Value;
                SdkMessageRequestField messageRequestField2 = messageRequestField1;
                requestFields2.Add(key2, messageRequestField2);
            }
            Dictionary<int, SdkMessageRequestField> requestFields3 = this.RequestFields;
            requestFieldPosition = result.SdkMessageRequestFieldPosition;
            int index = requestFieldPosition.Value;
            SdkMessageRequestField messageRequestField = requestFields3[index];
        }
    }
}
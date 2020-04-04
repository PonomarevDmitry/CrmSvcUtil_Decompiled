using System;
using System.Collections.Generic;

namespace Microsoft.Crm.Services.Utility
{
    /// <summary>An SDK message request</summary>
    public sealed class SdkMessageRequest
    {
        private Guid _id;
        private SdkMessagePair _messagePair;
        private string _name;
        private Dictionary<int, SdkMessageRequestField> _requestFields;

        /// <summary>Constructor</summary>
        /// <param name="message">SDK Message</param>
        /// <param name="id">Message request id</param>
        /// <param name="name">Message request name</param>
        public SdkMessageRequest(SdkMessagePair message, Guid id, string name)
        {
            this._id = id;
            this._name = name;
            this._messagePair = message;
            this._requestFields = new Dictionary<int, SdkMessageRequestField>();
        }

        /// <summary>Gets the message request id</summary>
        public Guid Id
        {
            get
            {
                return this._id;
            }
        }

        /// <summary>Gets the message pair of the request</summary>
        public SdkMessagePair MessagePair
        {
            get
            {
                return this._messagePair;
            }
        }

        /// <summary>Gets the message request name</summary>
        public string Name
        {
            get
            {
                return this._name;
            }
        }

        /// <summary>Gets a dictionary of message request fields</summary>
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
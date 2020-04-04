using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Microsoft.Crm.Services.Utility
{
    public sealed class SdkMessages
    {
        private Dictionary<Guid, SdkMessage> _messages;

        private SdkMessages()
          : this((Dictionary<Guid, SdkMessage>)null)
        {
        }

        public SdkMessages(Dictionary<Guid, SdkMessage> messageCollection)
        {
            this._messages = messageCollection ?? new Dictionary<Guid, SdkMessage>();
        }

        public Dictionary<Guid, SdkMessage> MessageCollection
        {
            get
            {
                return this._messages;
            }
        }

        private void Fill(ResultSet resultSet)
        {
            if (resultSet.Results == null)
                return;
            foreach (Result result in resultSet.Results)
            {
                if (result.SdkMessageId != Guid.Empty && !this.MessageCollection.ContainsKey(result.SdkMessageId))
                {
                    SdkMessage sdkMessage = new SdkMessage(result.SdkMessageId, result.Name, result.IsPrivate, result.CustomizationLevel);
                    this.MessageCollection.Add(result.SdkMessageId, sdkMessage);
                }
                this.MessageCollection[result.SdkMessageId].Fill(result);
            }
        }

        internal static MessagePagingInfo FromFetchResult(
          SdkMessages messages,
          string xml)
        {
            ResultSet resultSet = (ResultSet)null;
            using (StringReader stringReader = new StringReader(xml))
                resultSet = new XmlSerializer(typeof(ResultSet), string.Empty).Deserialize((TextReader)stringReader) as ResultSet;
            messages.Fill(resultSet);
            return MessagePagingInfo.FromResultSet(resultSet);
        }
    }
}
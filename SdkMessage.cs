using System;
using System.Collections.Generic;

namespace Microsoft.Crm.Services.Utility
{
    public sealed class SdkMessage
    {
        private string _name;
        private Guid _id;
        private bool _isPrivate;
        private bool _isCustomAction;
        private Dictionary<Guid, SdkMessagePair> _sdkMessagePairs;
        private Dictionary<Guid, SdkMessageFilter> _sdkMessageFilters;

        public SdkMessage(Guid id, string name, bool isPrivate)
          : this(id, name, isPrivate, (byte)0)
        {
        }

        internal SdkMessage(Guid id, string name, bool isPrivate, byte customizationLevel)
        {
            this._id = id;
            this._isPrivate = isPrivate;
            this._name = name;
            this._isCustomAction = customizationLevel > (byte)0;
            this._sdkMessagePairs = new Dictionary<Guid, SdkMessagePair>();
            this._sdkMessageFilters = new Dictionary<Guid, SdkMessageFilter>();
        }

        public string Name
        {
            get
            {
                return this._name;
            }
        }

        public Guid Id
        {
            get
            {
                return this._id;
            }
        }

        public bool IsPrivate
        {
            get
            {
                return this._isPrivate;
            }
        }

        public bool IsCustomAction
        {
            get
            {
                return this._isCustomAction;
            }
        }

        public Dictionary<Guid, SdkMessagePair> SdkMessagePairs
        {
            get
            {
                return this._sdkMessagePairs;
            }
        }

        public Dictionary<Guid, SdkMessageFilter> SdkMessageFilters
        {
            get
            {
                return this._sdkMessageFilters;
            }
        }

        internal void Fill(Result result)
        {
            if (!this.SdkMessagePairs.ContainsKey(result.SdkMessagePairId))
            {
                SdkMessagePair sdkMessagePair = new SdkMessagePair(this, result.SdkMessagePairId, result.SdkMessagePairNamespace);
                this._sdkMessagePairs.Add(sdkMessagePair.Id, sdkMessagePair);
            }
            this.SdkMessagePairs[result.SdkMessagePairId].Fill(result);
            if (!this.SdkMessageFilters.ContainsKey(result.SdkMessageFilterId))
            {
                SdkMessageFilter sdkMessageFilter = new SdkMessageFilter(result.SdkMessageFilterId);
                this.SdkMessageFilters.Add(result.SdkMessageFilterId, sdkMessageFilter);
            }
            this.SdkMessageFilters[result.SdkMessageFilterId].Fill(result);
        }
    }
}
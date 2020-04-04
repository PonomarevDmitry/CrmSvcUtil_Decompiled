using System;
using System.Collections.Generic;

namespace Microsoft.Crm.Services.Utility
{
    /// <summary>An SDK Message</summary>
    public sealed class SdkMessage
    {
        private string _name;
        private Guid _id;
        private bool _isPrivate;
        private bool _isCustomAction;
        private Dictionary<Guid, SdkMessagePair> _sdkMessagePairs;
        private Dictionary<Guid, SdkMessageFilter> _sdkMessageFilters;

        /// <summary>Default constructor</summary>
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

        /// <summary>Gets the SDK message name</summary>
        public string Name
        {
            get
            {
                return this._name;
            }
        }

        /// <summary>Gets the SDK message id</summary>
        public Guid Id
        {
            get
            {
                return this._id;
            }
        }

        /// <summary>Gets whether the SDK message is private</summary>
        public bool IsPrivate
        {
            get
            {
                return this._isPrivate;
            }
        }

        /// <summary>Gets whether the SDK message is a custom action</summary>
        public bool IsCustomAction
        {
            get
            {
                return this._isCustomAction;
            }
        }

        /// <summary>Gets a dictionary of message pairs</summary>
        public Dictionary<Guid, SdkMessagePair> SdkMessagePairs
        {
            get
            {
                return this._sdkMessagePairs;
            }
        }

        /// <summary>Gets a dictionary of message filters</summary>
        public Dictionary<Guid, SdkMessageFilter> SdkMessageFilters
        {
            get
            {
                return this._sdkMessageFilters;
            }
        }

        /// <summary>Fills an SDK message from a given result</summary>
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
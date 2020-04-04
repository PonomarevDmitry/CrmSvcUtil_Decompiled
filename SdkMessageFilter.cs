using System;

namespace Microsoft.Crm.Services.Utility
{
    public sealed class SdkMessageFilter
    {
        private int _primaryObjectTypeCode;
        private int _secondaryObjectTypeCode;
        private Guid _id;
        private bool _isVisible;

        public SdkMessageFilter(Guid id)
        {
            this._id = id;
        }

        public SdkMessageFilter(
          Guid id,
          int primaryObjectTypeCode,
          int secondaryObjectTypeCode,
          bool isVisible)
        {
            this._id = id;
            this.PrimaryObjectTypeCode = primaryObjectTypeCode;
            this.SecondaryObjectTypeCode = secondaryObjectTypeCode;
            this.IsVisible = isVisible;
        }

        public Guid Id
        {
            get
            {
                return this._id;
            }
        }

        public int PrimaryObjectTypeCode
        {
            get
            {
                return this._primaryObjectTypeCode;
            }
            private set
            {
                this._primaryObjectTypeCode = value;
            }
        }

        public int SecondaryObjectTypeCode
        {
            get
            {
                return this._secondaryObjectTypeCode;
            }
            private set
            {
                this._secondaryObjectTypeCode = value;
            }
        }

        public bool IsVisible
        {
            get
            {
                return this._isVisible;
            }
            private set
            {
                this._isVisible = value;
            }
        }

        internal void Fill(Result result)
        {
            this.PrimaryObjectTypeCode = result.SdkMessagePrimaryOTCFilter;
            this.SecondaryObjectTypeCode = result.SdkMessageSecondaryOTCFilter;
            this.IsVisible = false;
        }
    }
}
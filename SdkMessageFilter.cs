using System;

namespace Microsoft.Crm.Services.Utility
{
    /// <summary>An SDK message filter</summary>
    public sealed class SdkMessageFilter
    {
        private int _primaryObjectTypeCode;
        private int _secondaryObjectTypeCode;
        private Guid _id;
        private bool _isVisible;

        /// <summary>Constructor</summary>
        /// <param name="id">Message filter id</param>
        public SdkMessageFilter(Guid id)
        {
            this._id = id;
        }

        /// <summary>Constructor</summary>
        /// <param name="id">Message filter id</param>
        /// <param name="primaryObjectTypeCode">Primary object type code</param>
        /// <param name="secondaryObjectTypeCode">Secondary object type code</param>
        /// <param name="isVisible">Whether the message filter is visible</param>
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

        /// <summary>Gets the message filter id</summary>
        public Guid Id
        {
            get
            {
                return this._id;
            }
        }

        /// <summary>
        /// Gets or sets the message filter primary object type code
        /// </summary>
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

        /// <summary>Gets or sets the message secondary object type code</summary>
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

        /// <summary>Gets or sets whether the message filter is visible</summary>
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
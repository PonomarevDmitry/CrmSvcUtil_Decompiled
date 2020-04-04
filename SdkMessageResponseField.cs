namespace Microsoft.Crm.Services.Utility
{
    /// <summary>An SDK message response field</summary>
    public sealed class SdkMessageResponseField
    {
        private int _index;
        private string _name;
        private string _clrFormatter;
        private string _value;

        /// <summary>Constructor</summary>
        /// <param name="index">Field index</param>
        /// <param name="name">Field name</param>
        /// <param name="clrFormatter">Field CLR formatter</param>
        /// <param name="value">Field value</param>
        public SdkMessageResponseField(int index, string name, string clrFormatter, string value)
        {
            this._clrFormatter = clrFormatter;
            this._index = index;
            this._name = name;
            this._value = value;
        }

        /// <summary>Gets the message response field index</summary>
        public int Index
        {
            get
            {
                return this._index;
            }
        }

        /// <summary>Gets the message response field name</summary>
        public string Name
        {
            get
            {
                return this._name;
            }
        }

        /// <summary>Gets the message response field CLR formatter</summary>
        public string CLRFormatter
        {
            get
            {
                return this._clrFormatter;
            }
        }

        /// <summary>Gets the message response field value</summary>
        public string Value
        {
            get
            {
                return this._value;
            }
        }
    }
}
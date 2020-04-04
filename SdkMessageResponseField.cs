using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Crm.Services.Utility
{
    public sealed class SdkMessageResponseField
    {
        private int _index;
        private string _name;
        private string _clrFormatter;
        private string _value;

        public SdkMessageResponseField(int index, string name, string clrFormatter, string value)
        {
            this._clrFormatter = clrFormatter;
            this._index = index;
            this._name = name;
            this._value = value;
        }

        public int Index
        {
            get
            {
                return this._index;
            }
        }

        public string Name
        {
            get
            {
                return this._name;
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "CLR")]
        public string CLRFormatter
        {
            get
            {
                return this._clrFormatter;
            }
        }

        public string Value
        {
            get
            {
                return this._value;
            }
        }
    }
}
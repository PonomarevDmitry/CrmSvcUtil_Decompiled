using System;
using System.Xml.Serialization;

namespace Microsoft.Crm.Services.Utility
{
    /// <summary>Set of SDK message results</summary>
    [XmlType(Namespace = "")]
    [XmlRoot(ElementName = "resultset", Namespace = "")]
    [Serializable]
    public sealed class ResultSet
    {
        private Result[] _results;
        private string _pagingCookie;
        private int _moreRecords;

        /// <summary>Gets or sets an array of results</summary>
        [XmlElement("result")]
        public Result[] Results
        {
            get
            {
                return this._results;
            }
            set
            {
                this._results = value;
            }
        }

        /// <summary>Gets or sets the paging cookie</summary>
        [XmlAttribute("paging-cookie")]
        public string PagingCookie
        {
            get
            {
                return this._pagingCookie;
            }
            set
            {
                this._pagingCookie = value;
            }
        }

        /// <summary>Gets or sets a flag for more records</summary>
        [XmlAttribute("morerecords")]
        public int MoreRecords
        {
            get
            {
                return this._moreRecords;
            }
            set
            {
                this._moreRecords = value;
            }
        }
    }
}
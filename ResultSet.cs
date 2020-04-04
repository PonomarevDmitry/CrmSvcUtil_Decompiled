using System;
using System.Xml.Serialization;

namespace Microsoft.Crm.Services.Utility
{
    [XmlRoot(ElementName = "resultset", Namespace = "")]
    [XmlType(Namespace = "")]
    [Serializable]
    public sealed class ResultSet
    {
        private Result[] _results;
        private string _pagingCookie;
        private int _moreRecords;

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
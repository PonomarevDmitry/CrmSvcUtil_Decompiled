using System;
using System.Globalization;

namespace Microsoft.Crm.Services.Utility
{
    public sealed class MessagePagingInfo
    {
        public string PagingCookig { get; set; }

        public bool HasMoreRecords { get; set; }

        public static MessagePagingInfo FromResultSet(ResultSet resultSet)
        {
            return new MessagePagingInfo()
            {
                PagingCookig = resultSet.PagingCookie,
                HasMoreRecords = Convert.ToBoolean((object)resultSet.MoreRecords, (IFormatProvider)CultureInfo.InvariantCulture)
            };
        }
    }
}
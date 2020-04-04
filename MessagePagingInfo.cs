using System;
using System.Globalization;

namespace Microsoft.Crm.Services.Utility
{
    internal sealed class MessagePagingInfo
    {
        internal string PagingCookig { get; set; }

        internal bool HasMoreRecords { get; set; }

        internal static MessagePagingInfo FromResultSet(ResultSet resultSet)
        {
            return new MessagePagingInfo()
            {
                PagingCookig = resultSet.PagingCookie,
                HasMoreRecords = Convert.ToBoolean((object)resultSet.MoreRecords, (IFormatProvider)CultureInfo.InvariantCulture)
            };
        }
    }
}
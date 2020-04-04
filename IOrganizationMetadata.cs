using Microsoft.Xrm.Sdk.Metadata;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Crm.Services.Utility
{
    public interface IOrganizationMetadata
    {
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        EntityMetadata[] Entities { get; }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        OptionSetMetadataBase[] OptionSets { get; }

        SdkMessages Messages { get; }
    }
}
using Microsoft.Xrm.Sdk.Metadata;

namespace Microsoft.Crm.Services.Utility
{
    public interface IOrganizationMetadata
    {
        EntityMetadata[] Entities { get; }

        OptionSetMetadataBase[] OptionSets { get; }

        SdkMessages Messages { get; }
    }
}
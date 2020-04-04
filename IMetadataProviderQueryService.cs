using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace Microsoft.Crm.Services.Utility
{
    public interface IMetadataProviderQueryService
    {
        EntityMetadata[] RetrieveEntities(IOrganizationService service);

        OptionSetMetadataBase[] RetrieveOptionSets(IOrganizationService service);

        SdkMessages RetrieveSdkRequests(IOrganizationService service);
    }
}
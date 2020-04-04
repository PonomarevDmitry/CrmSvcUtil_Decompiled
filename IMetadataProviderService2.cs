using System;

namespace Microsoft.Crm.Services.Utility
{
    public interface IMetadataProviderService2 : IMetadataProviderService
    {
        IOrganizationMetadata LoadMetadata(IServiceProvider service);
    }
}
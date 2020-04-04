using System;

namespace Microsoft.Crm.Services.Utility
{
    /// <summary>
    /// Interface that provides metadata for a given organization.
    /// </summary>
    public interface IMetadataProviderService2 : IMetadataProviderService
    {
        /// <summary>Loads metadata for the given service</summary>
        /// <param name="service">Service to query</param>
        /// <returns>IOrganizationMetadata</returns>
        IOrganizationMetadata LoadMetadata(IServiceProvider service);
    }
}
using Microsoft.Xrm.Sdk.Metadata;
using System.Diagnostics;
using System.Reflection;

namespace Microsoft.Crm.Services.Utility
{
    internal sealed class OrganizationMetadata : IOrganizationMetadata
    {
        private readonly EntityMetadata[] _entities;
        private readonly OptionSetMetadataBase[] _optionSets;
        private readonly SdkMessages _sdkMessages;

        internal OrganizationMetadata(
          EntityMetadata[] entities,
          OptionSetMetadataBase[] optionSets,
          SdkMessages messages)
        {
            Trace.TraceInformation("Entering {0}", (object)MethodBase.GetCurrentMethod().Name);
            this._entities = entities;
            this._optionSets = optionSets;
            this._sdkMessages = messages;
            Trace.TraceInformation("Exiting {0}", (object)MethodBase.GetCurrentMethod().Name);
        }

        EntityMetadata[] IOrganizationMetadata.Entities
        {
            get
            {
                return this._entities;
            }
        }

        OptionSetMetadataBase[] IOrganizationMetadata.OptionSets
        {
            get
            {
                return this._optionSets;
            }
        }

        SdkMessages IOrganizationMetadata.Messages
        {
            get
            {
                return this._sdkMessages;
            }
        }
    }
}
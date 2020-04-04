using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;

namespace Microsoft.Crm.Services.Utility
{
    public interface INamingService
    {
        string GetNameForOptionSet(
          EntityMetadata entityMetadata,
          OptionSetMetadataBase optionSetMetadata,
          IServiceProvider services);

        string GetNameForOption(
          OptionSetMetadataBase optionSetMetadata,
          OptionMetadata optionMetadata,
          IServiceProvider services);

        string GetNameForEntity(EntityMetadata entityMetadata, IServiceProvider services);

        string GetNameForAttribute(
          EntityMetadata entityMetadata,
          AttributeMetadata attributeMetadata,
          IServiceProvider services);

        string GetNameForRelationship(
          EntityMetadata entityMetadata,
          RelationshipMetadataBase relationshipMetadata,
          EntityRole? reflexiveRole,
          IServiceProvider services);

        string GetNameForServiceContext(IServiceProvider services);

        string GetNameForEntitySet(EntityMetadata entityMetadata, IServiceProvider services);

        string GetNameForMessagePair(SdkMessagePair messagePair, IServiceProvider services);

        string GetNameForRequestField(
          SdkMessageRequest request,
          SdkMessageRequestField requestField,
          IServiceProvider services);

        string GetNameForResponseField(
          SdkMessageResponse response,
          SdkMessageResponseField responseField,
          IServiceProvider services);
    }
}
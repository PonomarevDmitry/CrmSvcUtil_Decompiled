using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.CodeDom;

namespace Microsoft.Crm.Services.Utility
{
    internal interface ITypeMappingService
    {
        CodeTypeReference GetTypeForEntity(
          EntityMetadata entityMetadata,
          IServiceProvider services);

        CodeTypeReference GetTypeForAttributeType(
          EntityMetadata entityMetadata,
          AttributeMetadata attributeMetadata,
          IServiceProvider services);

        CodeTypeReference GetTypeForRelationship(
          RelationshipMetadataBase relationshipMetadata,
          EntityMetadata otherEntityMetadata,
          IServiceProvider services);

        CodeTypeReference GetTypeForRequestField(
          SdkMessageRequestField requestField,
          IServiceProvider services);

        CodeTypeReference GetTypeForResponseField(
          SdkMessageResponseField responseField,
          IServiceProvider services);
    }
}
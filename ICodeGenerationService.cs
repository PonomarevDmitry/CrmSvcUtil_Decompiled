using Microsoft.Xrm.Sdk.Metadata;
using System;

namespace Microsoft.Crm.Services.Utility
{
    public interface ICodeGenerationService
    {
        void Write(
          IOrganizationMetadata organizationMetadata,
          string language,
          string outputFile,
          string targetNamespace,
          IServiceProvider services);

        CodeGenerationType GetTypeForOptionSet(
          EntityMetadata entityMetadata,
          OptionSetMetadataBase optionSetMetadata,
          IServiceProvider services);

        CodeGenerationType GetTypeForOption(
          OptionSetMetadataBase optionSetMetadata,
          OptionMetadata optionMetadata,
          IServiceProvider services);

        CodeGenerationType GetTypeForEntity(
          EntityMetadata entityMetadata,
          IServiceProvider services);

        CodeGenerationType GetTypeForAttribute(
          EntityMetadata entityMetadata,
          AttributeMetadata attributeMetadata,
          IServiceProvider services);

        CodeGenerationType GetTypeForMessagePair(
          SdkMessagePair messagePair,
          IServiceProvider services);

        CodeGenerationType GetTypeForRequestField(
          SdkMessageRequest request,
          SdkMessageRequestField requestField,
          IServiceProvider services);

        CodeGenerationType GetTypeForResponseField(
          SdkMessageResponse response,
          SdkMessageResponseField responseField,
          IServiceProvider services);
    }
}
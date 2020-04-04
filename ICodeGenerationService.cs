// Decompiled with JetBrains decompiler
// Type: Microsoft.Crm.Services.Utility.ICodeGenerationService
// Assembly: CrmSvcUtil, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 756175FA-E97D-49FF-B443-B7A725C9F163
// Assembly location: C:\Users\dmitriy.ponomarev\AppData\Roaming\MscrmTools\XrmToolBox\NugetPlugins\DLaB.Xrm.EarlyBoundGenerator.1.2019.3.19\lib\net462\plugins\DLaB.EarlyBoundGenerator\CrmSvcUtil.exe

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

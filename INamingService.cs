// Decompiled with JetBrains decompiler
// Type: Microsoft.Crm.Services.Utility.INamingService
// Assembly: CrmSvcUtil, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 756175FA-E97D-49FF-B443-B7A725C9F163
// Assembly location: C:\Users\dmitriy.ponomarev\AppData\Roaming\MscrmTools\XrmToolBox\NugetPlugins\DLaB.Xrm.EarlyBoundGenerator.1.2019.3.19\lib\net462\plugins\DLaB.EarlyBoundGenerator\CrmSvcUtil.exe

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

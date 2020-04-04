// Decompiled with JetBrains decompiler
// Type: Microsoft.Crm.Services.Utility.ICodeWriterFilterService
// Assembly: CrmSvcUtil, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 756175FA-E97D-49FF-B443-B7A725C9F163
// Assembly location: C:\Users\dmitriy.ponomarev\AppData\Roaming\MscrmTools\XrmToolBox\NugetPlugins\DLaB.Xrm.EarlyBoundGenerator.1.2019.3.19\lib\net462\plugins\DLaB.EarlyBoundGenerator\CrmSvcUtil.exe

using Microsoft.Xrm.Sdk.Metadata;
using System;

namespace Microsoft.Crm.Services.Utility
{
  public interface ICodeWriterFilterService
  {
    bool GenerateOptionSet(OptionSetMetadataBase optionSetMetadata, IServiceProvider services);

    bool GenerateOption(OptionMetadata optionMetadata, IServiceProvider services);

    bool GenerateEntity(EntityMetadata entityMetadata, IServiceProvider services);

    bool GenerateAttribute(AttributeMetadata attributeMetadata, IServiceProvider services);

    bool GenerateRelationship(
      RelationshipMetadataBase relationshipMetadata,
      EntityMetadata otherEntityMetadata,
      IServiceProvider services);

    bool GenerateServiceContext(IServiceProvider services);
  }
}

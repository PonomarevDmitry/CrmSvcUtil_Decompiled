// Decompiled with JetBrains decompiler
// Type: Microsoft.Crm.Services.Utility.CodeWriterFilterService
// Assembly: CrmSvcUtil, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 756175FA-E97D-49FF-B443-B7A725C9F163
// Assembly location: C:\Users\dmitriy.ponomarev\AppData\Roaming\MscrmTools\XrmToolBox\NugetPlugins\DLaB.Xrm.EarlyBoundGenerator.1.2019.3.19\lib\net462\plugins\DLaB.EarlyBoundGenerator\CrmSvcUtil.exe

using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;

namespace Microsoft.Crm.Services.Utility
{
  internal sealed class CodeWriterFilterService : ICodeWriterFilterService, ICodeWriterMessageFilterService
  {
    private static List<string> _excludedNamespaces = new List<string>();
    private string _messageNamespace;
    private bool _generateMessages;
    private bool _generateCustomActions;
    private bool _generateServiceContext;

    static CodeWriterFilterService()
    {
      CodeWriterFilterService._excludedNamespaces.Add("http://schemas.microsoft.com/xrm/2011/contracts");
    }

    internal CodeWriterFilterService(CrmSvcUtilParameters parameters)
    {
      this._messageNamespace = parameters.MessageNamespace;
      this._generateMessages = parameters.GenerateMessages;
      this._generateCustomActions = parameters.GenerateCustomActions;
      this._generateServiceContext = !string.IsNullOrWhiteSpace(parameters.ServiceContextName);
    }

    bool ICodeWriterFilterService.GenerateOptionSet(
      OptionSetMetadataBase optionSetMetadata,
      IServiceProvider services)
    {
      return optionSetMetadata.OptionSetType.Value == OptionSetType.State;
    }

    bool ICodeWriterFilterService.GenerateOption(
      OptionMetadata option,
      IServiceProvider services)
    {
      return true;
    }

    bool ICodeWriterFilterService.GenerateEntity(
      EntityMetadata entityMetadata,
      IServiceProvider services)
    {
      if (entityMetadata == null)
        return false;
      if (entityMetadata.IsIntersect.GetValueOrDefault() || string.Equals(entityMetadata.LogicalName, "activityparty", StringComparison.Ordinal) || string.Equals(entityMetadata.LogicalName, "calendarrule", StringComparison.Ordinal))
        return true;
      IMetadataProviderService service = (IMetadataProviderService) services.GetService(typeof (IMetadataProviderService));
      foreach (SdkMessage sdkMessage in (!(service is IMetadataProviderService2) ? service.LoadMetadata() : ((IMetadataProviderService2) service).LoadMetadata(services)).Messages.MessageCollection.Values)
      {
        if (!sdkMessage.IsPrivate)
        {
          foreach (SdkMessageFilter sdkMessageFilter in sdkMessage.SdkMessageFilters.Values)
          {
            if (entityMetadata.ObjectTypeCode.HasValue && sdkMessageFilter.PrimaryObjectTypeCode == entityMetadata.ObjectTypeCode.Value || entityMetadata.ObjectTypeCode.HasValue && sdkMessageFilter.SecondaryObjectTypeCode == entityMetadata.ObjectTypeCode.Value)
              return true;
          }
        }
      }
      return false;
    }

    bool ICodeWriterFilterService.GenerateAttribute(
      AttributeMetadata attributeMetadata,
      IServiceProvider services)
    {
      return !this.IsNotExposedChildAttribute(attributeMetadata) && (attributeMetadata.IsValidForCreate.GetValueOrDefault() || attributeMetadata.IsValidForRead.GetValueOrDefault() || attributeMetadata.IsValidForUpdate.GetValueOrDefault());
    }

    private bool IsNotExposedChildAttribute(AttributeMetadata attributeMetadata)
    {
      if (!string.IsNullOrEmpty(attributeMetadata.AttributeOf) && !(attributeMetadata is ImageAttributeMetadata) && !attributeMetadata.LogicalName.EndsWith("_url", StringComparison.OrdinalIgnoreCase))
        return !attributeMetadata.LogicalName.EndsWith("_timestamp", StringComparison.OrdinalIgnoreCase);
      return false;
    }

    bool ICodeWriterFilterService.GenerateRelationship(
      RelationshipMetadataBase relationshipMetadata,
      EntityMetadata otherEntityMetadata,
      IServiceProvider services)
    {
      ICodeWriterFilterService service = (ICodeWriterFilterService) services.GetService(typeof (ICodeWriterFilterService));
      if (otherEntityMetadata == null || string.Equals(otherEntityMetadata.LogicalName, "calendarrule", StringComparison.Ordinal))
        return false;
      return service.GenerateEntity(otherEntityMetadata, services);
    }

    bool ICodeWriterFilterService.GenerateServiceContext(
      IServiceProvider services)
    {
      return this._generateServiceContext;
    }

    bool ICodeWriterMessageFilterService.GenerateSdkMessage(
      SdkMessage message,
      IServiceProvider services)
    {
      return (this._generateMessages || this._generateCustomActions) && (!message.IsPrivate && message.SdkMessageFilters.Count != 0);
    }

    bool ICodeWriterMessageFilterService.GenerateSdkMessagePair(
      SdkMessagePair messagePair,
      IServiceProvider services)
    {
      if (!this._generateMessages && !this._generateCustomActions || CodeWriterFilterService._excludedNamespaces.Contains(messagePair.MessageNamespace.ToUpperInvariant()) || this._generateCustomActions && !messagePair.Message.IsCustomAction)
        return false;
      if (string.IsNullOrEmpty(this._messageNamespace))
        return true;
      return string.Equals(this._messageNamespace, messagePair.MessageNamespace, StringComparison.OrdinalIgnoreCase);
    }
  }
}

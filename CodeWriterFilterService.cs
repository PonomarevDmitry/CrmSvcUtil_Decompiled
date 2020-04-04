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
            IMetadataProviderService service = (IMetadataProviderService)services.GetService(typeof(IMetadataProviderService));
            foreach (SdkMessage sdkMessage in (!(service is IMetadataProviderService2) ? service.LoadMetadata() : ((IMetadataProviderService2)service).LoadMetadata(services)).Messages.MessageCollection.Values)
            {
                if (!sdkMessage.IsPrivate)
                {
                    foreach (SdkMessageFilter sdkMessageFilter in sdkMessage.SdkMessageFilters.Values)
                    {
                        int? objectTypeCode = entityMetadata.ObjectTypeCode;
                        if (objectTypeCode.HasValue)
                        {
                            int primaryObjectTypeCode = sdkMessageFilter.PrimaryObjectTypeCode;
                            objectTypeCode = entityMetadata.ObjectTypeCode;
                            int num = objectTypeCode.Value;
                            if (primaryObjectTypeCode == num)
                                return true;
                        }
                        objectTypeCode = entityMetadata.ObjectTypeCode;
                        if (objectTypeCode.HasValue)
                        {
                            int secondaryObjectTypeCode = sdkMessageFilter.SecondaryObjectTypeCode;
                            objectTypeCode = entityMetadata.ObjectTypeCode;
                            int num = objectTypeCode.Value;
                            if (secondaryObjectTypeCode == num)
                                return true;
                        }
                    }
                }
            }
            return false;
        }

        bool ICodeWriterFilterService.GenerateAttribute(
          AttributeMetadata attributeMetadata,
          IServiceProvider services)
        {
            if (this.IsNotExposedChildAttribute(attributeMetadata))
                return false;
            bool? nullable = attributeMetadata.IsValidForCreate;
            if (!nullable.GetValueOrDefault())
            {
                nullable = attributeMetadata.IsValidForRead;
                if (!nullable.GetValueOrDefault())
                {
                    nullable = attributeMetadata.IsValidForUpdate;
                    if (!nullable.GetValueOrDefault())
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// If true a child attribute cannot be published or externally consumed.
        /// </summary>
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
            ICodeWriterFilterService service = (ICodeWriterFilterService)services.GetService(typeof(ICodeWriterFilterService));
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
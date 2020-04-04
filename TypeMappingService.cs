using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.Crm.Services.Utility
{
    internal sealed class TypeMappingService : ITypeMappingService
    {
        private Dictionary<AttributeTypeCode, Type> _attributeTypeMapping;
        private string _namespace;

        internal TypeMappingService(CrmSvcUtilParameters parameters)
        {
            this._namespace = parameters.Namespace;
            this._attributeTypeMapping = new Dictionary<AttributeTypeCode, Type>();
            this._attributeTypeMapping.Add(AttributeTypeCode.Boolean, typeof(bool));
            this._attributeTypeMapping.Add(AttributeTypeCode.ManagedProperty, typeof(BooleanManagedProperty));
            this._attributeTypeMapping.Add(AttributeTypeCode.CalendarRules, typeof(object));
            this._attributeTypeMapping.Add(AttributeTypeCode.Customer, typeof(EntityReference));
            this._attributeTypeMapping.Add(AttributeTypeCode.DateTime, typeof(DateTime));
            this._attributeTypeMapping.Add(AttributeTypeCode.Decimal, typeof(Decimal));
            this._attributeTypeMapping.Add(AttributeTypeCode.Double, typeof(double));
            this._attributeTypeMapping.Add(AttributeTypeCode.Integer, typeof(int));
            this._attributeTypeMapping.Add(AttributeTypeCode.EntityName, typeof(string));
            this._attributeTypeMapping.Add(AttributeTypeCode.BigInt, typeof(long));
            this._attributeTypeMapping.Add(AttributeTypeCode.Lookup, typeof(EntityReference));
            this._attributeTypeMapping.Add(AttributeTypeCode.Memo, typeof(string));
            this._attributeTypeMapping.Add(AttributeTypeCode.Money, typeof(Money));
            this._attributeTypeMapping.Add(AttributeTypeCode.Owner, typeof(EntityReference));
            this._attributeTypeMapping.Add(AttributeTypeCode.Picklist, typeof(OptionSetValue));
            this._attributeTypeMapping.Add(AttributeTypeCode.Status, typeof(OptionSetValue));
            this._attributeTypeMapping.Add(AttributeTypeCode.String, typeof(string));
            this._attributeTypeMapping.Add(AttributeTypeCode.Uniqueidentifier, typeof(Guid));
        }

        private string Namespace
        {
            get
            {
                return this._namespace;
            }
        }

        CodeTypeReference ITypeMappingService.GetTypeForEntity(
          EntityMetadata entityMetadata,
          IServiceProvider services)
        {
            return this.TypeRef(((INamingService)services.GetService(typeof(INamingService))).GetNameForEntity(entityMetadata, services));
        }

        CodeTypeReference ITypeMappingService.GetTypeForAttributeType(
          EntityMetadata entityMetadata,
          AttributeMetadata attributeMetadata,
          IServiceProvider services)
        {
            Type type = typeof(object);
            if (attributeMetadata.AttributeType.HasValue)
            {
                AttributeTypeCode key = attributeMetadata.AttributeType.Value;
                if (this._attributeTypeMapping.ContainsKey(key))
                {
                    type = this._attributeTypeMapping[key];
                }
                else
                {
                    if (key == AttributeTypeCode.PartyList)
                        return this.BuildCodeTypeReferenceForPartyList(services);
                    if (attributeMetadata is ImageAttributeMetadata)
                    {
                        type = typeof(byte[]);
                    }
                    else
                    {
                        OptionSetMetadataBase attributeOptionSet = TypeMappingService.GetAttributeOptionSet(attributeMetadata);
                        if (attributeOptionSet != null)
                            return this.BuildCodeTypeReferenceForOptionSet(attributeMetadata.LogicalName, entityMetadata, attributeOptionSet, services);
                    }
                }
                if (type.IsValueType)
                    type = typeof(Nullable<>).MakeGenericType(type);
            }
            return TypeMappingService.TypeRef(type);
        }

        CodeTypeReference ITypeMappingService.GetTypeForRelationship(
          RelationshipMetadataBase relationshipMetadata,
          EntityMetadata otherEntityMetadata,
          IServiceProvider services)
        {
            return this.TypeRef(((INamingService)services.GetService(typeof(INamingService))).GetNameForEntity(otherEntityMetadata, services));
        }

        CodeTypeReference ITypeMappingService.GetTypeForRequestField(
          SdkMessageRequestField requestField,
          IServiceProvider services)
        {
            return this.GetTypeForField(requestField.CLRFormatter, requestField.IsGeneric);
        }

        CodeTypeReference ITypeMappingService.GetTypeForResponseField(
          SdkMessageResponseField responseField,
          IServiceProvider services)
        {
            return this.GetTypeForField(responseField.CLRFormatter, false);
        }

        private CodeTypeReference BuildCodeTypeReferenceForOptionSet(
          string attributeName,
          EntityMetadata entityMetadata,
          OptionSetMetadataBase attributeOptionSet,
          IServiceProvider services)
        {
            ICodeWriterFilterService service1 = (ICodeWriterFilterService)services.GetService(typeof(ICodeWriterFilterService));
            INamingService service2 = (INamingService)services.GetService(typeof(INamingService));
            ICodeGenerationService service3 = (ICodeGenerationService)services.GetService(typeof(ICodeGenerationService));
            OptionSetMetadataBase optionSetMetadata = attributeOptionSet;
            IServiceProvider services1 = services;
            if (service1.GenerateOptionSet(optionSetMetadata, services1))
            {
                string nameForOptionSet = service2.GetNameForOptionSet(entityMetadata, attributeOptionSet, services);
                CodeGenerationType typeForOptionSet = service3.GetTypeForOptionSet(entityMetadata, attributeOptionSet, services);
                switch (typeForOptionSet)
                {
                    case CodeGenerationType.Class:
                        return this.TypeRef(nameForOptionSet);
                    case CodeGenerationType.Enum:
                    case CodeGenerationType.Struct:
                        return TypeMappingService.TypeRef(typeof(Nullable<>), this.TypeRef(nameForOptionSet));
                    default:
                        Trace.TraceWarning("Cannot map type for atttribute {0} with OptionSet type {1} which has CodeGenerationType {2}", (object)attributeName, (object)attributeOptionSet.Name, (object)typeForOptionSet);
                        break;
                }
            }
            return TypeMappingService.TypeRef(typeof(object));
        }

        private CodeTypeReference BuildCodeTypeReferenceForPartyList(
          IServiceProvider services)
        {
            IMetadataProviderService service1 = (IMetadataProviderService)services.GetService(typeof(IMetadataProviderService));
            ICodeWriterFilterService filterService = (ICodeWriterFilterService)services.GetService(typeof(ICodeWriterFilterService));
            INamingService service2 = (INamingService)services.GetService(typeof(INamingService));
            EntityMetadata entityMetadata = ((IEnumerable<EntityMetadata>)service1.LoadMetadata().Entities).FirstOrDefault<EntityMetadata>((Func<EntityMetadata, bool>)(entity =>
          {
              if (string.Equals(entity.LogicalName, "activityparty", StringComparison.Ordinal))
                  return filterService.GenerateEntity(entity, services);
              return false;
          }));
            if (entityMetadata == null)
                return TypeMappingService.TypeRef(typeof(IEnumerable<>), TypeMappingService.TypeRef(typeof(Entity)));
            return TypeMappingService.TypeRef(typeof(IEnumerable<>), this.TypeRef(service2.GetNameForEntity(entityMetadata, services)));
        }

        internal static OptionSetMetadataBase GetAttributeOptionSet(
          AttributeMetadata attribute)
        {
            OptionSetMetadataBase optionSetMetadataBase = (OptionSetMetadataBase)null;
            Type type = attribute.GetType();
            if (type == typeof(BooleanAttributeMetadata))
                optionSetMetadataBase = (OptionSetMetadataBase)((BooleanAttributeMetadata)attribute).OptionSet;
            else if (type == typeof(StateAttributeMetadata))
                optionSetMetadataBase = (OptionSetMetadataBase)((EnumAttributeMetadata)attribute).OptionSet;
            else if (type == typeof(PicklistAttributeMetadata))
                optionSetMetadataBase = (OptionSetMetadataBase)((EnumAttributeMetadata)attribute).OptionSet;
            else if (type == typeof(StatusAttributeMetadata))
                optionSetMetadataBase = (OptionSetMetadataBase)((EnumAttributeMetadata)attribute).OptionSet;
            return optionSetMetadataBase;
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private CodeTypeReference GetTypeForField(string clrFormatter, bool isGeneric)
        {
            CodeTypeReference codeTypeReference = TypeMappingService.TypeRef(typeof(object));
            if (isGeneric)
                codeTypeReference = new CodeTypeReference(new CodeTypeParameter("T"));
            else if (!string.IsNullOrEmpty(clrFormatter))
            {
                Type type = Type.GetType(clrFormatter, false);
                if (type != (Type)null)
                {
                    codeTypeReference = TypeMappingService.TypeRef(type);
                }
                else
                {
                    string[] strArray = clrFormatter.Split(new char[1]
                    {
            ','
                    }, StringSplitOptions.RemoveEmptyEntries);
                    if (strArray != null && strArray.Length != 0)
                        codeTypeReference = new CodeTypeReference(strArray[0]);
                }
            }
            return codeTypeReference;
        }

        private CodeTypeReference TypeRef(string typeName)
        {
            if (string.IsNullOrWhiteSpace(this.Namespace))
                return new CodeTypeReference(typeName);
            return new CodeTypeReference(string.Format((IFormatProvider)CultureInfo.InvariantCulture, "{0}.{1}", (object)this.Namespace, (object)typeName));
        }

        private static CodeTypeReference TypeRef(Type type)
        {
            return new CodeTypeReference(type);
        }

        private static CodeTypeReference TypeRef(
          Type type,
          CodeTypeReference typeParameter)
        {
            return new CodeTypeReference(type.FullName, new CodeTypeReference[1]
            {
        typeParameter
            });
        }
    }
}
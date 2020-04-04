// Decompiled with JetBrains decompiler
// Type: Microsoft.Crm.Services.Utility.CodeGenerationService
// Assembly: CrmSvcUtil, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 756175FA-E97D-49FF-B443-B7A725C9F163
// Assembly location: C:\Users\dmitriy.ponomarev\AppData\Roaming\MscrmTools\XrmToolBox\NugetPlugins\DLaB.Xrm.EarlyBoundGenerator.1.2019.3.19\lib\net462\plugins\DLaB.EarlyBoundGenerator\CrmSvcUtil.exe

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Microsoft.Crm.Services.Utility
{
    internal sealed class CodeGenerationService : ICodeGenerationService
    {
        private static Type AttributeLogicalNameAttribute = typeof(Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute);
        private static Type EntityLogicalNameAttribute = typeof(Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute);
        private static Type RelationshipSchemaNameAttribute = typeof(Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute);
        private static Type ObsoleteFieldAttribute = typeof(ObsoleteAttribute);
        private static Type ServiceContextBaseType = typeof(OrganizationServiceContext);
        private static Type EntityClassBaseType = typeof(Entity);
        private static Type RequestClassBaseType = typeof(OrganizationRequest);
        private static Type ResponseClassBaseType = typeof(OrganizationResponse);
        private static string RequestClassSuffix = "Request";
        private static string ResponseClassSuffix = "Response";
        private static string RequestNamePropertyName = "RequestName";
        private static string ParametersPropertyName = "Parameters";
        private static string ResultsPropertyName = "Results";

        void ICodeGenerationService.Write(
          IOrganizationMetadata organizationMetadata,
          string language,
          string outputFile,
          string outputNamespace,
          IServiceProvider services)
        {
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStart("Entering {0}", (object)MethodBase.GetCurrentMethod().Name);
            ServiceProvider serviceProvider = services as ServiceProvider;
            CodeNamespace codenamespace = CodeGenerationService.BuildCodeDom(organizationMetadata, outputNamespace, serviceProvider);
            CodeGenerationService.WriteFile(outputFile, language, codenamespace, serviceProvider);
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStop("Exiting {0}", (object)MethodBase.GetCurrentMethod().Name);
        }

        CodeGenerationType ICodeGenerationService.GetTypeForOptionSet(
          EntityMetadata entityMetadata,
          OptionSetMetadataBase optionSetMetadata,
          IServiceProvider services)
        {
            return CodeGenerationType.Enum;
        }

        CodeGenerationType ICodeGenerationService.GetTypeForOption(
          OptionSetMetadataBase optionSetMetadata,
          OptionMetadata optionMetadata,
          IServiceProvider services)
        {
            return CodeGenerationType.Field;
        }

        CodeGenerationType ICodeGenerationService.GetTypeForEntity(
          EntityMetadata entityMetadata,
          IServiceProvider services)
        {
            return CodeGenerationType.Class;
        }

        CodeGenerationType ICodeGenerationService.GetTypeForAttribute(
          EntityMetadata entityMetadata,
          AttributeMetadata attributeMetadata,
          IServiceProvider services)
        {
            return CodeGenerationType.Property;
        }

        CodeGenerationType ICodeGenerationService.GetTypeForMessagePair(
          SdkMessagePair messagePair,
          IServiceProvider services)
        {
            return CodeGenerationType.Class;
        }

        CodeGenerationType ICodeGenerationService.GetTypeForRequestField(
          SdkMessageRequest request,
          SdkMessageRequestField requestField,
          IServiceProvider services)
        {
            return CodeGenerationType.Property;
        }

        CodeGenerationType ICodeGenerationService.GetTypeForResponseField(
          SdkMessageResponse response,
          SdkMessageResponseField responseField,
          IServiceProvider services)
        {
            return CodeGenerationType.Property;
        }

        private static CodeNamespace BuildCodeDom(
          IOrganizationMetadata organizationMetadata,
          string outputNamespace,
          ServiceProvider serviceProvider)
        {
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStart("Entering {0}", (object)MethodBase.GetCurrentMethod().Name);
            CodeNamespace codeNamespace = CodeGenerationService.Namespace(outputNamespace);
            codeNamespace.Types.AddRange(CodeGenerationService.BuildOptionSets(organizationMetadata.OptionSets, serviceProvider));
            codeNamespace.Types.AddRange(CodeGenerationService.BuildEntities(organizationMetadata.Entities, serviceProvider));
            codeNamespace.Types.AddRange(CodeGenerationService.BuildServiceContext(organizationMetadata.Entities, serviceProvider));
            codeNamespace.Types.AddRange(CodeGenerationService.BuildMessages(organizationMetadata.Messages, serviceProvider));
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStop("Exiting {0}", (object)MethodBase.GetCurrentMethod().Name);
            return codeNamespace;
        }

        private static void WriteFile(
          string outputFile,
          string language,
          CodeNamespace codenamespace,
          ServiceProvider serviceProvider)
        {
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStart("Entering {0}", (object)MethodBase.GetCurrentMethod().Name);
            CodeCompileUnit codeCompileUnit = new CodeCompileUnit();
            codeCompileUnit.Namespaces.Add(codenamespace);
            codeCompileUnit.AssemblyCustomAttributes.Add(CodeGenerationService.Attribute(typeof(ProxyTypesAssemblyAttribute)));
            serviceProvider.CodeCustomizationService.CustomizeCodeDom(codeCompileUnit, (IServiceProvider)serviceProvider);
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BlankLinesBetweenMembers = true;
            options.BracingStyle = "C";
            options.IndentString = "\t";
            options.VerbatimOrder = true;
            using (StreamWriter streamWriter = new StreamWriter(outputFile))
            {
                using (CodeDomProvider provider = CodeDomProvider.CreateProvider(language))
                    provider.GenerateCodeFromCompileUnit(codeCompileUnit, (TextWriter)streamWriter, options);
            }
            CrmSvcUtil.crmSvcUtilLogger.TraceInformation("Exit {0}: Code file written to {1}", (object)MethodBase.GetCurrentMethod().Name, (object)outputFile);
            Console.Out.WriteLine(string.Format((IFormatProvider)CultureInfo.InvariantCulture, "Code written to {0}.", (object)Path.GetFullPath(outputFile)));
        }

        private static CodeTypeDeclarationCollection BuildOptionSets(
          OptionSetMetadataBase[] optionSetMetadata,
          ServiceProvider serviceProvider)
        {
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStart("Entering {0}", (object)MethodBase.GetCurrentMethod().Name);
            CodeTypeDeclarationCollection declarationCollection = new CodeTypeDeclarationCollection();
            foreach (OptionSetMetadataBase optionSetMetadataBase in optionSetMetadata)
            {
                if (serviceProvider.CodeFilterService.GenerateOptionSet(optionSetMetadataBase, (IServiceProvider)serviceProvider) && optionSetMetadataBase.IsGlobal.HasValue && optionSetMetadataBase.IsGlobal.Value)
                {
                    CodeTypeDeclaration codeTypeDeclaration = CodeGenerationService.BuildOptionSet((EntityMetadata)null, optionSetMetadataBase, serviceProvider);
                    if (codeTypeDeclaration != null)
                        declarationCollection.Add(codeTypeDeclaration);
                    else
                        CrmSvcUtil.crmSvcUtilLogger.TraceInformation("Skipping OptionSet {0} of type {1} from being generated.", (object)optionSetMetadataBase.Name, (object)optionSetMetadataBase.GetType());
                }
                else
                    CrmSvcUtil.crmSvcUtilLogger.TraceInformation("Skipping OptionSet {0} from being generated.", (object)optionSetMetadataBase.Name);
            }
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStop("Exiting {0}", (object)MethodBase.GetCurrentMethod().Name);
            return declarationCollection;
        }

        private static CodeTypeDeclaration BuildOptionSet(
          EntityMetadata entity,
          OptionSetMetadataBase optionSet,
          ServiceProvider serviceProvider)
        {
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStart("Entering {0}", (object)MethodBase.GetCurrentMethod().Name);
            CodeTypeDeclaration codeTypeDeclaration = CodeGenerationService.Enum(serviceProvider.NamingService.GetNameForOptionSet(entity, optionSet, (IServiceProvider)serviceProvider), CodeGenerationService.Attribute(typeof(DataContractAttribute)));
            OptionSetMetadata optionSetMetadata = optionSet as OptionSetMetadata;
            if (optionSetMetadata == null)
                return (CodeTypeDeclaration)null;
            foreach (OptionMetadata option in (Collection<OptionMetadata>)optionSetMetadata.Options)
            {
                if (serviceProvider.CodeFilterService.GenerateOption(option, (IServiceProvider)serviceProvider))
                    codeTypeDeclaration.Members.Add(CodeGenerationService.BuildOption(optionSet, option, serviceProvider));
                else
                    CrmSvcUtil.crmSvcUtilLogger.TraceInformation("Skipping {0}.Option {1} from being generated.", (object)optionSet.Name, (object)option.Value.Value);
            }
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStop("Exiting {0}: OptionSet Enumeration {1} defined", (object)MethodBase.GetCurrentMethod().Name, (object)codeTypeDeclaration.Name);
            return codeTypeDeclaration;
        }

        private static CodeTypeMember BuildOption(
          OptionSetMetadataBase optionSet,
          OptionMetadata option,
          ServiceProvider serviceProvider)
        {
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStart("Entering {0}", (object)MethodBase.GetCurrentMethod().Name);
            CodeMemberField codeMemberField = CodeGenerationService.Field(serviceProvider.NamingService.GetNameForOption(optionSet, option, (IServiceProvider)serviceProvider), typeof(int), (object)option.Value.Value, CodeGenerationService.Attribute(typeof(EnumMemberAttribute)));
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStop("Exiting {0}: {1}.Option {2} defined", (object)MethodBase.GetCurrentMethod().Name, (object)optionSet.Name, (object)codeMemberField.Name);
            return (CodeTypeMember)codeMemberField;
        }

        private static CodeTypeDeclarationCollection BuildEntities(
          EntityMetadata[] entityMetadata,
          ServiceProvider serviceProvider)
        {
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStart("Entering {0}", (object)MethodBase.GetCurrentMethod().Name);
            CodeTypeDeclarationCollection declarationCollection = new CodeTypeDeclarationCollection();
            foreach (EntityMetadata entityMetadata1 in (IEnumerable<EntityMetadata>)((IEnumerable<EntityMetadata>)entityMetadata).OrderBy<EntityMetadata, string>((Func<EntityMetadata, string>)(metadata => metadata.LogicalName)))
            {
                if (serviceProvider.CodeFilterService.GenerateEntity(entityMetadata1, (IServiceProvider)serviceProvider))
                    declarationCollection.AddRange(CodeGenerationService.BuildEntity(entityMetadata1, serviceProvider));
                else
                    CrmSvcUtil.crmSvcUtilLogger.TraceInformation("Skipping Entity {0} from being generated.", (object)entityMetadata1.LogicalName);
            }
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStop("Exiting {0}", (object)MethodBase.GetCurrentMethod().Name);
            return declarationCollection;
        }

        private static CodeTypeDeclarationCollection BuildEntity(
          EntityMetadata entity,
          ServiceProvider serviceProvider)
        {
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStart("Entering {0}", (object)MethodBase.GetCurrentMethod().Name);
            CodeTypeDeclarationCollection declarationCollection = new CodeTypeDeclarationCollection();
            CodeTypeDeclaration entityClass = CodeGenerationService.Class(serviceProvider.NamingService.GetNameForEntity(entity, (IServiceProvider)serviceProvider), CodeGenerationService.TypeRef(CodeGenerationService.EntityClassBaseType), CodeGenerationService.Attribute(typeof(DataContractAttribute)), CodeGenerationService.Attribute(CodeGenerationService.EntityLogicalNameAttribute, CodeGenerationService.AttributeArg((object)entity.LogicalName)));
            CodeGenerationService.InitializeEntityClass(entityClass, entity);
            CodeTypeMember attributeMember = (CodeTypeMember)null;
            foreach (AttributeMetadata attributeMetadata in (IEnumerable<AttributeMetadata>)((IEnumerable<AttributeMetadata>)entity.Attributes).OrderBy<AttributeMetadata, string>((Func<AttributeMetadata, string>)(metadata => metadata.LogicalName)))
            {
                if (serviceProvider.CodeFilterService.GenerateAttribute(attributeMetadata, (IServiceProvider)serviceProvider))
                {
                    attributeMember = CodeGenerationService.BuildAttribute(entity, attributeMetadata, serviceProvider);
                    entityClass.Members.Add(attributeMember);
                    if (entity.PrimaryIdAttribute == attributeMetadata.LogicalName && attributeMetadata.IsPrimaryId.GetValueOrDefault())
                        entityClass.Members.Add(CodeGenerationService.BuildIdProperty(entity, attributeMetadata, serviceProvider));
                }
                else
                    CrmSvcUtil.crmSvcUtilLogger.TraceInformation("Skipping {0}.Attribute {1} from being generated.", (object)entity.LogicalName, (object)attributeMetadata.LogicalName);

                CodeTypeDeclaration codeTypeDeclaration = CodeGenerationService.BuildAttributeOptionSet(entity, attributeMetadata, attributeMember, serviceProvider);
                if (codeTypeDeclaration != null)
                    declarationCollection.Add(codeTypeDeclaration);
            }
            entityClass.Members.AddRange(CodeGenerationService.BuildOneToManyRelationships(entity, serviceProvider));
            entityClass.Members.AddRange(CodeGenerationService.BuildManyToManyRelationships(entity, serviceProvider));
            entityClass.Members.AddRange(CodeGenerationService.BuildManyToOneRelationships(entity, serviceProvider));
            declarationCollection.Add(entityClass);
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStop("Exiting {0}: Entity Class {1} defined", (object)MethodBase.GetCurrentMethod().Name, (object)entityClass.Name);
            return declarationCollection;
        }

        private static void InitializeEntityClass(
          CodeTypeDeclaration entityClass,
          EntityMetadata entity)
        {
            entityClass.BaseTypes.Add(CodeGenerationService.TypeRef(typeof(INotifyPropertyChanging)));
            entityClass.BaseTypes.Add(CodeGenerationService.TypeRef(typeof(INotifyPropertyChanged)));
            entityClass.Members.Add(CodeGenerationService.EntityConstructor());
            entityClass.Members.Add(CodeGenerationService.EntityLogicalNameConstant(entity));
            entityClass.Members.Add(CodeGenerationService.EntityTypeCodeConstant(entity));
            entityClass.Members.Add((CodeTypeMember)CodeGenerationService.Event("PropertyChanged", typeof(PropertyChangedEventHandler), typeof(INotifyPropertyChanged)));
            entityClass.Members.Add((CodeTypeMember)CodeGenerationService.Event("PropertyChanging", typeof(PropertyChangingEventHandler), typeof(INotifyPropertyChanging)));
            entityClass.Members.Add((CodeTypeMember)CodeGenerationService.RaiseEvent("OnPropertyChanged", "PropertyChanged", typeof(PropertyChangedEventArgs)));
            entityClass.Members.Add((CodeTypeMember)CodeGenerationService.RaiseEvent("OnPropertyChanging", "PropertyChanging", typeof(PropertyChangingEventArgs)));
            entityClass.Comments.AddRange(CodeGenerationService.CommentSummary(entity.Description));
        }

        private static CodeTypeMember EntityLogicalNameConstant(EntityMetadata entity)
        {
            CodeMemberField codeMemberField = CodeGenerationService.Field("EntityLogicalName", typeof(string), (object)entity.LogicalName);
            codeMemberField.Attributes = MemberAttributes.Const | MemberAttributes.Public;
            return (CodeTypeMember)codeMemberField;
        }

        private static CodeTypeMember EntityTypeCodeConstant(EntityMetadata entity)
        {
            CodeMemberField codeMemberField = CodeGenerationService.Field("EntityTypeCode", typeof(int), (object)entity.ObjectTypeCode.GetValueOrDefault());
            codeMemberField.Attributes = MemberAttributes.Const | MemberAttributes.Public;
            return (CodeTypeMember)codeMemberField;
        }

        private static CodeTypeMember EntityConstructor()
        {
            CodeConstructor codeConstructor = CodeGenerationService.Constructor();
            codeConstructor.BaseConstructorArgs.Add((CodeExpression)CodeGenerationService.VarRef("EntityLogicalName"));
            codeConstructor.Comments.AddRange(CodeGenerationService.CommentSummary("Default Constructor."));
            return (CodeTypeMember)codeConstructor;
        }

        private static CodeTypeMember BuildAttribute(
          EntityMetadata entity,
          AttributeMetadata attribute,
          ServiceProvider serviceProvider)
        {
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStart("Entering {0}", (object)MethodBase.GetCurrentMethod().Name);
            CodeTypeReference forAttributeType = serviceProvider.TypeMappingService.GetTypeForAttributeType(entity, attribute, (IServiceProvider)serviceProvider);
            CodeMemberProperty codeMemberProperty = CodeGenerationService.PropertyGet(forAttributeType, serviceProvider.NamingService.GetNameForAttribute(entity, attribute, (IServiceProvider)serviceProvider));
            codeMemberProperty.HasSet = attribute.IsValidForCreate.GetValueOrDefault() || attribute.IsValidForUpdate.GetValueOrDefault();
            codeMemberProperty.HasGet = attribute.IsValidForRead.GetValueOrDefault() || codeMemberProperty.HasSet;
            if (codeMemberProperty.HasGet)
                codeMemberProperty.GetStatements.AddRange(CodeGenerationService.BuildAttributeGet(attribute, forAttributeType));
            if (codeMemberProperty.HasSet)
                codeMemberProperty.SetStatements.AddRange(CodeGenerationService.BuildAttributeSet(entity, attribute, codeMemberProperty.Name));
            codeMemberProperty.CustomAttributes.Add(CodeGenerationService.Attribute(CodeGenerationService.AttributeLogicalNameAttribute, CodeGenerationService.AttributeArg((object)attribute.LogicalName)));
            if (attribute.DeprecatedVersion != null)
                codeMemberProperty.CustomAttributes.Add(CodeGenerationService.Attribute(CodeGenerationService.ObsoleteFieldAttribute));
            codeMemberProperty.Comments.AddRange(CodeGenerationService.CommentSummary(attribute.Description));
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStop("Exiting {0}: {1}.Attribute {2} defined", (object)MethodBase.GetCurrentMethod().Name, (object)entity.LogicalName, (object)codeMemberProperty.Name);
            return (CodeTypeMember)codeMemberProperty;
        }

        private static CodeStatementCollection BuildAttributeGet(
          AttributeMetadata attribute,
          CodeTypeReference targetType)
        {
            CodeStatementCollection statementCollection = new CodeStatementCollection();
            if (attribute.AttributeType.GetValueOrDefault() == AttributeTypeCode.PartyList && targetType.TypeArguments.Count > 0)
                statementCollection.AddRange(CodeGenerationService.BuildEntityCollectionAttributeGet(attribute.LogicalName, targetType));
            else
                statementCollection.Add((CodeStatement)CodeGenerationService.Return((CodeExpression)CodeGenerationService.ThisMethodInvoke("GetAttributeValue", targetType, (CodeExpression)CodeGenerationService.StringLiteral(attribute.LogicalName))));
            return statementCollection;
        }

        private static CodeStatementCollection BuildAttributeSet(
          EntityMetadata entity,
          AttributeMetadata attribute,
          string propertyName)
        {
            CodeStatementCollection statementCollection = new CodeStatementCollection();
            statementCollection.Add((CodeExpression)CodeGenerationService.ThisMethodInvoke("OnPropertyChanging", (CodeExpression)CodeGenerationService.StringLiteral(propertyName)));
            if (attribute.AttributeType.GetValueOrDefault() == AttributeTypeCode.PartyList)
                statementCollection.Add(CodeGenerationService.BuildEntityCollectionAttributeSet(attribute.LogicalName));
            else
                statementCollection.Add((CodeExpression)CodeGenerationService.ThisMethodInvoke("SetAttributeValue", (CodeExpression)CodeGenerationService.StringLiteral(attribute.LogicalName), (CodeExpression)CodeGenerationService.VarRef("value")));
            if (entity.PrimaryIdAttribute == attribute.LogicalName && attribute.IsPrimaryId.GetValueOrDefault())
                statementCollection.Add((CodeStatement)CodeGenerationService.If((CodeExpression)CodeGenerationService.PropRef((CodeExpression)CodeGenerationService.VarRef("value"), "HasValue"), (CodeStatement)CodeGenerationService.AssignValue((CodeExpression)CodeGenerationService.BaseProp("Id"), (CodeExpression)CodeGenerationService.PropRef((CodeExpression)CodeGenerationService.VarRef("value"), "Value")), (CodeStatement)CodeGenerationService.AssignValue((CodeExpression)CodeGenerationService.BaseProp("Id"), CodeGenerationService.GuidEmpty())));
            statementCollection.Add((CodeExpression)CodeGenerationService.ThisMethodInvoke("OnPropertyChanged", (CodeExpression)CodeGenerationService.StringLiteral(propertyName)));
            return statementCollection;
        }

        private static CodeStatementCollection BuildEntityCollectionAttributeGet(
          string attributeLogicalName,
          CodeTypeReference propertyType)
        {
            return new CodeStatementCollection()
      {
        (CodeStatement) CodeGenerationService.Var(typeof (EntityCollection), "collection", (CodeExpression) CodeGenerationService.ThisMethodInvoke("GetAttributeValue", CodeGenerationService.TypeRef(typeof (EntityCollection)), (CodeExpression) CodeGenerationService.StringLiteral(attributeLogicalName))),
        (CodeStatement) CodeGenerationService.If((CodeExpression) CodeGenerationService.And((CodeExpression) CodeGenerationService.NotNull((CodeExpression) CodeGenerationService.VarRef("collection")), (CodeExpression) CodeGenerationService.NotNull((CodeExpression) CodeGenerationService.PropRef((CodeExpression) CodeGenerationService.VarRef("collection"), "Entities"))), (CodeStatement) CodeGenerationService.Return((CodeExpression) CodeGenerationService.StaticMethodInvoke(typeof (Enumerable), "Cast", propertyType.TypeArguments[0], (CodeExpression) CodeGenerationService.PropRef((CodeExpression) CodeGenerationService.VarRef("collection"), "Entities"))), (CodeStatement) CodeGenerationService.Return((CodeExpression) CodeGenerationService.Null()))
      };
        }

        private static CodeStatement BuildEntityCollectionAttributeSet(
          string attributeLogicalName)
        {
            return (CodeStatement)CodeGenerationService.If(CodeGenerationService.ValueNull(), (CodeExpression)CodeGenerationService.ThisMethodInvoke("SetAttributeValue", (CodeExpression)CodeGenerationService.StringLiteral(attributeLogicalName), (CodeExpression)CodeGenerationService.VarRef("value")), (CodeExpression)CodeGenerationService.ThisMethodInvoke("SetAttributeValue", (CodeExpression)CodeGenerationService.StringLiteral(attributeLogicalName), (CodeExpression)CodeGenerationService.New(CodeGenerationService.TypeRef(typeof(EntityCollection)), (CodeExpression)CodeGenerationService.New(CodeGenerationService.TypeRef(typeof(List<Entity>)), (CodeExpression)CodeGenerationService.VarRef("value")))));
        }

        private static CodeTypeMember BuildIdProperty(
          EntityMetadata entity,
          AttributeMetadata attribute,
          ServiceProvider serviceProvider)
        {
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStart("Entering {0}", (object)MethodBase.GetCurrentMethod().Name);
            CodeMemberProperty codeMemberProperty = CodeGenerationService.PropertyGet(CodeGenerationService.TypeRef(typeof(Guid)), "Id");
            codeMemberProperty.CustomAttributes.Add(CodeGenerationService.Attribute(CodeGenerationService.AttributeLogicalNameAttribute, CodeGenerationService.AttributeArg((object)attribute.LogicalName)));
            codeMemberProperty.Attributes = MemberAttributes.Public | MemberAttributes.Override;
            codeMemberProperty.HasSet = attribute.IsValidForCreate.GetValueOrDefault() || attribute.IsValidForUpdate.GetValueOrDefault();
            codeMemberProperty.HasGet = attribute.IsValidForRead.GetValueOrDefault() || codeMemberProperty.HasSet;
            codeMemberProperty.GetStatements.Add((CodeStatement)CodeGenerationService.Return((CodeExpression)CodeGenerationService.BaseProp("Id")));
            if (codeMemberProperty.HasSet)
                codeMemberProperty.SetStatements.Add((CodeStatement)CodeGenerationService.AssignValue((CodeExpression)CodeGenerationService.ThisProp(serviceProvider.NamingService.GetNameForAttribute(entity, attribute, (IServiceProvider)serviceProvider)), (CodeExpression)CodeGenerationService.VarRef("value")));
            else
                codeMemberProperty.SetStatements.Add((CodeStatement)CodeGenerationService.AssignValue((CodeExpression)CodeGenerationService.BaseProp("Id"), (CodeExpression)CodeGenerationService.VarRef("value")));
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStop("Exiting {0}: {1}.Attribute Id defined", (object)MethodBase.GetCurrentMethod().Name, (object)entity.LogicalName);
            return (CodeTypeMember)codeMemberProperty;
        }

        private static CodeTypeDeclaration BuildAttributeOptionSet(
          EntityMetadata entity,
          AttributeMetadata attribute,
          CodeTypeMember attributeMember,
          ServiceProvider serviceProvider)
        {
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStart("Entering {0}", (object)MethodBase.GetCurrentMethod().Name);
            OptionSetMetadataBase attributeOptionSet = TypeMappingService.GetAttributeOptionSet(attribute);
            if (attributeOptionSet == null || !serviceProvider.CodeFilterService.GenerateOptionSet(attributeOptionSet, (IServiceProvider)serviceProvider))
            {
                if (attributeOptionSet != null)
                    CrmSvcUtil.crmSvcUtilLogger.TraceMethodStop("Exiting {0}: No type created for {1}", (object)MethodBase.GetCurrentMethod().Name, (object)attributeOptionSet.Name);
                return (CodeTypeDeclaration)null;
            }
            CodeTypeDeclaration codeTypeDeclaration = CodeGenerationService.BuildOptionSet(entity, attributeOptionSet, serviceProvider);
            if (codeTypeDeclaration == null)
            {
                CrmSvcUtil.crmSvcUtilLogger.TraceMethodStop("Exiting {0}: No type created for {1} of type {2}", (object)MethodBase.GetCurrentMethod().Name, (object)attributeOptionSet.Name, (object)attributeOptionSet.GetType());
                return (CodeTypeDeclaration)null;
            }
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStop("Exiting {0}: Type {1} created for {2}", (object)MethodBase.GetCurrentMethod().Name, (object)codeTypeDeclaration.Name, (object)attributeOptionSet.Name);
            CodeGenerationService.UpdateAttributeMemberStatements(attribute, attributeMember);
            return codeTypeDeclaration;
        }

        private static void UpdateAttributeMemberStatements(
          AttributeMetadata attribute,
          CodeTypeMember attributeMember)
        {
            CodeMemberProperty codeMemberProperty = attributeMember as CodeMemberProperty;
            if (codeMemberProperty.HasGet)
            {
                codeMemberProperty.GetStatements.Clear();
                codeMemberProperty.GetStatements.AddRange(CodeGenerationService.BuildOptionSetAttributeGet(attribute, codeMemberProperty.Type));
            }
            if (!codeMemberProperty.HasSet)
                return;
            codeMemberProperty.SetStatements.Clear();
            codeMemberProperty.SetStatements.AddRange(CodeGenerationService.BuildOptionSetAttributeSet(attribute, codeMemberProperty.Name));
        }

        private static CodeStatementCollection BuildOptionSetAttributeGet(
          AttributeMetadata attribute,
          CodeTypeReference attributeType)
        {
            CodeTypeReference codeTypeReference = attributeType;
            if (codeTypeReference.TypeArguments.Count > 0)
                codeTypeReference = codeTypeReference.TypeArguments[0];
            return new CodeStatementCollection(new CodeStatement[2]
            {
        (CodeStatement) CodeGenerationService.Var(typeof (OptionSetValue), "optionSet", (CodeExpression) CodeGenerationService.ThisMethodInvoke("GetAttributeValue", CodeGenerationService.TypeRef(typeof (OptionSetValue)), (CodeExpression) CodeGenerationService.StringLiteral(attribute.LogicalName))),
        (CodeStatement) CodeGenerationService.If((CodeExpression) CodeGenerationService.NotNull((CodeExpression) CodeGenerationService.VarRef("optionSet")), (CodeStatement) CodeGenerationService.Return((CodeExpression) CodeGenerationService.Cast(codeTypeReference, (CodeExpression) CodeGenerationService.ConvertEnum(codeTypeReference, "optionSet"))), (CodeStatement) CodeGenerationService.Return((CodeExpression) CodeGenerationService.Null()))
            });
        }

        private static CodeStatementCollection BuildOptionSetAttributeSet(
          AttributeMetadata attribute,
          string propertyName)
        {
            return new CodeStatementCollection()
      {
        (CodeExpression) CodeGenerationService.ThisMethodInvoke("OnPropertyChanging", (CodeExpression) CodeGenerationService.StringLiteral(propertyName)),
        (CodeStatement) CodeGenerationService.If(CodeGenerationService.ValueNull(), (CodeExpression) CodeGenerationService.ThisMethodInvoke("SetAttributeValue", (CodeExpression) CodeGenerationService.StringLiteral(attribute.LogicalName), (CodeExpression) CodeGenerationService.Null()), (CodeExpression) CodeGenerationService.ThisMethodInvoke("SetAttributeValue", (CodeExpression) CodeGenerationService.StringLiteral(attribute.LogicalName), (CodeExpression) CodeGenerationService.New(CodeGenerationService.TypeRef(typeof (OptionSetValue)), (CodeExpression) CodeGenerationService.Cast(CodeGenerationService.TypeRef(typeof (int)), (CodeExpression) CodeGenerationService.VarRef("value"))))),
        (CodeExpression) CodeGenerationService.ThisMethodInvoke("OnPropertyChanged", (CodeExpression) CodeGenerationService.StringLiteral(propertyName))
      };
        }

        private static CodeTypeMember BuildCalendarRuleAttribute(
          EntityMetadata entity,
          EntityMetadata otherEntity,
          OneToManyRelationshipMetadata oneToMany,
          ServiceProvider serviceProvider)
        {
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStart("Entering {0}", (object)MethodBase.GetCurrentMethod().Name);
            CodeMemberProperty codeMemberProperty = CodeGenerationService.PropertyGet(CodeGenerationService.IEnumerable(serviceProvider.TypeMappingService.GetTypeForRelationship((RelationshipMetadataBase)oneToMany, otherEntity, (IServiceProvider)serviceProvider)), "CalendarRules");
            codeMemberProperty.GetStatements.AddRange(CodeGenerationService.BuildEntityCollectionAttributeGet("calendarrules", codeMemberProperty.Type));
            codeMemberProperty.SetStatements.Add((CodeExpression)CodeGenerationService.ThisMethodInvoke("OnPropertyChanging", (CodeExpression)CodeGenerationService.StringLiteral(codeMemberProperty.Name)));
            codeMemberProperty.SetStatements.Add(CodeGenerationService.BuildEntityCollectionAttributeSet("calendarrules"));
            codeMemberProperty.SetStatements.Add((CodeExpression)CodeGenerationService.ThisMethodInvoke("OnPropertyChanged", (CodeExpression)CodeGenerationService.StringLiteral(codeMemberProperty.Name)));
            codeMemberProperty.CustomAttributes.Add(CodeGenerationService.Attribute(CodeGenerationService.AttributeLogicalNameAttribute, CodeGenerationService.AttributeArg((object)"calendarrules")));
            codeMemberProperty.Comments.AddRange(CodeGenerationService.CommentSummary("1:N " + oneToMany.SchemaName));
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStop("Exiting {0}: {1}.Attribute {2} defined", (object)MethodBase.GetCurrentMethod().Name, (object)entity.LogicalName, (object)codeMemberProperty.Name);
            return (CodeTypeMember)codeMemberProperty;
        }

        private static CodeTypeMemberCollection BuildOneToManyRelationships(
          EntityMetadata entity,
          ServiceProvider serviceProvider)
        {
            CodeTypeMemberCollection memberCollection = new CodeTypeMemberCollection();
            if (entity.OneToManyRelationships == null)
                return memberCollection;
            foreach (OneToManyRelationshipMetadata oneToMany in (IEnumerable<OneToManyRelationshipMetadata>)((IEnumerable<OneToManyRelationshipMetadata>)entity.OneToManyRelationships).OrderBy<OneToManyRelationshipMetadata, string>((Func<OneToManyRelationshipMetadata, string>)(metadata => metadata.SchemaName)))
            {
                EntityMetadata entityMetadata = CodeGenerationService.GetEntityMetadata(oneToMany.ReferencingEntity, serviceProvider);
                if (entityMetadata == null)
                    CrmSvcUtil.crmSvcUtilLogger.TraceInformation("Skipping {0}.OneToMany {1} from being generated. Correlating entity not returned.", (object)entity.LogicalName, (object)oneToMany.SchemaName);
                else if (string.Equals(oneToMany.SchemaName, "calendar_calendar_rules", StringComparison.Ordinal) || string.Equals(oneToMany.SchemaName, "service_calendar_rules", StringComparison.Ordinal))
                    memberCollection.Add(CodeGenerationService.BuildCalendarRuleAttribute(entity, entityMetadata, oneToMany, serviceProvider));
                else if (serviceProvider.CodeFilterService.GenerateEntity(entityMetadata, (IServiceProvider)serviceProvider) && serviceProvider.CodeFilterService.GenerateRelationship((RelationshipMetadataBase)oneToMany, entityMetadata, (IServiceProvider)serviceProvider))
                    memberCollection.Add(CodeGenerationService.BuildOneToMany(entity, entityMetadata, oneToMany, serviceProvider));
                else
                    CrmSvcUtil.crmSvcUtilLogger.TraceInformation("Skipping {0}.OneToMany {1} from being generated.", (object)entity.LogicalName, (object)oneToMany.SchemaName);
            }
            return memberCollection;
        }

        private static CodeTypeMember BuildOneToMany(
          EntityMetadata entity,
          EntityMetadata otherEntity,
          OneToManyRelationshipMetadata oneToMany,
          ServiceProvider serviceProvider)
        {
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStart("Entering {0}", (object)MethodBase.GetCurrentMethod().Name);
            CodeTypeReference typeForRelationship = serviceProvider.TypeMappingService.GetTypeForRelationship((RelationshipMetadataBase)oneToMany, otherEntity, (IServiceProvider)serviceProvider);
            EntityRole? nullable = oneToMany.ReferencingEntity == entity.LogicalName ? new EntityRole?(EntityRole.Referenced) : new EntityRole?();
            CodeMemberProperty codeMemberProperty = CodeGenerationService.PropertyGet(CodeGenerationService.IEnumerable(typeForRelationship), serviceProvider.NamingService.GetNameForRelationship(entity, (RelationshipMetadataBase)oneToMany, nullable, (IServiceProvider)serviceProvider));
            codeMemberProperty.GetStatements.Add(CodeGenerationService.BuildRelationshipGet("GetRelatedEntities", (RelationshipMetadataBase)oneToMany, typeForRelationship, nullable));
            codeMemberProperty.SetStatements.AddRange(CodeGenerationService.BuildRelationshipSet("SetRelatedEntities", (RelationshipMetadataBase)oneToMany, typeForRelationship, codeMemberProperty.Name, nullable));
            codeMemberProperty.CustomAttributes.Add(CodeGenerationService.BuildRelationshipSchemaNameAttribute(oneToMany.SchemaName, nullable));
            codeMemberProperty.Comments.AddRange(CodeGenerationService.CommentSummary("1:N " + oneToMany.SchemaName));
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStop("Exiting {0}: {1}.OneToMany {2} defined", (object)MethodBase.GetCurrentMethod().Name, (object)entity.LogicalName, (object)codeMemberProperty.Name);
            return (CodeTypeMember)codeMemberProperty;
        }

        private static CodeTypeMemberCollection BuildManyToManyRelationships(
          EntityMetadata entity,
          ServiceProvider serviceProvider)
        {
            CodeTypeMemberCollection memberCollection = new CodeTypeMemberCollection();
            if (entity.ManyToManyRelationships == null)
                return memberCollection;
            foreach (ManyToManyRelationshipMetadata manyToMany in (IEnumerable<ManyToManyRelationshipMetadata>)((IEnumerable<ManyToManyRelationshipMetadata>)entity.ManyToManyRelationships).OrderBy<ManyToManyRelationshipMetadata, string>((Func<ManyToManyRelationshipMetadata, string>)(metadata => metadata.SchemaName)))
            {
                EntityMetadata entityMetadata = CodeGenerationService.GetEntityMetadata(entity.LogicalName != manyToMany.Entity1LogicalName ? manyToMany.Entity1LogicalName : manyToMany.Entity2LogicalName, serviceProvider);
                if (entityMetadata == null)
                    CrmSvcUtil.crmSvcUtilLogger.TraceInformation("Skipping {0}.ManyToMany {1} from being generated. Correlating entity not returned.", (object)entity.LogicalName, (object)manyToMany.SchemaName);
                else if (serviceProvider.CodeFilterService.GenerateEntity(entityMetadata, (IServiceProvider)serviceProvider) && serviceProvider.CodeFilterService.GenerateRelationship((RelationshipMetadataBase)manyToMany, entityMetadata, (IServiceProvider)serviceProvider))
                {
                    if (entityMetadata.LogicalName != entity.LogicalName)
                    {
                        string nameForRelationship = serviceProvider.NamingService.GetNameForRelationship(entity, (RelationshipMetadataBase)manyToMany, new EntityRole?(), (IServiceProvider)serviceProvider);
                        CodeTypeMember many = CodeGenerationService.BuildManyToMany(entity, entityMetadata, manyToMany, nameForRelationship, new EntityRole?(), serviceProvider);
                        memberCollection.Add(many);
                    }
                    else
                    {
                        string nameForRelationship1 = serviceProvider.NamingService.GetNameForRelationship(entity, (RelationshipMetadataBase)manyToMany, new EntityRole?(EntityRole.Referencing), (IServiceProvider)serviceProvider);
                        CodeTypeMember many1 = CodeGenerationService.BuildManyToMany(entity, entityMetadata, manyToMany, nameForRelationship1, new EntityRole?(EntityRole.Referencing), serviceProvider);
                        memberCollection.Add(many1);
                        string nameForRelationship2 = serviceProvider.NamingService.GetNameForRelationship(entity, (RelationshipMetadataBase)manyToMany, new EntityRole?(EntityRole.Referenced), (IServiceProvider)serviceProvider);
                        CodeTypeMember many2 = CodeGenerationService.BuildManyToMany(entity, entityMetadata, manyToMany, nameForRelationship2, new EntityRole?(EntityRole.Referenced), serviceProvider);
                        memberCollection.Add(many2);
                    }
                }
                else
                    CrmSvcUtil.crmSvcUtilLogger.TraceInformation("Skipping {0}.ManyToMany {1} from being generated.", (object)entity.LogicalName, (object)manyToMany.SchemaName);
            }
            return memberCollection;
        }

        private static CodeTypeMember BuildManyToMany(
          EntityMetadata entity,
          EntityMetadata otherEntity,
          ManyToManyRelationshipMetadata manyToMany,
          string propertyName,
          EntityRole? entityRole,
          ServiceProvider serviceProvider)
        {
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStart("Entering {0}", (object)MethodBase.GetCurrentMethod().Name);
            CodeTypeReference typeForRelationship = serviceProvider.TypeMappingService.GetTypeForRelationship((RelationshipMetadataBase)manyToMany, otherEntity, (IServiceProvider)serviceProvider);
            CodeMemberProperty codeMemberProperty = CodeGenerationService.PropertyGet(CodeGenerationService.IEnumerable(typeForRelationship), propertyName);
            codeMemberProperty.GetStatements.Add(CodeGenerationService.BuildRelationshipGet("GetRelatedEntities", (RelationshipMetadataBase)manyToMany, typeForRelationship, entityRole));
            codeMemberProperty.SetStatements.AddRange(CodeGenerationService.BuildRelationshipSet("SetRelatedEntities", (RelationshipMetadataBase)manyToMany, typeForRelationship, propertyName, entityRole));
            codeMemberProperty.CustomAttributes.Add(CodeGenerationService.BuildRelationshipSchemaNameAttribute(manyToMany.SchemaName, entityRole));
            codeMemberProperty.Comments.AddRange(CodeGenerationService.CommentSummary("N:N " + manyToMany.SchemaName));
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStop("Exiting {0}: {1}.ManyToMany {2} defined", (object)MethodBase.GetCurrentMethod().Name, (object)entity.LogicalName, (object)propertyName);
            return (CodeTypeMember)codeMemberProperty;
        }

        private static CodeTypeMemberCollection BuildManyToOneRelationships(
          EntityMetadata entity,
          ServiceProvider serviceProvider)
        {
            CodeTypeMemberCollection memberCollection = new CodeTypeMemberCollection();
            if (entity.ManyToOneRelationships == null)
                return memberCollection;
            foreach (OneToManyRelationshipMetadata manyToOne in (IEnumerable<OneToManyRelationshipMetadata>)((IEnumerable<OneToManyRelationshipMetadata>)entity.ManyToOneRelationships).OrderBy<OneToManyRelationshipMetadata, string>((Func<OneToManyRelationshipMetadata, string>)(metadata => metadata.SchemaName)))
            {
                EntityMetadata entityMetadata = CodeGenerationService.GetEntityMetadata(manyToOne.ReferencedEntity, serviceProvider);
                if (entityMetadata == null)
                    CrmSvcUtil.crmSvcUtilLogger.TraceInformation("Skipping {0}.ManyToOne {1} from being generated. Correlating entity not returned.", (object)entity.LogicalName, (object)manyToOne.SchemaName);
                else if (serviceProvider.CodeFilterService.GenerateEntity(entityMetadata, (IServiceProvider)serviceProvider) && serviceProvider.CodeFilterService.GenerateRelationship((RelationshipMetadataBase)manyToOne, entityMetadata, (IServiceProvider)serviceProvider))
                {
                    CodeTypeMember one = CodeGenerationService.BuildManyToOne(entity, entityMetadata, manyToOne, serviceProvider);
                    if (one != null)
                        memberCollection.Add(one);
                }
                else
                    CrmSvcUtil.crmSvcUtilLogger.TraceInformation("Skipping {0}.ManyToOne {1} from being generated.", (object)entity.LogicalName, (object)manyToOne.SchemaName);
            }
            return memberCollection;
        }

        private static CodeTypeMember BuildManyToOne(
          EntityMetadata entity,
          EntityMetadata otherEntity,
          OneToManyRelationshipMetadata manyToOne,
          ServiceProvider serviceProvider)
        {
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStart("Entering {0}", (object)MethodBase.GetCurrentMethod().Name);
            CodeTypeReference typeForRelationship = serviceProvider.TypeMappingService.GetTypeForRelationship((RelationshipMetadataBase)manyToOne, otherEntity, (IServiceProvider)serviceProvider);
            EntityRole? nullable = otherEntity.LogicalName == entity.LogicalName ? new EntityRole?(EntityRole.Referencing) : new EntityRole?();
            CodeMemberProperty codeMemberProperty = CodeGenerationService.PropertyGet(typeForRelationship, serviceProvider.NamingService.GetNameForRelationship(entity, (RelationshipMetadataBase)manyToOne, nullable, (IServiceProvider)serviceProvider));
            codeMemberProperty.GetStatements.Add(CodeGenerationService.BuildRelationshipGet("GetRelatedEntity", (RelationshipMetadataBase)manyToOne, typeForRelationship, nullable));
            AttributeMetadata attributeMetadata = ((IEnumerable<AttributeMetadata>)entity.Attributes).SingleOrDefault<AttributeMetadata>((Func<AttributeMetadata, bool>)(attribute => attribute.LogicalName == manyToOne.ReferencingAttribute));
            if (attributeMetadata == null)
            {
                CrmSvcUtil.crmSvcUtilLogger.TraceMethodStop("Exiting {0}: {1}.ManyToOne {2} not generated since referencing attribute is not generated.", (object)MethodBase.GetCurrentMethod().Name, (object)entity.LogicalName, (object)manyToOne.SchemaName);
                return (CodeTypeMember)null;
            }
            if (attributeMetadata.IsValidForCreate.GetValueOrDefault() || attributeMetadata.IsValidForUpdate.GetValueOrDefault())
                codeMemberProperty.SetStatements.AddRange(CodeGenerationService.BuildRelationshipSet("SetRelatedEntity", (RelationshipMetadataBase)manyToOne, typeForRelationship, codeMemberProperty.Name, nullable));
            codeMemberProperty.CustomAttributes.Add(CodeGenerationService.Attribute(CodeGenerationService.AttributeLogicalNameAttribute, CodeGenerationService.AttributeArg((object)manyToOne.ReferencingAttribute)));
            codeMemberProperty.CustomAttributes.Add(CodeGenerationService.BuildRelationshipSchemaNameAttribute(manyToOne.SchemaName, nullable));
            codeMemberProperty.Comments.AddRange(CodeGenerationService.CommentSummary("N:1 " + manyToOne.SchemaName));
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStop("Exiting {0}: {1}.ManyToOne {2} defined", (object)MethodBase.GetCurrentMethod().Name, (object)entity.LogicalName, (object)codeMemberProperty.Name);
            return (CodeTypeMember)codeMemberProperty;
        }

        private static CodeStatement BuildRelationshipGet(
          string methodName,
          RelationshipMetadataBase relationship,
          CodeTypeReference targetType,
          EntityRole? entityRole)
        {
            CodeExpression codeExpression = entityRole.HasValue ? (CodeExpression)CodeGenerationService.FieldRef(typeof(EntityRole), entityRole.ToString()) : (CodeExpression)CodeGenerationService.Null();
            return (CodeStatement)CodeGenerationService.Return((CodeExpression)CodeGenerationService.ThisMethodInvoke(methodName, targetType, (CodeExpression)CodeGenerationService.StringLiteral(relationship.SchemaName), codeExpression));
        }

        private static CodeStatementCollection BuildRelationshipSet(
          string methodName,
          RelationshipMetadataBase relationship,
          CodeTypeReference targetType,
          string propertyName,
          EntityRole? entityRole)
        {
            CodeStatementCollection statementCollection = new CodeStatementCollection();
            CodeExpression codeExpression = entityRole.HasValue ? (CodeExpression)CodeGenerationService.FieldRef(typeof(EntityRole), entityRole.ToString()) : (CodeExpression)CodeGenerationService.Null();
            statementCollection.Add((CodeExpression)CodeGenerationService.ThisMethodInvoke("OnPropertyChanging", (CodeExpression)CodeGenerationService.StringLiteral(propertyName)));
            statementCollection.Add((CodeExpression)CodeGenerationService.ThisMethodInvoke(methodName, targetType, (CodeExpression)CodeGenerationService.StringLiteral(relationship.SchemaName), codeExpression, (CodeExpression)CodeGenerationService.VarRef("value")));
            statementCollection.Add((CodeExpression)CodeGenerationService.ThisMethodInvoke("OnPropertyChanged", (CodeExpression)CodeGenerationService.StringLiteral(propertyName)));
            return statementCollection;
        }

        private static CodeAttributeDeclaration BuildRelationshipSchemaNameAttribute(
          string relationshipSchemaName,
          EntityRole? entityRole)
        {
            if (entityRole.HasValue)
                return CodeGenerationService.Attribute(CodeGenerationService.RelationshipSchemaNameAttribute, CodeGenerationService.AttributeArg((object)relationshipSchemaName), CodeGenerationService.AttributeArg((object)CodeGenerationService.FieldRef(typeof(EntityRole), entityRole.ToString())));
            return CodeGenerationService.Attribute(CodeGenerationService.RelationshipSchemaNameAttribute, CodeGenerationService.AttributeArg((object)relationshipSchemaName));
        }

        private static EntityMetadata GetEntityMetadata(
          string entityLogicalName,
          ServiceProvider serviceProvider)
        {
            if (serviceProvider.MetadataProviderService is IMetadataProviderService2)
                return ((IEnumerable<EntityMetadata>)((IMetadataProviderService2)serviceProvider.MetadataProviderService).LoadMetadata((IServiceProvider)serviceProvider).Entities).SingleOrDefault<EntityMetadata>((Func<EntityMetadata, bool>)(e => e.LogicalName == entityLogicalName));
            return ((IEnumerable<EntityMetadata>)serviceProvider.MetadataProviderService.LoadMetadata().Entities).SingleOrDefault<EntityMetadata>((Func<EntityMetadata, bool>)(e => e.LogicalName == entityLogicalName));
        }

        private static CodeTypeDeclarationCollection BuildServiceContext(
          EntityMetadata[] entityMetadata,
          ServiceProvider serviceProvider)
        {
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStart("Entering {0}", (object)MethodBase.GetCurrentMethod().Name);
            CodeTypeDeclarationCollection declarationCollection = new CodeTypeDeclarationCollection();
            if (serviceProvider.CodeFilterService.GenerateServiceContext((IServiceProvider)serviceProvider))
            {
                CodeTypeDeclaration codeTypeDeclaration = CodeGenerationService.Class(serviceProvider.NamingService.GetNameForServiceContext((IServiceProvider)serviceProvider), CodeGenerationService.ServiceContextBaseType);
                codeTypeDeclaration.Members.Add(CodeGenerationService.ServiceContextConstructor());
                codeTypeDeclaration.Comments.AddRange(CodeGenerationService.CommentSummary("Represents a source of entities bound to a CRM service. It tracks and manages changes made to the retrieved entities."));
                foreach (EntityMetadata entityMetadata1 in (IEnumerable<EntityMetadata>)((IEnumerable<EntityMetadata>)entityMetadata).OrderBy<EntityMetadata, string>((Func<EntityMetadata, string>)(metadata => metadata.LogicalName)))
                {
                    if (serviceProvider.CodeFilterService.GenerateEntity(entityMetadata1, (IServiceProvider)serviceProvider) && !string.Equals(entityMetadata1.LogicalName, "calendarrule", StringComparison.Ordinal))
                        codeTypeDeclaration.Members.Add(CodeGenerationService.BuildEntitySet(entityMetadata1, serviceProvider));
                    else
                        CrmSvcUtil.crmSvcUtilLogger.TraceInformation("Skipping {0} entity set and AddTo method from being generated.", (object)entityMetadata1.LogicalName);
                }
                declarationCollection.Add(codeTypeDeclaration);
            }
            else
                CrmSvcUtil.crmSvcUtilLogger.TraceInformation("Skipping data context from being generated.");
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStop("Exiting {0}", (object)MethodBase.GetCurrentMethod().Name);
            return declarationCollection;
        }

        private static CodeTypeMember ServiceContextConstructor()
        {
            CodeConstructor codeConstructor = CodeGenerationService.Constructor(CodeGenerationService.Param(CodeGenerationService.TypeRef(typeof(IOrganizationService)), "service"));
            codeConstructor.BaseConstructorArgs.Add((CodeExpression)CodeGenerationService.VarRef("service"));
            codeConstructor.Comments.AddRange(CodeGenerationService.CommentSummary("Constructor."));
            return (CodeTypeMember)codeConstructor;
        }

        private static CodeTypeMember BuildEntitySet(
          EntityMetadata entity,
          ServiceProvider serviceProvider)
        {
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStart("Entering {0}", (object)MethodBase.GetCurrentMethod().Name);
            CodeTypeReference typeForEntity = serviceProvider.TypeMappingService.GetTypeForEntity(entity, (IServiceProvider)serviceProvider);
            CodeMemberProperty codeMemberProperty = CodeGenerationService.PropertyGet(CodeGenerationService.IQueryable(typeForEntity), serviceProvider.NamingService.GetNameForEntitySet(entity, (IServiceProvider)serviceProvider), (CodeStatement)CodeGenerationService.Return((CodeExpression)CodeGenerationService.ThisMethodInvoke("CreateQuery", typeForEntity)));
            codeMemberProperty.Comments.AddRange(CodeGenerationService.CommentSummary(string.Format((IFormatProvider)CultureInfo.InvariantCulture, "Gets a binding to the set of all <see cref=\"{0}\"/> entities.", (object)typeForEntity.BaseType)));
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStop("Exiting {0}: {1} entity set '{2}' defined", (object)MethodBase.GetCurrentMethod().Name, (object)entity.LogicalName, (object)codeMemberProperty.Name);
            return (CodeTypeMember)codeMemberProperty;
        }

        private static CodeTypeDeclarationCollection BuildMessages(
          SdkMessages sdkMessages,
          ServiceProvider serviceProvider)
        {
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStart("Entering {0}", (object)MethodBase.GetCurrentMethod().Name);
            CodeTypeDeclarationCollection declarationCollection = new CodeTypeDeclarationCollection();
            foreach (SdkMessage sdkMessage in sdkMessages.MessageCollection.Values)
            {
                if (serviceProvider.CodeMessageFilterService.GenerateSdkMessage(sdkMessage, (IServiceProvider)serviceProvider))
                    declarationCollection.AddRange(CodeGenerationService.BuildMessage(sdkMessage, serviceProvider));
                else
                    CrmSvcUtil.crmSvcUtilLogger.TraceInformation("Skipping SDK Message {0} from being generated.", (object)sdkMessage.Name);
            }
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStop("Exiting {0}", (object)MethodBase.GetCurrentMethod().Name);
            return declarationCollection;
        }

        private static CodeTypeDeclarationCollection BuildMessage(
          SdkMessage message,
          ServiceProvider serviceProvider)
        {
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStart("Entering {0}", (object)MethodBase.GetCurrentMethod().Name);
            CodeTypeDeclarationCollection declarationCollection = new CodeTypeDeclarationCollection();
            foreach (SdkMessagePair sdkMessagePair in message.SdkMessagePairs.Values)
            {
                if (serviceProvider.CodeMessageFilterService.GenerateSdkMessagePair(sdkMessagePair, (IServiceProvider)serviceProvider))
                {
                    declarationCollection.Add(CodeGenerationService.BuildMessageRequest(sdkMessagePair, sdkMessagePair.Request, serviceProvider));
                    declarationCollection.Add(CodeGenerationService.BuildMessageResponse(sdkMessagePair, sdkMessagePair.Response, serviceProvider));
                }
                else
                    CrmSvcUtil.crmSvcUtilLogger.TraceInformation("Skipping {0}.Message Pair from being generated.", (object)message.Name, (object)sdkMessagePair.Request.Name);
            }
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStop("Exiting {0}", (object)MethodBase.GetCurrentMethod().Name);
            return declarationCollection;
        }

        private static CodeTypeDeclaration BuildMessageRequest(
          SdkMessagePair messagePair,
          SdkMessageRequest sdkMessageRequest,
          ServiceProvider serviceProvider)
        {
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStart("Entering {0}", (object)MethodBase.GetCurrentMethod().Name);
            CodeTypeDeclaration requestClass = CodeGenerationService.Class(string.Format((IFormatProvider)CultureInfo.InvariantCulture, "{0}{1}", (object)serviceProvider.NamingService.GetNameForMessagePair(messagePair, (IServiceProvider)serviceProvider), (object)CodeGenerationService.RequestClassSuffix), CodeGenerationService.RequestClassBaseType, CodeGenerationService.Attribute(typeof(DataContractAttribute), CodeGenerationService.AttributeArg("Namespace", (object)messagePair.MessageNamespace)), CodeGenerationService.Attribute(typeof(RequestProxyAttribute), CodeGenerationService.AttributeArg((string)null, (object)messagePair.Request.Name)));
            bool flag = false;
            CodeStatementCollection statementCollection = new CodeStatementCollection();
            if (sdkMessageRequest.RequestFields != null & sdkMessageRequest.RequestFields.Count > 0)
            {
                foreach (SdkMessageRequestField field in sdkMessageRequest.RequestFields.Values)
                {
                    CodeMemberProperty requestField = CodeGenerationService.BuildRequestField(sdkMessageRequest, field, serviceProvider);
                    if (requestField.Type.Options == CodeTypeReferenceOptions.GenericTypeParameter)
                    {
                        CrmSvcUtil.crmSvcUtilLogger.TraceInformation("Request Field {0} is generic.  Adding generic parameter to the {1} class.", (object)requestField.Name, (object)requestClass.Name);
                        flag = true;
                        CodeGenerationService.ConvertRequestToGeneric(messagePair, requestClass, requestField);
                    }
                    requestClass.Members.Add((CodeTypeMember)requestField);
                    if (!field.IsOptional)
                        statementCollection.Add((CodeStatement)CodeGenerationService.AssignProp(requestField.Name, (CodeExpression)new CodeDefaultValueExpression(requestField.Type)));
                }
            }
            if (!flag)
            {
                CodeConstructor codeConstructor = CodeGenerationService.Constructor();
                codeConstructor.Statements.Add((CodeStatement)CodeGenerationService.AssignProp(CodeGenerationService.RequestNamePropertyName, (CodeExpression)new CodePrimitiveExpression((object)messagePair.Request.Name)));
                codeConstructor.Statements.AddRange(statementCollection);
                requestClass.Members.Add((CodeTypeMember)codeConstructor);
            }
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStop("Exiting {0}: SDK Request Class {1} defined", (object)MethodBase.GetCurrentMethod().Name, (object)requestClass.Name);
            return requestClass;
        }

        private static void ConvertRequestToGeneric(
          SdkMessagePair messagePair,
          CodeTypeDeclaration requestClass,
          CodeMemberProperty requestField)
        {
            requestClass.TypeParameters.Add(new CodeTypeParameter(requestField.Type.BaseType)
            {
                HasConstructorConstraint = true,
                Constraints = {
          new CodeTypeReference(CodeGenerationService.EntityClassBaseType)
        }
            });
            requestClass.Members.Add((CodeTypeMember)CodeGenerationService.Constructor((CodeExpression)CodeGenerationService.New(requestField.Type)));
            CodeConstructor codeConstructor = CodeGenerationService.Constructor(CodeGenerationService.Param(requestField.Type, "target"), (CodeStatement)CodeGenerationService.AssignProp(requestField.Name, (CodeExpression)CodeGenerationService.VarRef("target")));
            codeConstructor.Statements.Add((CodeStatement)CodeGenerationService.AssignProp(CodeGenerationService.RequestNamePropertyName, (CodeExpression)new CodePrimitiveExpression((object)messagePair.Request.Name)));
            requestClass.Members.Add((CodeTypeMember)codeConstructor);
        }

        private static CodeTypeDeclaration BuildMessageResponse(
          SdkMessagePair messagePair,
          SdkMessageResponse sdkMessageResponse,
          ServiceProvider serviceProvider)
        {
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStart("Entering {0}", (object)MethodBase.GetCurrentMethod().Name);
            CodeTypeDeclaration codeTypeDeclaration = CodeGenerationService.Class(string.Format((IFormatProvider)CultureInfo.InvariantCulture, "{0}{1}", (object)serviceProvider.NamingService.GetNameForMessagePair(messagePair, (IServiceProvider)serviceProvider), (object)CodeGenerationService.ResponseClassSuffix), CodeGenerationService.ResponseClassBaseType, CodeGenerationService.Attribute(typeof(DataContractAttribute), CodeGenerationService.AttributeArg("Namespace", (object)messagePair.MessageNamespace)), CodeGenerationService.Attribute(typeof(ResponseProxyAttribute), CodeGenerationService.AttributeArg((string)null, (object)messagePair.Request.Name)));
            codeTypeDeclaration.Members.Add((CodeTypeMember)CodeGenerationService.Constructor());
            if (sdkMessageResponse != null && sdkMessageResponse.ResponseFields != null & sdkMessageResponse.ResponseFields.Count > 0)
            {
                foreach (SdkMessageResponseField field in sdkMessageResponse.ResponseFields.Values)
                    codeTypeDeclaration.Members.Add((CodeTypeMember)CodeGenerationService.BuildResponseField(sdkMessageResponse, field, serviceProvider));
            }
            else
                CrmSvcUtil.crmSvcUtilLogger.TraceInformation("SDK Response Class {0} has not fields", (object)codeTypeDeclaration.Name);
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStop("Exiting {0}: SDK Response Class {1} defined", (object)MethodBase.GetCurrentMethod().Name, (object)codeTypeDeclaration.Name);
            return codeTypeDeclaration;
        }

        private static CodeMemberProperty BuildRequestField(
          SdkMessageRequest request,
          SdkMessageRequestField field,
          ServiceProvider serviceProvider)
        {
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStart("Entering {0}", (object)MethodBase.GetCurrentMethod().Name);
            CodeTypeReference typeForRequestField = serviceProvider.TypeMappingService.GetTypeForRequestField(field, (IServiceProvider)serviceProvider);
            CodeMemberProperty codeMemberProperty = CodeGenerationService.PropertyGet(typeForRequestField, serviceProvider.NamingService.GetNameForRequestField(request, field, (IServiceProvider)serviceProvider));
            codeMemberProperty.HasSet = true;
            codeMemberProperty.HasGet = true;
            codeMemberProperty.GetStatements.Add(CodeGenerationService.BuildRequestFieldGetStatement(field, typeForRequestField));
            codeMemberProperty.SetStatements.Add((CodeStatement)CodeGenerationService.BuildRequestFieldSetStatement(field));
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStop("Exiting {0}: {1}.Request Property {2} defined", (object)MethodBase.GetCurrentMethod().Name, (object)request.Name, (object)codeMemberProperty.Name);
            return codeMemberProperty;
        }

        private static CodeStatement BuildRequestFieldGetStatement(
          SdkMessageRequestField field,
          CodeTypeReference targetType)
        {
            return (CodeStatement)CodeGenerationService.If((CodeExpression)CodeGenerationService.ContainsParameter(field.Name), (CodeStatement)CodeGenerationService.Return((CodeExpression)CodeGenerationService.Cast(targetType, (CodeExpression)CodeGenerationService.PropertyIndexer(CodeGenerationService.ParametersPropertyName, field.Name))), (CodeStatement)CodeGenerationService.Return((CodeExpression)new CodeDefaultValueExpression(targetType)));
        }

        private static CodeAssignStatement BuildRequestFieldSetStatement(
          SdkMessageRequestField field)
        {
            return CodeGenerationService.AssignValue((CodeExpression)CodeGenerationService.PropertyIndexer(CodeGenerationService.ParametersPropertyName, field.Name));
        }

        private static CodeMemberProperty BuildResponseField(
          SdkMessageResponse response,
          SdkMessageResponseField field,
          ServiceProvider serviceProvider)
        {
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStart("Entering {0}", (object)MethodBase.GetCurrentMethod().Name);
            CodeTypeReference forResponseField = serviceProvider.TypeMappingService.GetTypeForResponseField(field, (IServiceProvider)serviceProvider);
            CodeMemberProperty codeMemberProperty = CodeGenerationService.PropertyGet(forResponseField, serviceProvider.NamingService.GetNameForResponseField(response, field, (IServiceProvider)serviceProvider));
            codeMemberProperty.HasSet = false;
            codeMemberProperty.HasGet = true;
            codeMemberProperty.GetStatements.Add(CodeGenerationService.BuildResponseFieldGetStatement(field, forResponseField));
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStop("Exiting {0}: {1}.Response Property {2} defined", (object)MethodBase.GetCurrentMethod().Name, (object)response.Id, (object)codeMemberProperty.Name);
            return codeMemberProperty;
        }

        private static CodeStatement BuildResponseFieldGetStatement(
          SdkMessageResponseField field,
          CodeTypeReference targetType)
        {
            return (CodeStatement)CodeGenerationService.If((CodeExpression)CodeGenerationService.ContainsResult(field.Name), (CodeStatement)CodeGenerationService.Return((CodeExpression)CodeGenerationService.Cast(targetType, (CodeExpression)CodeGenerationService.PropertyIndexer(CodeGenerationService.ResultsPropertyName, field.Name))), (CodeStatement)CodeGenerationService.Return((CodeExpression)new CodeDefaultValueExpression(targetType)));
        }

        private static CodeNamespace Namespace(string name)
        {
            return new CodeNamespace(name);
        }

        private static CodeTypeDeclaration Class(
          string name,
          Type baseType,
          params CodeAttributeDeclaration[] attrs)
        {
            return CodeGenerationService.Class(name, CodeGenerationService.TypeRef(baseType), attrs);
        }

        private static CodeTypeDeclaration Class(
          string name,
          CodeTypeReference baseType,
          params CodeAttributeDeclaration[] attrs)
        {
            CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration(name);
            codeTypeDeclaration.IsClass = true;
            codeTypeDeclaration.TypeAttributes = TypeAttributes.Public;
            codeTypeDeclaration.BaseTypes.Add(baseType);
            if (attrs != null)
                codeTypeDeclaration.CustomAttributes.AddRange(attrs);
            codeTypeDeclaration.IsPartial = true;
            codeTypeDeclaration.CustomAttributes.Add(CodeGenerationService.Attribute(typeof(GeneratedCodeAttribute), CodeGenerationService.AttributeArg((object)CrmSvcUtil.ApplicationName), CodeGenerationService.AttributeArg((object)CrmSvcUtil.ApplicationVersion)));
            return codeTypeDeclaration;
        }

        private static CodeTypeDeclaration Enum(
          string name,
          params CodeAttributeDeclaration[] attrs)
        {
            CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration(name);
            codeTypeDeclaration.IsEnum = true;
            codeTypeDeclaration.TypeAttributes = TypeAttributes.Public;
            if (attrs != null)
                codeTypeDeclaration.CustomAttributes.AddRange(attrs);
            codeTypeDeclaration.CustomAttributes.Add(CodeGenerationService.Attribute(typeof(GeneratedCodeAttribute), CodeGenerationService.AttributeArg((object)CrmSvcUtil.ApplicationName), CodeGenerationService.AttributeArg((object)CrmSvcUtil.ApplicationVersion)));
            return codeTypeDeclaration;
        }

        private static CodeTypeReference TypeRef(Type type)
        {
            return new CodeTypeReference(type);
        }

        private static CodeAttributeDeclaration Attribute(Type type)
        {
            return new CodeAttributeDeclaration(CodeGenerationService.TypeRef(type));
        }

        private static CodeAttributeDeclaration Attribute(
          Type type,
          params CodeAttributeArgument[] args)
        {
            return new CodeAttributeDeclaration(CodeGenerationService.TypeRef(type), args);
        }

        private static CodeAttributeArgument AttributeArg(object value)
        {
            CodeExpression codeExpression = value as CodeExpression;
            if (codeExpression != null)
                return CodeGenerationService.AttributeArg((string)null, codeExpression);
            return CodeGenerationService.AttributeArg((string)null, value);
        }

        private static CodeAttributeArgument AttributeArg(
          string name,
          object value)
        {
            return CodeGenerationService.AttributeArg(name, (CodeExpression)new CodePrimitiveExpression(value));
        }

        private static CodeAttributeArgument AttributeArg(
          string name,
          CodeExpression value)
        {
            if (!string.IsNullOrEmpty(name))
                return new CodeAttributeArgument(name, value);
            return new CodeAttributeArgument(value);
        }

        private static CodeMemberProperty PropertyGet(
          CodeTypeReference type,
          string name,
          params CodeStatement[] stmts)
        {
            CodeMemberProperty codeMemberProperty = new CodeMemberProperty();
            codeMemberProperty.Type = type;
            codeMemberProperty.Name = name;
            codeMemberProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            codeMemberProperty.HasGet = true;
            codeMemberProperty.HasSet = false;
            codeMemberProperty.GetStatements.AddRange(stmts);
            return codeMemberProperty;
        }

        private static CodeMemberEvent Event(
          string name,
          Type type,
          Type implementationType)
        {
            CodeMemberEvent codeMemberEvent = new CodeMemberEvent();
            codeMemberEvent.Name = name;
            codeMemberEvent.Type = CodeGenerationService.TypeRef(type);
            codeMemberEvent.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            if (implementationType != (Type)null)
                codeMemberEvent.ImplementationTypes.Add(CodeGenerationService.TypeRef(implementationType));
            return codeMemberEvent;
        }

        private static CodeMemberMethod RaiseEvent(
          string methodName,
          string eventName,
          Type eventArgsType)
        {
            CodeMemberMethod codeMemberMethod1 = new CodeMemberMethod();
            codeMemberMethod1.Name = methodName;
            CodeMemberMethod codeMemberMethod2 = codeMemberMethod1;
            codeMemberMethod2.Parameters.Add(CodeGenerationService.Param(CodeGenerationService.TypeRef(typeof(string)), "propertyName"));
            CodeEventReferenceExpression referenceExpression = new CodeEventReferenceExpression((CodeExpression)CodeGenerationService.This(), eventName);
            codeMemberMethod2.Statements.Add((CodeStatement)CodeGenerationService.If((CodeExpression)CodeGenerationService.NotNull((CodeExpression)referenceExpression), (CodeExpression)new CodeDelegateInvokeExpression((CodeExpression)referenceExpression, new CodeExpression[2]
            {
        (CodeExpression) CodeGenerationService.This(),
        (CodeExpression) CodeGenerationService.New(CodeGenerationService.TypeRef(eventArgsType), (CodeExpression) CodeGenerationService.VarRef("propertyName"))
            })));
            return codeMemberMethod2;
        }

        private static CodeMethodInvokeExpression ContainsParameter(
          string parameterName)
        {
            return new CodeMethodInvokeExpression((CodeExpression)CodeGenerationService.ThisProp(CodeGenerationService.ParametersPropertyName), "Contains", new CodeExpression[1]
            {
        (CodeExpression) CodeGenerationService.StringLiteral(parameterName)
            });
        }

        private static CodeMethodInvokeExpression ContainsResult(
          string resultName)
        {
            return new CodeMethodInvokeExpression((CodeExpression)CodeGenerationService.ThisProp(CodeGenerationService.ResultsPropertyName), "Contains", new CodeExpression[1]
            {
        (CodeExpression) CodeGenerationService.StringLiteral(resultName)
            });
        }

        private static CodeConditionStatement If(
          CodeExpression condition,
          CodeExpression trueCode)
        {
            return CodeGenerationService.If(condition, (CodeStatement)new CodeExpressionStatement(trueCode), (CodeStatement)null);
        }

        private static CodeConditionStatement If(
          CodeExpression condition,
          CodeExpression trueCode,
          CodeExpression falseCode)
        {
            return CodeGenerationService.If(condition, (CodeStatement)new CodeExpressionStatement(trueCode), (CodeStatement)new CodeExpressionStatement(falseCode));
        }

        private static CodeConditionStatement If(
          CodeExpression condition,
          CodeStatement trueStatement)
        {
            return CodeGenerationService.If(condition, trueStatement, (CodeStatement)null);
        }

        private static CodeConditionStatement If(
          CodeExpression condition,
          CodeStatement trueStatement,
          CodeStatement falseStatement)
        {
            CodeConditionStatement conditionStatement = new CodeConditionStatement(condition, new CodeStatement[1]
            {
        trueStatement
            });
            if (falseStatement != null)
                conditionStatement.FalseStatements.Add(falseStatement);
            return conditionStatement;
        }

        private static CodeFieldReferenceExpression FieldRef(
          Type targetType,
          string fieldName)
        {
            return new CodeFieldReferenceExpression((CodeExpression)new CodeTypeReferenceExpression(targetType), fieldName);
        }

        private static CodeMemberField Field(
          string name,
          Type type,
          object value,
          params CodeAttributeDeclaration[] attrs)
        {
            CodeMemberField codeMemberField = new CodeMemberField(type, name);
            codeMemberField.InitExpression = (CodeExpression)new CodePrimitiveExpression(value);
            if (attrs != null)
                codeMemberField.CustomAttributes.AddRange(attrs);
            return codeMemberField;
        }

        private static CodeParameterDeclarationExpression Param(
          CodeTypeReference type,
          string name)
        {
            return new CodeParameterDeclarationExpression(type, name);
        }

        private static CodeTypeParameter TypeParam(
          string name,
          params Type[] constraints)
        {
            CodeTypeParameter codeTypeParameter = new CodeTypeParameter(name);
            if (constraints != null)
                codeTypeParameter.Constraints.AddRange(((IEnumerable<Type>)constraints).Select<Type, CodeTypeReference>(new Func<Type, CodeTypeReference>(CodeGenerationService.TypeRef)).ToArray<CodeTypeReference>());
            return codeTypeParameter;
        }

        private static CodeVariableReferenceExpression VarRef(string name)
        {
            return new CodeVariableReferenceExpression(name);
        }

        private static CodeVariableDeclarationStatement Var(
          Type type,
          string name,
          CodeExpression init)
        {
            return new CodeVariableDeclarationStatement(type, name, init);
        }

        private static CodeConstructor Constructor(params CodeExpression[] thisArgs)
        {
            CodeConstructor codeConstructor = new CodeConstructor();
            codeConstructor.Attributes = MemberAttributes.Public;
            if (thisArgs != null)
                codeConstructor.ChainedConstructorArgs.AddRange(thisArgs);
            return codeConstructor;
        }

        private static CodeConstructor Constructor(
          CodeParameterDeclarationExpression arg,
          params CodeStatement[] statements)
        {
            CodeConstructor codeConstructor = new CodeConstructor();
            codeConstructor.Attributes = MemberAttributes.Public;
            if (arg != null)
                codeConstructor.Parameters.Add(arg);
            if (statements != null)
                codeConstructor.Statements.AddRange(statements);
            return codeConstructor;
        }

        private static CodeObjectCreateExpression New(
          CodeTypeReference createType,
          params CodeExpression[] args)
        {
            return new CodeObjectCreateExpression(createType, args);
        }

        private static CodeAssignStatement AssignProp(
          string propName,
          CodeExpression value)
        {
            return new CodeAssignStatement()
            {
                Left = (CodeExpression)CodeGenerationService.ThisProp(propName),
                Right = value
            };
        }

        private static CodeAssignStatement AssignValue(CodeExpression target)
        {
            return CodeGenerationService.AssignValue(target, (CodeExpression)new CodeVariableReferenceExpression("value"));
        }

        private static CodeAssignStatement AssignValue(
          CodeExpression target,
          CodeExpression value)
        {
            return new CodeAssignStatement(target, value);
        }

        private static CodePropertyReferenceExpression BaseProp(
          string propertyName)
        {
            return new CodePropertyReferenceExpression((CodeExpression)new CodeBaseReferenceExpression(), propertyName);
        }

        private static CodeIndexerExpression PropertyIndexer(
          string propertyName,
          string index)
        {
            return new CodeIndexerExpression((CodeExpression)CodeGenerationService.ThisProp(propertyName), new CodeExpression[1]
            {
        (CodeExpression) new CodePrimitiveExpression((object) index)
            });
        }

        private static CodePropertyReferenceExpression PropRef(
          CodeExpression expression,
          string propertyName)
        {
            return new CodePropertyReferenceExpression(expression, propertyName);
        }

        private static CodePropertyReferenceExpression ThisProp(
          string propertyName)
        {
            return new CodePropertyReferenceExpression((CodeExpression)CodeGenerationService.This(), propertyName);
        }

        private static CodeThisReferenceExpression This()
        {
            return new CodeThisReferenceExpression();
        }

        private static CodeMethodInvokeExpression ThisMethodInvoke(
          string methodName,
          params CodeExpression[] parameters)
        {
            return new CodeMethodInvokeExpression(new CodeMethodReferenceExpression((CodeExpression)CodeGenerationService.This(), methodName), parameters);
        }

        private static CodeMethodInvokeExpression ThisMethodInvoke(
          string methodName,
          CodeTypeReference typeParameter,
          params CodeExpression[] parameters)
        {
            return new CodeMethodInvokeExpression(new CodeMethodReferenceExpression((CodeExpression)CodeGenerationService.This(), methodName, new CodeTypeReference[1]
            {
        typeParameter
            }), parameters);
        }

        private static CodeMethodInvokeExpression StaticMethodInvoke(
          Type targetObject,
          string methodName,
          params CodeExpression[] parameters)
        {
            return new CodeMethodInvokeExpression(new CodeMethodReferenceExpression((CodeExpression)new CodeTypeReferenceExpression(targetObject), methodName), parameters);
        }

        private static CodeMethodInvokeExpression StaticMethodInvoke(
          Type targetObject,
          string methodName,
          CodeTypeReference typeParameter,
          params CodeExpression[] parameters)
        {
            return new CodeMethodInvokeExpression(new CodeMethodReferenceExpression((CodeExpression)new CodeTypeReferenceExpression(targetObject), methodName, new CodeTypeReference[1]
            {
        typeParameter
            }), parameters);
        }

        private static CodeCastExpression Cast(
          CodeTypeReference targetType,
          CodeExpression expression)
        {
            return new CodeCastExpression(targetType, expression);
        }

        private static CodeCommentStatementCollection CommentSummary(
          Label comment)
        {
            return CodeGenerationService.CommentSummary(comment.UserLocalizedLabel != null ? comment.UserLocalizedLabel.Label : (comment.LocalizedLabels.Any<LocalizedLabel>() ? comment.LocalizedLabels.First<LocalizedLabel>().Label : string.Empty));
        }

        private static CodeCommentStatementCollection CommentSummary(
          string comment)
        {
            return new CodeCommentStatementCollection()
      {
        new CodeCommentStatement("<summary>", true),
        new CodeCommentStatement(comment, true),
        new CodeCommentStatement("</summary>", true)
      };
        }

        private static CodePrimitiveExpression StringLiteral(string value)
        {
            return new CodePrimitiveExpression((object)value);
        }

        private static CodeMethodReturnStatement Return()
        {
            return new CodeMethodReturnStatement();
        }

        private static CodeMethodReturnStatement Return(
          CodeExpression expression)
        {
            return new CodeMethodReturnStatement(expression);
        }

        private static CodeMethodInvokeExpression ConvertEnum(
          CodeTypeReference type,
          string variableName)
        {
            return new CodeMethodInvokeExpression((CodeExpression)new CodeTypeReferenceExpression(CodeGenerationService.TypeRef(typeof(Enum))), "ToObject", new CodeExpression[2]
            {
        (CodeExpression) new CodeTypeOfExpression(type),
        (CodeExpression) new CodePropertyReferenceExpression((CodeExpression) CodeGenerationService.VarRef(variableName), "Value")
            });
        }

        private static CodeExpression ValueNull()
        {
            return (CodeExpression)new CodeBinaryOperatorExpression((CodeExpression)CodeGenerationService.VarRef("value"), CodeBinaryOperatorType.IdentityEquality, (CodeExpression)CodeGenerationService.Null());
        }

        private static CodePrimitiveExpression Null()
        {
            return new CodePrimitiveExpression((object)null);
        }

        private static CodeBinaryOperatorExpression NotNull(
          CodeExpression expression)
        {
            return new CodeBinaryOperatorExpression(expression, CodeBinaryOperatorType.IdentityInequality, (CodeExpression)CodeGenerationService.Null());
        }

        private static CodeExpression GuidEmpty()
        {
            return (CodeExpression)CodeGenerationService.PropRef((CodeExpression)new CodeTypeReferenceExpression(typeof(Guid)), "Empty");
        }

        private static CodeExpression False()
        {
            return (CodeExpression)new CodePrimitiveExpression((object)false);
        }

        private static CodeTypeReference IEnumerable(CodeTypeReference typeParameter)
        {
            return new CodeTypeReference(typeof(IEnumerable<>).FullName, new CodeTypeReference[1]
            {
        typeParameter
            });
        }

        private static CodeTypeReference IQueryable(CodeTypeReference typeParameter)
        {
            return new CodeTypeReference(typeof(IQueryable<>).FullName, new CodeTypeReference[1]
            {
        typeParameter
            });
        }

        private static CodeThrowExceptionStatement ThrowArgumentNull(
          string paramName)
        {
            return new CodeThrowExceptionStatement((CodeExpression)CodeGenerationService.New(CodeGenerationService.TypeRef(typeof(ArgumentNullException)), (CodeExpression)CodeGenerationService.StringLiteral(paramName)));
        }

        private static CodeBinaryOperatorExpression Or(
          CodeExpression left,
          CodeExpression right)
        {
            return new CodeBinaryOperatorExpression(left, CodeBinaryOperatorType.BooleanOr, right);
        }

        private static CodeBinaryOperatorExpression Equal(
          CodeExpression left,
          CodeExpression right)
        {
            return new CodeBinaryOperatorExpression(left, CodeBinaryOperatorType.IdentityEquality, right);
        }

        private static CodeBinaryOperatorExpression And(
          CodeExpression left,
          CodeExpression right)
        {
            return new CodeBinaryOperatorExpression(left, CodeBinaryOperatorType.BooleanAnd, right);
        }
    }
}

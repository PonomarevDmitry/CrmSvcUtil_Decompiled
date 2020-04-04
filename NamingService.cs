using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.Crm.Services.Utility
{
    internal sealed class NamingService : INamingService
    {
        private static Regex nameRegex = new Regex("[a-z0-9_]*", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
        private const string ConflictResolutionSuffix = "1";
        private const string ReferencingReflexiveRelationshipPrefix = "Referencing";
        private const string ReferencedReflexiveRelationshipPrefix = "Referenced";
        private string _serviceContextName;
        private Dictionary<string, int> _nameMap;
        private Dictionary<string, string> _knowNames;
        private List<string> _reservedAttributeNames;

        internal NamingService(CrmSvcUtilParameters parameters)
        {
            this._serviceContextName = string.IsNullOrWhiteSpace(parameters.ServiceContextName) ? typeof(OrganizationServiceContext).Name + "1" : parameters.ServiceContextName;
            this._nameMap = new Dictionary<string, int>();
            this._knowNames = new Dictionary<string, string>();
            this._reservedAttributeNames = new List<string>();
            foreach (MemberInfo property in typeof(Entity).GetProperties())
                this._reservedAttributeNames.Add(property.Name);
        }

        string INamingService.GetNameForOptionSet(
          EntityMetadata entityMetadata,
          OptionSetMetadataBase optionSetMetadata,
          IServiceProvider services)
        {
            if (this._knowNames.ContainsKey(optionSetMetadata.MetadataId.Value.ToString()))
                return this._knowNames[optionSetMetadata.MetadataId.Value.ToString()];
            string str = optionSetMetadata.OptionSetType.Value != OptionSetType.State ? this.CreateValidTypeName(optionSetMetadata.Name) : this.CreateValidTypeName(entityMetadata.SchemaName + "State");
            this._knowNames.Add(optionSetMetadata.MetadataId.Value.ToString(), str);
            return str;
        }

        string INamingService.GetNameForOption(
          OptionSetMetadataBase optionSetMetadata,
          OptionMetadata optionMetadata,
          IServiceProvider services)
        {
            if (this._knowNames.ContainsKey(optionSetMetadata.MetadataId.Value.ToString() + optionMetadata.Value.Value.ToString((IFormatProvider)CultureInfo.InvariantCulture)))
                return this._knowNames[optionSetMetadata.MetadataId.Value.ToString() + optionMetadata.Value.Value.ToString((IFormatProvider)CultureInfo.InvariantCulture)];
            string name = string.Empty;
            StateOptionMetadata stateOptionMetadata = optionMetadata as StateOptionMetadata;
            if (stateOptionMetadata != null)
            {
                name = stateOptionMetadata.InvariantName;
            }
            else
            {
                foreach (LocalizedLabel localizedLabel in (Collection<LocalizedLabel>)optionMetadata.Label.LocalizedLabels)
                {
                    if (localizedLabel.LanguageCode == 1033)
                        name = localizedLabel.Label;
                }
            }
            if (string.IsNullOrEmpty(name))
                name = string.Format((IFormatProvider)CultureInfo.InvariantCulture, "UnknownLabel{0}", (object)optionMetadata.Value.Value);
            string validName = NamingService.CreateValidName(name);
            this._knowNames.Add(optionSetMetadata.MetadataId.Value.ToString() + optionMetadata.Value.Value.ToString((IFormatProvider)CultureInfo.InvariantCulture), validName);
            return validName;
        }

        string INamingService.GetNameForEntity(
          EntityMetadata entityMetadata,
          IServiceProvider services)
        {
            if (this._knowNames.ContainsKey(entityMetadata.MetadataId.Value.ToString()))
                return this._knowNames[entityMetadata.MetadataId.Value.ToString()];
            string validTypeName = this.CreateValidTypeName(string.IsNullOrEmpty(StaticNamingService.GetNameForEntity(entityMetadata)) ? entityMetadata.SchemaName : StaticNamingService.GetNameForEntity(entityMetadata));
            this._knowNames.Add(entityMetadata.MetadataId.Value.ToString(), validTypeName);
            return validTypeName;
        }

        string INamingService.GetNameForAttribute(
          EntityMetadata entityMetadata,
          AttributeMetadata attributeMetadata,
          IServiceProvider services)
        {
            Dictionary<string, string> knowNames1 = this._knowNames;
            Guid guid1 = entityMetadata.MetadataId.Value;
            string str1 = guid1.ToString();
            guid1 = attributeMetadata.MetadataId.Value;
            string str2 = guid1.ToString();
            string key1 = str1 + str2;
            if (knowNames1.ContainsKey(key1))
            {
                Dictionary<string, string> knowNames2 = this._knowNames;
                Guid guid2 = entityMetadata.MetadataId.Value;
                string str3 = guid2.ToString();
                guid2 = attributeMetadata.MetadataId.Value;
                string str4 = guid2.ToString();
                string index = str3 + str4;
                return knowNames2[index];
            }
            string validName = NamingService.CreateValidName(StaticNamingService.GetNameForAttribute(attributeMetadata) ?? attributeMetadata.SchemaName);
            INamingService service = (INamingService)services.GetService(typeof(INamingService));
            if (this._reservedAttributeNames.Contains(validName) || validName == service.GetNameForEntity(entityMetadata, services))
                validName += "1";
            Dictionary<string, string> knowNames3 = this._knowNames;
            Guid guid3 = entityMetadata.MetadataId.Value;
            string str5 = guid3.ToString();
            guid3 = attributeMetadata.MetadataId.Value;
            string str6 = guid3.ToString();
            string key2 = str5 + str6;
            string str7 = validName;
            knowNames3.Add(key2, str7);
            return validName;
        }

        string INamingService.GetNameForRelationship(
          EntityMetadata entityMetadata,
          RelationshipMetadataBase relationshipMetadata,
          EntityRole? reflexiveRole,
          IServiceProvider services)
        {
            string str1 = reflexiveRole.HasValue ? reflexiveRole.Value.ToString() : string.Empty;
            Dictionary<string, string> knowNames1 = this._knowNames;
            Guid? metadataId = entityMetadata.MetadataId;
            string str2 = metadataId.Value.ToString();
            metadataId = relationshipMetadata.MetadataId;
            string str3 = metadataId.Value.ToString();
            string str4 = str1;
            string key1 = str2 + str3 + str4;
            if (knowNames1.ContainsKey(key1))
            {
                Dictionary<string, string> knowNames2 = this._knowNames;
                metadataId = entityMetadata.MetadataId;
                Guid guid = metadataId.Value;
                string str5 = guid.ToString();
                metadataId = relationshipMetadata.MetadataId;
                guid = metadataId.Value;
                string str6 = guid.ToString();
                string str7 = str1;
                string index = str5 + str6 + str7;
                return knowNames2[index];
            }
            string validName = NamingService.CreateValidName(!reflexiveRole.HasValue ? relationshipMetadata.SchemaName : (reflexiveRole.Value == EntityRole.Referenced ? "Referenced" + relationshipMetadata.SchemaName : "Referencing" + relationshipMetadata.SchemaName));
            Dictionary<string, string> dictionary = this._knowNames.Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>)(d => d.Key.StartsWith(entityMetadata.MetadataId.Value.ToString()))).ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>)(d => d.Key), (Func<KeyValuePair<string, string>, string>)(d => d.Value));
            INamingService service = (INamingService)services.GetService(typeof(INamingService));
            if (this._reservedAttributeNames.Contains(validName) || validName == service.GetNameForEntity(entityMetadata, services) || dictionary.ContainsValue(validName))
                validName += "1";
            Dictionary<string, string> knowNames3 = this._knowNames;
            metadataId = entityMetadata.MetadataId;
            Guid guid1 = metadataId.Value;
            string str8 = guid1.ToString();
            metadataId = relationshipMetadata.MetadataId;
            guid1 = metadataId.Value;
            string str9 = guid1.ToString();
            string str10 = str1;
            string key2 = str8 + str9 + str10;
            string str11 = validName;
            knowNames3.Add(key2, str11);
            return validName;
        }

        string INamingService.GetNameForServiceContext(IServiceProvider services)
        {
            return this._serviceContextName;
        }

        string INamingService.GetNameForEntitySet(
          EntityMetadata entityMetadata,
          IServiceProvider services)
        {
            return ((INamingService)services.GetService(typeof(INamingService))).GetNameForEntity(entityMetadata, services) + "Set";
        }

        string INamingService.GetNameForMessagePair(
          SdkMessagePair messagePair,
          IServiceProvider services)
        {
            if (this._knowNames.ContainsKey(messagePair.Id.ToString()))
                return this._knowNames[messagePair.Id.ToString()];
            string validTypeName = this.CreateValidTypeName(messagePair.Request.Name);
            this._knowNames.Add(messagePair.Id.ToString(), validTypeName);
            return validTypeName;
        }

        string INamingService.GetNameForRequestField(
          SdkMessageRequest request,
          SdkMessageRequestField requestField,
          IServiceProvider services)
        {
            if (this._knowNames.ContainsKey(request.Id.ToString() + requestField.Index.ToString((IFormatProvider)CultureInfo.InvariantCulture)))
                return this._knowNames[request.Id.ToString() + requestField.Index.ToString((IFormatProvider)CultureInfo.InvariantCulture)];
            string validName = NamingService.CreateValidName(requestField.Name);
            this._knowNames.Add(request.Id.ToString() + requestField.Index.ToString((IFormatProvider)CultureInfo.InvariantCulture), validName);
            return validName;
        }

        string INamingService.GetNameForResponseField(
          SdkMessageResponse response,
          SdkMessageResponseField responseField,
          IServiceProvider services)
        {
            if (this._knowNames.ContainsKey(response.Id.ToString() + responseField.Index.ToString((IFormatProvider)CultureInfo.InvariantCulture)))
                return this._knowNames[response.Id.ToString() + responseField.Index.ToString((IFormatProvider)CultureInfo.InvariantCulture)];
            string validName = NamingService.CreateValidName(responseField.Name);
            this._knowNames.Add(response.Id.ToString() + responseField.Index.ToString((IFormatProvider)CultureInfo.InvariantCulture), validName);
            return validName;
        }

        private string CreateValidTypeName(string name)
        {
            string validName = NamingService.CreateValidName(name);
            if (this._nameMap.ContainsKey(validName))
            {
                int num = ++this._nameMap[validName];
                return string.Format((IFormatProvider)CultureInfo.InvariantCulture, "{0}{1}", (object)validName, (object)num);
            }
            this._nameMap.Add(name, 0);
            return validName;
        }

        private static string CreateValidName(string name)
        {
            string input = name.Replace("$", "CurrencySymbol_").Replace("(", "_");
            StringBuilder stringBuilder = new StringBuilder();
            for (Match match = NamingService.nameRegex.Match(input); match.Success; match = match.NextMatch())
                stringBuilder.Append(match.Value);
            return stringBuilder.ToString();
        }
    }
}
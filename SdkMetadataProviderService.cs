using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.ServiceModel.Description;
using System.Xml.Linq;

namespace Microsoft.Crm.Services.Utility
{
    internal sealed class SdkMetadataProviderService : IMetadataProviderService
    {
        private readonly CrmSvcUtilParameters _parameters;
        private IOrganizationMetadata _organizationMetadata;

        internal SdkMetadataProviderService(CrmSvcUtilParameters parameters)
        {
            this._parameters = parameters;
        }

        private CrmSvcUtilParameters Parameters
        {
            get
            {
                return this._parameters;
            }
        }

        IOrganizationMetadata IMetadataProviderService.LoadMetadata()
        {
            if (this._organizationMetadata == null)
            {
                using (OrganizationServiceProxy organizationServiceEndpoint = this.CreateOrganizationServiceEndpoint())
                {
                    organizationServiceEndpoint.Timeout = new TimeSpan(0, 5, 0);
                    this._organizationMetadata = this.CreateOrganizationMetadata(this.RetrieveEntities((IOrganizationService)organizationServiceEndpoint), this.RetrieveOptionSets((IOrganizationService)organizationServiceEndpoint), this.RetrieveSdkRequests((IOrganizationService)organizationServiceEndpoint));
                }
            }
            return this._organizationMetadata;
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private EntityMetadata[] RetrieveEntities(IOrganizationService service)
        {
            OrganizationRequest request = new OrganizationRequest("RetrieveAllEntities");
            request.Parameters["EntityFilters"] = (object)(EntityFilters.Entity | EntityFilters.Attributes | EntityFilters.Relationships);
            request.Parameters["RetrieveAsIfPublished"] = (object)false;
            return (EntityMetadata[])service.Execute(request).Results["EntityMetadata"];
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private OptionSetMetadataBase[] RetrieveOptionSets(
          IOrganizationService service)
        {
            OrganizationRequest request = new OrganizationRequest("RetrieveAllOptionSets");
            request.Parameters["RetrieveAsIfPublished"] = (object)true;
            return (OptionSetMetadataBase[])service.Execute(request).Results["OptionSetMetadata"];
        }

        private SdkMessages RetrieveSdkRequests(IOrganizationService service)
        {
            string fetchQuery = "<fetch distinct='true' version='1.0'>\r\n\t<entity name='sdkmessage'>\r\n\t\t<attribute name='name'/>\r\n\t\t<attribute name='isprivate'/>\r\n\t\t<attribute name='sdkmessageid'/>\r\n\t\t<attribute name='customizationlevel'/>\r\n\t\t<link-entity name='sdkmessagepair' alias='sdkmessagepair' to='sdkmessageid' from='sdkmessageid' link-type='inner'>\r\n\t\t\t<filter>\r\n\t\t\t\t<condition alias='sdkmessagepair' attribute='endpoint' operator='eq' value='2011/Organization.svc' />\r\n\t\t\t</filter>\r\n\t\t\t<attribute name='sdkmessagepairid'/>\r\n\t\t\t<attribute name='namespace'/>\r\n\t\t\t<link-entity name='sdkmessagerequest' alias='sdkmessagerequest' to='sdkmessagepairid' from='sdkmessagepairid' link-type='outer'>\r\n\t\t\t\t<attribute name='sdkmessagerequestid'/>\r\n\t\t\t\t<attribute name='name'/>\r\n\t\t\t\t<link-entity name='sdkmessagerequestfield' alias='sdkmessagerequestfield' to='sdkmessagerequestid' from='sdkmessagerequestid' link-type='outer'>\r\n\t\t\t\t\t<attribute name='name'/>\r\n\t\t\t\t\t<attribute name='optional'/>\r\n\t\t\t\t\t<attribute name='position'/>\r\n\t\t\t\t\t<attribute name='publicname'/>\r\n\t\t\t\t\t<attribute name='clrparser'/>\r\n\t\t\t\t\t<order attribute='sdkmessagerequestfieldid' descending='false' />\r\n\t\t\t\t</link-entity>\r\n\t\t\t\t<link-entity name='sdkmessageresponse' alias='sdkmessageresponse' to='sdkmessagerequestid' from='sdkmessagerequestid' link-type='outer'>\r\n\t\t\t\t\t<attribute name='sdkmessageresponseid'/>\r\n\t\t\t\t\t<link-entity name='sdkmessageresponsefield' alias='sdkmessageresponsefield' to='sdkmessageresponseid' from='sdkmessageresponseid' link-type='outer'>\r\n\t\t\t\t\t\t<attribute name='publicname'/>\r\n\t\t\t\t\t\t<attribute name='value'/>\r\n\t\t\t\t\t\t<attribute name='clrformatter'/>\r\n\t\t\t\t\t\t<attribute name='name'/>\r\n\t\t\t\t\t\t<attribute name='position' />\r\n\t\t\t\t\t</link-entity>\r\n\t\t\t\t</link-entity>\r\n\t\t\t</link-entity>\r\n\t\t</link-entity>\r\n\t\t<link-entity name='sdkmessagefilter' alias='sdmessagefilter' to='sdkmessageid' from='sdkmessageid' link-type='inner'>\r\n\t\t\t<filter>\r\n\t\t\t\t<condition alias='sdmessagefilter' attribute='isvisible' operator='eq' value='1' />\r\n\t\t\t</filter>\r\n\t\t\t<attribute name='sdkmessagefilterid'/>\r\n\t\t\t<attribute name='primaryobjecttypecode'/>\r\n\t\t\t<attribute name='secondaryobjecttypecode'/>\r\n\t\t</link-entity>\r\n\t\t<order attribute='sdkmessageid' descending='false' />\r\n\t </entity>\r\n</fetch>";
            MessagePagingInfo messagePagingInfo = (MessagePagingInfo)null;
            int pageNumber = 1;
            SdkMessages messages = new SdkMessages((Dictionary<Guid, SdkMessage>)null);
            OrganizationRequest request = new OrganizationRequest("ExecuteFetch");
            while (messagePagingInfo == null || messagePagingInfo.HasMoreRecords)
            {
                string str = fetchQuery;
                if (messagePagingInfo != null)
                    str = this.SetPagingCookie(fetchQuery, messagePagingInfo.PagingCookig, pageNumber);
                request.Parameters["FetchXml"] = (object)str;
                OrganizationResponse organizationResponse = service.Execute(request);
                messagePagingInfo = SdkMessages.FromFetchResult(messages, (string)organizationResponse.Results["FetchXmlResult"]);
                ++pageNumber;
            }
            return messages;
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private string SetPagingCookie(string fetchQuery, string pagingCookie, int pageNumber)
        {
            XDocument xdocument = XDocument.Parse(fetchQuery);
            if (pagingCookie != null)
                xdocument.Root.Add((object)new XAttribute(XName.Get("paging-cookie"), (object)pagingCookie));
            xdocument.Root.Add((object)new XAttribute(XName.Get("page"), (object)pageNumber.ToString((IFormatProvider)CultureInfo.InvariantCulture)));
            return xdocument.ToString();
        }

        private ClientCredentials CreateDeviceCredentials(
          IServiceConfiguration<IOrganizationService> orgServiceConfig)
        {
            if (orgServiceConfig.AuthenticationType != AuthenticationProviderType.LiveId)
                return (ClientCredentials)null;
            if (string.IsNullOrEmpty(this.Parameters.DeviceID) || string.IsNullOrEmpty(this.Parameters.DevicePassword))
                return DeviceIdManager.LoadOrRegisterDevice(CrmSvcUtil.ApplicationId, orgServiceConfig.CurrentIssuer.IssuerAddress.Uri);
            return new ClientCredentials()
            {
                UserName = {
          UserName = this.Parameters.DeviceID,
          Password = this.Parameters.DevicePassword
        }
            };
        }

        private ClientCredentials CreateCredentials(
          IServiceConfiguration<IOrganizationService> orgServiceConfig)
        {
            ClientCredentials clientCredentials = new ClientCredentials();
            if (this.ShouldUseWindowsCredentials(orgServiceConfig))
            {
                clientCredentials.Windows.ClientCredential = new NetworkCredential();
                clientCredentials.Windows.ClientCredential.UserName = SdkMetadataProviderService.GetValueOrDefault(this.Parameters.UserName, (string)null);
                clientCredentials.Windows.ClientCredential.Password = SdkMetadataProviderService.GetValueOrDefault(this.Parameters.Password, string.Empty);
                clientCredentials.Windows.ClientCredential.Domain = SdkMetadataProviderService.GetValueOrDefault(this.Parameters.Domain, (string)null);
            }
            else
            {
                clientCredentials.UserName.UserName = SdkMetadataProviderService.GetValueOrDefault(this.Parameters.UserName, (string)null);
                clientCredentials.UserName.Password = SdkMetadataProviderService.GetValueOrDefault(this.Parameters.Password, (string)null);
            }
            return clientCredentials;
        }

        private bool ShouldUseWindowsCredentials(
          IServiceConfiguration<IOrganizationService> orgServiceConfig)
        {
            return (orgServiceConfig.AuthenticationType != AuthenticationProviderType.OnlineFederation || string.IsNullOrEmpty(this.Parameters.UserName)) && (orgServiceConfig.IssuerEndpoints == null || !orgServiceConfig.IssuerEndpoints.ContainsKey(TokenServiceCredentialType.Username.ToString()));
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private IOrganizationMetadata CreateOrganizationMetadata(
          EntityMetadata[] entityMetadata,
          OptionSetMetadataBase[] optionSetMetadata,
          SdkMessages messages)
        {
            return (IOrganizationMetadata)new OrganizationMetadata(entityMetadata, optionSetMetadata, messages);
        }

        [SuppressMessage("Microsoft.Usage", "CA9888:DisposeObjectsCorrectly", Justification = "Disposing the Proxy makes the channel unusable")]
        private OrganizationServiceProxy CreateOrganizationServiceEndpoint()
        {
            Uri result = (Uri)null;
            if (!Uri.TryCreate(this.Parameters.Url, UriKind.RelativeOrAbsolute, out result))
                throw new InvalidOperationException(string.Format((IFormatProvider)CultureInfo.InvariantCulture, "Cannot connect to organization service at {0}", (object)this.Parameters.Url));
            IServiceConfiguration<IOrganizationService> configuration = ServiceConfigurationFactory.CreateConfiguration<IOrganizationService>(result);
            ClientCredentials credentials = this.CreateCredentials(configuration);
            ClientCredentials deviceCredentials = this.CreateDeviceCredentials(configuration);
            return new OrganizationServiceProxy(result, (Uri)null, credentials, deviceCredentials);
        }

        private static string GetValueOrDefault(string value, string defaultValue)
        {
            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;
            return value;
        }
    }
}
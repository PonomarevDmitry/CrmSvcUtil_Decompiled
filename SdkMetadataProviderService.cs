using CrmSvcUtil.InteractiveLogin;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Text;
using System.Xml.Linq;

namespace Microsoft.Crm.Services.Utility
{
    internal sealed class SdkMetadataProviderService : IMetadataProviderService2, IMetadataProviderService
    {
        private readonly CrmSvcUtilParameters _parameters;
        private IOrganizationMetadata _organizationMetadata;
        private CrmServiceClient crmSvcCli;

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

        public IOrganizationMetadata LoadMetadata()
        {
            if (this._organizationMetadata == null)
            {
                IOrganizationService organizationServiceEndpoint = this.CreateOrganizationServiceEndpoint();
                if (organizationServiceEndpoint == null)
                    throw new Exception("Connection to CRM is not established. Aborting process.");
                this.SetConnectionTimeoutValues();
                this._organizationMetadata = this.CreateOrganizationMetadata(this.RetrieveEntities(organizationServiceEndpoint), this.RetrieveOptionSets(organizationServiceEndpoint), this.RetrieveSdkRequests(organizationServiceEndpoint));
            }
            return this._organizationMetadata;
        }

        public IOrganizationMetadata LoadMetadata(IServiceProvider service)
        {
            if (this._organizationMetadata == null)
            {
                if (service == null)
                    return (IOrganizationMetadata)null;
                ServiceProvider serviceProvider = (ServiceProvider)service;
                IOrganizationService organizationServiceEndpoint = this.CreateOrganizationServiceEndpoint();
                if (organizationServiceEndpoint == null)
                    throw new Exception("Connection to CRM is not established. Aborting process.");
                this.SetConnectionTimeoutValues();
                this._organizationMetadata = this.CreateOrganizationMetadata(serviceProvider.MetadataProviderQueryServcie.RetrieveEntities(organizationServiceEndpoint), serviceProvider.MetadataProviderQueryServcie.RetrieveOptionSets(organizationServiceEndpoint), serviceProvider.MetadataProviderQueryServcie.RetrieveSdkRequests(organizationServiceEndpoint));
            }
            return this._organizationMetadata;
        }

        /// <summary>
        /// Updates the timeout value to extend the amount of item that a request will wait.
        /// </summary>
        private void SetConnectionTimeoutValues()
        {
            if (this.crmSvcCli == null)
                return;
            if (this.crmSvcCli.ActiveAuthenticationType == Microsoft.Xrm.Tooling.Connector.AuthenticationType.OAuth)
            {
                if (this.crmSvcCli.OrganizationWebProxyClient == null)
                    return;
                this.crmSvcCli.OrganizationWebProxyClient.InnerChannel.OperationTimeout = TimeSpan.FromMinutes(20.0);
                this.crmSvcCli.OrganizationWebProxyClient.Endpoint.Binding.SendTimeout = TimeSpan.FromMinutes(20.0);
                this.crmSvcCli.OrganizationWebProxyClient.Endpoint.Binding.ReceiveTimeout = TimeSpan.FromMinutes(20.0);
            }
            else
            {
                if (this.crmSvcCli.OrganizationServiceProxy == null)
                    return;
                this.crmSvcCli.OrganizationServiceProxy.Timeout = TimeSpan.FromMinutes(20.0);
            }
        }

        private EntityMetadata[] RetrieveEntities(IOrganizationService service)
        {
            OrganizationRequest request = new OrganizationRequest("RetrieveAllEntities");
            request.Parameters["EntityFilters"] = (object)(EntityFilters.Entity | EntityFilters.Attributes | EntityFilters.Relationships);
            request.Parameters["RetrieveAsIfPublished"] = (object)false;
            return (EntityMetadata[])service.Execute(request).Results["EntityMetadata"];
        }

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

        private string SetPagingCookie(string fetchQuery, string pagingCookie, int pageNumber)
        {
            XDocument xdocument = XDocument.Parse(fetchQuery);
            if (pagingCookie != null)
                xdocument.Root.Add((object)new XAttribute(XName.Get("paging-cookie"), (object)pagingCookie));
            xdocument.Root.Add((object)new XAttribute(XName.Get("page"), (object)pageNumber.ToString((IFormatProvider)CultureInfo.InvariantCulture)));
            return xdocument.ToString();
        }

        private IOrganizationMetadata CreateOrganizationMetadata(
          EntityMetadata[] entityMetadata,
          OptionSetMetadataBase[] optionSetMetadata,
          SdkMessages messages)
        {
            return (IOrganizationMetadata)new OrganizationMetadata(entityMetadata, optionSetMetadata, messages);
        }

        private IOrganizationService CreateOrganizationServiceEndpoint()
        {
            string str = string.IsNullOrEmpty(this.Parameters.ConnectionProfileName) ? "default" : this.Parameters.ConnectionProfileName;
            if (!string.IsNullOrEmpty(this.Parameters.ConnectionAppName))
            {
                CRMInteractiveLogin interactiveLogin = new CRMInteractiveLogin();
                interactiveLogin.HostProfileName = str;
                interactiveLogin.HostApplicatioNameOveride = this.Parameters.ConnectionAppName;
                interactiveLogin.ShowDialog();
                if (interactiveLogin.CrmConnectionMgr != null && interactiveLogin.CrmConnectionMgr.CrmSvc != null && interactiveLogin.CrmConnectionMgr.CrmSvc.IsReady)
                {
                    this.crmSvcCli = interactiveLogin.CrmConnectionMgr.CrmSvc;
                    if (this.crmSvcCli.OrganizationServiceProxy == null)
                        return (IOrganizationService)this.crmSvcCli.OrganizationWebProxyClient;
                    return (IOrganizationService)this.crmSvcCli.OrganizationServiceProxy;
                }
                Console.WriteLine(this.crmSvcCli.LastCrmError);
                return (IOrganizationService)null;
            }
            if (!this.Parameters.UseInteractiveLogin)
            {
                this.crmSvcCli = new CrmServiceClient(this.GetConnectionString());
                if (this.crmSvcCli != null && this.crmSvcCli.IsReady)
                {
                    if (this.crmSvcCli.OrganizationServiceProxy == null)
                        return (IOrganizationService)this.crmSvcCli.OrganizationWebProxyClient;
                    return (IOrganizationService)this.crmSvcCli.OrganizationServiceProxy;
                }
                Console.WriteLine(this.crmSvcCli.LastCrmError);
                return (IOrganizationService)null;
            }
            CRMInteractiveLogin interactiveLogin1 = new CRMInteractiveLogin();
            interactiveLogin1.ForceDirectLogin = true;
            interactiveLogin1.HostProfileName = str;
            if (!string.IsNullOrEmpty(this.Parameters.ConnectionAppName))
                interactiveLogin1.HostApplicatioNameOveride = this.Parameters.ConnectionAppName;
            if (!interactiveLogin1.ShowDialog().Value)
            {
                Console.WriteLine("User aborted Login");
                return (IOrganizationService)null;
            }
            try
            {
                if (interactiveLogin1.CrmConnectionMgr != null && interactiveLogin1.CrmConnectionMgr.CrmSvc != null && interactiveLogin1.CrmConnectionMgr.CrmSvc.IsReady)
                {
                    this.crmSvcCli = interactiveLogin1.CrmConnectionMgr.CrmSvc;
                    return this.crmSvcCli.OrganizationServiceProxy != null ? (IOrganizationService)this.crmSvcCli.OrganizationServiceProxy : (IOrganizationService)this.crmSvcCli.OrganizationWebProxyClient;
                }
                Console.WriteLine(this.crmSvcCli.LastCrmError);
                return (IOrganizationService)null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to Login:");
                Console.WriteLine(ex.Message);
                return (IOrganizationService)null;
            }
        }

        private static string GetValueOrDefault(string value, string defaultValue)
        {
            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;
            return value;
        }

        /// <summary>
        /// Builds a connection string from the passed in parameters.
        /// </summary>
        /// <returns></returns>
        private string GetConnectionString()
        {
            if (!string.IsNullOrEmpty(this.Parameters.ConnectionString))
                return this.Parameters.ConnectionString;
            Uri result = (Uri)null;
            if (!Uri.TryCreate(this.Parameters.Url, UriKind.RelativeOrAbsolute, out result))
                throw new InvalidOperationException(string.Format((IFormatProvider)CultureInfo.InvariantCulture, "Cannot connect to organization service at {0}", (object)this.Parameters.Url));
            StringBuilder builder = new StringBuilder();
            DbConnectionStringBuilder.AppendKeyValuePair(builder, "Server", result.ToString());
            DbConnectionStringBuilder.AppendKeyValuePair(builder, "UserName", this.Parameters.UserName);
            DbConnectionStringBuilder.AppendKeyValuePair(builder, "Password", this.Parameters.Password);
            if (!string.IsNullOrEmpty(this.Parameters.Domain))
            {
                DbConnectionStringBuilder.AppendKeyValuePair(builder, "Domain", this.Parameters.Domain);
                DbConnectionStringBuilder.AppendKeyValuePair(builder, "AuthType", "AD");
            }
            else if (this.Parameters.UseOAuth)
            {
                DbConnectionStringBuilder.AppendKeyValuePair(builder, "AuthType", "OAuth");
                DbConnectionStringBuilder.AppendKeyValuePair(builder, "ClientId", "2ad88395-b77d-4561-9441-d0e40824f9bc");
                DbConnectionStringBuilder.AppendKeyValuePair(builder, "RedirectUri", "app://5d3e90d6-aa8e-48a8-8f2c-58b45cc67315");
                DbConnectionStringBuilder.AppendKeyValuePair(builder, "LoginPrompt", "Never");
            }
            else
                DbConnectionStringBuilder.AppendKeyValuePair(builder, "AuthType", "Office365");
            return builder.ToString();
        }
    }
}
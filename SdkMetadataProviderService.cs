// Decompiled with JetBrains decompiler
// Type: Microsoft.Crm.Services.Utility.SdkMetadataProviderService
// Assembly: CrmSvcUtil, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 756175FA-E97D-49FF-B443-B7A725C9F163
// Assembly location: C:\Users\dmitriy.ponomarev\AppData\Roaming\MscrmTools\XrmToolBox\NugetPlugins\DLaB.Xrm.EarlyBoundGenerator.1.2019.3.19\lib\net462\plugins\DLaB.EarlyBoundGenerator\CrmSvcUtil.exe

using CrmSvcUtil.InteractiveLogin;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Tooling.Connector;
using System;
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

        private CrmSvcUtilParameters Parameters => this._parameters;

        public IOrganizationMetadata LoadMetadata()
        {
            if (this._organizationMetadata == null)
            {
                IOrganizationService organizationServiceEndpoint = this.CreateOrganizationServiceEndpoint();
                if (organizationServiceEndpoint == null)
                {
                    throw new Exception("Connection to CRM is not established. Aborting process.");
                }

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
                {
                    return null;
                }

                ServiceProvider serviceProvider = (ServiceProvider)service;
                IOrganizationService organizationServiceEndpoint = this.CreateOrganizationServiceEndpoint();
                if (organizationServiceEndpoint == null)
                {
                    throw new Exception("Connection to CRM is not established. Aborting process.");
                }

                this.SetConnectionTimeoutValues();
                this._organizationMetadata = this.CreateOrganizationMetadata(serviceProvider.MetadataProviderQueryServcie.RetrieveEntities(organizationServiceEndpoint), serviceProvider.MetadataProviderQueryServcie.RetrieveOptionSets(organizationServiceEndpoint), serviceProvider.MetadataProviderQueryServcie.RetrieveSdkRequests(organizationServiceEndpoint));
            }
            return this._organizationMetadata;
        }

        private void SetConnectionTimeoutValues()
        {
            if (this.crmSvcCli == null)
            {
                return;
            }

            if (this.crmSvcCli.ActiveAuthenticationType == Microsoft.Xrm.Tooling.Connector.AuthenticationType.OAuth)
            {
                if (this.crmSvcCli.OrganizationWebProxyClient == null)
                {
                    return;
                }

                this.crmSvcCli.OrganizationWebProxyClient.InnerChannel.OperationTimeout = TimeSpan.FromMinutes(20.0);
                this.crmSvcCli.OrganizationWebProxyClient.Endpoint.Binding.SendTimeout = TimeSpan.FromMinutes(20.0);
                this.crmSvcCli.OrganizationWebProxyClient.Endpoint.Binding.ReceiveTimeout = TimeSpan.FromMinutes(20.0);
            }
            else
            {
                if (this.crmSvcCli.OrganizationServiceProxy == null)
                {
                    return;
                }

                this.crmSvcCli.OrganizationServiceProxy.Timeout = TimeSpan.FromMinutes(20.0);
            }
        }

        private EntityMetadata[] RetrieveEntities(IOrganizationService service)
        {
            OrganizationRequest request = new OrganizationRequest("RetrieveAllEntities");
            request.Parameters["EntityFilters"] = EntityFilters.Entity | EntityFilters.Attributes | EntityFilters.Relationships;
            request.Parameters["RetrieveAsIfPublished"] = false;
            return (EntityMetadata[])service.Execute(request).Results["EntityMetadata"];
        }

        private OptionSetMetadataBase[] RetrieveOptionSets(
          IOrganizationService service)
        {
            OrganizationRequest request = new OrganizationRequest("RetrieveAllOptionSets");
            request.Parameters["RetrieveAsIfPublished"] = true;
            return (OptionSetMetadataBase[])service.Execute(request).Results["OptionSetMetadata"];
        }

        private SdkMessages RetrieveSdkRequests(IOrganizationService service)
        {
            string fetchQuery = @"<fetch distinct='true' version='1.0'>
    <entity name='sdkmessage'>
        <attribute name='name'/>
        <attribute name='isprivate'/>
        <attribute name='sdkmessageid'/>
        <attribute name='customizationlevel'/>

        <link-entity name='sdkmessagepair' alias='sdkmessagepair' to='sdkmessageid' from='sdkmessageid' link-type='inner'>
            <filter>
                <condition alias='sdkmessagepair' attribute='endpoint' operator='eq' value='2011/Organization.svc' />
            </filter>
            <attribute name='sdkmessagepairid'/>
            <attribute name='namespace'/>
            <link-entity name='sdkmessagerequest' alias='sdkmessagerequest' to='sdkmessagepairid' from='sdkmessagepairid' link-type='outer'>
                <attribute name='sdkmessagerequestid'/>
                <attribute name='name'/>
                <link-entity name='sdkmessagerequestfield' alias='sdkmessagerequestfield' to='sdkmessagerequestid' from='sdkmessagerequestid' link-type='outer'>
                    <attribute name='name'/>
                    <attribute name='optional'/>
                    <attribute name='position'/>
                    <attribute name='publicname'/>
                    <attribute name='clrparser'/>
                    <order attribute='sdkmessagerequestfieldid' descending='false' />
                </link-entity>
                <link-entity name='sdkmessageresponse' alias='sdkmessageresponse' to='sdkmessagerequestid' from='sdkmessagerequestid' link-type='outer'>
                    <attribute name='sdkmessageresponseid'/>
                    <link-entity name='sdkmessageresponsefield' alias='sdkmessageresponsefield' to='sdkmessageresponseid' from='sdkmessageresponseid' link-type='outer'>
                        <attribute name='publicname'/>
                        <attribute name='value'/>
                        <attribute name='clrformatter'/>
                        <attribute name='name'/>
                        <attribute name='position' />
                    </link-entity>
                </link-entity>
            </link-entity>
        </link-entity>

        <link-entity name='sdkmessagefilter' alias='sdmessagefilter' to='sdkmessageid' from='sdkmessageid' link-type='inner'>
            <filter>
                <condition alias='sdmessagefilter' attribute='isvisible' operator='eq' value='1' />
            </filter>
            <attribute name='sdkmessagefilterid'/>
            <attribute name='primaryobjecttypecode'/>
            <attribute name='secondaryobjecttypecode'/>
        </link-entity>

        <order attribute='sdkmessageid' descending='false' />
    </entity>
</fetch>
";

            MessagePagingInfo messagePagingInfo = null;
            int pageNumber = 1;
            SdkMessages messages = new SdkMessages(null);
            OrganizationRequest request = new OrganizationRequest("ExecuteFetch");
            while (messagePagingInfo == null || messagePagingInfo.HasMoreRecords)
            {
                string str = fetchQuery;
                if (messagePagingInfo != null)
                {
                    str = this.SetPagingCookie(fetchQuery, messagePagingInfo.PagingCookig, pageNumber);
                }

                request.Parameters["FetchXml"] = str;
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
            {
                xdocument.Root.Add(new XAttribute(XName.Get("paging-cookie"), pagingCookie));
            }

            xdocument.Root.Add(new XAttribute(XName.Get("page"), pageNumber.ToString(CultureInfo.InvariantCulture)));
            return xdocument.ToString();
        }

        private IOrganizationMetadata CreateOrganizationMetadata(
          EntityMetadata[] entityMetadata,
          OptionSetMetadataBase[] optionSetMetadata,
          SdkMessages messages)
        {
            return new OrganizationMetadata(entityMetadata, optionSetMetadata, messages);
        }

        private IOrganizationService CreateOrganizationServiceEndpoint()
        {
            string str = string.IsNullOrEmpty(this.Parameters.ConnectionProfileName) ? "default" : this.Parameters.ConnectionProfileName;
            if (!string.IsNullOrEmpty(this.Parameters.ConnectionAppName))
            {
                CRMInteractiveLogin interactiveLogin = new CRMInteractiveLogin
                {
                    HostProfileName = str,
                    HostApplicatioNameOveride = this.Parameters.ConnectionAppName
                };
                interactiveLogin.ShowDialog();
                if (interactiveLogin.CrmConnectionMgr != null && interactiveLogin.CrmConnectionMgr.CrmSvc != null && interactiveLogin.CrmConnectionMgr.CrmSvc.IsReady)
                {
                    this.crmSvcCli = interactiveLogin.CrmConnectionMgr.CrmSvc;
                    if (this.crmSvcCli.OrganizationServiceProxy == null)
                    {
                        return this.crmSvcCli.OrganizationWebProxyClient;
                    }

                    return this.crmSvcCli.OrganizationServiceProxy;
                }
                Console.WriteLine(this.crmSvcCli.LastCrmError);
                return null;
            }
            if (!this.Parameters.UseInteractiveLogin)
            {
                this.crmSvcCli = new CrmServiceClient(this.GetConnectionString());
                if (this.crmSvcCli != null && this.crmSvcCli.IsReady)
                {
                    if (this.crmSvcCli.OrganizationServiceProxy == null)
                    {
                        return this.crmSvcCli.OrganizationWebProxyClient;
                    }

                    return this.crmSvcCli.OrganizationServiceProxy;
                }
                Console.WriteLine(this.crmSvcCli.LastCrmError);
                return null;
            }
            CRMInteractiveLogin interactiveLogin1 = new CRMInteractiveLogin
            {
                ForceDirectLogin = true,
                HostProfileName = str
            };
            if (!string.IsNullOrEmpty(this.Parameters.ConnectionAppName))
            {
                interactiveLogin1.HostApplicatioNameOveride = this.Parameters.ConnectionAppName;
            }

            if (!interactiveLogin1.ShowDialog().Value)
            {
                Console.WriteLine("User aborted Login");
                return null;
            }
            try
            {
                if (interactiveLogin1.CrmConnectionMgr != null && interactiveLogin1.CrmConnectionMgr.CrmSvc != null && interactiveLogin1.CrmConnectionMgr.CrmSvc.IsReady)
                {
                    this.crmSvcCli = interactiveLogin1.CrmConnectionMgr.CrmSvc;
                    return this.crmSvcCli.OrganizationServiceProxy != null ? this.crmSvcCli.OrganizationServiceProxy : (IOrganizationService)this.crmSvcCli.OrganizationWebProxyClient;
                }
                Console.WriteLine(this.crmSvcCli.LastCrmError);
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to Login:");
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        private static string GetValueOrDefault(string value, string defaultValue)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return defaultValue;
            }

            return value;
        }

        private string GetConnectionString()
        {
            if (!string.IsNullOrEmpty(this.Parameters.ConnectionString))
            {
                return this.Parameters.ConnectionString;
            }

            if (!Uri.TryCreate(this.Parameters.Url, UriKind.RelativeOrAbsolute, out Uri result))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Cannot connect to organization service at {0}", this.Parameters.Url));
            }

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
            {
                DbConnectionStringBuilder.AppendKeyValuePair(builder, "AuthType", "Office365");
            }

            return builder.ToString();
        }
    }
}

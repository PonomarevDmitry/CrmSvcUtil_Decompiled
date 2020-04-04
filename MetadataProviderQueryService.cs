using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;

namespace Microsoft.Crm.Services.Utility
{
    internal sealed class MetadataProviderQueryService : IMetadataProviderQueryService
    {
        private readonly CrmSvcUtilParameters _parameters;

        internal MetadataProviderQueryService(CrmSvcUtilParameters parameters)
        {
            this._parameters = parameters;
            CrmSvcUtil.crmSvcUtilLogger.Log("Creating Default Metadata Provider Query Service");
        }

        public EntityMetadata[] RetrieveEntities(IOrganizationService service)
        {
            OrganizationRequest request = new OrganizationRequest("RetrieveAllEntities");
            request.Parameters["EntityFilters"] = (object)(EntityFilters.Entity | EntityFilters.Attributes | EntityFilters.Relationships);
            request.Parameters["RetrieveAsIfPublished"] = (object)false;
            return (EntityMetadata[])service.Execute(request).Results["EntityMetadata"];
        }

        public OptionSetMetadataBase[] RetrieveOptionSets(
          IOrganizationService service)
        {
            OrganizationRequest request = new OrganizationRequest("RetrieveAllOptionSets");
            request.Parameters["RetrieveAsIfPublished"] = (object)true;
            return (OptionSetMetadataBase[])service.Execute(request).Results["OptionSetMetadata"];
        }

        public SdkMessages RetrieveSdkRequests(IOrganizationService service)
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
    }
}
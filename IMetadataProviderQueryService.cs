// Decompiled with JetBrains decompiler
// Type: Microsoft.Crm.Services.Utility.IMetadataProviderQueryService
// Assembly: CrmSvcUtil, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 756175FA-E97D-49FF-B443-B7A725C9F163
// Assembly location: C:\Users\dmitriy.ponomarev\AppData\Roaming\MscrmTools\XrmToolBox\NugetPlugins\DLaB.Xrm.EarlyBoundGenerator.1.2019.3.19\lib\net462\plugins\DLaB.EarlyBoundGenerator\CrmSvcUtil.exe

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace Microsoft.Crm.Services.Utility
{
  public interface IMetadataProviderQueryService
  {
    EntityMetadata[] RetrieveEntities(IOrganizationService service);

    OptionSetMetadataBase[] RetrieveOptionSets(IOrganizationService service);

    SdkMessages RetrieveSdkRequests(IOrganizationService service);
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.Crm.Services.Utility.OrganizationMetadata
// Assembly: CrmSvcUtil, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 756175FA-E97D-49FF-B443-B7A725C9F163
// Assembly location: C:\Users\dmitriy.ponomarev\AppData\Roaming\MscrmTools\XrmToolBox\NugetPlugins\DLaB.Xrm.EarlyBoundGenerator.1.2019.3.19\lib\net462\plugins\DLaB.EarlyBoundGenerator\CrmSvcUtil.exe

using Microsoft.Xrm.Sdk.Metadata;
using System.Reflection;

namespace Microsoft.Crm.Services.Utility
{
  internal sealed class OrganizationMetadata : IOrganizationMetadata
  {
    private readonly EntityMetadata[] _entities;
    private readonly OptionSetMetadataBase[] _optionSets;
    private readonly SdkMessages _sdkMessages;

    internal OrganizationMetadata(
      EntityMetadata[] entities,
      OptionSetMetadataBase[] optionSets,
      SdkMessages messages)
    {
      CrmSvcUtil.crmSvcUtilLogger.TraceMethodStart("Entering {0}", (object) MethodBase.GetCurrentMethod().Name);
      this._entities = entities;
      this._optionSets = optionSets;
      this._sdkMessages = messages;
      CrmSvcUtil.crmSvcUtilLogger.TraceMethodStop("Exiting {0}", (object) MethodBase.GetCurrentMethod().Name);
    }

    EntityMetadata[] IOrganizationMetadata.Entities
    {
      get
      {
        return this._entities;
      }
    }

    OptionSetMetadataBase[] IOrganizationMetadata.OptionSets
    {
      get
      {
        return this._optionSets;
      }
    }

    SdkMessages IOrganizationMetadata.Messages
    {
      get
      {
        return this._sdkMessages;
      }
    }
  }
}

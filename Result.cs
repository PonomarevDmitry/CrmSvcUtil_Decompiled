// Decompiled with JetBrains decompiler
// Type: Microsoft.Crm.Services.Utility.Result
// Assembly: CrmSvcUtil, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 756175FA-E97D-49FF-B443-B7A725C9F163
// Assembly location: C:\Users\dmitriy.ponomarev\AppData\Roaming\MscrmTools\XrmToolBox\NugetPlugins\DLaB.Xrm.EarlyBoundGenerator.1.2019.3.19\lib\net462\plugins\DLaB.EarlyBoundGenerator\CrmSvcUtil.exe

using System;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Crm.Services.Utility
{
  [XmlType]
  [Serializable]
  public sealed class Result
  {
    private string _nameField;
    private bool _isPrivateField;
    private byte _customizationLevel;
    private Guid _sdkMessageIdField;
    private Guid _sdkMessageRequestIdField;
    private Guid _sdkMessagePairIdField;
    private string _sdkMessagePairNamespace;
    private Guid _sdkMessageFilterIdField;
    private string _sdkMessageRequestNameField;
    private string _sdkMessageRequestFieldNameField;
    private bool _sdkMessageRequestFieldIsOptionalField;
    private string _sdkMessageRequestFieldParserField;
    private string _sdkMessageRequestFieldCLRParserField;
    private Guid _sdkMessageResponseIdField;
    private string _sdkMessageResponseFieldValueField;
    private string _sdkMessageResponseFieldFormatterField;
    private string _sdkMessageResponseFieldCLRFormatterField;
    private string _sdkMessageResponseFieldNameField;
    private int? _sdkMessageRequestFieldPositionField;
    private int? _sdkMessageResponseFieldPositionField;
    private int _sdkMessageFilterPrimaryOTCField;
    private int _sdkMessageFilterSecondaryOTCField;

    [XmlElement(ElementName = "name", Form = XmlSchemaForm.Unqualified)]
    public string Name
    {
      get
      {
        return this._nameField;
      }
      set
      {
        this._nameField = value;
      }
    }

    [XmlElement(ElementName = "isprivate", Form = XmlSchemaForm.Unqualified)]
    public bool IsPrivate
    {
      get
      {
        return this._isPrivateField;
      }
      set
      {
        this._isPrivateField = value;
      }
    }

    [XmlElement(ElementName = "customizationlevel", Form = XmlSchemaForm.Unqualified)]
    public byte CustomizationLevel
    {
      get
      {
        return this._customizationLevel;
      }
      set
      {
        this._customizationLevel = value;
      }
    }

    [XmlElement(ElementName = "sdkmessageid", Form = XmlSchemaForm.Unqualified)]
    public Guid SdkMessageId
    {
      get
      {
        return this._sdkMessageIdField;
      }
      set
      {
        this._sdkMessageIdField = value;
      }
    }

    [XmlElement("sdkmessagepair.sdkmessagepairid", Form = XmlSchemaForm.Unqualified)]
    public Guid SdkMessagePairId
    {
      get
      {
        return this._sdkMessagePairIdField;
      }
      set
      {
        this._sdkMessagePairIdField = value;
      }
    }

    [XmlElement("sdkmessagepair.namespace", Form = XmlSchemaForm.Unqualified)]
    public string SdkMessagePairNamespace
    {
      get
      {
        return this._sdkMessagePairNamespace;
      }
      set
      {
        this._sdkMessagePairNamespace = value;
      }
    }

    [XmlElement("sdkmessagerequest.sdkmessagerequestid", Form = XmlSchemaForm.Unqualified)]
    public Guid SdkMessageRequestId
    {
      get
      {
        return this._sdkMessageRequestIdField;
      }
      set
      {
        this._sdkMessageRequestIdField = value;
      }
    }

    [XmlElement("sdkmessagerequest.name", Form = XmlSchemaForm.Unqualified)]
    public string SdkMessageRequestName
    {
      get
      {
        return this._sdkMessageRequestNameField;
      }
      set
      {
        this._sdkMessageRequestNameField = value;
      }
    }

    [XmlElement("sdkmessagerequestfield.name", Form = XmlSchemaForm.Unqualified)]
    public string SdkMessageRequestFieldName
    {
      get
      {
        return this._sdkMessageRequestFieldNameField;
      }
      set
      {
        this._sdkMessageRequestFieldNameField = value;
      }
    }

    [XmlElement("sdkmessagerequestfield.optional", Form = XmlSchemaForm.Unqualified)]
    public bool SdkMessageRequestFieldIsOptional
    {
      get
      {
        return this._sdkMessageRequestFieldIsOptionalField;
      }
      set
      {
        this._sdkMessageRequestFieldIsOptionalField = value;
      }
    }

    [XmlElement("sdkmessagerequestfield.parser", Form = XmlSchemaForm.Unqualified)]
    public string SdkMessageRequestFieldParser
    {
      get
      {
        return this._sdkMessageRequestFieldParserField;
      }
      set
      {
        this._sdkMessageRequestFieldParserField = value;
      }
    }

    [XmlElement("sdkmessagerequestfield.clrparser", Form = XmlSchemaForm.Unqualified)]
    public string SdkMessageRequestFieldClrParser
    {
      get
      {
        return this._sdkMessageRequestFieldCLRParserField;
      }
      set
      {
        this._sdkMessageRequestFieldCLRParserField = value;
      }
    }

    [XmlElement("sdkmessageresponse.sdkmessageresponseid", Form = XmlSchemaForm.Unqualified)]
    public Guid SdkMessageResponseId
    {
      get
      {
        return this._sdkMessageResponseIdField;
      }
      set
      {
        this._sdkMessageResponseIdField = value;
      }
    }

    [XmlElement("sdkmessageresponsefield.value", Form = XmlSchemaForm.Unqualified)]
    public string SdkMessageResponseFieldValue
    {
      get
      {
        return this._sdkMessageResponseFieldValueField;
      }
      set
      {
        this._sdkMessageResponseFieldValueField = value;
      }
    }

    [XmlElement("sdkmessageresponsefield.formatter", Form = XmlSchemaForm.Unqualified)]
    public string SdkMessageResponseFieldFormatter
    {
      get
      {
        return this._sdkMessageResponseFieldFormatterField;
      }
      set
      {
        this._sdkMessageResponseFieldFormatterField = value;
      }
    }

    [XmlElement("sdkmessageresponsefield.clrformatter", Form = XmlSchemaForm.Unqualified)]
    public string SdkMessageResponseFieldClrFormatter
    {
      get
      {
        return this._sdkMessageResponseFieldCLRFormatterField;
      }
      set
      {
        this._sdkMessageResponseFieldCLRFormatterField = value;
      }
    }

    [XmlElement("sdkmessageresponsefield.name", Form = XmlSchemaForm.Unqualified)]
    public string SdkMessageResponseFieldName
    {
      get
      {
        return this._sdkMessageResponseFieldNameField;
      }
      set
      {
        this._sdkMessageResponseFieldNameField = value;
      }
    }

    [XmlElement("sdkmessagerequestfield.position", Form = XmlSchemaForm.Unqualified, IsNullable = true)]
    public int? SdkMessageRequestFieldPosition
    {
      get
      {
        return this._sdkMessageRequestFieldPositionField;
      }
      set
      {
        this._sdkMessageRequestFieldPositionField = value;
      }
    }

    [XmlElement("sdkmessageresponsefield.position", Form = XmlSchemaForm.Unqualified, IsNullable = true)]
    public int? SdkMessageResponseFieldPosition
    {
      get
      {
        return this._sdkMessageResponseFieldPositionField;
      }
      set
      {
        this._sdkMessageResponseFieldPositionField = value;
      }
    }

    [XmlElement("sdmessagefilter.sdkmessagefilterid", Form = XmlSchemaForm.Unqualified)]
    public Guid SdkMessageFilterId
    {
      get
      {
        return this._sdkMessageFilterIdField;
      }
      set
      {
        this._sdkMessageFilterIdField = value;
      }
    }

    [XmlElement("sdmessagefilter.primaryobjecttypecode", Form = XmlSchemaForm.Unqualified)]
    public int SdkMessagePrimaryOTCFilter
    {
      get
      {
        return this._sdkMessageFilterPrimaryOTCField;
      }
      set
      {
        this._sdkMessageFilterPrimaryOTCField = value;
      }
    }

    [XmlElement("sdmessagefilter.secondaryobjecttypecode", Form = XmlSchemaForm.Unqualified)]
    public int SdkMessageSecondaryOTCFilter
    {
      get
      {
        return this._sdkMessageFilterSecondaryOTCField;
      }
      set
      {
        this._sdkMessageFilterSecondaryOTCField = value;
      }
    }
  }
}

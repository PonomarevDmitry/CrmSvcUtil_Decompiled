// Decompiled with JetBrains decompiler
// Type: Microsoft.Crm.Services.Utility.ResultSet
// Assembly: CrmSvcUtil, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 756175FA-E97D-49FF-B443-B7A725C9F163
// Assembly location: C:\Users\dmitriy.ponomarev\AppData\Roaming\MscrmTools\XrmToolBox\NugetPlugins\DLaB.Xrm.EarlyBoundGenerator.1.2019.3.19\lib\net462\plugins\DLaB.EarlyBoundGenerator\CrmSvcUtil.exe

using System;
using System.Xml.Serialization;

namespace Microsoft.Crm.Services.Utility
{
  [XmlRoot(ElementName = "resultset", Namespace = "")]
  [XmlType(Namespace = "")]
  [Serializable]
  public sealed class ResultSet
  {
    private Result[] _results;
    private string _pagingCookie;
    private int _moreRecords;

    [XmlElement("result")]
    public Result[] Results
    {
      get
      {
        return this._results;
      }
      set
      {
        this._results = value;
      }
    }

    [XmlAttribute("paging-cookie")]
    public string PagingCookie
    {
      get
      {
        return this._pagingCookie;
      }
      set
      {
        this._pagingCookie = value;
      }
    }

    [XmlAttribute("morerecords")]
    public int MoreRecords
    {
      get
      {
        return this._moreRecords;
      }
      set
      {
        this._moreRecords = value;
      }
    }
  }
}

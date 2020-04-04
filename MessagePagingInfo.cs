﻿// Decompiled with JetBrains decompiler
// Type: Microsoft.Crm.Services.Utility.MessagePagingInfo
// Assembly: CrmSvcUtil, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 756175FA-E97D-49FF-B443-B7A725C9F163
// Assembly location: C:\Users\dmitriy.ponomarev\AppData\Roaming\MscrmTools\XrmToolBox\NugetPlugins\DLaB.Xrm.EarlyBoundGenerator.1.2019.3.19\lib\net462\plugins\DLaB.EarlyBoundGenerator\CrmSvcUtil.exe

using System;
using System.Globalization;

namespace Microsoft.Crm.Services.Utility
{
  public sealed class MessagePagingInfo
  {
    public string PagingCookig { get; set; }

    public bool HasMoreRecords { get; set; }

    public static MessagePagingInfo FromResultSet(ResultSet resultSet)
    {
      return new MessagePagingInfo()
      {
        PagingCookig = resultSet.PagingCookie,
        HasMoreRecords = Convert.ToBoolean((object) resultSet.MoreRecords, (IFormatProvider) CultureInfo.InvariantCulture)
      };
    }
  }
}

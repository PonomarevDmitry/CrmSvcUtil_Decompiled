// Decompiled with JetBrains decompiler
// Type: Microsoft.Crm.Services.Utility.ServiceFactory
// Assembly: CrmSvcUtil, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 756175FA-E97D-49FF-B443-B7A725C9F163
// Assembly location: C:\Users\dmitriy.ponomarev\AppData\Roaming\MscrmTools\XrmToolBox\NugetPlugins\DLaB.Xrm.EarlyBoundGenerator.1.2019.3.19\lib\net462\plugins\DLaB.EarlyBoundGenerator\CrmSvcUtil.exe

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;

namespace Microsoft.Crm.Services.Utility
{
  internal static class ServiceFactory
  {
    internal static TIService CreateInstance<TIService>(
      TIService defaultServiceInstance,
      string parameterValue,
      CrmSvcUtilParameters parameters)
    {
      string index = typeof (TIService).Name.Substring(1);
      CrmSvcUtil.crmSvcUtilLogger.TraceInformation("Creating instance of {0}", (object) typeof (TIService).Name);
      string typeName = parameterValue;
      if (string.IsNullOrEmpty(typeName))
        typeName = ConfigurationManager.AppSettings[index];
      if (string.IsNullOrEmpty(typeName))
        return defaultServiceInstance;
      CrmSvcUtil.crmSvcUtilLogger.TraceInformation("Looking for extension named {0}", (object) typeName);
      Type type = Type.GetType(typeName, false);
      if (type == (Type) null)
        throw new NotSupportedException("Could not load provider of type '" + typeName + "'");
      if (type.GetInterface(typeof (TIService).FullName) == (Type) null)
        throw new NotSupportedException("Type '" + typeName + "'does not implement interface " + typeof (TIService).FullName);
      if (type.IsAbstract)
        throw new NotSupportedException("Cannot instantiate abstract type '" + typeName + "'.");
      ConstructorInfo constructor1 = type.GetConstructor(new Type[2]
      {
        typeof (TIService),
        typeof (IDictionary<string, string>)
      });
      if (constructor1 != (ConstructorInfo) null)
        return (TIService) constructor1.Invoke(new object[2]
        {
          (object) defaultServiceInstance,
          (object) parameters.ToDictionary()
        });
      ConstructorInfo constructor2 = type.GetConstructor(new Type[1]
      {
        typeof (TIService)
      });
      if (constructor2 != (ConstructorInfo) null)
        return (TIService) constructor2.Invoke(new object[1]
        {
          (object) defaultServiceInstance
        });
      ConstructorInfo constructor3 = type.GetConstructor(new Type[1]
      {
        typeof (IDictionary<string, string>)
      });
      if (!(constructor3 != (ConstructorInfo) null))
        return (TIService) Activator.CreateInstance(type);
      return (TIService) constructor3.Invoke(new object[1]
      {
        (object) parameters.ToDictionary()
      });
    }
  }
}

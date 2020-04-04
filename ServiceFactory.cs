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
            string index = typeof(TIService).Name.Substring(1);
            CrmSvcUtil.crmSvcUtilLogger.TraceInformation("Creating instance of {0}", (object)typeof(TIService).Name);
            string typeName = parameterValue;
            if (string.IsNullOrEmpty(typeName))
                typeName = ConfigurationManager.AppSettings[index];
            if (string.IsNullOrEmpty(typeName))
                return defaultServiceInstance;
            CrmSvcUtil.crmSvcUtilLogger.TraceInformation("Looking for extension named {0}", (object)typeName);
            Type type = Type.GetType(typeName, false);
            if (type == (Type)null)
                throw new NotSupportedException("Could not load provider of type '" + typeName + "'");
            if (type.GetInterface(typeof(TIService).FullName) == (Type)null)
                throw new NotSupportedException("Type '" + typeName + "'does not implement interface " + typeof(TIService).FullName);
            if (type.IsAbstract)
                throw new NotSupportedException("Cannot instantiate abstract type '" + typeName + "'.");
            ConstructorInfo constructor1 = type.GetConstructor(new Type[2]
            {
        typeof (TIService),
        typeof (IDictionary<string, string>)
            });
            if (constructor1 != (ConstructorInfo)null)
                return (TIService)constructor1.Invoke(new object[2]
                {
          (object) defaultServiceInstance,
          (object) parameters.ToDictionary()
                });
            ConstructorInfo constructor2 = type.GetConstructor(new Type[1]
            {
        typeof (TIService)
            });
            if (constructor2 != (ConstructorInfo)null)
                return (TIService)constructor2.Invoke(new object[1]
                {
          (object) defaultServiceInstance
                });
            ConstructorInfo constructor3 = type.GetConstructor(new Type[1]
            {
        typeof (IDictionary<string, string>)
            });
            if (!(constructor3 != (ConstructorInfo)null))
                return (TIService)Activator.CreateInstance(type);
            return (TIService)constructor3.Invoke(new object[1]
            {
        (object) parameters.ToDictionary()
            });
        }
    }
}
// Decompiled with JetBrains decompiler
// Type: Microsoft.Crm.Services.Utility.MethodTracer
// Assembly: CrmSvcUtil, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 756175FA-E97D-49FF-B443-B7A725C9F163
// Assembly location: C:\Users\dmitriy.ponomarev\AppData\Roaming\MscrmTools\XrmToolBox\NugetPlugins\DLaB.Xrm.EarlyBoundGenerator.1.2019.3.19\lib\net462\plugins\DLaB.EarlyBoundGenerator\CrmSvcUtil.exe

using System;
using System.Diagnostics;
using System.Reflection;

namespace Microsoft.Crm.Services.Utility
{
  internal static class MethodTracer
  {
    internal static void Enter()
    {
      StackTrace stackTrace = new StackTrace();
      MethodTracer.LogMessage("Entering {0}", stackTrace.GetFrame(Math.Min(1, stackTrace.FrameCount - 1)).GetMethod());
    }

    internal static void Exit()
    {
      StackTrace stackTrace = new StackTrace();
      MethodTracer.LogMessage("Exiting {0}", stackTrace.GetFrame(Math.Min(1, stackTrace.FrameCount - 1)).GetMethod());
    }

    internal static void LogMessage(string message)
    {
      StackTrace stackTrace = new StackTrace();
      MethodTracer.LogMessage(message, stackTrace.GetFrame(Math.Min(1, stackTrace.FrameCount - 1)).GetMethod());
    }

    internal static void LogWarning(string message)
    {
      StackTrace stackTrace = new StackTrace();
      MethodTracer.LogWarning(message, stackTrace.GetFrame(Math.Min(1, stackTrace.FrameCount - 1)).GetMethod());
    }

    private static void LogMessage(string message, MethodBase method)
    {
      CrmSvcUtil.crmSvcUtilLogger.TraceInformation(message, (object) MethodTracer.GetMethodString(method));
    }

    private static void LogWarning(string message, MethodBase method)
    {
      CrmSvcUtil.crmSvcUtilLogger.TraceWarning(message, (object) MethodTracer.GetMethodString(method));
      CrmSvcUtil.crmSvcUtilLogger.Log(string.Format(message, (object) MethodTracer.GetMethodString(method)), TraceEventType.Warning);
    }

    private static string GetMethodString(MethodBase method)
    {
      return method.DeclaringType.Name + "." + method.Name;
    }
  }
}

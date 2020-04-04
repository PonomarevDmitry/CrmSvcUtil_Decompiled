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
            Trace.TraceInformation(message, (object)MethodTracer.GetMethodString(method));
        }

        private static void LogWarning(string message, MethodBase method)
        {
            Trace.TraceWarning(message, (object)MethodTracer.GetMethodString(method));
        }

        private static string GetMethodString(MethodBase method)
        {
            return method.DeclaringType.Name + "." + method.Name;
        }
    }
}
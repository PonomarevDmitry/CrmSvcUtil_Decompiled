using Microsoft.Xrm.Sdk;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Security;

namespace Microsoft.Crm.Services.Utility
{
    internal sealed class CrmSvcUtil
    {
        internal static readonly Guid ApplicationId = new Guid("321f8931-15b2-4d9b-b894-b18f91ba6a5a");
        private CrmSvcUtilParameters _parameters;
        private ServiceProvider _serviceProvider;

        private CrmSvcUtil()
        {
            MethodTracer.Enter();
            this._parameters = new CrmSvcUtilParameters();
            MethodTracer.Exit();
        }

        private CrmSvcUtilParameters Parameters
        {
            get
            {
                return this._parameters;
            }
        }

        private ServiceProvider ServiceProvider
        {
            get
            {
                if (this._serviceProvider == null)
                {
                    this._serviceProvider = new ServiceProvider();
                    this._serviceProvider.InitializeServices(this.Parameters);
                }
                return this._serviceProvider;
            }
        }

        internal static string ApplicationName
        {
            get
            {
                return ((AssemblyTitleAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false)[0]).Title;
            }
        }

        private static string ApplicationDescription
        {
            get
            {
                return ((AssemblyDescriptionAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)[0]).Description;
            }
        }

        internal static string ApplicationVersion
        {
            get
            {
                return ((AssemblyFileVersionAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true)[0]).Version;
            }
        }

        private static string ApplicationCopyright
        {
            get
            {
                return ((AssemblyCopyrightAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)[0]).Copyright;
            }
        }

        private void LoadArguments(string[] args)
        {
            this._parameters.LoadArguments(args);
        }

        private int Run()
        {
            MethodTracer.Enter();
            if (!this.Parameters.NoLogo)
                CrmSvcUtil.ShowLogo();
            IOrganizationMetadata organizationMetadata = this.ServiceProvider.MetadataProviderService.LoadMetadata();
            if (organizationMetadata == null)
            {
                Trace.TraceError("{0} returned null metadata", (object)typeof(IMetadataProviderService).Name);
                return 1;
            }
            this.WriteCode(organizationMetadata);
            MethodTracer.LogMessage("Exiting {0} with exit code 0");
            return 0;
        }

        private void WriteCode(IOrganizationMetadata organizationMetadata)
        {
            MethodTracer.Enter();
            this.ServiceProvider.CodeGenerationService.Write(organizationMetadata, this.Parameters.Language, this.Parameters.OutputFile, this.Parameters.Namespace, (IServiceProvider)this.ServiceProvider);
            MethodTracer.Exit();
        }

        private void ShowHelp()
        {
            MethodTracer.Enter();
            CrmSvcUtil.ShowLogo();
            this.Parameters.ShowUsage();
            MethodTracer.Exit();
        }

        private static void ShowLogo()
        {
            MethodTracer.Enter();
            Console.Out.WriteLine(string.Format((IFormatProvider)CultureInfo.InvariantCulture, "{0} : {1} [Version {2}]", (object)CrmSvcUtil.ApplicationName, (object)CrmSvcUtil.ApplicationDescription, (object)CrmSvcUtil.ApplicationVersion));
            Console.Out.WriteLine(CrmSvcUtil.ApplicationCopyright);
            Console.Out.WriteLine();
            MethodTracer.Exit();
        }

        private static int Main(string[] args)
        {
            CrmSvcUtil crmSvcUtil = (CrmSvcUtil)null;
            try
            {
                crmSvcUtil = new CrmSvcUtil();
                crmSvcUtil.LoadArguments(args);
                if (!crmSvcUtil.Parameters.ShowHelp)
                {
                    if (crmSvcUtil.Parameters.VerifyArguments())
                        goto label_5;
                }
                crmSvcUtil.ShowHelp();
                MethodTracer.LogWarning("Exiting {0} with exit code 1");
                return 1;
            }
            catch (InvalidOperationException ex)
            {
                crmSvcUtil.ShowHelp();
                MethodTracer.LogWarning("Exiting {0} with exit code 1");
                return 1;
            }
        label_5:
            try
            {
                crmSvcUtil.Run();
                MethodTracer.LogMessage("Exiting {0} with exit code 0");
                return 0;
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Console.Error.WriteLine();
                Console.Error.WriteLine("Exiting program with exception: {0}", (object)ex.Detail.Message);
                Console.Error.WriteLine("Enable tracing and view the trace files for more information.");
                Trace.TraceError("Exiting program with exit code 2 due to exception : {0}", (object)ex.Detail);
                return 2;
            }
            catch (MessageSecurityException ex)
            {
                Console.Error.WriteLine();
                Console.Error.WriteLine("Exiting program with exception: {0}", (object)ex.InnerException.Message);
                Console.Error.WriteLine("Enable tracing and view the trace files for more information.");
                Trace.TraceError("Exiting program with exit code 2 due to exception : {0}", (object)ex.InnerException);
                return 2;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine();
                Console.Error.WriteLine("Exiting program with exception: {0}", (object)ex.Message);
                Console.Error.WriteLine("Enable tracing and view the trace files for more information.");
                Trace.TraceError("Exiting program with exit code 2 due to exception : {0}", (object)ex);
                return 2;
            }
        }
    }
}
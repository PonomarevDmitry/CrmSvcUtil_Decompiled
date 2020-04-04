using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Microsoft.Crm.Services.Utility
{
    internal sealed class CrmSvcUtilParameters : ICommandLineArgumentSource
    {
        private CommandLineParser _parser;
        private string _sdkUrl;
        private string _language;
        private bool _generateMessages;
        private string _outputFile;
        private string _namespace;
        private string _serviceContextName;
        private string _messageNamespace;
        private bool _noLogo;
        private bool _showHelp;
        private bool _privateMessages;
        private bool _generateCustomActions;
        private string _codeCustomizationService;
        private string _codeGenerationService;
        private string _codeWriterFilterService;
        private string _codeWriterMessageFilterService;
        private string _metadataProviderService;
        private string _metadataProviderService2;
        private string _metadataQueryProvider;
        private string _namingService;
        private string _userName;
        private string _password;
        private string _domain;
        private string _deviceID;
        private string _devicePassword;
        private Dictionary<string, string> _unknownParameters;
        private Dictionary<string, string> _parametersAsDictionary;

        internal CrmSvcUtilParameters()
        {
            MethodTracer.Enter();
            this._parser = new CommandLineParser((ICommandLineArgumentSource)this);
            this._generateMessages = false;
            this._generateCustomActions = false;
            this._unknownParameters = new Dictionary<string, string>();
            this.Language = "CS";
            MethodTracer.Exit();
        }

        [CommandLineArgument(ArgumentType.Optional | ArgumentType.Binary, "nologo", Description = "Suppresses the banner.")]
        internal bool NoLogo
        {
            get
            {
                return this._noLogo;
            }
            set
            {
                this._noLogo = value;
            }
        }

        [CommandLineArgument(ArgumentType.Optional, "language", Description = "The language to use for the generated proxy code.  This can be either 'CS' or 'VB'.  The default language is 'CS'.", ParameterDescription = "<language>", Shortcut = "l")]
        internal string Language
        {
            get
            {
                return this._language;
            }
            set
            {
                if (!CodeDomProvider.IsDefinedLanguage(value))
                    throw new InvalidOperationException(string.Format((IFormatProvider)CultureInfo.InvariantCulture, "Language {0} is not a support CodeDom Language.", (object)value));
                this._language = value;
            }
        }

        [CommandLineArgument(ArgumentType.Required, "url", Description = "A url or path to the SDK endpoint to contact for metadata.", ParameterDescription = "<url>", SampleUsageValue = "http://localhost/Organization1/XRMServices/2011/Organization.svc")]
        internal string Url
        {
            get
            {
                return this._sdkUrl;
            }
            set
            {
                this._sdkUrl = value;
            }
        }

        [CommandLineArgument(ArgumentType.Required, "out", Description = "The filename for the generated proxy code.", ParameterDescription = "<filename>", SampleUsageValue = "GeneratedCode.cs", Shortcut = "o")]
        internal string OutputFile
        {
            get
            {
                return this._outputFile;
            }
            set
            {
                this._outputFile = value;
            }
        }

        [CommandLineArgument(ArgumentType.Optional, "namespace", Description = "The namespace for the generated proxy code.  The default namespace is the global namespace.", ParameterDescription = "<namespace>", Shortcut = "n")]
        internal string Namespace
        {
            get
            {
                return this._namespace;
            }
            set
            {
                this._namespace = value;
            }
        }

        [CommandLineArgument(ArgumentType.Optional | ArgumentType.Binary, "interactivelogin", Description = "Presents a login dialog to log into the service with, if passed all other connect info is ignored.", Shortcut = "il")]
        public bool UseInteractiveLogin { get; set; }

        [CommandLineArgument(ArgumentType.Optional, "connectionstring", Description = "Connection String to use when connecting to CRM. If provided, all other connect info is ignored.", Shortcut = "connstr")]
        public string ConnectionString { get; set; }

        [CommandLineArgument(ArgumentType.Optional | ArgumentType.Hidden, "useoauth", Description = "when set, try to login with oAuth to CRM Online")]
        public bool UseOAuth { get; set; }

        [CommandLineArgument(ArgumentType.Optional, "username", Description = "Username to use when connecting to the server for authentication.", ParameterDescription = "<username>", Shortcut = "u")]
        internal string UserName
        {
            get
            {
                return this._userName;
            }
            set
            {
                this._userName = value;
            }
        }

        [CommandLineArgument(ArgumentType.Optional, "password", Description = "Password to use when connecting to the server for authentication.", ParameterDescription = "<password>", Shortcut = "p")]
        internal string Password
        {
            get
            {
                return this._password;
            }
            set
            {
                this._password = value;
            }
        }

        [CommandLineArgument(ArgumentType.Optional, "domain", Description = "Domain to authenticate against when connecting to the server.", ParameterDescription = "<domain>", Shortcut = "d")]
        internal string Domain
        {
            get
            {
                return this._domain;
            }
            set
            {
                this._domain = value;
            }
        }

        [CommandLineArgument(ArgumentType.Optional, "serviceContextName", Description = "The name for the generated service context. If a value is passed in, it will be used for the Service Context.  If not, no Service Context will be generated", ParameterDescription = "<service context name>")]
        internal string ServiceContextName
        {
            get
            {
                return this._serviceContextName;
            }
            set
            {
                this._serviceContextName = value;
            }
        }

        [CommandLineArgument(ArgumentType.Optional | ArgumentType.Hidden, "messageNamespace", Description = "Namespace of messages to generate.", ParameterDescription = "<message namespace>", Shortcut = "m")]
        internal string MessageNamespace
        {
            get
            {
                return this._messageNamespace;
            }
            set
            {
                this._messageNamespace = value;
            }
        }

        [CommandLineArgument(ArgumentType.Optional | ArgumentType.Binary, "help", Description = "Show this usage message.", Shortcut = "?")]
        internal bool ShowHelp
        {
            get
            {
                return this._showHelp;
            }
            set
            {
                this._showHelp = value;
            }
        }

        [CommandLineArgument(ArgumentType.Optional | ArgumentType.Hidden, "codecustomization", Description = "Full name of the type to use as the ICustomizeCodeDomService", ParameterDescription = "<typename>")]
        internal string CodeCustomizationService
        {
            get
            {
                return this._codeCustomizationService;
            }
            set
            {
                this._codeCustomizationService = value;
            }
        }

        [CommandLineArgument(ArgumentType.Optional | ArgumentType.Hidden, "codewriterfilter", Description = "Full name of the type to use as the ICodeWriterFilterService", ParameterDescription = "<typename>")]
        internal string CodeWriterFilterService
        {
            get
            {
                return this._codeWriterFilterService;
            }
            set
            {
                this._codeWriterFilterService = value;
            }
        }

        [CommandLineArgument(ArgumentType.Optional | ArgumentType.Hidden, "codewritermessagefilter", Description = "Full name of the type to use as the ICodeWriterMessageFilterService", ParameterDescription = "<typename>")]
        internal string CodeWriterMessageFilterService
        {
            get
            {
                return this._codeWriterMessageFilterService;
            }
            set
            {
                this._codeWriterMessageFilterService = value;
            }
        }

        [CommandLineArgument(ArgumentType.Optional | ArgumentType.Hidden, "metadataproviderservice", Description = "Full name of the type to use as the IMetadataProviderService", ParameterDescription = "<typename>")]
        internal string MetadataProviderService
        {
            get
            {
                return this._metadataProviderService;
            }
            set
            {
                this._metadataProviderService = value;
            }
        }

        [CommandLineArgument(ArgumentType.Optional | ArgumentType.Hidden, "metadataproviderqueryservice", Description = "Full name of the type to use as the IMetaDataProviderQueryService", ParameterDescription = "<typename>")]
        internal string MetadataQueryProvider
        {
            get
            {
                return this._metadataQueryProvider;
            }
            set
            {
                this._metadataQueryProvider = value;
            }
        }

        [CommandLineArgument(ArgumentType.Optional | ArgumentType.Hidden, "codegenerationservice", Description = "Full name of the type to use as the ICodeGenerationService", ParameterDescription = "<typename>")]
        internal string CodeGenerationService
        {
            get
            {
                return this._codeGenerationService;
            }
            set
            {
                this._codeGenerationService = value;
            }
        }

        [CommandLineArgument(ArgumentType.Optional | ArgumentType.Hidden, "namingservice", Description = "Full name of the type to use as the INamingService", ParameterDescription = "<typename>")]
        internal string NamingService
        {
            get
            {
                return this._namingService;
            }
            set
            {
                this._namingService = value;
            }
        }

        [CommandLineArgument(ArgumentType.Optional | ArgumentType.Binary | ArgumentType.Hidden, "private", Description = "Generate unsupported classes", ParameterDescription = "<private>")]
        internal bool Private
        {
            get
            {
                return this._privateMessages;
            }
            set
            {
                this._privateMessages = value;
            }
        }

        [CommandLineArgument(ArgumentType.Optional | ArgumentType.Binary, "generateActions", Description = "Generate wrapper classes for custom actions", Shortcut = "a")]
        internal bool GenerateCustomActions
        {
            get
            {
                return this._generateCustomActions;
            }
            set
            {
                this._generateCustomActions = value;
            }
        }

        [CommandLineArgument(ArgumentType.Optional | ArgumentType.Binary | ArgumentType.Hidden, "includeMessages", Description = "Generate messages")]
        internal bool GenerateMessages
        {
            get
            {
                return this._generateMessages;
            }
            set
            {
                this._generateMessages = value;
            }
        }

        [CommandLineArgument(ArgumentType.Optional, "deviceid", Description = "Device ID to use when connecting to the online server for authentication.", ParameterDescription = "<deviceid>", Shortcut = "di")]
        internal string DeviceID
        {
            get
            {
                return this._deviceID;
            }
            set
            {
                this._deviceID = value;
            }
        }

        [CommandLineArgument(ArgumentType.Optional, "devicepassword", Description = "Device Password to use when connecting to the online server for authentication.", ParameterDescription = "<devicepassword>", Shortcut = "dp")]
        internal string DevicePassword
        {
            get
            {
                return this._devicePassword;
            }
            set
            {
                this._devicePassword = value;
            }
        }

        [CommandLineArgument(ArgumentType.Optional | ArgumentType.Hidden, "connectionprofilename", Description = "connection profile name used")]
        public string ConnectionProfileName { get; set; }

        [CommandLineArgument(ArgumentType.Optional | ArgumentType.Hidden, "connectionname", Description = "Application Name whose connection to use")]
        public string ConnectionAppName { get; set; }

        internal IDictionary<string, string> ToDictionary()
        {
            if (this._parametersAsDictionary == null)
            {
                this._parametersAsDictionary = new Dictionary<string, string>();
                foreach (PropertyInfo property in typeof(CrmSvcUtilParameters).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    object[] customAttributes = property.GetCustomAttributes(typeof(CommandLineArgumentAttribute), false);
                    if (customAttributes != null && customAttributes.Length > 0)
                    {
                        CommandLineArgumentAttribute argumentAttribute = (CommandLineArgumentAttribute)customAttributes[0];
                        object obj = property.GetValue((object)this, (object[])null);
                        if (obj != null)
                            this._parametersAsDictionary.Add(argumentAttribute.Name, obj.ToString());
                    }
                }
                foreach (string key in this._unknownParameters.Keys)
                    this._parametersAsDictionary.Add(key, this._unknownParameters[key]);
            }
            return (IDictionary<string, string>)this._parametersAsDictionary;
        }

        private CommandLineParser Parser
        {
            get
            {
                return this._parser;
            }
        }

        private bool ContainsUnknownParameters
        {
            get
            {
                return this._unknownParameters.Count != 0 && string.IsNullOrWhiteSpace(this.CodeCustomizationService) && (string.IsNullOrWhiteSpace(this.CodeGenerationService) && string.IsNullOrWhiteSpace(this.MetadataQueryProvider)) && (string.IsNullOrWhiteSpace(this.CodeWriterFilterService) && string.IsNullOrWhiteSpace(this.CodeWriterMessageFilterService) && (string.IsNullOrWhiteSpace(this.MetadataProviderService) && string.IsNullOrWhiteSpace(this.NamingService)));
            }
        }

        internal void LoadArguments(string[] args)
        {
            MethodTracer.Enter();
            this.Parser.ParseArguments(args);
            MethodTracer.Exit();
        }

        internal bool VerifyArguments()
        {
            MethodTracer.Enter();
            if (!this.Parser.VerifyArguments())
            {
                MethodTracer.LogWarning("Exiting {0} with false return value due to the parser finding invalid arguments");
                return false;
            }
            if (this.ContainsUnknownParameters)
            {
                MethodTracer.LogWarning("Exiting {0} with false return value due to finding unknown parameters");
                return false;
            }
            if (!string.IsNullOrEmpty(this.UserName) && string.IsNullOrEmpty(this.Password))
            {
                MethodTracer.LogWarning("Exiting {0} with false return value due to invalid credentials");
                return false;
            }
            MethodTracer.LogMessage("Exiting {0} with true return value");
            return true;
        }

        internal void ShowUsage()
        {
            MethodTracer.Enter();
            this.Parser.WriteUsage();
            MethodTracer.Exit();
        }

        void ICommandLineArgumentSource.OnUnknownArgument(
          string argumentName,
          string argumentValue)
        {
            CrmSvcUtil.crmSvcUtilLogger.TraceInformation("{0}: Found unknown argument {1}{2}.", (object)MethodBase.GetCurrentMethod().Name, (object)'/', (object)argumentName);
            this._unknownParameters[argumentName] = argumentValue;
        }

        void ICommandLineArgumentSource.OnInvalidArgument(string argument)
        {
            CrmSvcUtil.crmSvcUtilLogger.TraceError("Exiting {0}: Found string {1} in arguments array that could not be parsed.", (object)MethodBase.GetCurrentMethod().Name, (object)argument);
            throw new InvalidOperationException(string.Format((IFormatProvider)CultureInfo.InvariantCulture, "Argument '{0}' could not be parsed.", (object)argument));
        }
    }
}
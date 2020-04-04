using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;

namespace Microsoft.Crm.Services.Utility
{
    /// <remarks>Utility class to parse command line arguments.</remarks>
    internal sealed class CommandLineParser
    {
        /// <summary>The object that contains the properties to set.</summary>
        private ICommandLineArgumentSource _argsSource;
        /// <summary>
        /// A mapping of argument switches to command line arguments.
        /// </summary>
        private Dictionary<string, CommandLineArgument> _argumentsMap;
        /// <summary>A list of all of the arguments that are supported</summary>
        private List<CommandLineArgument> _arguments;

        /// <summary>
        /// Creates a new command line parser for the given object.
        /// </summary>
        /// <param name="argsSource">The object containing the properties representing the command line args to set.</param>
        internal CommandLineParser(ICommandLineArgumentSource argsSource)
        {
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStart("Entering {0}", (object)MethodBase.GetCurrentMethod().Name);
            CrmSvcUtil.crmSvcUtilLogger.TraceInformation("Creating CommandLineParser for {0}.", (object)argsSource.GetType().Name);
            this._argsSource = argsSource;
            this._arguments = new List<CommandLineArgument>();
            this._argumentsMap = this.GetPropertyMap();
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStop("Exiting {0}", (object)MethodBase.GetCurrentMethod().Name);
        }

        private ICommandLineArgumentSource ArgumentsSource
        {
            get
            {
                return this._argsSource;
            }
        }

        private List<CommandLineArgument> Arguments
        {
            get
            {
                return this._arguments;
            }
        }

        private Dictionary<string, CommandLineArgument> ArgumentsMap
        {
            get
            {
                return this._argumentsMap;
            }
        }

        internal void ParseArguments(string[] args)
        {
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStart("Entering {0}", (object)MethodBase.GetCurrentMethod().Name);
            if (args != null)
            {
                foreach (string str in args)
                {
                    if (CommandLineParser.IsArgument(str))
                    {
                        string argumentValue = (string)null;
                        string argumentName = CommandLineParser.GetArgumentName(str, out argumentValue);
                        if (!string.IsNullOrEmpty(argumentName) && this.ArgumentsMap.ContainsKey(argumentName))
                        {
                            CrmSvcUtil.crmSvcUtilLogger.TraceInformation("Setting argument {0} to value {1}", (object)argumentName, (object)argumentValue);
                            this.ArgumentsMap[argumentName].SetValue((object)this.ArgumentsSource, argumentValue);
                        }
                        else
                            this.ArgumentsSource.OnUnknownArgument(argumentName, argumentValue);
                    }
                    else
                        this.ArgumentsSource.OnInvalidArgument(str);
                }
            }
            this.ParseConfigArguments();
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStop("Exiting {0}", (object)MethodBase.GetCurrentMethod().Name);
        }

        private void ParseConfigArguments()
        {
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStart("Entering {0}", (object)MethodBase.GetCurrentMethod().Name);
            foreach (string allKey in ConfigurationManager.AppSettings.AllKeys)
            {
                string upperInvariant = allKey.ToUpperInvariant();
                string appSetting = ConfigurationManager.AppSettings[allKey];
                if (this.ArgumentsMap.ContainsKey(upperInvariant) && !this.ArgumentsMap[upperInvariant].IsSet)
                {
                    CrmSvcUtil.crmSvcUtilLogger.TraceInformation("Setting argument {0} to config value {1}", (object)upperInvariant, (object)appSetting);
                    this.ArgumentsMap[upperInvariant].SetValue((object)this.ArgumentsSource, appSetting);
                }
                else
                    CrmSvcUtil.crmSvcUtilLogger.TraceInformation("Skipping config value {0} as it is an unknown argument.", (object)upperInvariant);
            }
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStop("Exiting {0}", (object)MethodBase.GetCurrentMethod().Name);
        }

        internal bool VerifyArguments()
        {
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStart("Entering {0}", (object)MethodBase.GetCurrentMethod().Name);
            if (this.ArgumentsMap.ContainsKey("CONNECTIONNAME") && this.ArgumentsMap["CONNECTIONNAME"].IsSet && (this.ArgumentsMap.ContainsKey("OUT") && this.ArgumentsMap["OUT"].IsSet || this.ArgumentsMap.ContainsKey("O") && this.ArgumentsMap["O"].IsSet) || this.ArgumentsMap.ContainsKey("INTERACTIVELOGIN") && this.ArgumentsMap["INTERACTIVELOGIN"].IsSet && (this.ArgumentsMap.ContainsKey("OUT") && this.ArgumentsMap["OUT"].IsSet || this.ArgumentsMap.ContainsKey("O") && this.ArgumentsMap["O"].IsSet) || this.ArgumentsMap.ContainsKey("CONNECTIONSTRING") && this.ArgumentsMap["CONNECTIONSTRING"].IsSet && (this.ArgumentsMap.ContainsKey("OUT") && this.ArgumentsMap["OUT"].IsSet || this.ArgumentsMap.ContainsKey("O") && this.ArgumentsMap["O"].IsSet))
                return true;
            foreach (CommandLineArgument commandLineArgument in this.ArgumentsMap.Values)
            {
                if (commandLineArgument.IsRequired && !commandLineArgument.IsSet)
                {
                    CrmSvcUtil.crmSvcUtilLogger.TraceMethodStop("Exiting {0} with false return value because argument {1} is not set.", (object)MethodBase.GetCurrentMethod().Name, (object)commandLineArgument.Name);
                    return false;
                }
            }
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStop("Exiting {0} with true return value", (object)MethodBase.GetCurrentMethod().Name);
            return true;
        }

        internal void WriteUsage()
        {
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStart("Entering {0}", (object)MethodBase.GetCurrentMethod().Name);
            Console.Out.WriteLine();
            Console.Out.WriteLine("Options:");
            foreach (CommandLineArgument commandLineArgument in this.Arguments)
            {
                if (!commandLineArgument.IsHidden)
                    Console.Out.WriteLine(commandLineArgument.ToString());
            }
            Console.Out.WriteLine();
            Console.Out.WriteLine("Example:");
            Console.Out.WriteLine(this.GetSampleUsage());
            Console.Out.WriteLine();
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStop("Exiting {0}", (object)MethodBase.GetCurrentMethod().Name);
        }

        private string GetSampleUsage()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(Path.GetFileName(Assembly.GetExecutingAssembly().Location));
            foreach (CommandLineArgument commandLineArgument in this.Arguments)
            {
                if (!commandLineArgument.IsHidden && commandLineArgument.IsRequired && !string.IsNullOrEmpty(commandLineArgument.SampleUsageValue))
                    stringBuilder.Append(commandLineArgument.ToSampleString());
            }
            return CommandLineArgument.WrapLine(stringBuilder.ToString());
        }

        /// <summary>Populates the command line arguments map.</summary>
        private Dictionary<string, CommandLineArgument> GetPropertyMap()
        {
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStart("Entering {0}", (object)MethodBase.GetCurrentMethod().Name);
            Dictionary<string, CommandLineArgument> propertyMap = new Dictionary<string, CommandLineArgument>();
            foreach (PropertyInfo property in this.ArgumentsSource.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.SetProperty))
            {
                CrmSvcUtil.crmSvcUtilLogger.TraceInformation("Checking property {0} for command line attribution", (object)property.Name);
                CommandLineArgumentAttribute commandLineAttribute = CommandLineParser.GetCommandLineAttribute(property);
                if (commandLineAttribute == null)
                {
                    CrmSvcUtil.crmSvcUtilLogger.TraceInformation("Skipping property {0} since it does not have command line attribution", (object)property.Name);
                }
                else
                {
                    CrmSvcUtil.crmSvcUtilLogger.TraceInformation("Creating CommandLineArgument for Property {0}", (object)property.Name);
                    CommandLineArgument commandLineArgument = new CommandLineArgument(property, commandLineAttribute);
                    this.Arguments.Add(commandLineArgument);
                    CommandLineParser.CreateMapEntry(propertyMap, property, commandLineArgument, "shortcut", commandLineArgument.Shortcut);
                    CommandLineParser.CreateMapEntry(propertyMap, property, commandLineArgument, "name", commandLineArgument.Name);
                }
            }
            CrmSvcUtil.crmSvcUtilLogger.TraceMethodStop("Exiting {0} with PropertyMap length of {1} ", (object)MethodBase.GetCurrentMethod().Name, (object)propertyMap.Count);
            return propertyMap;
        }

        private static bool CreateMapEntry(
          Dictionary<string, CommandLineArgument> propertyMap,
          PropertyInfo property,
          CommandLineArgument argument,
          string type,
          string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                CrmSvcUtil.crmSvcUtilLogger.TraceInformation("Property {0} has defined a {1} {2}", (object)property.Name, (object)type, (object)value);
                propertyMap.Add(value.ToUpperInvariant(), argument);
                return true;
            }
            CrmSvcUtil.crmSvcUtilLogger.TraceWarning("Property {0} does not have a {1} defined", (object)property.Name, (object)type);
            return false;
        }

        private static CommandLineArgumentAttribute GetCommandLineAttribute(
          PropertyInfo property)
        {
            object[] customAttributes = property.GetCustomAttributes(typeof(CommandLineArgumentAttribute), false);
            if (customAttributes == null || customAttributes.Length == 0)
                return (CommandLineArgumentAttribute)null;
            return (CommandLineArgumentAttribute)customAttributes[0];
        }

        private static bool IsArgument(string argument)
        {
            return argument[0] == '/';
        }

        private static string GetArgumentName(string argument, out string argumentValue)
        {
            argumentValue = (string)null;
            string str = (string)null;
            if (argument[0] == '/')
            {
                int num = argument.IndexOf(':');
                if (num != -1)
                {
                    str = argument.Substring(1, num - 1);
                    argumentValue = argument.Substring(num + 1);
                }
                else
                    str = argument.Substring(1);
            }
            return str.ToUpperInvariant();
        }
    }
}
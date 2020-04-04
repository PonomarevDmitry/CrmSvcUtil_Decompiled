using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace Microsoft.Crm.Services.Utility
{
    internal sealed class CommandLineParser
    {
        private ICommandLineArgumentSource _argsSource;
        private Dictionary<string, CommandLineArgument> _argumentsMap;
        private List<CommandLineArgument> _arguments;

        internal CommandLineParser(ICommandLineArgumentSource argsSource)
        {
            Trace.TraceInformation("Entering {0}", (object)MethodBase.GetCurrentMethod().Name);
            Trace.TraceInformation("Creating CommandLineParser for {0}.", (object)argsSource.GetType().Name);
            this._argsSource = argsSource;
            this._arguments = new List<CommandLineArgument>();
            this._argumentsMap = this.GetPropertyMap();
            Trace.TraceInformation("Exiting {0}", (object)MethodBase.GetCurrentMethod().Name);
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
            Trace.TraceInformation("Entering {0}", (object)MethodBase.GetCurrentMethod().Name);
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
                            Trace.TraceInformation("Setting argument {0} to value {1}", (object)argumentName, (object)argumentValue);
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
            Trace.TraceInformation("Exiting {0}", (object)MethodBase.GetCurrentMethod().Name);
        }

        private void ParseConfigArguments()
        {
            Trace.TraceInformation("Entering {0}", (object)MethodBase.GetCurrentMethod().Name);
            foreach (string allKey in ConfigurationManager.AppSettings.AllKeys)
            {
                string upperInvariant = allKey.ToUpperInvariant();
                string appSetting = ConfigurationManager.AppSettings[allKey];
                if (this.ArgumentsMap.ContainsKey(upperInvariant) && !this.ArgumentsMap[upperInvariant].IsSet)
                {
                    Trace.TraceInformation("Setting argument {0} to config value {1}", (object)upperInvariant, (object)appSetting);
                    this.ArgumentsMap[upperInvariant].SetValue((object)this.ArgumentsSource, appSetting);
                }
                else
                    Trace.TraceInformation("Skipping config value {0} as it is an unknown argument.", (object)upperInvariant);
            }
            Trace.TraceInformation("Exiting {0}", (object)MethodBase.GetCurrentMethod().Name);
        }

        internal bool VerifyArguments()
        {
            Trace.TraceInformation("Entering {0}", (object)MethodBase.GetCurrentMethod().Name);
            foreach (CommandLineArgument commandLineArgument in this.ArgumentsMap.Values)
            {
                if (commandLineArgument.IsRequired && !commandLineArgument.IsSet)
                {
                    Trace.TraceInformation("Exiting {0} with false return value because argument {1} is not set.", (object)MethodBase.GetCurrentMethod().Name, (object)commandLineArgument.Name);
                    return false;
                }
            }
            Trace.TraceInformation("Exiting {0} with true return value", (object)MethodBase.GetCurrentMethod().Name);
            return true;
        }

        internal void WriteUsage()
        {
            Trace.TraceInformation("Entering {0}", (object)MethodBase.GetCurrentMethod().Name);
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
            Trace.TraceInformation("Exiting {0}", (object)MethodBase.GetCurrentMethod().Name);
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

        private Dictionary<string, CommandLineArgument> GetPropertyMap()
        {
            Trace.TraceInformation("Entering {0}", (object)MethodBase.GetCurrentMethod().Name);
            Dictionary<string, CommandLineArgument> propertyMap = new Dictionary<string, CommandLineArgument>();
            foreach (PropertyInfo property in this.ArgumentsSource.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.SetProperty))
            {
                Trace.TraceInformation("Checking property {0} for command line attribution", (object)property.Name);
                CommandLineArgumentAttribute commandLineAttribute = CommandLineParser.GetCommandLineAttribute(property);
                if (commandLineAttribute == null)
                {
                    Trace.TraceInformation("Skipping property {0} since it does not have command line attribution", (object)property.Name);
                }
                else
                {
                    Trace.TraceInformation("Creating CommandLineArgument for Property {0}", (object)property.Name);
                    CommandLineArgument commandLineArgument = new CommandLineArgument(property, commandLineAttribute);
                    this.Arguments.Add(commandLineArgument);
                    CommandLineParser.CreateMapEntry(propertyMap, property, commandLineArgument, "shortcut", commandLineArgument.Shortcut);
                    CommandLineParser.CreateMapEntry(propertyMap, property, commandLineArgument, "name", commandLineArgument.Name);
                }
            }
            Trace.TraceInformation("Exiting {0} with PropertyMap length of {1} ", (object)MethodBase.GetCurrentMethod().Name, (object)propertyMap.Count);
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
                Trace.TraceInformation("Property {0} has defined a {1} {2}", (object)property.Name, (object)type, (object)value);
                propertyMap.Add(value.ToUpperInvariant(), argument);
                return true;
            }
            Trace.TraceWarning("Property {0} does not have a {1} defined", (object)property.Name, (object)type);
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
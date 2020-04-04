using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

namespace Microsoft.Crm.Services.Utility
{
    internal sealed class CommandLineArgument
    {
        internal const char ArgumentStartChar = '/';
        internal const char ArgumentSeparatorChar = ':';
        private const string ShortFormFormat = "Short form is '{0}{1}{2}'.";
        private PropertyInfo _argumentProperty;
        private CommandLineArgumentAttribute _argumentAttribute;
        private bool _isSet;

        internal CommandLineArgument(
          PropertyInfo argumentProperty,
          CommandLineArgumentAttribute argumentAttribute)
        {
            Trace.TraceInformation("Entering {0}", (object)MethodBase.GetCurrentMethod().Name);
            Trace.TraceInformation("Creating CommandLineArgument for ArgumentProperty {0} and ArgumentAttribute {1}", (object)argumentProperty.Name, (object)argumentAttribute.ToString());
            this._argumentAttribute = argumentAttribute;
            this._argumentProperty = argumentProperty;
            Trace.TraceInformation("Exiting {0}", (object)MethodBase.GetCurrentMethod().Name);
        }

        internal bool HasShortcut
        {
            get
            {
                return !string.IsNullOrEmpty(this._argumentAttribute.Shortcut);
            }
        }

        internal string Shortcut
        {
            get
            {
                return this._argumentAttribute.Shortcut;
            }
        }

        internal string Name
        {
            get
            {
                return this._argumentAttribute.Name;
            }
        }

        internal string ParameterDescription
        {
            get
            {
                return this._argumentAttribute.ParameterDescription;
            }
        }

        internal bool IsSet
        {
            get
            {
                return this._isSet;
            }
            private set
            {
                this._isSet = value;
            }
        }

        internal bool IsCollection
        {
            get
            {
                if (!(this.ArgumentProperty.PropertyType.GetInterface("IList", true) != (Type)null))
                    return this.ArgumentProperty.PropertyType.GetInterface(typeof(IList<>).FullName, true) != (Type)null;
                return true;
            }
        }

        internal bool IsRequired
        {
            get
            {
                return (this._argumentAttribute.Type & ArgumentType.Required) == ArgumentType.Required;
            }
        }

        internal bool SupportsMultiple
        {
            get
            {
                return (this._argumentAttribute.Type & ArgumentType.Multiple) == ArgumentType.Multiple;
            }
        }

        internal bool IsFlag
        {
            get
            {
                return (this._argumentAttribute.Type & ArgumentType.Binary) == ArgumentType.Binary;
            }
        }

        internal bool IsHidden
        {
            get
            {
                return (this._argumentAttribute.Type & ArgumentType.Hidden) == ArgumentType.Hidden;
            }
        }

        internal string Description
        {
            get
            {
                return this._argumentAttribute.Description;
            }
        }

        internal string SampleUsageValue
        {
            get
            {
                return this._argumentAttribute.SampleUsageValue;
            }
        }

        private PropertyInfo ArgumentProperty
        {
            get
            {
                return this._argumentProperty;
            }
        }

        private static int? WrapLength
        {
            get
            {
                try
                {
                    return new int?(Console.WindowWidth);
                }
                catch (IOException ex)
                {
                    return new int?();
                }
            }
        }

        internal void SetValue(object argTarget, string argValue)
        {
            Trace.TraceInformation("Entering {0}", (object)MethodBase.GetCurrentMethod().Name);
            Trace.TraceInformation("Attempting to set the Argument {0} with the value {1}", (object)argTarget.ToString(), (object)CommandLineArgument.ToNullableString((object)argValue));
            if (this.IsSet && !this.SupportsMultiple)
            {
                Trace.TraceError("Attempt to set argument {0} multiple times", (object)this.ArgumentProperty.Name);
                throw new InvalidOperationException(string.Format((IFormatProvider)CultureInfo.InvariantCulture, "Cannot set command line argument {0} multiple times", (object)this.ArgumentProperty.Name));
            }
            if (this.IsCollection)
                this.PopulateCollectionParameter(argTarget, argValue);
            else if (this.IsFlag)
            {
                Trace.TraceInformation("Setting flag property {0} to true", (object)this.ArgumentProperty.Name);
                this.ArgumentProperty.SetValue(argTarget, (object)true, (object[])null);
            }
            else
            {
                Trace.TraceInformation("Setting property {0} to value {1}", (object)this.ArgumentProperty.Name, (object)CommandLineArgument.ToNullableString((object)argValue));
                Trace.TraceInformation("Converting parameter value as ArgumentProperty {0} is defined as type {1}.", (object)this.ArgumentProperty.Name, (object)this.ArgumentProperty.PropertyType.Name);
                object obj = Convert.ChangeType((object)argValue, this.ArgumentProperty.PropertyType, (IFormatProvider)CultureInfo.InvariantCulture);
                this.ArgumentProperty.SetValue(argTarget, obj, (object[])null);
            }
            this.IsSet = true;
            Trace.TraceInformation("Exiting {0}", (object)MethodBase.GetCurrentMethod().Name);
        }

        private void PopulateCollectionParameter(object argTarget, string argValue)
        {
            Trace.TraceInformation("Entering {0}", (object)MethodBase.GetCurrentMethod().Name);
            IList list = this.ArgumentProperty.GetValue(argTarget, (object[])null) as IList;
            if (list == null)
            {
                Trace.TraceError("ArgumentProperty {0} did not return an IList as expected", (object)this.ArgumentProperty.ToString());
                throw new InvalidOperationException(string.Format((IFormatProvider)CultureInfo.InvariantCulture, "ArgumentProperty {0} did not return an IList as expected.", (object)this.ArgumentProperty.ToString()));
            }
            Type[] genericArguments = this.ArgumentProperty.PropertyType.GetGenericArguments();
            if (genericArguments == null || genericArguments.Length == 0)
            {
                Trace.TraceInformation("Adding parameter value directly as ArgumentProperty {0} is not defined as a generic.", (object)this.ArgumentProperty.Name);
                list.Add((object)argValue);
            }
            else
            {
                Trace.TraceInformation("Casting parameter value as ArgumentProperty {0} is defined as a generic of type {1}.", (object)this.ArgumentProperty.Name, (object)genericArguments[0].Name);
                object obj = Convert.ChangeType((object)argValue, genericArguments[0], (IFormatProvider)CultureInfo.InvariantCulture);
                Trace.TraceInformation("Argument value casted to {0} successfully.", (object)genericArguments[0].Name);
                list.Add(obj);
            }
            Trace.TraceInformation("Exiting {0}", (object)MethodBase.GetCurrentMethod().Name);
        }

        public override string ToString()
        {
            Trace.TraceInformation("Entering {0}", (object)MethodBase.GetCurrentMethod().Name);
            StringBuilder stringBuilder1 = new StringBuilder();
            stringBuilder1.AppendLine(this.ToDescriptionString());
            StringBuilder stringBuilder2 = new StringBuilder("  ");
            stringBuilder2.Append(this.Description);
            if (this.HasShortcut)
            {
                string empty = ':'.ToString();
                if (this.IsFlag)
                    empty = string.Empty;
                string str = string.Format((IFormatProvider)CultureInfo.InvariantCulture, "Short form is '{0}{1}{2}'.", (object)'/', (object)this.Shortcut, (object)empty);
                stringBuilder2.AppendFormat((IFormatProvider)CultureInfo.InvariantCulture, "  {0}", (object)str);
            }
            stringBuilder1.AppendLine(CommandLineArgument.WrapLine(stringBuilder2.ToString()));
            Trace.TraceInformation("Exiting {0} with return value {1}", (object)MethodBase.GetCurrentMethod().Name, (object)stringBuilder1.ToString());
            return stringBuilder1.ToString();
        }

        internal string ToSampleString()
        {
            return this.ToSwitchString(this.SampleUsageValue);
        }

        private string ToDescriptionString()
        {
            if (this.IsFlag)
                return this.ToSwitchString(string.Empty);
            return this.ToSwitchString(this.ParameterDescription);
        }

        private string ToSwitchString(string value)
        {
            string format = " {0}{1}{2}{3}";
            string empty = ':'.ToString();
            if (this.IsFlag)
                empty = string.Empty;
            return string.Format((IFormatProvider)CultureInfo.InvariantCulture, format, (object)'/', (object)this.Name, (object)empty, (object)value);
        }

        private static string ToNullableString(object value)
        {
            if (value == null)
                return "<NULL>";
            return value.ToString();
        }

        internal static string WrapLine(string text)
        {
            int? wrapLength = CommandLineArgument.WrapLength;
            if (!wrapLength.HasValue)
                return text;
            string[] strArray = text.Split((char[])null);
            StringBuilder stringBuilder = new StringBuilder();
            int num1 = 0;
            foreach (string str in strArray)
            {
                int length = str.Length;
                int num2 = num1 + length + 1;
                int? nullable = wrapLength;
                int valueOrDefault = nullable.GetValueOrDefault();
                if (num2 >= valueOrDefault & nullable.HasValue)
                {
                    num1 = length + 1;
                    stringBuilder.Append("\n  " + str);
                }
                else
                {
                    num1 += length + 1;
                    stringBuilder.Append(str);
                }
                stringBuilder.Append(' ');
            }
            return stringBuilder.ToString();
        }
    }
}
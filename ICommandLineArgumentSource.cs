namespace Microsoft.Crm.Services.Utility
{
    internal interface ICommandLineArgumentSource
    {
        void OnUnknownArgument(string argumentName, string argumentValue);

        void OnInvalidArgument(string argument);
    }
}
using System;
using System.ComponentModel;

namespace Microsoft.Crm.Services.Utility
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface ICodeWriterMessageFilterService
    {
        bool GenerateSdkMessage(SdkMessage sdkMessage, IServiceProvider services);

        bool GenerateSdkMessagePair(SdkMessagePair sdkMessagePair, IServiceProvider services);
    }
}
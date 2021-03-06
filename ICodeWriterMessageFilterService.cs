using System;
using System.ComponentModel;

namespace Microsoft.Crm.Services.Utility
{
    /// <summary>Interface for code writer message filter service</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface ICodeWriterMessageFilterService
    {
        /// <summary>
        /// Returns true to generate code for the SDK Message and false otherwise.
        /// </summary>
        bool GenerateSdkMessage(SdkMessage sdkMessage, IServiceProvider services);

        /// <summary>
        /// Returns true to generate code for the SDK Message Pair and false otherwise.
        /// </summary>
        bool GenerateSdkMessagePair(SdkMessagePair sdkMessagePair, IServiceProvider services);
    }
}
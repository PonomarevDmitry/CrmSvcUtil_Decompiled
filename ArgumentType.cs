using System;

namespace Microsoft.Crm.Services.Utility
{
    [Flags]
    internal enum ArgumentType
    {
        Optional = 1,
        Required = 2,
        Multiple = 4,
        Binary = 8,
        Hidden = 16, // 0x00000010
    }
}
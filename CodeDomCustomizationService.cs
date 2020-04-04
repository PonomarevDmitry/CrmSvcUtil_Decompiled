using System;
using System.CodeDom;

namespace Microsoft.Crm.Services.Utility
{
    internal sealed class CodeDomCustomizationService : ICustomizeCodeDomService
    {
        internal CodeDomCustomizationService()
        {
        }

        void ICustomizeCodeDomService.CustomizeCodeDom(
          CodeCompileUnit codeUnit,
          IServiceProvider services)
        {
        }
    }
}
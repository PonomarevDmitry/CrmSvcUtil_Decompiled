using System;
using System.CodeDom;

namespace Microsoft.Crm.Services.Utility
{
    public interface ICustomizeCodeDomService
    {
        void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services);
    }
}
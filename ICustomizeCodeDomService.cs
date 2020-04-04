using System;
using System.CodeDom;

namespace Microsoft.Crm.Services.Utility
{
    /// <summary>
    /// Interface that can be used to customize the CodeDom before it generates code.
    /// </summary>
    public interface ICustomizeCodeDomService
    {
        /// <summary>
        /// Customize the generated types before code is generated
        /// </summary>
        void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services);
    }
}
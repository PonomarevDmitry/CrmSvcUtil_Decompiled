using System;
using System.Collections.Generic;

namespace Microsoft.Crm.Services.Utility
{
    internal sealed class ServiceProvider : IServiceProvider
    {
        private Dictionary<Type, object> _services;

        internal ServiceProvider()
        {
            this._services = new Dictionary<Type, object>();
        }

        internal ITypeMappingService TypeMappingService
        {
            get
            {
                return (ITypeMappingService)this._services[typeof(ITypeMappingService)];
            }
        }

        internal IMetadataProviderService MetadataProviderService
        {
            get
            {
                return (IMetadataProviderService)this._services[typeof(IMetadataProviderService)];
            }
        }

        internal IMetadataProviderQueryService MetadataProviderQueryServcie
        {
            get
            {
                return (IMetadataProviderQueryService)this._services[typeof(IMetadataProviderQueryService)];
            }
        }

        internal ICustomizeCodeDomService CodeCustomizationService
        {
            get
            {
                return (ICustomizeCodeDomService)this._services[typeof(ICustomizeCodeDomService)];
            }
        }

        internal ICodeGenerationService CodeGenerationService
        {
            get
            {
                return (ICodeGenerationService)this._services[typeof(ICodeGenerationService)];
            }
        }

        internal ICodeWriterFilterService CodeFilterService
        {
            get
            {
                return (ICodeWriterFilterService)this._services[typeof(ICodeWriterFilterService)];
            }
        }

        internal ICodeWriterMessageFilterService CodeMessageFilterService
        {
            get
            {
                return (ICodeWriterMessageFilterService)this._services[typeof(ICodeWriterMessageFilterService)];
            }
        }

        internal INamingService NamingService
        {
            get
            {
                return (INamingService)this._services[typeof(INamingService)];
            }
        }

        object IServiceProvider.GetService(Type serviceType)
        {
            if (this._services.ContainsKey(serviceType))
                return this._services[serviceType];
            return (object)null;
        }

        internal void InitializeServices(CrmSvcUtilParameters parameters)
        {
            CodeWriterFilterService writerFilterService = new CodeWriterFilterService(parameters);
            this._services.Add(typeof(ICodeWriterFilterService), (object)ServiceFactory.CreateInstance<ICodeWriterFilterService>((ICodeWriterFilterService)writerFilterService, parameters.CodeWriterFilterService, parameters));
            this._services.Add(typeof(ICodeWriterMessageFilterService), (object)ServiceFactory.CreateInstance<ICodeWriterMessageFilterService>((ICodeWriterMessageFilterService)writerFilterService, parameters.CodeWriterMessageFilterService, parameters));
            this._services.Add(typeof(IMetadataProviderService), (object)ServiceFactory.CreateInstance<IMetadataProviderService>((IMetadataProviderService)new SdkMetadataProviderService(parameters), parameters.MetadataProviderService, parameters));
            this._services.Add(typeof(IMetadataProviderQueryService), (object)ServiceFactory.CreateInstance<IMetadataProviderQueryService>((IMetadataProviderQueryService)new MetadataProviderQueryService(parameters), parameters.MetadataQueryProvider, parameters));
            this._services.Add(typeof(ICodeGenerationService), (object)ServiceFactory.CreateInstance<ICodeGenerationService>((ICodeGenerationService)new Microsoft.Crm.Services.Utility.CodeGenerationService(), parameters.CodeGenerationService, parameters));
            this._services.Add(typeof(INamingService), (object)ServiceFactory.CreateInstance<INamingService>((INamingService)new Microsoft.Crm.Services.Utility.NamingService(parameters), parameters.NamingService, parameters));
            this._services.Add(typeof(ICustomizeCodeDomService), (object)ServiceFactory.CreateInstance<ICustomizeCodeDomService>((ICustomizeCodeDomService)new CodeDomCustomizationService(), parameters.CodeCustomizationService, parameters));
            this._services.Add(typeof(ITypeMappingService), (object)new Microsoft.Crm.Services.Utility.TypeMappingService(parameters));
        }
    }
}
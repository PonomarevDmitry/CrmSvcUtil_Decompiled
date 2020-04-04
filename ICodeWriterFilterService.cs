using Microsoft.Xrm.Sdk.Metadata;
using System;

namespace Microsoft.Crm.Services.Utility
{
    /// <summary>
    /// Interface that can be used to filter out specific pieces of metadata from having code generated for it.
    /// </summary>
    public interface ICodeWriterFilterService
    {
        /// <summary>
        /// Returns true to generate code for the OptionSet and false otherwise.
        /// </summary>
        bool GenerateOptionSet(OptionSetMetadataBase optionSetMetadata, IServiceProvider services);

        /// <summary>
        /// Returns true to generate code for the Option and false otherwise.
        /// </summary>
        bool GenerateOption(OptionMetadata optionMetadata, IServiceProvider services);

        /// <summary>
        /// Returns true to generate code for the Entity and false otherwise.
        /// </summary>
        bool GenerateEntity(EntityMetadata entityMetadata, IServiceProvider services);

        /// <summary>
        /// Returns true to generate code for the Attribute and false otherwise.
        /// </summary>
        bool GenerateAttribute(AttributeMetadata attributeMetadata, IServiceProvider services);

        /// <summary>
        /// Returns true to generate code for the 1:N, N:N, or N:1 relationship and false otherwise.
        /// </summary>
        bool GenerateRelationship(
          RelationshipMetadataBase relationshipMetadata,
          EntityMetadata otherEntityMetadata,
          IServiceProvider services);

        /// <summary>
        /// Returns true to generate code for the data context and false otherwise.
        /// </summary>
        bool GenerateServiceContext(IServiceProvider services);
    }
}
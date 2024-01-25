using System;
using Tableau.Migration.Api.Rest.Models;

namespace Tableau.Migration.Content.Permissions
{
    /// <summary>
    /// The interface for content permissions responses.
    /// </summary>
    public interface IPermissions
    {
        /// <summary>
        /// The collection of Grantee Capabilities for this content item.
        /// </summary>
        IGranteeCapability[] GranteeCapabilities { get; set; }

        /// <summary>
        /// The ID of the parent content item 
        /// The parent content can be one of the types in 
        /// <see cref="ParentContentTypeNames"/>.
        /// </summary>
        public Guid? ParentId { get; set; }
    }
}

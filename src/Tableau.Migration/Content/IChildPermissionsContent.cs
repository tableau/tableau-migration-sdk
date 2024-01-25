using System;
using System.Collections.Generic;

namespace Tableau.Migration.Content
{
    /// <summary>
    /// Interface to be inherited by content items that have permissions.
    /// </summary>
    public interface IChildPermissionsContent : IContentReference, IPermissionsContent
    {
        /// <summary>
        /// Gets the type of the child items.
        /// </summary>
        Type ChildType { get; }

        /// <summary>
        /// Gets the child items of the content item that have permissions
        /// </summary>
        IEnumerable<IContentReference> ChildPermissionContentItems { get; }

        /// <summary>
        /// Gets whether or not child item permissions should be migrated.
        /// </summary>
        bool ShouldMigrateChildPermissions { get; }
    }
}

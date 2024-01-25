using System.Collections.Generic;

namespace Tableau.Migration.Content
{
    /// <summary>
    /// Interface for a group content item with users.
    /// </summary>
    public interface IPublishableGroup : IGroup
    {
        /// <summary>
        /// Gets or sets the users assigned to the group.
        /// </summary>
        IList<IGroupUser> Users { get; set; }
    }
}
using System.Collections.Generic;
using System.Linq;

namespace Tableau.Migration.Content
{
    internal sealed class PublishableGroup : Group, IPublishableGroup
    {
        /// <inheritdoc/>
        public IList<IGroupUser> Users { get; set; }

        public PublishableGroup(IGroup group, IEnumerable<IGroupUser> users)
            : base(group)
        {
            Users = users.ToList();
        }
    }
}

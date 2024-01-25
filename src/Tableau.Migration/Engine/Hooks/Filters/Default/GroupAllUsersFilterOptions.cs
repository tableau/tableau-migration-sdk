using System.Collections.Generic;
using System.Linq;

namespace Tableau.Migration.Engine.Hooks.Filters.Default
{
    /// <summary>
    /// Options for <see cref="GroupAllUsersFilter"/>.
    /// </summary>
    public class GroupAllUsersFilterOptions
    {
        /// <summary>
        /// Gets or sets the translated names of the "All Users" group.
        /// </summary>
        public List<string> AllUsersGroupNames { get; init; } = new List<string>();

        /// <summary>
        /// Creates a new <see cref="GroupAllUsersFilterOptions"/> instance.
        /// </summary>
        public GroupAllUsersFilterOptions()
        { }

        /// <summary>
        /// Creates a new <see cref="GroupAllUsersFilterOptions"/> instance.
        /// </summary>
        /// <param name="allUsersGroupNames">The "All Users" group name translations.</param>
        public GroupAllUsersFilterOptions(IEnumerable<string> allUsersGroupNames)
        {
            AllUsersGroupNames.AddRange(allUsersGroupNames.Distinct());
        }
    }
}

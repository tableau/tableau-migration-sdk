using System.Collections.Immutable;
using System.Linq;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;

namespace Tableau.Migration.Api
{
    internal static class GroupsResponseExtensions
    {
        internal static IImmutableList<IGroup> GetGroupsFromResponse(this GroupsResponse response)
        {
            // Take all items
            return response.Items
                // Convert them all to type Group
                .Select(g => new Group(g))
                // Produce immutable list of type IGroup and return
                .ToImmutableArray<IGroup>();
        }
    }
}

using System.Collections.Immutable;
using System.Linq;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;

namespace Tableau.Migration.Api
{
    internal static class UsersResponseExtensions
    {
        internal static IImmutableList<IUser> GetUsersFromResponse(this UsersResponse response)
        {
            // Take all items
            return response.Items
                // Convert them all to type User
                .Select(u => new User(u))
                // Produce immutable list of type IGroup and return
                .ToImmutableArray<IUser>();
        }
    }
}

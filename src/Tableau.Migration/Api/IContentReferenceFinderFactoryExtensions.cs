using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Search;

namespace Tableau.Migration.Api
{
    internal static class IContentReferenceFinderFactoryExtensions
    {
        public static async Task<IContentReference> FindProjectAsync(
            this IContentReferenceFinderFactory finderFactory,
            [NotNull] IWithProjectType? response,
            CancellationToken cancel)
        {
            Guard.AgainstNull(response, nameof(response));

            var project = Guard.AgainstNull(response.Project, () => nameof(response.Project));
            var projectId = Guard.AgainstDefaultValue(project.Id, () => nameof(response.Project.Id));

            var projectFinder = finderFactory.ForContentType<IProject>();

            var foundProject = await projectFinder.FindByIdAsync(projectId, cancel).ConfigureAwait(false);

            return Guard.AgainstNull(foundProject, nameof(foundProject));
        }

        public static async Task<IContentReference> FindOwnerAsync(
            this IContentReferenceFinderFactory finderFactory,
            [NotNull] IWithOwnerType? response,
            CancellationToken cancel)
        {
            Guard.AgainstNull(response, nameof(response));

            var owner = Guard.AgainstNull(response.Owner, () => nameof(response.Owner));
            var ownerId = Guard.AgainstDefaultValue(owner.Id, () => nameof(response.Owner.Id));

            var userFinder = finderFactory.ForContentType<IUser>();

            var foundOwner = await userFinder.FindByIdAsync(ownerId, cancel).ConfigureAwait(false);

            return Guard.AgainstNull(foundOwner, nameof(foundOwner));
        }
    }
}

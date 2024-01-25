using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Api
{
    internal abstract class ContentApiClientBase : ApiClientBase, IContentApiClient
    {
        protected readonly IContentReferenceFinderFactory ContentFinderFactory;

        protected readonly Lazy<IContentReferenceFinder<IProject>> ProjectFinder;

        protected readonly Lazy<IContentReferenceFinder<IUser>> UserFinder;

        public string UrlPrefix { get; }

        public ContentApiClientBase(
            IRestRequestBuilderFactory restRequestBuilderFactory,
            IContentReferenceFinderFactory finderFactory,
            ILoggerFactory loggerFactory,
            ISharedResourcesLocalizer sharedResourcesLocalizer,
            string? urlPrefix = null)
            : base(restRequestBuilderFactory, loggerFactory, sharedResourcesLocalizer)
        {
            UrlPrefix = urlPrefix ?? RestUrlPrefixes.GetUrlPrefix(GetType());

            ContentFinderFactory = finderFactory;
            ProjectFinder = new(ContentFinderFactory.ForContentType<IProject>);
            UserFinder = new(ContentFinderFactory.ForContentType<IUser>);
        }

        protected async Task<IContentReference> FindProjectAsync(
            [NotNull] IWithProjectType? response,
            CancellationToken cancel)
        {
            Guard.AgainstNull(response, nameof(response));

            var project = Guard.AgainstNull(response.Project, () => nameof(response.Project));
            var projectId = Guard.AgainstDefaultValue(project.Id, () => nameof(response.Project.Id));

            var foundProject = await ProjectFinder.Value.FindByIdAsync(projectId, cancel).ConfigureAwait(false);

            return Guard.AgainstNull(foundProject, nameof(foundProject));
        }

        protected async Task<IContentReference> FindOwnerAsync(
            [NotNull] IWithOwnerType? response,
            CancellationToken cancel)
        {
            Guard.AgainstNull(response, nameof(response));
            
            var owner = Guard.AgainstNull(response.Owner, () => nameof(response.Owner));
            var ownerId = Guard.AgainstDefaultValue(owner.Id, () => nameof(response.Owner.Id));

            var foundOwner = await UserFinder.Value.FindByIdAsync(ownerId, cancel).ConfigureAwait(false);

            return Guard.AgainstNull(foundOwner, nameof(foundOwner));
        }
    }
}

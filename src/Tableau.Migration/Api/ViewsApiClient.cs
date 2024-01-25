using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Permissions;
using Tableau.Migration.Api.Tags;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Api
{
    internal sealed class ViewsApiClient : ContentApiClientBase, IViewsApiClient
    {
        public ViewsApiClient(
            IRestRequestBuilderFactory restRequestBuilderFactory,
            IPermissionsApiClientFactory permissionsClientFactory,
            IContentReferenceFinderFactory finderFactory,
            ILoggerFactory loggerFactory,
            ISharedResourcesLocalizer sharedResourcesLocalizer,
            ITagsApiClientFactory tagsClientFactory)
            : base(restRequestBuilderFactory, finderFactory, loggerFactory, sharedResourcesLocalizer)
        {
            Permissions = permissionsClientFactory.Create(this);
            Tags = tagsClientFactory.Create(this);
        }

        #region - IPermissionsContentApiClientImplementation -

        /// <inheritdoc />
        public IPermissionsApiClient Permissions { get; }

        #endregion

        #region - ITagsContentApiClient Implementation -

        /// <inheritdoc />
        public ITagsApiClient Tags { get; }

        #endregion
    }
}
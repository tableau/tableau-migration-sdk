using System;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Permissions;
using Tableau.Migration.Api.Tags;
using Tableau.Migration.Content;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Interface for authenticated site-specific API clients
    /// </summary>
    public interface ISitesApiClient : IAsyncDisposable, IContentApiClient
    {
        /// <summary>
        /// The API client for group operations.
        /// </summary>
        IGroupsApiClient Groups { get; }

        /// <summary>
        /// The API client for job operations.
        /// </summary>
        IJobsApiClient Jobs { get; }

        /// <summary>
        /// The API client for project operations.
        /// </summary>
        IProjectsApiClient Projects { get; }

        /// <summary>
        /// The API client for user operations.
        /// </summary>
        IUsersApiClient Users { get; }

        /// <summary>
        /// The API client for data source operations.
        /// </summary>
        IDataSourcesApiClient DataSources { get; }

        /// <summary>
        /// The API client for workbook operations.
        /// </summary>
        IWorkbooksApiClient Workbooks { get; }

        /// <summary>
        /// The API client for views operations.
        /// </summary>
        IViewsApiClient Views { get; }

        /// <summary>
        /// Gets the site with the specified ID.
        /// </summary>
        /// <param name="siteId">The site's ID.</param>
        /// <param name="cancel">The cancellation token.</param>
        /// <returns>The site with the specified ID.</returns>
        Task<IResult<ISite>> GetSiteAsync(Guid siteId, CancellationToken cancel);

        /// <summary>
        /// Gets the site with the specified content URL.
        /// </summary>
        /// <param name="contentUrl">The site's content URL.</param>
        /// <param name="cancel">The cancellation token.</param>
        /// <returns>The site with the specified content URL.</returns>
        Task<IResult<ISite>> GetSiteAsync(string contentUrl, CancellationToken cancel);

        /// <summary>
        /// Gets the <see cref="IPagedListApiClient{TContent}"/> for the given content type.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <returns>The list API client for the given content type.</returns>
        /// <exception cref="ArgumentException">If a list API client for the given content type is not supported.</exception>
        IPagedListApiClient<TContent> GetListApiClient<TContent>();

        /// <summary>
        /// Gets the <see cref="IPullApiClient{TContent, TPublish}"/> for the given content and publish types.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <typeparam name="TPublish">The publish type.</typeparam>
        /// <returns>The pull API client for the given content and publish types.</returns>
        /// <exception cref="ArgumentException">If a pull API client for the given types is not supported.</exception>
        IPullApiClient<TContent, TPublish> GetPullApiClient<TContent, TPublish>()
            where TPublish : class;

        /// <summary>
        /// Gets the <see cref="IPublishApiClient{TPublish, TPublishResult}"/> for the given content publish type.
        /// </summary>
        /// <typeparam name="TPublish">The content publish type.</typeparam>
        /// <typeparam name="TPublishResult">The publish result type.</typeparam>
        /// <returns>The publish API client for the given content publish type.</returns>
        /// <exception cref="ArgumentException">If a publish API client for the given content publish type is not supported.</exception>
        IPublishApiClient<TPublish, TPublishResult> GetPublishApiClient<TPublish, TPublishResult>()
            where TPublishResult : class, IContentReference;

        /// <summary>
        /// Gets the <see cref="IBatchPublishApiClient{TPublish}"/> for the given content publish type.
        /// </summary>
        /// <typeparam name="TPublish">The content publish type.</typeparam>
        /// <returns>The batch publish API client for the given content publish type.</returns>
        /// <exception cref="ArgumentException">If a batch publish API client for the given content publish type is not supported.</exception>
        IBatchPublishApiClient<TPublish> GetBatchPublishApiClient<TPublish>();

        /// <summary>
        /// Gets the <see cref="IPermissionsApiClient"/> for the given content type.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <returns>The permissions API client for the given content type.</returns>
        /// <exception cref="ArgumentException">If a permissions API client for the given content type is not supported.</exception>
        IPermissionsApiClient GetPermissionsApiClient<TContent>();

        /// <summary>
        /// Gets the <see cref="IPermissionsApiClient"/> for the given content type.
        /// </summary>
        /// <returns>The permissions API client for the given content type.</returns>
        /// <exception cref="ArgumentException">If a permissions API client for the given content type is not supported.</exception>
        IPermissionsApiClient GetPermissionsApiClient(Type type);

        /// <summary>
        /// Gets the <see cref="ITagsApiClient"/> for the given content type.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <returns>The tags API client for the given content type.</returns>
        /// <exception cref="ArgumentException">If a tags API client for the given content type is not supported.</exception>
        ITagsApiClient GetTagsApiClient<TContent>()
            where TContent : IWithTags;

        /// <summary>
        /// Gets the <see cref="IOwnershipApiClient"/> for the given content type.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <returns>The ownership API client for the given content type.</returns>
        /// <exception cref="ArgumentException">If a ownership API client for the given content type is not supported.</exception>
        IOwnershipApiClient GetOwnershipApiClient<TContent>()
            where TContent : IWithOwner;

        /// <summary>
        /// Gets the <see cref="IConnectionsApiClient"/> for the given content type.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <returns>The embedded connections API client for the given content type.</returns>
        /// <exception cref="ArgumentException">If a ownership API client for the given content type is not supported.</exception>
        IConnectionsApiClient GetConnectionsApiClient<TContent>()
            where TContent : IWithConnections;
    }
}

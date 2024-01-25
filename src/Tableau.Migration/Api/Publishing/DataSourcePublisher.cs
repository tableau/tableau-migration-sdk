using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Api.Publishing
{
    internal class DataSourcePublisher : FilePublisherBase<IPublishDataSourceOptions, CommitDataSourcePublishRequest, IDataSource>, IDataSourcePublisher
    {
        public DataSourcePublisher(
            IRestRequestBuilderFactory restRequestBuilderFactory,
            IContentReferenceFinderFactory finderFactory,
            IServerSessionProvider sessionProvider,
            ISharedResourcesLocalizer sharedResourcesLocalizer,
            IHttpStreamProcessor httpStreamProcessor)
            : base(
                  restRequestBuilderFactory,
                  finderFactory,
                  sessionProvider,
                  sharedResourcesLocalizer,
                  httpStreamProcessor,
                  RestUrlPrefixes.DataSources)
        { }

        protected override CommitDataSourcePublishRequest BuildCommitRequest(IPublishDataSourceOptions options)
            => new(options);

        protected override async Task<IResult<IDataSource>> SendCommitRequestAsync(
            IPublishDataSourceOptions options,
            string uploadSessionId,
            MultipartContent content,
            CancellationToken cancel)
        {
            var request = RestRequestBuilderFactory
               .CreateUri(ContentTypeUrlPrefix)
               .WithQuery("uploadSessionId", uploadSessionId)
               .WithQuery("datasourceType", options.FileType)
               .WithQuery("overwrite", options.Overwrite.ToString().ToLower())
               .ForPostRequest()
               .WithContent(content);

            var result = await request
                .SendAsync<DataSourceResponse>(cancel)
                    .ToResultAsync<DataSourceResponse, IDataSource>(async (r, c) =>
                    {
                        var project = await ContentFinderFactory.FindProjectAsync(r.Item, c).ConfigureAwait(false);
                        var owner = await ContentFinderFactory.FindOwnerAsync(r.Item, c).ConfigureAwait(false);
                        return new DataSource(r.Item, project, owner);
                    },
                    SharedResourcesLocalizer,
                    cancel)
                .ConfigureAwait(false);

            return result;
        }
    }
}

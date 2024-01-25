using System.Collections.Immutable;
using System.Linq;
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
    internal class WorkbookPublisher : FilePublisherBase<IPublishWorkbookOptions, CommitWorkbookPublishRequest, IResultWorkbook>, IWorkbookPublisher
    {
        public WorkbookPublisher(
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
                  RestUrlPrefixes.Workbooks)
        { }

        protected override CommitWorkbookPublishRequest BuildCommitRequest(IPublishWorkbookOptions options)
            => new(options);

        protected override async Task<IResult<IResultWorkbook>> SendCommitRequestAsync(
            IPublishWorkbookOptions options,
            string uploadSessionId,
            MultipartContent content,
            CancellationToken cancel)
        {
            var request = RestRequestBuilderFactory
               .CreateUri(ContentTypeUrlPrefix)
               .WithQuery("uploadSessionId", uploadSessionId)
               .WithQuery("skipConnectionCheck", options.SkipConnectionCheck.ToString().ToLower())
               .WithQuery("workbookType", options.FileType)
               .WithQuery("overwrite", options.Overwrite.ToString().ToLower())
               .ForPostRequest()
               .WithContent(content);

            var result = await request
                .SendAsync<WorkbookResponse>(cancel)
                    .ToResultAsync<WorkbookResponse, IResultWorkbook>(async (r, c) =>
                    {
                        var project = await ContentFinderFactory.FindProjectAsync(r.Item, c).ConfigureAwait(false);
                        var owner = await ContentFinderFactory.FindOwnerAsync(r.Item, c).ConfigureAwait(false);
                        var views = r.Item.Views.Select(v => (IView)new View(v, project, r.Item.Name))
                            .ToImmutableArray();

                        return new ResultWorkbook(r.Item, project, owner, views);
                    },
                    SharedResourcesLocalizer,
                    cancel)
                .ConfigureAwait(false);

            return result;
        }
    }
}

// Copyright (c) 2023, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the ""License"") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an ""AS IS"" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

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

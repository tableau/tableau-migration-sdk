//
//  Copyright (c) 2024, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the "License") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Publishing;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Api
{
    internal class CustomViewPublisher :
        FilePublisherBase<IPublishCustomViewOptions, CommitCustomViewPublishRequest, ICustomView>,
        ICustomViewPublisher
    {
        public CustomViewPublisher(
        IRestRequestBuilderFactory restRequestBuilderFactory,
        IContentReferenceFinderFactory finderFactory,
        IServerSessionProvider sessionProvider,
        ILoggerFactory loggerFactory,
        ISharedResourcesLocalizer sharedResourcesLocalizer,
        IHttpStreamProcessor httpStreamProcessor)
        : base(
              restRequestBuilderFactory,
              finderFactory,
              sessionProvider,
              loggerFactory,
              sharedResourcesLocalizer,
              httpStreamProcessor,
              RestUrlPrefixes.CustomViews)
        { }

        protected override CommitCustomViewPublishRequest BuildCommitRequest(IPublishCustomViewOptions options)
            => new(options);

        protected async override Task<IResult<ICustomView>> SendCommitRequestAsync(
            IPublishCustomViewOptions options,
            string uploadSessionId,
            MultipartContent content,
            CancellationToken cancel)
        {
            var sendResult = await RestRequestBuilderFactory
               .CreateUri(ContentTypeUrlPrefix, useExperimental: true)
               .WithQuery("uploadSessionId", uploadSessionId)
               .ForPostRequest()
               .WithContent(content)
               .SendAsync<CustomViewResponse>(cancel)
               .ConfigureAwait(false);

            var result = sendResult
                .ToResult<CustomViewResponse, ICustomView>(r =>
                {
                    var workbook = new ContentReferenceStub(r.Item!.Workbook!.Id, string.Empty, new ContentLocation());
                    var owner = new ContentReferenceStub(r.Item!.Owner!.Id, string.Empty, new ContentLocation());

                    return new CustomView(r.Item, workbook, owner);
                },
                SharedResourcesLocalizer);

            return result;
        }
    }
}

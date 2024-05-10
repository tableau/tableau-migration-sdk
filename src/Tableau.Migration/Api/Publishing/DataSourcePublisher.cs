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
    internal class DataSourcePublisher : FilePublisherBase<IPublishDataSourceOptions, CommitDataSourcePublishRequest, IDataSourceDetails>, IDataSourcePublisher
    {
        public DataSourcePublisher(
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
                  RestUrlPrefixes.DataSources)
        { }

        protected override CommitDataSourcePublishRequest BuildCommitRequest(IPublishDataSourceOptions options)
            => new(options);

        protected override async Task<IResult<IDataSourceDetails>> SendCommitRequestAsync(
            IPublishDataSourceOptions options,
            string uploadSessionId,
            MultipartContent content,
            CancellationToken cancel)
        {
            var result = await RestRequestBuilderFactory
               .CreateUri(ContentTypeUrlPrefix)
               .WithQuery("uploadSessionId", uploadSessionId)
               .WithQuery("datasourceType", options.FileType)
               .WithQuery("overwrite", options.Overwrite.ToString().ToLower())
               .ForPostRequest()
               .WithContent(content)
               .SendAsync<DataSourceResponse>(cancel)
               .ToResultAsync<DataSourceResponse, IDataSourceDetails>(async (response, cancel) =>
                {
                    var dataSource = Guard.AgainstNull(response.Item, () => response.Item);

                    var project = await ContentFinderFactory.FindProjectAsync(dataSource, Logger, SharedResourcesLocalizer, true, cancel).ConfigureAwait(false);
                    var owner = await ContentFinderFactory.FindOwnerAsync(dataSource, Logger, SharedResourcesLocalizer, true, cancel).ConfigureAwait(false);

                    return new DataSourceDetails(dataSource, project, owner);
                },
                SharedResourcesLocalizer,
                cancel)
                .ConfigureAwait(false);

            return result;
        }
    }
}

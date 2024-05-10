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

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Api
{
    internal class ConnectionManager : ApiClientBase, IConnectionManager
    {
        public ConnectionManager(
            IRestRequestBuilderFactory restRequestBuilderFactory,
            ILoggerFactory loggerFactory,
            ISharedResourcesLocalizer sharedResourcesLocalizer)
            : base(restRequestBuilderFactory, loggerFactory, sharedResourcesLocalizer)
        { }

        /// <inheritdoc/>
        public async Task<IResult<ImmutableList<IConnection>>> ListConnectionsAsync(
            string urlPrefix,
            Guid contentItemId,
            CancellationToken cancel)
        {
            var getResult = await RestRequestBuilderFactory
                .CreateUri($"{urlPrefix}/{contentItemId.ToUrlSegment()}/connections")
                .ForGetRequest()
                .SendAsync<ConnectionsResponse>(cancel)
                .ToResultAsync((response) =>
                {
                    var connections = response.Items;

                    if (!connections.Any())
                    {
                        return new List<IConnection>().ToImmutableList();
                    }

                    return connections.ToList().Select(c => (IConnection)new Connection(c)).ToImmutableList();
                },
                SharedResourcesLocalizer)
                .ConfigureAwait(false);

            return getResult;
        }

        /// <inheritdoc/>
        public async Task<IResult<IConnection>> UpdateConnectionAsync(
            string urlPrefix,
            Guid contentItemId,
            Guid connectionId,
            IUpdateConnectionOptions options,
            CancellationToken cancel)
        {
            var updateResult = await RestRequestBuilderFactory
                .CreateUri($"{urlPrefix}/{contentItemId.ToUrlSegment()}/connections/{connectionId.ToUrlSegment()}")
                .ForPutRequest()
                .WithXmlContent(new UpdateConnectionRequest(options))
                .SendAsync<ConnectionResponse>(cancel)
                .ToResultAsync<ConnectionResponse, IConnection>(
                (response) => new Connection(response.Item!),
                SharedResourcesLocalizer)
                .ConfigureAwait(false);

            return updateResult;
        }
    }
}

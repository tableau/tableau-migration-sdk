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
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Simulation.Rest.Net.Requests;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal class RestUpdateConnectionResponseBuilder<TSimulatedData> : RestApiResponseBuilderBase<ConnectionResponse>
        where TSimulatedData : SimulatedDataWithConnections
    {
        private readonly string ContentTypeUrlPrefix = string.Empty;
        private ConcurrentDictionary<Guid, byte[]> _files;
        public RestUpdateConnectionResponseBuilder(
            TableauData data,
            ConcurrentDictionary<Guid, byte[]> files,
            IHttpContentSerializer serializer,
            string contentTypeUrlPrefix)
            : base(data, serializer, requiresAuthentication: true)
        {
            _files = files;
            ContentTypeUrlPrefix = contentTypeUrlPrefix;
        }

        protected override ValueTask<(ConnectionResponse Response, HttpStatusCode ResponseCode)> BuildResponseAsync(
            HttpRequestMessage request,
            CancellationToken cancel)
        {
            var id = request.GetIdAfterSegment(ContentTypeUrlPrefix);
            if (id == null)
            {
                return ValueTask.FromResult(
                    BuildEmptyErrorResponse(HttpStatusCode.BadRequest, 0, "Invalid content item  ID.", string.Empty));
            }

            var simulatedContentItem = Encoding.Default
                    .GetString(_files[id.Value])
                    .FromXml<TSimulatedData>();

            if (simulatedContentItem == null)
            {
                return ValueTask.FromResult(
                   BuildEmptyErrorResponse(HttpStatusCode.NotFound, 0, $"No content item found for the content item ID {id}.", string.Empty));
            }


            var connections = simulatedContentItem.Connections;

            if (connections == null)
            {
                return ValueTask.FromResult(
                   BuildEmptyErrorResponse(HttpStatusCode.NotFound, 0, $"No connections found for the content item ID {id}.", string.Empty));
            }
            var connectionId = request.GetLastSegment();
            var connectionToUpdate = connections.FirstOrDefault(c => c.Id.ToString() == connectionId);

            if (connectionToUpdate == null)
            {
                return ValueTask.FromResult(
                   BuildEmptyErrorResponse(HttpStatusCode.NotFound, 0, $"No connection found for the connection ID {connectionId}.", string.Empty));
            }

            var connectionUpdate = request.GetTableauServerRequest<UpdateConnectionRequest>()?.Connection;

            if (connectionUpdate == null)
            {
                return ValueTask.FromResult(
                   BuildEmptyErrorResponse(HttpStatusCode.NotFound, 0, $"Invalid request.", string.Empty));
            }

            connectionToUpdate.Update(connectionUpdate);

            return ValueTask.FromResult((
                new ConnectionResponse()
                {
                    Item = new ConnectionResponse.ConnectionType(connectionToUpdate)
                }
                , HttpStatusCode.OK));
        }
    }
}

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

using System.Linq;
using System.Net;
using System.Net.Http;
using Tableau.Migration.Api.Simulation.Rest.Net;
using Tableau.Migration.Api.Simulation.Rest.Net.Requests;
using Tableau.Migration.Api.Simulation.Rest.Net.Responses;
using Tableau.Migration.Net.Simulation;
using static Tableau.Migration.Api.Simulation.Rest.Net.Requests.RestUrlPatterns;
using CloudResponse = Tableau.Migration.Api.Rest.Models.Responses.Cloud;
using ServerResponse = Tableau.Migration.Api.Rest.Models.Responses.Server;

namespace Tableau.Migration.Api.Simulation.Rest.Api
{
    /// <summary>
    /// Object that defines simulation of Tableau REST API schedule methods.
    /// </summary>
    public sealed class TasksRestApiSimulator
    {
        /// <summary>
        /// Gets the simulated schedule query API method.
        /// </summary>
        public MethodSimulator ListServerExtractRefreshTasks { get; }

        /// <summary>
        /// Creates a new <see cref="TasksRestApiSimulator"/> object.
        /// </summary>
        /// <param name="simulator">A response simulator to setup with REST API methods.</param>
        public TasksRestApiSimulator(TableauApiResponseSimulator simulator)
        {
            ListServerExtractRefreshTasks = 
                simulator.Data.IsTableauServer
                    ? simulator.SetupRestGetList<ServerResponse.ExtractRefreshTasksResponse, ServerResponse.ExtractRefreshTasksResponse.TaskType>(
                        SiteUrl("tasks/extractRefreshes"), 
                        (d,r) => d.ServerExtractRefreshTasks,
                        null,
                        true)
                    : simulator.SetupRestGetList<CloudResponse.ExtractRefreshTasksResponse, CloudResponse.ExtractRefreshTasksResponse.TaskType>(
                        SiteUrl("tasks/extractRefreshes"),
                        (d, r) => d.CloudExtractRefreshTasks,
                        null,
                        true);

            simulator.SetupRestPost(
                SiteUrl("tasks/extractRefreshes"), 
                new RestExtractRefreshTaskCreateResponseBuilder(simulator.Data, simulator.Serializer));

            simulator.SetupRestDelete(
                SiteEntityUrl("tasks/extractRefreshes"),
                new RestDeleteResponseBuilder(simulator.Data, DeleteExtractRefresh, simulator.Serializer));
        }

        private HttpStatusCode DeleteExtractRefresh(TableauData data, HttpRequestMessage request)
        {
            var extractRefreshId = request.GetIdAfterSegment("extractRefreshes");

            if (extractRefreshId is null)
            {
                return HttpStatusCode.BadRequest;
            }
            var extractRefreshTask = data.CloudExtractRefreshTasks.FirstOrDefault(cert => cert.ExtractRefresh!.Id == extractRefreshId);

            if (extractRefreshTask is not null)
            {
                data.CloudExtractRefreshTasks.Remove(extractRefreshTask);
            }

            return HttpStatusCode.NoContent;
        }
    }
}

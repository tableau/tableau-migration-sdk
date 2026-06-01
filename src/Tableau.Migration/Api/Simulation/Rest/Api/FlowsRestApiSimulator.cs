//
//  Copyright (c) 2025, Salesforce, Inc.
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
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Simulation.Rest.Net;
using Tableau.Migration.Api.Simulation.Rest.Net.Requests;
using Tableau.Migration.Api.Simulation.Rest.Net.Responses;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Simulation;

using static Tableau.Migration.Api.Simulation.Rest.Net.Requests.RestUrlPatterns;

namespace Tableau.Migration.Api.Simulation.Rest.Api
{
    /// <summary>
    /// Object that defines simulation of Tableau REST API flow methods.
    /// </summary>
    public sealed class FlowsRestApiSimulator : EmbeddedCredentialsRestApiSimulatorBase<FlowResponse.FlowType, FlowResponse.FlowType.TagType>
    {
        /// <summary>
        /// Gets the simulated flow query API method.
        /// </summary>
        public MethodSimulator QueryFlow { get; }

        /// <summary>
        /// Gets the simulated flows query API method.
        /// </summary>
        public MethodSimulator QueryFlows { get; }

        /// <summary>
        /// Gets the simulated flow download API method.
        /// </summary>
        public MethodSimulator DownloadFlow { get; }

        /// <summary>
        /// Gets the simulated commit flow upload API method.
        /// </summary>
        public MethodSimulator CommitFlowUpload { get; }

        /// <summary>
        /// Gets the simulated update flow API method.
        /// </summary>
        public MethodSimulator UpdateFlow { get; }

        /// <summary>
        /// Gets the simulated list connections API method.
        /// </summary>
        public MethodSimulator QueryFlowConnections { get; }

        /// <summary>
        /// Gets the simulated update connection API method.
        /// </summary>
        public MethodSimulator UpdateConnectionAsync { get; }

        /// <summary>
        /// Creates a new <see cref="FlowsRestApiSimulator"/> object.
        /// </summary>
        /// <param name="simulator">A response simulator to setup with REST API methods.</param>
        public FlowsRestApiSimulator(TableauApiResponseSimulator simulator)
            : base(simulator, RestUrlKeywords.Flows,
                  data => data.Flows, data => data.FlowKeychains)
        {
            QueryFlow = simulator.SetupRestGet<FlowResponse, FlowResponse.FlowType>(
                SiteEntityUrl(ContentTypeUrlPrefix),
                BuildQueryFlowDelegate());

            QueryFlows = simulator.SetupRestPagedList<FlowsResponse, FlowsResponse.FlowType>(
                 SiteUrl(ContentTypeUrlPrefix),
                 (data) => data.Flows.Select(f => new FlowsResponse.FlowType(f)).ToList());

            CommitFlowUpload = simulator.SetupRestPost(
                SiteUrl(RestUrlKeywords.Flows),
                new RestCommitFlowUploadResponseBuilder(simulator.Data, simulator.Serializer),
                SiteCommitFileUploadQueryString("flowType"));

            DownloadFlow = simulator.SetupRestDownloadById(
                SiteEntityUrl(ContentTypeUrlPrefix, RestUrlKeywords.Content),
                (data) => data.FlowFiles, 4);

            UpdateFlow = simulator.SetupRestPut<UpdateFlowResponse, UpdateFlowResponse.FlowType>(
                SiteEntityUrl(ContentTypeUrlPrefix),
                BuildUpdateFlowDelegate());

            QueryFlowConnections = simulator.SetupRestGetList<ConnectionsResponse, ConnectionsResponse.ConnectionType>(
                SiteEntityUrl(ContentTypeUrlPrefix, RestUrlKeywords.Connections),
                BuildListConnectionsDelegate());

            UpdateConnectionAsync = simulator.SetupRestPut(
                SiteEntityUrl(ContentTypeUrlPrefix, $"{RestUrlKeywords.Connections}/{new Regex(GuidPattern, RegexOptions.IgnoreCase)}"),
                new RestUpdateConnectionResponseBuilder<SimulatedFlowData>(
                    simulator.Data,
                    simulator.Data.FlowFiles,
                    simulator.Serializer,
                    ContentTypeUrlPrefix));
        }

        #region - Response Delegates -

        private Func<TableauData, HttpRequestMessage, ICollection<ConnectionsResponse.ConnectionType>> BuildListConnectionsDelegate()
        {
            return (data, request) =>
            {
                var id = request.GetIdAfterSegment(ContentTypeUrlPrefix) ?? throw new ArgumentNullException($"Flow ID should not be null");
                if (!data.FlowFiles.ContainsKey(id))
                {
                    throw new InvalidOperationException($"No data found for Flow ID {id}");
                }

                var flowFile = data.FlowFiles[id];
                var flowFileText = Encoding.Default.GetString(flowFile);
                if (string.IsNullOrEmpty(flowFileText))
                {
                    return Array.Empty<ConnectionsResponse.ConnectionType>();
                }

                try
                {
                    using var _ = JsonDocument.Parse(flowFileText);
                    // Connection list could be built from root.GetProperty("connections") if needed; for now return empty.
                    return Array.Empty<ConnectionsResponse.ConnectionType>();
                }
                catch (JsonException)
                {
                    return Array.Empty<ConnectionsResponse.ConnectionType>();
                }
            };
        }

        private Func<TableauData, HttpRequestMessage, FlowResponse.FlowType?> BuildQueryFlowDelegate()
        {
            return (data, request) =>
            {
                var flowId = request.GetIdAfterSegment(ContentTypeUrlPrefix);

                if (flowId is null)
                    return null;

                var flow = data.Flows.FirstOrDefault(f => f.Id == flowId.Value);

                if (flow is null)
                    return null;

                return new FlowResponse.FlowType
                {
                    Id = flow.Id,
                    Name = flow.Name,
                    Description = flow.Description,
                    WebpageUrl = flow.WebpageUrl,
                    FileType = flow.FileType,
                    CreatedAt = flow.CreatedAt,
                    UpdatedAt = flow.UpdatedAt,
                    Project = flow.Project is null ? null : new FlowResponse.FlowType.ProjectType
                    {
                        Id = flow.Project.Id,
                        Name = flow.Project.Name
                    },
                    Owner = flow.Owner is null ? null : new FlowResponse.FlowType.OwnerType
                    {
                        Id = flow.Owner.Id,
                        Name = flow.Owner.Name
                    },
                    Tags = flow.Tags?.Select(t => new FlowResponse.FlowType.TagType { Label = t.Label }).ToArray() ?? Array.Empty<FlowResponse.FlowType.TagType>()
                };
            };
        }

        private Func<TableauData, HttpRequestMessage, UpdateFlowResponse.FlowType?> BuildUpdateFlowDelegate()
        {
            return (data, request) =>
            {
                var flowId = request.GetIdAfterSegment(ContentTypeUrlPrefix);
                if (flowId is null)
                {
                    return null;
                }

                var flow = data.Flows.FirstOrDefault(f => f.Id == flowId.Value);
                if (flow is null)
                {
                    return null;
                }

                var updateFlowRequest = request.GetTableauServerRequest<UpdateFlowRequest>()?.Flow;
                if (updateFlowRequest is null)
                {
                    return null;
                }

                if (updateFlowRequest.Project is not null)
                {
                    flow.Project!.Id = updateFlowRequest.Project.Id;
                }

                if (updateFlowRequest.Owner is not null)
                {
                    flow.Owner!.Id = updateFlowRequest.Owner.Id;
                }

                flow.CreatedAt = DateTime.UtcNow.AddDays(-7).ToIso8601();
                flow.UpdatedAt = DateTime.UtcNow.ToIso8601();

                return new UpdateFlowResponse.FlowType
                {
                    Id = flow.Id,
                    Name = flow.Name,
                    Description = flow.Description,
                    WebpageUrl = flow.WebpageUrl,
                    FileType = flow.FileType,
                    CreatedAt = flow.CreatedAt,
                    UpdatedAt = flow.UpdatedAt
                };
            };
        }

        #endregion
    }
}


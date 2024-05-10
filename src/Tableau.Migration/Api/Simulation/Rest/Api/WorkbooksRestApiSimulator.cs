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
using System.Linq;
using System.Net.Http;
using System.Text;
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
    /// Object that defines simulation of Tableau REST API workbook methods.
    /// </summary>
    public sealed class WorkbooksRestApiSimulator : TagsRestApiSimulatorBase<WorkbookResponse.WorkbookType, WorkbookResponse.WorkbookType.TagType>
    {
        /// <summary>
        /// Gets the simulated workbook query API method.
        /// </summary>
        public MethodSimulator QueryWorkbook { get; }

        /// <summary>
        /// Gets the simulated workbook query API method.
        /// </summary>
        public MethodSimulator QueryWorkbooks { get; }

        /// <summary>
        /// Gets the simulated workbook download API method.
        /// </summary>
        public MethodSimulator DownloadWorkbook { get; }

        /// <summary>
        /// Gets the simulated commit workbook upload API method.
        /// </summary>
        public MethodSimulator CommitWorkbookUpload { get; }

        /// <summary>
        /// Gets the simulated update workbook API method.
        /// </summary>
        public MethodSimulator UpdateWorkbook { get; }

        /// <summary>
        /// Gets the simulated list connections API method.
        /// </summary>
        public MethodSimulator QueryWorkbookConnections { get; }

        /// <summary>
        /// Gets the simulated update connection API method.
        /// </summary>
        public MethodSimulator UpdateConnectionAsync { get; }

        /// <summary>
        /// Creates a new <see cref="WorkbooksRestApiSimulator"/> object.
        /// </summary>
        /// <param name="simulator">A response simulator to setup with REST API methods.</param>
        public WorkbooksRestApiSimulator(TableauApiResponseSimulator simulator)
            : base(
                  simulator,
                  RestUrlPrefixes.Workbooks,
                  (data) => data.Workbooks)
        {
            QueryWorkbook = simulator.SetupRestGet<WorkbookResponse, WorkbookResponse.WorkbookType>(
                SiteEntityUrl(ContentTypeUrlPrefix),
                 (data, request) =>
                 {
                     var id = request.GetIdAfterSegment(ContentTypeUrlPrefix);

                     return id is null ? null : data.Workbooks.FirstOrDefault(wb => wb.Id == id.Value);
                 });

            QueryWorkbooks = simulator.SetupRestPagedList<WorkbooksResponse, WorkbooksResponse.WorkbookType>(
                 SiteUrl(ContentTypeUrlPrefix),
                 (data) => data.Workbooks.Select(workbook => new WorkbooksResponse.WorkbookType(workbook)).ToList());

            CommitWorkbookUpload = simulator.SetupRestPost(
                SiteUrl(ContentTypeUrlPrefix),
                new RestCommitWorkbookUploadResponseBuilder(simulator.Data, simulator.Serializer),
                SiteCommitFileUploadQueryString("workbookType"));

            DownloadWorkbook = simulator.SetupRestDownloadById(
                SiteEntityUrl(ContentTypeUrlPrefix, "content"),
                (data) => data.WorkbookFiles, 4);

            UpdateWorkbook = simulator.SetupRestPut<UpdateWorkbookResponse, UpdateWorkbookResponse.WorkbookType>(
                SiteEntityUrl(ContentTypeUrlPrefix),
                BuildUpdateDelegate());

            QueryWorkbookConnections = simulator.SetupRestGetList<ConnectionsResponse, ConnectionsResponse.ConnectionType>(
                SiteEntityUrl(ContentTypeUrlPrefix, "connections"),
                BuildListConnectionsDelegate());

            UpdateConnectionAsync = simulator.SetupRestPut(
                SiteEntityUrl(ContentTypeUrlPrefix, $"connections/{new Regex(GuidPattern, RegexOptions.IgnoreCase)}"),
                new RestUpdateConnectionResponseBuilder<SimulatedWorkbookData>(
                    simulator.Data,
                    simulator.Data.WorkbookFiles,
                    simulator.Serializer,
                    ContentTypeUrlPrefix));

        }

        #region - Response Delegates -

        private Func<TableauData, HttpRequestMessage, ICollection<ConnectionsResponse.ConnectionType>> BuildListConnectionsDelegate()
        {
            return (data, request) =>
            {
                var id = request.GetIdAfterSegment(ContentTypeUrlPrefix);
                if (id is null)
                {
                    throw new ArgumentNullException($"Workbook ID should not be null");
                }

                if (!data.WorkbookFiles.ContainsKey(id.Value))
                {
                    throw new InvalidOperationException($"No data found for Workbook ID {id.Value}");
                }
                var wbFile = data.WorkbookFiles[id.Value];
                var wbFileText = Encoding.Default.GetString(wbFile);
                if (string.IsNullOrEmpty(wbFileText))
                {
                    return Array.Empty<ConnectionsResponse.ConnectionType>();
                }

                var simulatedWorkbook = wbFileText.FromXml<SimulatedDataSourceData>();
                if (simulatedWorkbook is null)
                {
                    return Array.Empty<ConnectionsResponse.ConnectionType>();
                }

                var connections = simulatedWorkbook
                    .Connections
                    .Select(c => new ConnectionsResponse.ConnectionType(c))
                    .ToArray();

                return connections ?? Array.Empty<ConnectionsResponse.ConnectionType>();
            };
        }

        private Func<TableauData, HttpRequestMessage, UpdateWorkbookResponse.WorkbookType?> BuildUpdateDelegate()
        {
            return (data, request) =>
            {
                var workbookId = request.GetIdAfterSegment(ContentTypeUrlPrefix);
                if (workbookId is null)
                {
                    return null;
                }

                var workbook = data.Workbooks.FirstOrDefault(ds => ds.Id == workbookId.Value);
                if (workbook is null)
                {
                    return null;
                }

                var updateWorkbookRequest = request.GetTableauServerRequest<UpdateWorkbookRequest>()?.Workbook;
                if (updateWorkbookRequest is null)
                {
                    return null;
                }

                if (updateWorkbookRequest.Name is not null)
                {
                    workbook.Name = updateWorkbookRequest.Name;
                }
                
                if (updateWorkbookRequest.Description is not null)
                {
                    workbook.Description = updateWorkbookRequest.Description;
                }

                if (updateWorkbookRequest.ShowTabsSpecified)
                {
                    workbook.ShowTabs = updateWorkbookRequest.ShowTabs;
                }

                if (updateWorkbookRequest.EncryptExtractsSpecified)
                {
                    workbook.EncryptExtracts = updateWorkbookRequest.EncryptExtracts;
                }

                if (updateWorkbookRequest.Project is not null)
                {
                    workbook.Project!.Id = updateWorkbookRequest.Project.Id;
                }

                if (updateWorkbookRequest.Owner is not null)
                {
                    workbook.Owner!.Id = updateWorkbookRequest.Owner.Id;
                }

                workbook.CreatedAt = DateTime.UtcNow.AddDays(-7).ToIso8601();
                workbook.UpdatedAt = DateTime.UtcNow.ToIso8601();

                return new UpdateWorkbookResponse.WorkbookType
                {
                    Id = workbook.Id,
                    Name = workbook.Name,
                    Description = workbook.Description,
                    ContentUrl = workbook.ContentUrl,
                    CreatedAt = workbook.CreatedAt,
                    UpdatedAt = workbook.UpdatedAt,
                    ShowTabs = workbook.ShowTabs,
                    EncryptExtracts = workbook.EncryptExtracts
                };
            };
        }

        #endregion
    }
}

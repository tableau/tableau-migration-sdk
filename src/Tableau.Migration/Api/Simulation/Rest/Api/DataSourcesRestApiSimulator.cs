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
    /// Object that defines simulation of Tableau REST API data source methods.
    /// </summary>
    public sealed class DataSourcesRestApiSimulator : TagsRestApiSimulatorBase<DataSourceResponse.DataSourceType, DataSourceResponse.DataSourceType.TagType>
    {
        /// <summary>
        /// Gets the simulated data source query API method.
        /// </summary>
        public MethodSimulator QueryDataSource { get; }

        /// <summary>
        /// Gets the simulated data sources query API method.
        /// </summary>
        public MethodSimulator QueryDataSources { get; }

        /// <summary>
        /// Gets the simulated data source download API method.
        /// </summary>
        public MethodSimulator DownloadDataSource { get; }

        /// <summary>
        /// Gets the simulated commit data source upload API method.
        /// </summary>
        public MethodSimulator CommitDataSourceUpload { get; }

        /// <summary>
        /// Gets the simulated update data source API method.
        /// </summary>
        public MethodSimulator UpdateDataSource { get; }


        /// <summary>
        /// Gets the simulated list connections API method.
        /// </summary>
        public MethodSimulator QueryDataSourceConnections { get; }


        /// <summary>
        /// Gets the simulated update connection API method.
        /// </summary>
        public MethodSimulator UpdateConnectionAsync { get; }

        /// <summary>
        /// Creates a new <see cref="DataSourcesRestApiSimulator"/> object.
        /// </summary>
        /// <param name="simulator">A response simulator to setup with REST API methods.</param>
        public DataSourcesRestApiSimulator(TableauApiResponseSimulator simulator)
            : base(
                  simulator,
                  RestUrlPrefixes.DataSources,
                  (data) => data.DataSources)
        {
            QueryDataSource = simulator.SetupRestGet<DataSourceResponse, DataSourceResponse.DataSourceType>(
                SiteEntityUrl(ContentTypeUrlPrefix),
                BuildQueryDataSourceDelegate());

            QueryDataSources = simulator.SetupRestPagedList<DataSourcesResponse, DataSourcesResponse.DataSourceType>(
                 SiteUrl(ContentTypeUrlPrefix),
                 (data) => data.DataSources.Select(dataSource => new DataSourcesResponse.DataSourceType(dataSource)).ToList());

            CommitDataSourceUpload = simulator.SetupRestPost(
                SiteUrl("datasources"),
                new RestCommitDataSourceUploadResponseBuilder(simulator.Data, simulator.Serializer),
                SiteCommitFileUploadQueryString("datasourceType"));

            DownloadDataSource = simulator.SetupRestDownloadById(
                SiteEntityUrl(ContentTypeUrlPrefix, "content"),
                (data) => data.DataSourceFiles, 4);

            UpdateDataSource = simulator.SetupRestPut<UpdateDataSourceResponse, UpdateDataSourceResponse.DataSourceType>(
                SiteEntityUrl(ContentTypeUrlPrefix),
                BuildUpdateDataSourceDelegate());

            QueryDataSourceConnections = simulator.SetupRestGetList<ConnectionsResponse, ConnectionsResponse.ConnectionType>(
                SiteEntityUrl(ContentTypeUrlPrefix, "connections"),
                BuildListConnectionsDelegate());

            UpdateConnectionAsync = simulator.SetupRestPut(
                SiteEntityUrl(ContentTypeUrlPrefix, $"connections/{new Regex(GuidPattern, RegexOptions.IgnoreCase)}"),
                new RestUpdateConnectionResponseBuilder<SimulatedDataSourceData>(
                    simulator.Data,
                    simulator.Data.DataSourceFiles,
                    simulator.Serializer,
                    ContentTypeUrlPrefix));
        }

        #region - Response Delegates -

        private Func<TableauData, HttpRequestMessage, ICollection<ConnectionsResponse.ConnectionType>> BuildListConnectionsDelegate()
        {
            return (data, request) =>
            {
                var id = request.GetIdAfterSegment(ContentTypeUrlPrefix) ?? throw new ArgumentNullException($"Data Source ID should not be null");
                if (!data.DataSourceFiles.ContainsKey(id))
                {
                    throw new InvalidOperationException($"No data found for Data Source ID {id}");
                }

                var dsFile = data.DataSourceFiles[id];
                var dsFileText = Encoding.Default.GetString(dsFile);
                if (string.IsNullOrEmpty(dsFileText))
                {
                    return Array.Empty<ConnectionsResponse.ConnectionType>();
                }

                var simulatedDataSource = dsFileText.FromXml<SimulatedDataSourceData>();
                if (simulatedDataSource is null)
                {
                    return Array.Empty<ConnectionsResponse.ConnectionType>();
                }

                var connections = simulatedDataSource
                    .Connections
                    .Select(c => new ConnectionsResponse.ConnectionType(c))
                    .ToArray();

                return connections ?? Array.Empty<ConnectionsResponse.ConnectionType>();
            };
        }

        private Func<TableauData, HttpRequestMessage, DataSourceResponse.DataSourceType?> BuildQueryDataSourceDelegate()
        {
            return (data, request) =>
            {
                var dataSourceId = request.GetIdAfterSegment(ContentTypeUrlPrefix);

                if (dataSourceId is null)
                    return null;

                var dataSource = data.DataSources.FirstOrDefault(ds => ds.Id == dataSourceId.Value);

                return dataSource is null
                    ? null
                    : new DataSourceResponse.DataSourceType(dataSource)
                    {
                        CertificationNote = dataSource.CertificationNote
                    };
            };
        }

        private Func<TableauData, HttpRequestMessage, UpdateDataSourceResponse.DataSourceType?> BuildUpdateDataSourceDelegate()
        {
            return (data, request) =>
            {
                var dataSourceId = request.GetIdAfterSegment(ContentTypeUrlPrefix);
                if (dataSourceId is null)
                {
                    return null;
                }

                var dataSource = data.DataSources.FirstOrDefault(ds => ds.Id == dataSourceId.Value);
                if (dataSource is null)
                {
                    return null;
                }

                var updateDataSourceRequest = request.GetTableauServerRequest<UpdateDataSourceRequest>()?.DataSource;
                if (updateDataSourceRequest is null)
                {
                    return null;
                }

                if (updateDataSourceRequest.Name is not null)
                {
                    dataSource.Name = updateDataSourceRequest.Name;
                }

                if (updateDataSourceRequest.IsCertifiedSpecified)
                {
                    dataSource.IsCertified = updateDataSourceRequest.IsCertified;
                    if (dataSource.IsCertified)
                    {
                        dataSource.CertificationNote = updateDataSourceRequest.CertificationNote;
                    }
                    else
                    {
                        dataSource.CertificationNote = "";
                    }
                }

                if (updateDataSourceRequest.EncryptExtractsSpecified)
                {
                    dataSource.EncryptExtracts = updateDataSourceRequest.EncryptExtracts;
                }

                if (updateDataSourceRequest.Project is not null)
                {
                    dataSource.Project!.Id = updateDataSourceRequest.Project.Id;
                }

                if (updateDataSourceRequest.Owner is not null)
                {
                    dataSource.Owner!.Id = updateDataSourceRequest.Owner.Id;
                }

                dataSource.CreatedAt = DateTime.UtcNow.AddDays(-7).ToIso8601();
                dataSource.UpdatedAt = DateTime.UtcNow.ToIso8601();

                return new UpdateDataSourceResponse.DataSourceType
                {
                    Id = dataSource.Id,
                    Name = dataSource.Name,
                    ContentUrl = dataSource.ContentUrl,
                    CreatedAt = dataSource.CreatedAt,
                    UpdatedAt = dataSource.UpdatedAt,
                    IsCertified = dataSource.IsCertified,
                    CertificationNote = dataSource.CertificationNote,
                    EncryptExtracts = dataSource.EncryptExtracts,
                    Project = new()
                    {
                        Id = dataSource.Project!.Id
                    },
                    Owner = new()
                    {
                        Id = dataSource.Owner!.Id
                    }
                };
            };
        }

        #endregion
    }
}
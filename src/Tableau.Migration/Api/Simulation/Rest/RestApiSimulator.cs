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

using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Simulation.Rest.Api;
using Tableau.Migration.Api.Simulation.Rest.Net;
using Tableau.Migration.Net.Simulation;

using static Tableau.Migration.Api.Simulation.Rest.Net.Requests.RestUrlPatterns;

namespace Tableau.Migration.Api.Simulation.Rest
{
    /// <summary>
    /// Object that defines simulation of Tableau REST API methods.
    /// </summary>
    public sealed class RestApiSimulator
    {
        /// <summary>
        /// Gets the simulated authentication API methods.
        /// </summary>
        public AuthRestApiSimulator Auth { get; }

        /// <summary>
        /// Gets the simulated data source API methods.
        /// </summary>
        public DataSourcesRestApiSimulator DataSources { get; }

        /// <summary>
        /// Gets the simulated group API methods.
        /// </summary>
        public GroupsRestApiSimulator Groups { get; }

        /// <summary>
        /// Gets the simulated job API methods.
        /// </summary>
        public JobsRestApiSimulator Jobs { get; }

        /// <summary>
        /// Gets the simulated project API methods.
        /// </summary>
        public ProjectsRestApiSimulator Projects { get; }

        /// <summary>
        /// Gets the simulated site API methods.
        /// </summary>
        public SitesRestApiSimulator Sites { get; }

        /// <summary>
        /// Gets the simulated user API methods.
        /// </summary>
        public UsersRestApiSimulator Users { get; }

        /// <summary>
        /// Gets the simulated workbook API methods.
        /// </summary>
        public WorkbooksRestApiSimulator Workbooks { get; }

        /// <summary>
        /// Gets the simulated view API methods.
        /// </summary>
        public ViewsRestApiSimulator Views { get; }

        /// <summary>
        /// Gets the simulated workbook API methods.
        /// </summary>
        public FileUploadsRestApiSimulator Files { get; }

        /// <summary>
        /// Gets the simulated server info query API method.
        /// </summary>
        public MethodSimulator QueryServerInfo { get; }

        /// <summary>
        /// Creates a new <see cref="RestApiSimulator"/> object.
        /// </summary>
        /// <param name="simulator">A response simulator to setup with REST API methods.</param>
        public RestApiSimulator(TableauApiResponseSimulator simulator)
        {
            Auth = new(simulator);
            DataSources = new(simulator);
            Groups = new(simulator);
            Jobs = new(simulator);
            Projects = new(simulator);
            Sites = new(simulator);
            Users = new(simulator);
            Workbooks = new(simulator);
            Files = new(simulator);
            Views = new(simulator);


            QueryServerInfo = simulator.SetupRestGet<ServerInfoResponse, ServerInfoResponse.ServerInfoType>(RestApiUrl("serverinfo"), d => d.ServerInfo, requiresAuthentication: false);
        }
    }
}

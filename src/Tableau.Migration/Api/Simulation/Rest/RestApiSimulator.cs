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
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Simulation.Rest.Api;
using Tableau.Migration.Api.Simulation.Rest.Net;
using Tableau.Migration.Api.Simulation.Rest.Net.Responses;
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
        /// Gets the simulated schedule API methods.
        /// </summary>
        public SchedulesRestApiSimulator Schedules { get; }

        /// <summary>
        /// Gets the simulated task API methods.
        /// </summary>
        public TasksRestApiSimulator Tasks { get; }

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
        /// Gets the simulated custom view API methods.
        /// </summary>
        public CustomViewsRestApiSimulator CustomViews { get; }

        /// <summary>
        /// Gets the simulated workbook API methods.
        /// </summary>
        public FileUploadsRestApiSimulator Files { get; }

        /// <summary>
        /// Gets the simulated server info query API method.
        /// </summary>
        public MethodSimulator QueryServerInfo { get; }

        /// <summary>
        /// Gets the simulated current server session query API method.
        /// </summary>
        public MethodSimulator GetCurrentServerSession { get; }

        /// <summary>
        /// Gets the simulated site query API method.
        /// </summary>
        public MethodSimulator QuerySites { get; }

        private static ServerSessionResponse.SessionType BuildCurrentSession(TableauData data)
        {
            var user = data.Users.Single(u => u.Id == data.SignIn!.User!.Id);
            var site = data.Sites.Single(s => s.Id == data.SignIn!.Site!.Id);

            var response = new ServerSessionResponse.SessionType
            {
                Site = new()
                {
                    Id = site.Id,
                    ContentUrl = site.ContentUrl,
                    Name = site.Name
                },
                User = new()
                {
                    AuthSetting = user.AuthSetting,
                    Id = user.Id,
                    Name = user.Name,
                    SiteRole = user.SiteRole
                }
            };

            var adminLevel = SiteRoleMapping.GetAdministratorLevel(user.SiteRole);
            if (!AdministratorLevels.IsAMatch(adminLevel, AdministratorLevels.None))
            {
                response.Site.ExtractEncryptionMode = site.ExtractEncryptionMode;
            }

            return response;
        }

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
            Schedules = new(simulator);
            Tasks = new(simulator);
            Projects = new(simulator);
            Sites = new(simulator);
            Users = new(simulator);
            Workbooks = new(simulator);
            Files = new(simulator);
            Views = new(simulator);
            CustomViews = new(simulator);

            QueryServerInfo = simulator.SetupRestGet<ServerInfoResponse, ServerInfoResponse.ServerInfoType>(RestApiUrl("serverinfo"), d => d.ServerInfo, requiresAuthentication: false);
            GetCurrentServerSession = simulator.SetupRestGet<ServerSessionResponse, ServerSessionResponse.SessionType>(RestApiUrl("sessions/current"), BuildCurrentSession);
            QuerySites = simulator.SetupRestGet(
                RestApiUrl($"sites"),
                new RestGetSitesResponseBuilder(simulator.Data, simulator.Serializer));
        }
    }
}

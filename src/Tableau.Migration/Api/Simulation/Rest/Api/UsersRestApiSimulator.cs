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
using System.Linq;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Simulation.Rest.Net;
using Tableau.Migration.Api.Simulation.Rest.Net.Requests;
using Tableau.Migration.Api.Simulation.Rest.Net.Responses;
using Tableau.Migration.Net.Simulation;

using static Tableau.Migration.Api.Simulation.Rest.Net.Requests.RestUrlPatterns;

namespace Tableau.Migration.Api.Simulation.Rest.Api
{
    /// <summary>
    /// Object that defines simulation of Tableau REST API user methods.
    /// </summary>
    public sealed class UsersRestApiSimulator
    {
        /// <summary>
        /// Gets the simulated user query API method.
        /// </summary>
        public MethodSimulator QueryUsers { get; }

        /// <summary>
        /// Gets the simulated user group query API method.
        /// </summary>
        public MethodSimulator QueryUserGroups { get; }

        /// <summary>
        /// Gets the simulated user import API method.
        /// </summary>
        public MethodSimulator ImportUsersToSite { get; }

        /// <summary>
        /// Gets the simulated user creation API method.
        /// </summary>
        public MethodSimulator AddUserToSite { get; }

        /// <summary>
        /// Gets the simulated user update API method.
        /// </summary>
        public MethodSimulator UpdateUser { get; }

        /// <summary>
        /// Creates a new <see cref="UsersRestApiSimulator"/> object.
        /// </summary>
        /// <param name="simulator">A response simulator to setup with REST API methods.</param>
        public UsersRestApiSimulator(TableauApiResponseSimulator simulator)
        {
            QueryUsers = simulator.SetupRestPagedList<UsersResponse, UsersResponse.UserType>(SiteUrl("users"), d => d.Users);

            QueryUserGroups = simulator.SetupRestPagedList<GroupsResponse, GroupsResponse.GroupType>(
                SiteEntityUrl("users", "groups"), (d, r) =>
                {
                    var userId = r.GetIdAfterSegment("users");

                    if (userId is null || !d.UserGroups.ContainsKey(userId.Value))
                    {
                        return Array.Empty<GroupsResponse.GroupType>().ToList();
                    }

                    return d.Groups
                        .Where(g => d.UserGroups[userId.Value].Contains(g.Id))
                        .ToList();
                });

            ImportUsersToSite = simulator.SetupRestPost<ImportJobResponse, ImportJobResponse.ImportJobType>(
                SiteUrl("users/import"),
                new RestUserImportResponseBuilder(simulator.Data, simulator.Serializer));

            AddUserToSite = simulator.SetupRestPost(SiteUrl("users"), new RestUserAddResponseBuilder(simulator.Data, simulator.Serializer));

            UpdateUser = simulator.SetupRestPut(
                SiteEntityUrl("users"),
                new RestUserUpdateResponseBuilder(simulator.Data, simulator.Serializer, (d, _) => d.Users));
        }
    }
}

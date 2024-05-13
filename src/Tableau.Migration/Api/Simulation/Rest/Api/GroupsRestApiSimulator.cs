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
using System.Net;
using System.Net.Http;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Simulation.Rest.Net;
using Tableau.Migration.Api.Simulation.Rest.Net.Requests;
using Tableau.Migration.Api.Simulation.Rest.Net.Responses;
using Tableau.Migration.Content;
using Tableau.Migration.Net.Simulation;
using static Tableau.Migration.Api.Simulation.Rest.Net.Requests.RestUrlPatterns;

namespace Tableau.Migration.Api.Simulation.Rest.Api
{
    /// <summary>
    /// Object that defines simulation of Tableau REST API group methods.
    /// </summary>
    public sealed class GroupsRestApiSimulator
    {
        /// <summary>
        /// Gets the simulated group query API method.
        /// </summary>
        public MethodSimulator QueryGroups { get; }

        /// <summary>
        /// Gets the simulated group user query API method.
        /// </summary>
        public MethodSimulator QueryGroupUsers { get; }

        /// <summary>
        /// Gets the simulated group user add API method.
        /// </summary>
        public MethodSimulator AddUserToGroup { get; }

        /// <summary>
        /// Gets the simulated group user remove API method.
        /// </summary>
        public MethodSimulator RemoveUserFromGroup { get; }

        /// <summary>
        /// Gets the simulated group create API method.
        /// </summary>
        public MethodSimulator AddGroup { get; }

        /// <summary>
        /// Creates a new <see cref="GroupsRestApiSimulator"/> object.
        /// </summary>
        /// <param name="simulator">A response simulator to setup with REST API methods.</param>
        public GroupsRestApiSimulator(TableauApiResponseSimulator simulator)
        {
            QueryGroups = simulator.SetupRestPagedList<GroupsResponse, GroupsResponse.GroupType>(SiteUrl("groups"),
                (data, request) =>
                {
                    var filters = request.ParseFilters();

                    if (filters.Count == 0)
                        return data.Groups;

                    var results = data.Groups.AsEnumerable();

                    var nameFilter = filters.GetFilterValue("name", "eq");
                    if (nameFilter is not null)
                    {
                        results = results.Where(p => Group.NameComparer.Equals(nameFilter, p.Name));
                    }

                    var domainFilter = filters.GetFilterValue("domain", "eq");
                    if (domainFilter is not null)
                    {
                        results = results.Where(p => Group.NameComparer.Equals(domainFilter, p.Domain?.Name));
                    }

                    return results.ToList();
                });

            QueryGroupUsers = simulator.SetupRestPagedList<UsersResponse, UsersResponse.UserType>(
                 SiteEntityUrl("groups", "users"),
                 (data, request) =>
                 {
                     var groupId = request.GetIdAfterSegment("groups");
                     if (groupId is null || !data.GroupUsers.ContainsKey(groupId.Value))
                     {
                         return Array.Empty<UsersResponse.UserType>().ToList();
                     }

                     return data.Users
                         .Where(g => data.GroupUsers[groupId.Value].Contains(g.Id))
                         .ToList();
                 });

            AddUserToGroup = simulator.SetupRestPost<AddUserResponse, AddUserResponse.UserType>(
                SiteEntityUrl("groups", "users"),
                new RestUserAddToGroupResponseBuilder(simulator.Data, simulator.Serializer));

            RemoveUserFromGroup = simulator.SetupRestDelete(
                SiteEntityUrl("groups", $"users/{GuidPattern}"),
                new RestDeleteResponseBuilder(simulator.Data, DeleteGroupUser, simulator.Serializer));

            AddGroup = simulator.SetupRestPost<CreateGroupResponse, CreateGroupResponse.GroupType>(
                SiteUrl("groups"),
                new RestGroupAddResponseBuilder(simulator.Data, simulator.Serializer));
        }

        private HttpStatusCode DeleteGroupUser(TableauData data, HttpRequestMessage request)
        {
            var groupId = request.GetIdAfterSegment("groups");
            var userIdText = request.GetLastSegment();
            if (groupId is null || !Guid.TryParse(userIdText, out var userId))
            {
                return HttpStatusCode.BadRequest;
            }

            data.RemoveUserFromGroup(userId, groupId.Value);
            return HttpStatusCode.NoContent;
        }
    }
}

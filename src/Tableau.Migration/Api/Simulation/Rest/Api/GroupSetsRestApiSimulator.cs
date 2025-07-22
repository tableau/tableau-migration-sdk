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
using System.Linq;
using System.Net;
using System.Net.Http;
using Tableau.Migration.Api.Rest;
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
    /// Object that defines simulation of Tableau REST API group set methods.
    /// </summary>
    public sealed class GroupSetsRestApiSimulator
    {
        /// <summary>
        /// Gets the simulated group set query API method.
        /// </summary>
        public MethodSimulator QueryGroupSets { get; }

        /// <summary>
        /// Gets the simulated group set get API method.
        /// </summary>
        public MethodSimulator GetGroupSet { get; }

        /// <summary>
        /// Gets the simulated group set create API method.
        /// </summary>
        public MethodSimulator CreateGroupSet { get; }

        /// <summary>
        /// Gets the simulated add group to group set API method.
        /// </summary>
        public MethodSimulator AddGroupToGroupSet { get; }

        /// <summary>
        /// Gets the simulated remove group from group set API method.
        /// </summary>
        public MethodSimulator RemoveGroupFromGroupSet { get; }

        /// <summary>
        /// Creates a new <see cref="GroupSetsRestApiSimulator"/> object.
        /// </summary>
        /// <param name="simulator">A response simulator to setup with REST API methods.</param>
        public GroupSetsRestApiSimulator(TableauApiResponseSimulator simulator)
        {
            QueryGroupSets = simulator.SetupRestPagedList<GroupSetsResponse, GroupSetsResponse.GroupSetType>(
                SiteUrl(RestUrlKeywords.GroupSets),
                (data, request) =>
                {
                    var filters = request.ParseFilters();

                    if (filters.Count == 0)
                    {
                        return data.GroupSets;
                    }

                    var results = data.GroupSets.AsEnumerable();

                    var nameFilter = filters.GetFilterValue("name", "eq");
                    if (nameFilter is not null)
                    {
                        results = results.Where(gs => GroupSet.NameComparer.Equals(nameFilter, gs.Name));
                    }

                    return results.ToList();
                });

            GetGroupSet = simulator.SetupRestGet<GroupSetResponse, GroupSetResponse.GroupSetType>(
                SiteEntityUrl(RestUrlKeywords.GroupSets),
                (data, request) =>
                {
                    var groupSetId = request.GetIdAfterSegment(RestUrlKeywords.GroupSets);
                    if (groupSetId is null)
                    {
                        return null;
                    }

                    var groupSet = data.GroupSets.FirstOrDefault(gs => gs.Id == groupSetId.Value);

                    if (groupSet is null)
                    {
                        return null;
                    }

                    // Create a new response with groups included
                    return new GroupSetResponse.GroupSetType
                    {
                        Id = groupSet.Id,
                        Name = groupSet.Name,
                        Groups = data.GetGroupSetGroups(groupSet.Id)
                            .Select(g => new GroupSetResponse.GroupSetType.GroupType { Id = g.Id })
                            .ToArray()
                    };
                });

            CreateGroupSet = simulator.SetupRestPost<CreateGroupSetResponse, CreateGroupSetResponse.GroupSetType>(
                SiteUrl(RestUrlKeywords.GroupSets),
                new RestGroupSetCreateResponseBuilder(simulator.Data, simulator.Serializer));

            AddGroupToGroupSet = simulator.SetupRestPut(
                SiteEntityUrl(RestUrlKeywords.GroupSets, $"{RestUrlKeywords.Groups}/{GuidPattern}"),
                new EmptyRestResponseBuilder(simulator.Data, simulator.Serializer, AddGroupToGroupSetDelegate, requiresAuthentication: true));

            RemoveGroupFromGroupSet = simulator.SetupRestDelete(
                SiteEntityUrl(RestUrlKeywords.GroupSets, $"{RestUrlKeywords.Groups}/{GuidPattern}"),
                new RestDeleteResponseBuilder(simulator.Data, DeleteGroupFromGroupSetDelegate, simulator.Serializer));
        }

        private void AddGroupToGroupSetDelegate(TableauData data, HttpRequestMessage request)
        {
            var groupSetId = request.GetIdAfterSegment(RestUrlKeywords.GroupSets);
            var groupIdText = request.GetLastSegment();
            if (groupSetId is not null && Guid.TryParse(groupIdText, out var groupId))
            {
                data.AddGroupToGroupSet(groupId, groupSetId.Value);
            }
        }

        private HttpStatusCode DeleteGroupFromGroupSetDelegate(TableauData data, HttpRequestMessage request)
        {
            var groupSetId = request.GetIdAfterSegment(RestUrlKeywords.GroupSets);
            var groupIdText = request.GetLastSegment();
            if (groupSetId is null || !Guid.TryParse(groupIdText, out var groupId))
            {
                return HttpStatusCode.BadRequest;
            }

            data.RemoveGroupFromGroupSet(groupId, groupSetId.Value);
            return HttpStatusCode.NoContent;
        }
    }
}
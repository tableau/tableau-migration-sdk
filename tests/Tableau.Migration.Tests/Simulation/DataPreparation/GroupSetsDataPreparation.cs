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

using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Simulation;

namespace Tableau.Migration.Tests.Simulation.DataPreparation
{
    /// <summary>
    /// Static class responsible for preparing group sets data for migration tests.
    /// </summary>
    public static class GroupSetsDataPreparation
    {
        public static List<GroupSetsResponse.GroupSetType> Prepare(List<GroupsResponse.GroupType>? groups, TableauApiSimulator apiSimulator, IFixture fixture, int? count)
        {
            var groupSets = new List<GroupSetsResponse.GroupSetType>();
            var numSourceGroupSets = count;

            for (int i = 0; i < numSourceGroupSets; i++)
            {
                var groupSet = fixture.Create<GroupSetsResponse.GroupSetType>();
                groupSets.Add(groupSet);
                apiSimulator.Data.AddGroupSet(groupSet);
            }

            if (groups == null)
            {
                return groupSets;
            }

            // Add groups to group sets for full functionality testing
            foreach (var groupSet in groupSets.Take(2))
            {
                foreach (var group in groups.Take(2))
                {
                    apiSimulator.Data.AddGroupToGroupSet(group.Id, groupSet.Id);
                }
            }

            return groupSets;
        }

        public static (List<GroupSetsResponse.GroupSetType> sourceGroupSets, List<GroupSetsResponse.GroupSetType> destinationGroupSets) PrepareMatching(
            List<GroupsResponse.GroupType> sourceGroups,List<GroupsResponse.GroupType> destinationGroups,TableauApiSimulator sourceApi,
            TableauApiSimulator destinationApi,IFixture fixture)
        {
            var groupSetNames = new[] { "Sales Team", "Marketing Team", "Engineering Team" };

            var sourceGroupSets = new List<GroupSetsResponse.GroupSetType>();
            var destinationGroupSets = new List<GroupSetsResponse.GroupSetType>();

            // Create source group sets
            foreach (var name in groupSetNames)
            {
                var groupSet = fixture.Create<GroupSetsResponse.GroupSetType>();
                groupSet.Name = name;
                sourceGroupSets.Add(groupSet);
                sourceApi.Data.AddGroupSet(groupSet);

                // Add 2 source groups to each source group set
                foreach (var group in sourceGroups.Take(2))
                {
                    sourceApi.Data.AddGroupToGroupSet(group.Id, groupSet.Id);
                }
            }

            // Create destination group sets with SAME names
            foreach (var name in groupSetNames)
            {
                var groupSet = fixture.Create<GroupSetsResponse.GroupSetType>();
                groupSet.Name = name;
                destinationGroupSets.Add(groupSet);
                destinationApi.Data.AddGroupSet(groupSet);

                // Add 1 destination group to each destination group set (different from source)
                foreach (var group in destinationGroups.Take(1))
                {
                    destinationApi.Data.AddGroupToGroupSet(group.Id, groupSet.Id);
                }
            }

            return (sourceGroupSets, destinationGroupSets);
        }
    }
}

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
using AutoFixture;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Simulation;
using Tableau.Migration.Config;

namespace Tableau.Migration.Tests.Simulation.DataPreparation
{
    /// <summary>
    /// Static class responsible for preparing groups data for migration tests.
    /// </summary>
    public static class GroupsDataPreparation
    {        
        /// <summary>
        /// Prepares the source data for migration tests.
        /// </summary>
        /// <param name="simulator">The source/destination API simulator.</param>
        /// <param name="fixture">The fixture for creating test data.</param>
        /// <param name="count">Optional count parameter to control the amount of data prepared.</param>
        /// <returns>The list of prepared groups.</returns>
        public static List<GroupsResponse.GroupType> Prepare(TableauApiSimulator simulator, IFixture fixture, int? count)
        {
            ArgumentNullException.ThrowIfNull(simulator);
            ArgumentNullException.ThrowIfNull(fixture);

            var groups = new List<GroupsResponse.GroupType>();
            var allSiteRoles = SiteRoles.GetAll();
            var numSourceGroups = count ?? (int)Math.Ceiling(ContentTypesOptions.Defaults.BATCH_SIZE * 1.5);

            for (int i = 0; i < numSourceGroups; i++)
            {
                var group = fixture.Create<GroupsResponse.GroupType>();
                group.Domain = i % 2 == 0 ? new GroupsResponse.GroupType.DomainType { Name = "local" } : fixture.Create<GroupsResponse.GroupType.DomainType>();
                group.Import!.SiteRole = allSiteRoles[i % allSiteRoles.Count];
                groups.Add(group);
                simulator.Data.Groups.Add(group);
            }

            return groups;
        }
    }
}
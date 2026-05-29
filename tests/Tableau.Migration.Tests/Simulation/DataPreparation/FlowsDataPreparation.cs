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
using System.Text;
using AutoFixture;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Simulation;
using Tableau.Migration.Net;
using Xunit;

namespace Tableau.Migration.Tests.Simulation.DataPreparation
{
    /// <summary>
    /// Static class responsible for preparing flows data for migration tests.
    /// </summary>
    public static class FlowsDataPreparation
    {
        /// <summary>
        /// Minimal valid flow JSON (nodes and connections objects) so the default flow JSON transformer can run during migration.
        /// </summary>
        public const string MinimalFlowJson = "{\"nodes\":{},\"connections\":{}}";

        /// <summary>
        /// Prepares the source data for migration tests.
        /// </summary>
        /// <param name="sourceApi">The source API simulator.</param>
        /// <param name="fixture">The fixture for creating test data.</param>
        /// <returns>The list of prepared flows.</returns>
        public static List<FlowResponse.FlowType> PrepareServerSource(
            TableauApiSimulator sourceApi,
            IFixture fixture)
        {
            var flows = new List<FlowResponse.FlowType>();

            var users = CommonDataPreparation.GetNonSupportUsers(sourceApi);
            var groups = sourceApi.Data.Groups;

            int counter = 0;
            foreach (var project in sourceApi.Data.Projects)
            {
                var flow = fixture.Build<FlowResponse.FlowType>()
                    .With(f => f.Project, CommonDataPreparation.CreateProjectReference<FlowResponse.FlowType.ProjectType>(project))
                    .Create();

                var owner = users[counter % users.Count];
                flow.Owner = CommonDataPreparation.CreateOwnerReference<FlowResponse.FlowType.OwnerType>(owner);

                sourceApi.Data.AddFlowPermissions(flow, CommonDataPreparation.CreatePermissions(users, groups, fixture));

                // Minimal flow JSON has no connections; do not add embedded credentials so the test does not assert keychain migration.
                Assert.NotNull(flow.Tags);
                Assert.NotEmpty(flow.Tags);

                byte[] flowData = Encoding.UTF8.GetBytes(MinimalFlowJson);
                sourceApi.Data.AddFlow(flow, flowData);
                flows.Add(flow);
                counter++;
            }

            return flows;
        }
    }
}

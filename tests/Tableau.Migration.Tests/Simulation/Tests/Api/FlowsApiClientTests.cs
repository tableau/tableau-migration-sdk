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

using System.Threading.Tasks;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Simulation.Tests.Api
{
    /// <summary>
    /// Simulation tests for flows API client against the flows REST API simulator.
    /// </summary>
    public class FlowsApiClientTests
    {
        /// <summary>
        /// Base class for flows API client tests.
        /// </summary>
        public class FlowsApiClientTest : ApiClientTestBase<IFlowsApiClient, FlowResponse.FlowType>
        { }

        /// <summary>
        /// GetPageAsync tests.
        /// </summary>
        public class GetPageAsync : FlowsApiClientTest
        {
            [Fact]
            public async Task SucceedsAsync()
            {
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                const int ProjectCount = 3;
                var projects = Api.Data.CreateProjects(AutoFixture, ProjectCount);

                const int OwnerCount = 4;
                var owners = Api.Data.CreateUsers(AutoFixture, OwnerCount);

                const int FlowCount = 11;
                for (var i = 0; i != FlowCount; i++)
                {
                    Api.Data.CreateFlow(AutoFixture, projects[i % projects.Count], owners[i % owners.Count]);
                }

                var result = await sitesClient.Flows.GetPageAsync(1, 5, Cancel);

                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
                Assert.Equal(5, result.Value.Count);

                result = await sitesClient.Flows.GetPageAsync(2, 5, Cancel);

                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
                Assert.Equal(5, result.Value.Count);

                result = await sitesClient.Flows.GetPageAsync(3, 5, Cancel);

                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
                Assert.Single(result.Value);
            }
        }

        /// <summary>
        /// GetByIdAsync tests.
        /// </summary>
        public class GetByIdAsync : FlowsApiClientTest
        {
            [Fact]
            public async Task SucceedsAsync()
            {
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var flow = Api.Data.CreateFlow(AutoFixture);

                var result = await sitesClient.Flows.GetByIdAsync(flow.Id, Cancel);

                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
                Assert.Equal(flow.Id, result.Value.Id);
                Assert.Equal(flow.Name, result.Value.Name);
            }
        }

        /// <summary>
        /// PullAsync and PublishAsync tests.
        /// </summary>
        public class PullAndPublishAsync : FlowsApiClientTest
        {
            [Fact]
            public async Task PullThenPublishSucceedsAsync()
            {
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var project = Api.Data.CreateProject(AutoFixture);
                var owner = Api.Data.CreateUser(AutoFixture);
                var flow = Api.Data.CreateFlow(AutoFixture, project, owner);

                var publishableFlowResult = await sitesClient.Flows.PullAsync(
                    new Flow(flow, new Project(project, null, Create<IContentReference>()), new User(owner)),
                    Cancel);

                Assert.Empty(publishableFlowResult.Errors);
                Assert.True(publishableFlowResult.Success);
                Assert.NotNull(publishableFlowResult.Value);

                var result = await sitesClient.Flows.PublishAsync(publishableFlowResult.Value, Cancel);

                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
            }
        }

        /// <summary>
        /// UpdateFlowAsync tests.
        /// </summary>
        public class UpdateFlowAsync : FlowsApiClientTest
        {
            [Fact]
            public async Task ChangeOwnerSucceedsAsync()
            {
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var project = Api.Data.CreateProject(AutoFixture);
                var owner = Api.Data.CreateUser(AutoFixture);
                var flow = Api.Data.CreateFlow(AutoFixture, project, owner);
                var newOwner = Api.Data.CreateUser(AutoFixture);

                var result = await sitesClient.Flows.UpdateFlowAsync(
                    flow.Id,
                    Cancel,
                    newOwnerId: newOwner.Id);

                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
            }

            [Fact]
            public async Task ChangeProjectSucceedsAsync()
            {
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var project = Api.Data.CreateProject(AutoFixture);
                var owner = Api.Data.CreateUser(AutoFixture);
                var flow = Api.Data.CreateFlow(AutoFixture, project, owner);
                var newProject = Api.Data.CreateProject(AutoFixture);

                var result = await sitesClient.Flows.UpdateFlowAsync(
                    flow.Id,
                    Cancel,
                    newProjectId: newProject.Id);

                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
            }
        }

        /// <summary>
        /// GetConnectionsAsync tests.
        /// </summary>
        public class GetConnectionsAsync : FlowsApiClientTest
        {
            [Fact]
            public async Task SucceedsAsync()
            {
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var flow = Api.Data.CreateFlow(AutoFixture);

                var result = await sitesClient.Flows.GetConnectionsAsync(flow.Id, Cancel);

                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
                // SimulatedFlowData may have 0 or 2 connections depending on CreateConnections in data prep
                Assert.NotNull(result.Value);
            }
        }
    }
}

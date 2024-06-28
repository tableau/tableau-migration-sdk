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

using Tableau.Migration.Api;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Simulation;
using Tableau.Migration.Engine.Endpoints;

namespace Tableau.Migration.Tests.Simulation
{
    /// <summary>
    /// <see cref="SimulationTestBase"/> implementation for test classes that require source and cloud destination servers (i.e. for migrations testing).
    /// </summary>
    public abstract class ServerToServerSimulationTestBase : SimulationTestBase
    {
        protected TableauApiSimulator SourceApi { get; }

        protected TableauSiteConnectionConfiguration SourceSiteConfig { get; }

        public TableauApiEndpointConfiguration SourceEndpointConfig { get; }

        protected TableauApiSimulator DestinationApi { get; }

        protected TableauSiteConnectionConfiguration DestinationSiteConfig { get; }

        public TableauApiEndpointConfiguration DestinationEndpointConfig { get; }

        public ServerToServerSimulationTestBase(string sourceUrl = "https://source", string destinationUrl = "https://destination")
        {
            SourceApi = RegisterTableauServerApiSimulator(sourceUrl, Create<UsersResponse.UserType>());
            SourceSiteConfig = BuildSiteConnectionConfiguration(SourceApi);
            SourceEndpointConfig = new(SourceSiteConfig);

            DestinationApi = RegisterTableauServerApiSimulator(destinationUrl, Create<UsersResponse.UserType>());
            DestinationSiteConfig = BuildSiteConnectionConfiguration(DestinationApi);
            DestinationEndpointConfig = new(DestinationSiteConfig);
        }
    }
}

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

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Simulation;
using Tableau.Migration.Engine.Endpoints;

namespace Tableau.Migration.Tests.Simulation
{
    /// <summary>
    /// <see cref="SimulationTestBase"/> implementation for test classes that require a source and multiple destination servers (i.e. for migrations testing).
    /// </summary>
    public abstract class MultiDestinationSimulationTestBase : SimulationTestBase
    {
        protected TableauApiSimulator SourceApi { get; }

        protected TableauSiteConnectionConfiguration SourceSiteConfig { get; }

        public TableauApiEndpointConfiguration SourceEndpointConfig { get; }

        protected ImmutableArray<TableauApiSimulator> DestinationApis { get; }

        protected ImmutableArray<TableauSiteConnectionConfiguration> DestinationSiteConfigs { get; }

        protected ImmutableArray<TableauApiEndpointConfiguration> DestinationEndpointConfigs { get; }

        public MultiDestinationSimulationTestBase(string sourceUrl, IEnumerable<string> destinationUrls)
        {
            SourceApi = RegisterTableauServerApiSimulator(sourceUrl, Create<UsersResponse.UserType>());
            SourceSiteConfig = BuildSiteConnectionConfiguration(SourceApi);
            SourceEndpointConfig = new(SourceSiteConfig);

            DestinationApis = destinationUrls
                .Select(u => RegisterTableauServerApiSimulator(u, Create<UsersResponse.UserType>()))
                .ToImmutableArray();

            DestinationSiteConfigs = DestinationApis
                .Select(a => BuildSiteConnectionConfiguration(a))
                .ToImmutableArray();

            DestinationEndpointConfigs = DestinationSiteConfigs
                .Select(c => new TableauApiEndpointConfiguration(c))
                .ToImmutableArray();
        }
    }
}

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
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation
{
    /// <summary>
    /// Default <see cref="ITableauApiSimulatorFactory"/> implementation.
    /// </summary>
    public class TableauApiSimulatorFactory : ITableauApiSimulatorFactory
    {
        private readonly ITableauApiSimulatorCollection _simulators;
        private readonly IHttpContentSerializer _serializer;

        /// <summary>
        /// Creates a new <see cref="TableauApiSimulatorFactory"/> object.
        /// </summary>
        /// <param name="simulators">The simulator collection.</param>
        /// <param name="serializer">The serializer.</param>
        public TableauApiSimulatorFactory(ITableauApiSimulatorCollection simulators, IHttpContentSerializer serializer)
        {
            _simulators = simulators;
            _serializer = serializer;
        }

        /// <inheritdoc />
        public TableauApiSimulator GetOrCreate(Uri serverUrl, bool isTableauServer)
        {
            var existing = _simulators.ForServer(serverUrl);
            if (existing is not null)
            {
                return existing;
            }

            var simulatorUser = new UsersResponse.UserType()
            {
                Id = Guid.NewGuid(),
                Domain = new() { Name = "local" },
                Name = "simulatorAdmin",
                FullName = "Simulator Admin",
                SiteRole = SiteRoles.SiteAdministratorCreator
            };

            var simulator = new TableauApiSimulator(serverUrl, _serializer, isTableauServer, simulatorUser);
            _simulators.AddOrUpdate(simulator);
            return simulator;
        }
    }
}

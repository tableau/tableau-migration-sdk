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
using System.Collections.Concurrent;
using Tableau.Migration.Net.Simulation;

namespace Tableau.Migration.Api.Simulation
{
    /// <summary>
    /// Default <see cref="ITableauApiSimulatorCollection"/> implementation.
    /// </summary>
    public class TableauApiSimulatorCollection : ITableauApiSimulatorCollection
    {
        private readonly ConcurrentDictionary<Uri, TableauApiSimulator> _simulators = new(BaseUrlComparer.Instance);

        /// <inheritdoc />
        public TableauApiSimulator? ForServer(Uri baseUrl)
        {
            if (_simulators.TryGetValue(baseUrl, out var simulator))
            {
                return simulator;
            }

            return null;
        }

        /// <inheritdoc />
        public void AddOrUpdate(TableauApiSimulator simulator)
            => _simulators.AddOrUpdate(simulator.ServerUrl, simulator, (k, v) => simulator);
    }
}

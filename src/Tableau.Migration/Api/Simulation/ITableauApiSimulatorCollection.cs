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

namespace Tableau.Migration.Api.Simulation
{
    /// <summary>
    /// Interface that contains <see cref="TableauApiSimulator"/>s registered to be used.
    /// </summary>
    public interface ITableauApiSimulatorCollection
    {
        /// <summary>
        /// Gets the API simulator for the given base URL.
        /// </summary>
        /// <param name="baseUrl">The base URL to get the API simulator for.</param>
        /// <returns>The API simulator, or null.</returns>
        TableauApiSimulator? ForServer(Uri baseUrl);

        /// <summary>
        /// Registers an API simulator to use by its base URL.
        /// </summary>
        /// <param name="simulator">The simulator to register.</param>
        void AddOrUpdate(TableauApiSimulator simulator);
    }
}

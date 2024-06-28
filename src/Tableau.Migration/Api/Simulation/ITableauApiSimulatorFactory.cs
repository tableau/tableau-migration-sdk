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
    /// Interface for an object that can create <see cref="TableauApiSimulator"/> objects.
    /// </summary>
    public interface ITableauApiSimulatorFactory
    {
        /// <summary>
        /// Creates an API simulator for the given server URL, or retrieves the existing simulator.
        /// </summary>
        /// <param name="serverUrl">The base server URL to get or create the API simulator for.</param>
        /// <param name="isTableauServer">Indicates whether the current Tableau Data is for Tableau Server (true) or Tableau Cloud (false).</param>
        /// /// <returns>The API simulator.</returns>
        TableauApiSimulator GetOrCreate(Uri serverUrl, bool isTableauServer);
    }
}

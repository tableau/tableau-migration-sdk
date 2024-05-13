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

namespace Tableau.Migration.Engine.Endpoints
{
    /// <summary>
    /// Interface for an object that can create <see cref="IMigrationEndpoint"/>s from a migration plan.
    /// </summary>
    public interface IMigrationEndpointFactory
    {
        /// <summary>
        /// Creates the source endpoint for the given migration plan.
        /// </summary>
        /// <param name="plan">The migration plan to configure the endpoint for.</param>
        /// <returns>The created source endpoint.</returns>
        /// <exception cref="ArgumentException">If the source endpoint type is not supported by the factory.</exception>
        ISourceEndpoint CreateSource(IMigrationPlan plan);

        /// <summary>
        /// Creates the destination endpoint for the given migration plan.
        /// </summary>
        /// <param name="plan">The migration plan to configure the endpoint for.</param>
        /// <returns>The created destination endpoint.</returns>
        /// <exception cref="ArgumentException">If the source endpoint type is not supported by the factory.</exception>
        IDestinationEndpoint CreateDestination(IMigrationPlan plan);
    }
}

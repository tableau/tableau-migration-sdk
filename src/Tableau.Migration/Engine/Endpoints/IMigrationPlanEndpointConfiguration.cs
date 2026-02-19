//
//  Copyright (c) 2026, Salesforce, Inc.
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

using Tableau.Migration.Engine.Services;

namespace Tableau.Migration.Engine.Endpoints
{
    /// <summary>
    /// Interface for configuration necessary to connect to a migration endpoint defined in the <see cref="IMigrationPlan"/>.
    /// </summary>
    public interface IMigrationPlanEndpointConfiguration
    {
        /// <summary>
        /// Gets the collection of registered service overrides for this endpoint.
        /// </summary>
        IMigrationServiceFactoryCollection Services { get; }

        /// <summary>
        /// Validates that the endpoint configuration has enough information to connect.
        /// </summary>
        /// <returns>The validation result.</returns>
        IResult Validate();
    }
}

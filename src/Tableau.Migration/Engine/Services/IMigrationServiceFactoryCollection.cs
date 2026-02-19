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

using System;

namespace Tableau.Migration.Engine.Services
{
    /// <summary>
    /// Interface for an object that contains service factory overrides registered for each service type.
    /// </summary>
    public interface IMigrationServiceFactoryCollection
    {
        /// <summary>
        /// Gets the service factory override for the given service type.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <returns>A service factory to use to get the service, or null to use the default service.</returns>
        MigrationServiceFactory? GetServiceFactory<TService>();

        /// <summary>
        /// Gets the service factory override for the given service type.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <returns>A service factory to use to get the service, or null to use the default service.</returns>
        MigrationServiceFactory? GetServiceFactory(Type serviceType);

        /// <summary>
        /// Gets a service, either from a registered service factory override, or from the service provider as a fallback.
        /// </summary>
        /// <typeparam name="TService">The service to get.</typeparam>
        /// <param name="services">The service provider.</param>
        /// <returns>The service.</returns>
        TService GetService<TService>(IServiceProvider services)
            where TService : notnull;
    }
}

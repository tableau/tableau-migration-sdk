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
using System.Collections.Immutable;

namespace Tableau.Migration.Engine.Services
{
    /// <summary>
    /// Interface for an object that can register service factory overrides for supported service types.
    /// </summary>
    public interface IMigrationServiceBuilder : IMigrationServiceFactoryCollection
    {
        /// <summary>
        /// Gets the list of service types that can be overriden with this service builder.
        /// </summary>
        IImmutableList<Type> SupportedServices { get; }

        /// <summary>
        /// Removes any previously registered service override for the given service type.
        /// </summary>
        /// <typeparam name="TService">The service type to remove any override for.</typeparam>
        /// <returns>This service builder, for fluent API usage.</returns>
        IMigrationServiceBuilder Remove<TService>();

        /// <summary>
        /// Removes any previously registered service override for the given service type.
        /// </summary>
        /// <param name="service">The service type to remove any override for.</param>
        /// <returns>This service builder, for fluent API usage.</returns>
        IMigrationServiceBuilder Remove(Type service);

        /// <summary>
        /// Overrides a service for the given service type.
        /// </summary>
        /// <typeparam name="TService">The service type to override.</typeparam>
        /// <param name="factory">The service factory to create the service with.</param>
        /// <returns>This service builder, for fluent API usage.</returns>
        IMigrationServiceBuilder Set<TService>(MigrationServiceFactory factory);

        /// <summary>
        /// Overrides a service for the given service type.
        /// </summary>
        /// <param name="service">The service type to override.</param>
        /// <param name="factory">The service factory to create the service with.</param>
        /// <returns>This service builder, for fluent API usage.</returns>
        IMigrationServiceBuilder Set(Type service, MigrationServiceFactory factory);
    }
}

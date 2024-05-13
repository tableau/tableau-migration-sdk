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

using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.TestComponents.Engine.Manifest;

namespace Tableau.Migration.TestComponents
{
    /// <summary>
    /// Static class containing extension methods for <see cref="IServiceCollection"/> objects.
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Registers services required for using the Tableau Migration SDK Test Components.
        /// </summary>
        /// <param name="services">The service collection to register services with.</param>        
        /// <returns>The same service collection as the <paramref name="services"/> parameter.</returns>
        public static IServiceCollection AddTestComponents(this IServiceCollection services)
        {
            services.AddSingleton<MigrationManifestSerializer>();
            return services;
        }
    }
}

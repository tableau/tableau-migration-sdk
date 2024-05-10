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

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Tableau.Migration.Api;
using Tableau.Migration.Config;
using Tableau.Migration.Engine;
using Tableau.Migration.Net;
using Tableau.Migration.Resources;

namespace Tableau.Migration
{
    /// <summary>
    /// Static class containing extension methods for <see cref="IServiceCollection"/> objects.
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        internal static IServiceCollection AddSharedResourcesLocalization(this IServiceCollection services)
        {
            //Register localization.
            services
                .AddLogging() //required for AddLocalization, users can customize afterwards by re-calling.
                .AddLocalization() //required for our shared resource localizer, users can customize afterwards by re-calling.
                .AddTransient<ISharedResourcesLocalizer>(provider =>
                {
                    var localizerFactory = provider.GetRequiredService<IStringLocalizerFactory>();
                    var localizer = localizerFactory.Create(typeof(SharedResources).Name, typeof(SharedResources).Assembly.FullName!);
                    return new SharedResourcesLocalizer(localizer);
                });

            return services;
        }

        /// <summary>
        /// Registers services required for using the Tableau Migration SDK.
        /// </summary>
        /// <param name="services">The service collection to register services with.</param>
        /// <param name="userOptions">The configuration options to initialize the SDK with</param>
        /// <returns>The same service collection as the <paramref name="services"/> parameter.</returns>
        public static IServiceCollection AddTableauMigrationSdk(this IServiceCollection services, IConfiguration? userOptions = null)
        {
            services.AddHttpServices();

            if (userOptions is not null)
            {
                services.Configure<MigrationSdkOptions>(nameof(MigrationSdkOptions), userOptions);
            }

            services.AddMigrationApiClient()
                .AddMigrationEngine();

            return services;
        }
    }
}

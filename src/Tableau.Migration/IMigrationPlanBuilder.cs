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
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Engine.Hooks.Filters;
using Tableau.Migration.Engine.Hooks.Mappings;
using Tableau.Migration.Engine.Hooks.Transformers;
using Tableau.Migration.Engine.Options;

namespace Tableau.Migration
{
    /// <summary>
    /// Interface for an object that can build <see cref="IMigrationPlan"/> objects.
    /// </summary>
    public interface IMigrationPlanBuilder
    {
        /// <summary>
        /// Initializes the plan to perform a migration of content between a Tableau Server and Tableau Cloud site.
        /// </summary>
        /// <returns>The same plan builder object for fluent API calls.</returns>
        IServerToCloudMigrationPlanBuilder ForServerToCloud();

        /// <summary>
        /// Clears all hooks, filters, mappings, and transformations.
        /// </summary>
        /// <returns>The same plan builder object for fluent API calls.</returns>
        IMigrationPlanBuilder ClearExtensions();

        /// <summary>
        /// Adds default hooks, filters, etc. that are common between all migration scenarios.
        /// </summary>
        /// <returns>The same plan builder object for fluent API calls.</returns>
        IMigrationPlanBuilder AppendDefaultExtensions();

        /// <summary>
        /// Sets or overwrites the configuration for the source endpoint to migrate content from.
        /// </summary>
        /// <param name="config">The endpoint configuration.</param>
        /// <returns>The same plan builder object for fluent API calls.</returns>
        IMigrationPlanBuilder FromSource(IMigrationPlanEndpointConfiguration config);

        /// <summary>
        /// Sets or overwrites the configuration for the source Tableau Server site to migrate content from.
        /// </summary>
        /// <param name="serverUrl">The base URL of the Tableau Server to connect to.</param>
        /// <param name="siteContentUrl">The URL namespace of the site to connect to. Can be empty string for default site.</param>
        /// <param name="accessTokenName">The name of the personal access token to use to sign into the site.</param>
        /// <param name="accessToken">The personal access token to use to sign into the site.</param>
        /// <param name="createApiSimulator">Whether or not to create an API simulator for the <paramref name="serverUrl"/>.</param>
        /// <returns>The same plan builder object for fluent API calls.</returns>
        IMigrationPlanBuilder FromSourceTableauServer(Uri serverUrl, string siteContentUrl, string accessTokenName, string accessToken, bool createApiSimulator = false);

        /// <summary>
        /// Sets or overwrites the configuration for the destination endpoint to migrate content to.
        /// </summary>
        /// <param name="config">The endpoint configuration.</param>
        /// <returns>The same plan builder object for fluent API calls.</returns>
        IMigrationPlanBuilder ToDestination(IMigrationPlanEndpointConfiguration config);

        /// <summary>
        /// Sets or overwrites the configuration for the destination Tableau Cloud site to migrate content to.
        /// </summary>
        /// <param name="podUrl">The base URL of Tableau Cloud pod to connect to.</param>
        /// <param name="siteContentUrl">The URL namespace of the site to connect to.</param>
        /// <param name="accessTokenName">The name of the personal access token to use to sign into the site.</param>
        /// <param name="accessToken">The personal access token to use to sign into the site.</param>
        /// <param name="createApiSimulator">Whether or not to create an API simulator for the <paramref name="podUrl"/>.</param>
        /// <returns>The same plan builder object for fluent API calls.</returns>
        IMigrationPlanBuilder ToDestinationTableauCloud(Uri podUrl, string siteContentUrl, string accessTokenName, string accessToken, bool createApiSimulator = false);

        /// <summary>
        /// Gets the per-plan options to supply.
        /// </summary>
        IMigrationPlanOptionsBuilder Options { get; }

        /// <summary>
        /// Gets the hooks to execute at various points during the migration, determined by hook type.
        /// </summary>
        IMigrationHookBuilder Hooks { get; }

        /// <summary>
        /// Validates that the plan that would be built has enough information to execute.
        /// </summary>
        /// <returns>The validation result.</returns>
        IResult Validate();

        /// <summary>
        /// Gets the mappings to execute at various points during the migration.
        /// </summary>
        IContentMappingBuilder Mappings { get; }

        /// <summary>
        /// Gets the filters to execute at various points during the migration.
        /// </summary>
        IContentFilterBuilder Filters { get; }

        /// <summary>
        /// Gets the transformations to execute at various points during the migration.
        /// </summary>
        IContentTransformerBuilder Transformers { get; }

        /// <summary>
        /// Finalizes the <see cref="IMigrationPlan"/> based on the current state.
        /// </summary>
        /// <returns>The created <see cref="IMigrationPlan"/>.</returns>
        IMigrationPlan Build();
    }
}

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
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks.Mappings;
using Tableau.Migration.Engine.Hooks.Mappings.Default;

namespace Tableau.Migration
{
    /// <summary>
    /// Interface for an object that can build <see cref="IMigrationPlan"/> objects
    /// that migrate content from Tableau Server to Tableau Cloud.
    /// </summary>
    public interface IServerToCloudMigrationPlanBuilder : IMigrationPlanBuilder
    {
        /// <summary>
        /// Appends default hooks, filters, mappings, and transformations for server-to-cloud migrations.
        /// This method is intended for upgrading existing plan builders - 
        /// new plan builders should use <see cref="IMigrationPlanBuilder.ForServerToCloud"/> instead.
        /// </summary>
        /// <returns>The same plan builder object for fluent API calls.</returns>
        IServerToCloudMigrationPlanBuilder AppendDefaultServerToCloudExtensions();

        #region - Authentication Type/Domain Mapping -

        /// <summary>
        /// Adds an object to map user and group domains based
        /// on the SAML authentication type.
        /// </summary>
        /// <param name="domain">The domain to map users and groups to.</param>
        /// <returns>The same plan builder object for fluent API calls.</returns>
        IServerToCloudMigrationPlanBuilder WithSamlAuthenticationType(string domain);

        /// <summary>
        /// Adds an object to map user and group domains based
        /// on the Tableau ID authentication type.
        /// </summary>
        /// <param name="mfa">Whether or not MFA is used, defaults to true.</param>
        /// <returns>The same plan builder object for fluent API calls.</returns>
        IServerToCloudMigrationPlanBuilder WithTableauIdAuthenticationType(bool mfa = true);

        /// <summary>
        /// Adds an object to map user and group domains based
        /// on the destination authentication type.
        /// </summary>
        /// <param name="authenticationType">The authentication type to assign to users.</param>
        /// <param name="userDomain">The domain to map users to.</param>
        /// <param name="groupDomain">The domain to map groups to.</param>
        /// <returns>The same plan builder object for fluent API calls.</returns>
        IServerToCloudMigrationPlanBuilder WithAuthenticationType(string authenticationType, string userDomain, string groupDomain);

        /// <summary>
        /// Adds an object to map user and group domains based
        /// on the destination authentication type.
        /// </summary>
        /// <param name="authenticationType">An authentication type to assign to users.</param>
        /// <param name="authenticationTypeMapping">The mapping to execute.</param>
        /// <returns>The same plan builder object for fluent API calls.</returns>
        IServerToCloudMigrationPlanBuilder WithAuthenticationType(string authenticationType, IAuthenticationTypeDomainMapping authenticationTypeMapping);

        /// <summary>
        /// Adds an object to map user and group domains based
        /// on the destination authentication type.
        /// </summary>
        /// <typeparam name="TMapping">The mapping type.</typeparam>
        /// <param name="authenticationType">An authentication type to assign to users.</param>
        /// <param name="authenticationTypeMappingFactory">An initializer function to create the object from, potentially from the migration-scoped dependency injection container.</param>
        /// <returns>The same plan builder object for fluent API calls.</returns>
        IServerToCloudMigrationPlanBuilder WithAuthenticationType<TMapping>(string authenticationType, Func<IServiceProvider, TMapping>? authenticationTypeMappingFactory = null)
            where TMapping : IAuthenticationTypeDomainMapping;

        /// <summary>
        /// Adds an object to map user and group domains based
        /// on the destination authentication type.
        /// </summary>
        /// <param name="authenticationType">An authentication type to assign to users.</param>
        /// <param name="callback">A callback to call for the mapping.</param>
        /// <returns>The same plan builder object for fluent API calls.</returns>
        IServerToCloudMigrationPlanBuilder WithAuthenticationType(string authenticationType,
            Func<ContentMappingContext<IUsernameContent>, CancellationToken, Task<string?>> callback);

        #endregion

        #region - WithTableauCloudUsernames -

        /// <summary>
        /// Adds an object to map usernames to be in the form of an email.
        /// </summary>
        /// <param name="mailDomain">
        /// A domain name to use to build email usernames for users that lack emails.
        /// Usernames will be generated as "{username}@<paramref name="mailDomain"/>".
        /// </param>
        /// <param name="useExistingEmail">
        /// Whether or not existing user emails should be used when available, defaults to true.
        /// </param>
        /// <returns>The same plan builder object for fluent API calls.</returns>
        IServerToCloudMigrationPlanBuilder WithTableauCloudUsernames(string mailDomain, bool useExistingEmail = true);

        /// <summary>
        /// Adds an object to map usernames to be in the form of an email.
        /// </summary>
        /// <param name="usernameMapping">The mapping to execute.</param>
        /// <returns>The same plan builder object for fluent API calls.</returns>
        IServerToCloudMigrationPlanBuilder WithTableauCloudUsernames(ITableauCloudUsernameMapping usernameMapping);

        /// <summary>
        /// Adds an object to map usernames to be in the form of an email.
        /// </summary>
        /// <typeparam name="TMapping">The mapping type.</typeparam>
        /// <param name="usernameMappingFactory">An initializer function to create the object from, potentially from the migration-scoped dependency injection container.</param>
        /// <returns>The same plan builder object for fluent API calls.</returns>
        IServerToCloudMigrationPlanBuilder WithTableauCloudUsernames<TMapping>(Func<IServiceProvider, TMapping>? usernameMappingFactory = null)
            where TMapping : ITableauCloudUsernameMapping;

        /// <summary>
        /// Adds an object to map usernames to be in the form of an email.
        /// </summary>
        /// <param name="callback">A callback to call for the mapping.</param>
        /// <returns>The same plan builder object for fluent API calls.</returns>
        IServerToCloudMigrationPlanBuilder WithTableauCloudUsernames(
            Func<ContentMappingContext<IUser>, CancellationToken, Task<ContentMappingContext<IUser>?>> callback);

        #endregion
    }
}

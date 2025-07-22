//
//  Copyright (c) 2025, Salesforce, Inc.
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Api.Simulation;
using Tableau.Migration.Content.Permissions;

namespace Tableau.Migration.Tests.Simulation.DataPreparation
{
    /// <summary>
    /// Base class for data preparation classes that provides common functionality.
    /// </summary>
    public static class CommonDataPreparation
    {
        /// <summary>
        /// Gets the non-support users from the source API.
        /// </summary>
        /// <param name="sourceApi">The source API simulator.</param>
        /// <returns>A list of non-support users.</returns>
        public static List<UsersResponse.UserType> GetNonSupportUsers(TableauApiSimulator sourceApi)
            => sourceApi.Data.Users.Where(u => u.SiteRole != SiteRoles.SupportUser).ToList();

        /// <summary>
        /// Creates permissions for users and groups.
        /// </summary>
        /// <param name="users">The list of users.</param>
        /// <param name="groups">The list of groups.</param>
        /// <param name="fixture">The fixture for creating test data.</param>
        /// <returns>A new permissions type with grantee capabilities.</returns>
        public static PermissionsType CreatePermissions(
            List<UsersResponse.UserType> users,
            IEnumerable<GroupsResponse.GroupType> groups,
            IFixture fixture)
        {
            return new PermissionsType
            {
                GranteeCapabilities = users.Select(u => new GranteeCapabilityType
                {
                    User = new GranteeCapabilityType.UserType
                    {
                        Id = u.Id
                    },
                    Capabilities = fixture.CreateMany<ICapability>(2).Select(c => new CapabilityType(c)).ToArray()
                })
                .Concat(groups.Select(g => new GranteeCapabilityType
                {
                    Group = new GranteeCapabilityType.GroupType
                    {
                        Id = g.Id
                    },
                    Capabilities = fixture.CreateMany<ICapability>(2).Select(c => new CapabilityType(c)).ToArray()
                }))
                .ToArray()
            };
        }

        /// <summary>
        /// Creates embedded credentials for a content item.
        /// </summary>
        /// <param name="keychainCollection">The collection to store the keychains.</param>
        /// <param name="users">The list of users.</param>
        /// <param name="itemId">The ID of the content item.</param>
        /// <param name="counter">A counter value to determine the type of keychain to create.</param>
        /// <param name="fixture">The fixture for creating test data.</param>
        /// <returns>The created keychain response.</returns>
        public static RetrieveKeychainResponse CreateEmbeddedCredentials(
            ConcurrentDictionary<Guid, RetrieveKeychainResponse> keychainCollection,
            List<UsersResponse.UserType> users,
            Guid itemId,
            int counter,
            IFixture fixture)
        {
            var keychains = (counter % 3) switch
            {
                0 => new RetrieveKeychainResponse(),
                1 => new RetrieveKeychainResponse(fixture.CreateMany<string>(), [users[counter % users.Count].Id]),
                _ => new RetrieveKeychainResponse(fixture.CreateMany<string>(), [users[counter % users.Count].Id, users[(counter + 1) % users.Count].Id])
            };

            keychainCollection.TryAdd(itemId, keychains);
            return keychains;
        }

        /// <summary>
        /// Creates connections for a content item.
        /// </summary>
        /// <typeparam name="T">The type of data that contains connections.</typeparam>
        /// <param name="data">The data object to add connections to.</param>
        /// <param name="embed">Whether to embed credentials in the connections.</param>
        /// <param name="fixture">The fixture for creating test data.</param>
        /// <param name="connectionCount">The number of connections to create.</param>
        public static void CreateConnections<T>(T data, bool embed, IFixture fixture, int connectionCount = 2)
            where T : SimulatedDataWithConnections
        {
            for (int i = 0; i < connectionCount; i++)
            {
                var conn = fixture.Create<SimulatedConnection>();
                data.Connections.Add(conn);

                if (embed)
                {
                    conn.Credentials = fixture.Create<SimulatedConnectionCredentials>();
                    conn.Credentials.Embed = "true";
                }
                else if (conn.Credentials is not null)
                {
                    conn.Credentials.Embed = null;
                }
            }
        }

        /// <summary>
        /// Creates an owner reference for a content item.
        /// </summary>
        /// <typeparam name="T">The type of owner reference to create.</typeparam>
        /// <param name="user">The user to create the owner reference for.</param>
        /// <returns>A new owner reference.</returns>
        public static T CreateOwnerReference<T>(UsersResponse.UserType user)
            where T : class, new()
        {
            var owner = new T();
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty is not null)
            {
                idProperty.SetValue(owner, user.Id);
            }
            return owner;
        }

        /// <summary>
        /// Creates a project reference for a content item.
        /// </summary>
        /// <typeparam name="T">The type of project reference to create.</typeparam>
        /// <param name="project">The project to create the reference for.</param>
        /// <returns>A new project reference.</returns>
        public static T CreateProjectReference<T>(ProjectsResponse.ProjectType project)
            where T : class, new()
        {
            var projectRef = new T();
            var idProperty = typeof(T).GetProperty("Id");
            var nameProperty = typeof(T).GetProperty("Name");
            if (idProperty is not null)
            {
                idProperty.SetValue(projectRef, project.Id);
            }
            if (nameProperty is not null)
            {
                nameProperty.SetValue(projectRef, project.Name);
            }
            return projectRef;
        }
    }
}
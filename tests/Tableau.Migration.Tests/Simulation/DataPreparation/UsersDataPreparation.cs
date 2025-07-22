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
using System.Collections.Generic;
using AutoFixture;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Simulation;
using Tableau.Migration.Config;

namespace Tableau.Migration.Tests.Simulation.DataPreparation
{
    /// <summary>
    /// Static class responsible for preparing users data for migration tests.
    /// </summary>
    public static class UsersDataPreparation
    {
        /// <summary>
        /// Prepares the source data for migration tests.
        /// </summary>
        /// <param name="sourceApi">The source API simulator.</param>
        /// <param name="fixture">The fixture for creating test data.</param>
        /// <param name="count">Optional count parameter to control the amount of data prepared.</param>
        /// <returns>A tuple containing non-support users and support users.</returns>
        public static (List<UsersResponse.UserType> NonSupportUsers, List<UsersResponse.UserType> SupportUsers) PrepareServerSource(
            TableauApiSimulator sourceApi,
            IFixture fixture,
            int? count = null)
        {
            ArgumentNullException.ThrowIfNull(sourceApi);
            ArgumentNullException.ThrowIfNull(fixture);

            var allSiteRoles = SiteRoles.GetAll();
            var numSourceUsers = count ?? (int)Math.Ceiling(ContentTypesOptions.Defaults.BATCH_SIZE * 2.5);

            var nonSupportUsers = new List<UsersResponse.UserType>();
            var supportUsers = new List<UsersResponse.UserType>();

            for (int i = 0; i < numSourceUsers; i++)
            {
                var user = fixture.Create<UsersResponse.UserType>();
                user.Domain = i % 2 == 0 ? new UsersResponse.UserType.DomainType { Name = "local" } : fixture.Create<UsersResponse.UserType.DomainType>();

                // Wrong - Work item in in backlog
                // This is not how the response should be built. The domain does not go into the name for UsersResponse.UserType
                // Replace domain name in the user name
                if (user.Name != null)
                {
                    var currentDomainName = TableauData.GetUserDomain(user)?.Name;
                    if (currentDomainName != null)
                    {
                        user.Name = user.Name.Replace(currentDomainName, user.Domain.Name);
                    }
                }
                user.SiteRole = allSiteRoles[i % allSiteRoles.Count];

                if (user.SiteRole == SiteRoles.SupportUser)
                {
                    supportUsers.Add(user);
                }
                else
                {
                    nonSupportUsers.Add(user);
                }

                sourceApi.Data.Users.Add(user);

                var savedCredentials = (i % 2) switch
                {
                    0 => new RetrieveKeychainResponse(),
                    _ => new RetrieveKeychainResponse(fixture.CreateMany<string>(), [user.Id])
                };
                sourceApi.Data.UserSavedCredentials.TryAdd(user.Id, savedCredentials);
                sourceApi.Data.UserFavorites[user.Id] = new();
            }

            return (nonSupportUsers, supportUsers);
        }
    }
}
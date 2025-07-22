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
using System.Collections.Immutable;
using System.Linq;
using AutoFixture;
using Tableau.Migration.Api.Simulation;
using Tableau.Migration.Content;

namespace Tableau.Migration.Tests.Simulation.DataPreparation
{
    /// <summary>
    /// Static class responsible for preparing favorites data for migration tests.
    /// </summary>
    public static class FavoritesDataPreparation
    {
        /// <summary>
        /// Prepares the source data for migration tests.
        /// </summary>
        /// <param name="sourceApi">The source API simulator.</param>
        /// <param name="fixture">The fixture for creating test data.</param>
        /// <returns>The prepared data.</returns>
        public static ImmutableDictionary<Guid, ConcurrentDictionary<(FavoriteContentType, Guid), string>> PrepareServerSource(
            TableauApiSimulator sourceApi,
            IFixture fixture)
        {
            ArgumentNullException.ThrowIfNull(sourceApi);
            ArgumentNullException.ThrowIfNull(fixture);

            var result = new ConcurrentDictionary<Guid, ConcurrentDictionary<(FavoriteContentType, Guid), string>>();

            foreach (var user in sourceApi.Data.Users)
            {
                AddUserFavorite(user.Id, FavoriteContentType.Project, sourceApi.Data.Projects.PickRandom().Id, sourceApi, fixture, result);
                AddUserFavorite(user.Id, FavoriteContentType.DataSource, sourceApi.Data.DataSources.PickRandom().Id, sourceApi, fixture, result);
                AddUserFavorite(user.Id, FavoriteContentType.Workbook, sourceApi.Data.Workbooks.PickRandom().Id, sourceApi, fixture, result);

                var viewId = sourceApi.Data.Workbooks.Where(x => !x.Views.IsNullOrEmpty()).FirstOrDefault()?.Views.PickRandom().Id;
                if (viewId.HasValue)
                {
                    AddUserFavorite(user.Id, FavoriteContentType.View, viewId.Value, sourceApi, fixture, result);
                }
            }

            return result.ToImmutableDictionary();
        }

        private static void AddUserFavorite(
            Guid userId,
            FavoriteContentType contentType,
            Guid contentId, TableauApiSimulator sourceApi,
            IFixture fixture,
            ConcurrentDictionary<Guid, ConcurrentDictionary<(FavoriteContentType, Guid), string>> result)
        {
            var label = fixture.Create<string>();
            var userFavorites = result.GetOrAdd(userId, (id) => new ConcurrentDictionary<(FavoriteContentType, Guid), string>());
            userFavorites[(contentType, contentId)] = label;
            sourceApi.Data.AddUserFavorite(userId, contentType, contentId, label);
        }
    }
}
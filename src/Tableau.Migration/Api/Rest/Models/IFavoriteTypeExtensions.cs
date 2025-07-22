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
using Tableau.Migration.Content;

namespace Tableau.Migration.Api.Rest.Models
{
    /// <summary>
    /// Static class with extension methods for <see cref="IFavoriteType"/> objects.
    /// </summary>
    public static class IFavoriteTypeExtensions
    {
        /// <summary>
        /// Gets the referenced content type of the favorite.
        /// </summary>
        public static FavoriteContentType GetContentType(this IFavoriteType favorite)
        {
            return favorite switch
            {
                IFavoriteType { DataSource: not null } => FavoriteContentType.DataSource,
                IFavoriteType { Flow: not null } => FavoriteContentType.Flow,
                IFavoriteType { Project: not null } => FavoriteContentType.Project,
                IFavoriteType { View: not null } => FavoriteContentType.View,
                IFavoriteType { Workbook: not null } => FavoriteContentType.Workbook,
                IFavoriteType { Collection: not null } => FavoriteContentType.Collection,
                _ => FavoriteContentType.Unknown
            };
        }

        /// <summary>
        /// Gets the referenced content ID of the favorite.
        /// </summary>
        public static Guid GetContentId(this IFavoriteType favorite)
        {
            return favorite switch
            {
                IFavoriteType { DataSource: not null } => favorite.DataSource.Id,
                IFavoriteType { Flow: not null } => favorite.Flow.Id,
                IFavoriteType { Project: not null } => favorite.Project.Id,
                IFavoriteType { View: not null } => favorite.View.Id,
                IFavoriteType { Workbook: not null } => favorite.Workbook.Id,
                IFavoriteType { Collection: not null } => favorite.Collection.Id,
                _ => Guid.Empty
            };
        }
    }
}

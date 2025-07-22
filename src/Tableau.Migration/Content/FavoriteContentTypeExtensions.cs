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
using Tableau.Migration.Engine.Pipelines;

namespace Tableau.Migration.Content
{
    /// <summary>
    /// Static class with extension methods for <see cref="FavoriteContentType"/>.
    /// </summary>
    public static class FavoriteContentTypeExtensions
    {
        /// <summary>
        /// Converts a favorite content type to its corresponding migration content type.
        /// </summary>
        /// <param name="contentType">The favorite content type.</param>
        /// <returns>The corresponding migration content type.</returns>
        /// <exception cref="ArgumentException">Thrown when the favorite content type cannot be converted to a migration content type.</exception>
        public static Type ToMigrationContentType(this FavoriteContentType contentType)
        {
            return contentType switch
            {
                FavoriteContentType.DataSource => MigrationPipelineContentType.DataSources.ContentType,
                FavoriteContentType.Flow => typeof(IFlow),
                FavoriteContentType.Project => MigrationPipelineContentType.Projects.ContentType,
                FavoriteContentType.View => MigrationPipelineContentType.Views.ContentType,
                FavoriteContentType.Workbook => MigrationPipelineContentType.Workbooks.ContentType,
                FavoriteContentType.Collection => typeof(ITableauCollection),
                _ => throw new ArgumentException($"Favorite content type {contentType} cannot be converted to a migration content type.", nameof(contentType))
            };
        }

        /// <summary>
        /// Returns whether or not migration of a FavoriteContentType is supported by the Migration SDK.
        /// </summary>
        /// <param name="contentType">The <see cref="FavoriteContentType"/> value.</param>
        /// <returns>Whether or not migration of the <see cref="FavoriteContentType"/> is supported.</returns>
        public static bool IsMigrationSupported(this FavoriteContentType contentType)
        {
            return contentType switch
            {
                FavoriteContentType.Unknown
                or FavoriteContentType.Flow
                or FavoriteContentType.Collection
                  => false,
                _ => true,
            };
        }
    }
}
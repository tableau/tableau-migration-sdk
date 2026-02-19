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

using Tableau.Migration.Content;
using Tableau.Migration.Content.Search;

namespace Tableau.Migration.Engine.Endpoints.Search
{
    /// <summary>
    /// <see cref="IContentReferenceFinderFactory"/> implementation that finds source references
    /// from the migration manifest.
    /// </summary>
    public class SourceContentReferenceFinderFactory : ISourceContentReferenceFinderFactory
    {
        private readonly IMigration _migration;

        /// <summary>
        /// Creates a new <see cref="SourceContentReferenceFinderFactory"/> object.
        /// </summary>
        /// <param name="migration">The migration.</param>
        public SourceContentReferenceFinderFactory(IMigration migration)
        {
            // We DI the migration instead of individual properties to avoid DI cycles. 
            _migration = migration;
        }

        /// <inheritdoc />
        public virtual IContentReferenceFinder<TContent> ForContentType<TContent>()
            where TContent : class, IContentReference
        {
            if(typeof(TContent) == typeof(IView))
            {
                return (IContentReferenceFinder<TContent>)_migration.Source.GetViewsContentClient();
            }
            else
            {
                return ForSourceContentType<TContent>();
            }
        }

        /// <inheritdoc />
        public virtual ISourceContentReferenceFinder<TContent> ForSourceContentType<TContent>()
            where TContent : class, IContentReference
            => _migration.Pipeline.CreateSourceContentReferenceFinder<TContent>();
    }
}

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

using System.Collections.Immutable;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Engine.Actions;
using Tableau.Migration.Engine.Conversion;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Endpoints.Caching;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Migrators;
using Tableau.Migration.Engine.Migrators.Batch;
using Tableau.Migration.Engine.Preparation;

namespace Tableau.Migration.Engine.Pipelines
{
    /// <summary>
    /// Interface for an object that defines the steps and strategies needed to perform a migration of Tableau data.
    /// </summary>
    public interface IMigrationPipeline
    {
        /// <summary>
        /// Builds the ordered actions in the pipeline to execute.
        /// </summary>
        ImmutableArray<IMigrationAction> BuildActions();

        /// <summary>
        /// Gets a content type level migrator for the given content type.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <returns>The content type level migrator.</returns>
        IContentMigrator<TContent> GetMigrator<TContent>()
            where TContent : class, IContentReference;

        /// <summary>
        /// Gets a loader for the given content type.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <returns>The content loader.</returns>
        IMigrationContentLoader<TContent> GetContentLoader<TContent>();

        /// <summary>
        /// Gets a batch level migrator for the given content type.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <returns>The batch level migrator.</returns>
        IContentBatchMigrator<TContent> GetBatchMigrator<TContent>()
            where TContent : class, IContentReference;

        /// <summary>
        /// Gets a content item preparer for the given content and publish types.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <typeparam name="TPrepare">The type being prepared.</typeparam>
        /// <typeparam name="TPublish">The publish type.</typeparam>
        /// <returns>The content preparer.</returns>
        IContentItemPreparer<TContent, TPublish> GetItemPreparer<TContent, TPrepare, TPublish>()
            where TContent : class, IContentReference
            where TPrepare : class, IContentReference
            where TPublish : class;

        /// <summary>
        /// Gets a content item converter for the given prepare and publish types.
        /// </summary>
        /// <typeparam name="TPrepare">The type being prepared.</typeparam>
        /// <typeparam name="TPublish">The publish type.</typeparam>
        /// <returns></returns>
        IContentItemConverter<TPrepare, TPublish> GetItemConverter<TPrepare, TPublish>()
            where TPrepare : class
            where TPublish : class;

        /// <summary>
        /// Gets the source content finder for the given content type.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <returns>The source content finder.</returns>
        ISourceContentReferenceFinder<TContent> CreateSourceContentReferenceFinder<TContent>()
            where TContent : IContentReference;

        /// <summary>
        /// Gets the destination content finder for the given content type.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <returns>The destination content finder.</returns>
        IDestinationContentReferenceFinder<TContent> CreateDestinationContentReferenceFinder<TContent>()
            where TContent : IContentReference;

        /// <summary>
        /// Gets the source cache loading strategy for the given content type.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <returns>The source cache loading strategy.</returns>
        IContentReferenceCacheLoadStrategy<TContent> CreateSourceCacheLoadStrategy<TContent>()
            where TContent : IContentReference;

        /// <summary>
        /// Gets the destination cache loading strategy for the given content type.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <returns>The destination cache loading strategy.</returns>
        IContentReferenceCacheLoadStrategy<TContent> CreateDestinationCacheLoadStrategy<TContent>()
            where TContent : IContentReference;

        /// <summary>
        /// Gets the source cache for the given content type.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <returns>The source cache.</returns>
        IContentReferenceCache CreateSourceCache<TContent>()
            where TContent : class, IContentReference;

        /// <summary>
        /// Gets the destination cache for the given content type.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <returns>The destination cache.</returns>
        IContentReferenceCache CreateDestinationCache<TContent>()
            where TContent : class, IContentReference;

        /// <summary>
        /// Gets the destination locked project cache.
        /// </summary>
        /// <returns>The destination locked project cache.</returns>
        ILockedProjectCache GetDestinationLockedProjectCache();
    }
}

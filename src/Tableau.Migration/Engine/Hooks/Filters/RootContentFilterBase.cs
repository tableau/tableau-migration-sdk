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

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks.Filters
{
    /// <summary>
    /// Base implementation for an object that can filter content of a specific content type, to determine which content to migrate.
    /// </summary>
    /// <typeparam name="TContent"><inheritdoc/></typeparam>
    public abstract class RootContentFilterBase<TContent> : IContentFilter<TContent>
        where TContent : IContentReference
    {
        /// <summary>
        /// Gets the string localizer.
        /// </summary>
        protected ISharedResourcesLocalizer? Localizer { get; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        protected ILogger<IContentFilter<TContent>>? Logger { get; }

        /// <summary>
        /// Gets or sets whether the filter is disabled.
        /// </summary>
        protected virtual bool Disabled { get; set; }

        /// <summary>
        /// Creates a new <see cref="RootContentFilterBase{TContent}"/> object.
        /// </summary>
        /// <param name="localizer">The string localizer.</param>
        /// <param name="logger">The logger.</param>
        public RootContentFilterBase(ISharedResourcesLocalizer? localizer, ILogger<IContentFilter<TContent>>? logger)
        {
            Localizer = localizer;
            Logger = logger;
        }

        /// <inheritdoc />
        public abstract Task<IEnumerable<ContentMigrationItem<TContent>>?> ExecuteAsync(IEnumerable<ContentMigrationItem<TContent>> ctx, CancellationToken cancel);
    }
}

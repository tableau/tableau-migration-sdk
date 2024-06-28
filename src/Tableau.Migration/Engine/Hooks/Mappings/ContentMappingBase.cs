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

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks.Mappings
{
    /// <summary>
    /// Base implementation for an object that can map content of a specific content type.
    /// </summary>
    /// <typeparam name="TContent"><inheritdoc/></typeparam>
    public abstract class ContentMappingBase<TContent> : IContentMapping<TContent>
        where TContent : IContentReference
    {
        private readonly string _typeName;

        /// <summary>
        /// Default constructor for ContentMappingBase
        /// </summary>
        /// <param name="localizer">A string localizer.</param>
        /// <param name="logger">Default logger.</param>
        public ContentMappingBase(
            ISharedResourcesLocalizer? localizer,
            ILogger<IContentMapping<TContent>>? logger)
        {
            Localizer = localizer;
            Logger = logger;
            _typeName = GetType().Name;
        }

        /// <summary>
        /// The string localizer.
        /// </summary>
        protected ISharedResourcesLocalizer? Localizer { get; }

        /// <summary>
        /// Default logger.
        /// </summary>
        protected ILogger<IContentMapping<TContent>>? Logger { get; }

        /// <inheritdoc />
        public async Task<ContentMappingContext<TContent>?> ExecuteAsync(ContentMappingContext<TContent> ctx, CancellationToken cancel)
        {
            var ret = await MapAsync(ctx, cancel).ConfigureAwait(false);

            if (Logger is not null && Localizer is not null)
            {
                Logger.LogDebug(
                    Localizer[SharedResourceKeys.ContentMappingBaseDebugMessage],
                    _typeName,
                    ctx.ContentItem.ToStringForLog(),
                    ctx.MappedLocation);
            }

            return ret;
        }

        /// <summary>
        /// Executes the mapping.
        /// </summary>
        /// <param name="ctx">The input context from the migration engine or previous hook.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>
        /// A task to await containing the context, 
        /// potentially modified to pass on to the next hook or migration engine, 
        /// or null to continue passing the same context as <paramref name="ctx"/>.
        /// </returns>
        public abstract Task<ContentMappingContext<TContent>?> MapAsync(ContentMappingContext<TContent> ctx, CancellationToken cancel);
    }
}


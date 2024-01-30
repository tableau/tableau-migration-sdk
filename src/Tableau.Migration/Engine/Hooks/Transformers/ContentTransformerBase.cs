﻿// Copyright (c) 2023, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the ""License"") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an ""AS IS"" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks.Transformers
{
    /// <summary>
    /// Base implementation for an object that can transform content of a specific content type
    /// </summary>
    /// <typeparam name="TPublish"><inheritdoc/></typeparam>
    public abstract class ContentTransformerBase<TPublish> : IContentTransformer<TPublish>
        where TPublish : IContentReference
    {
        private readonly ISharedResourcesLocalizer? _localizer;
        private readonly ILogger<IContentTransformer<TPublish>>? _logger;
        private readonly string _typeName;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="localizer">A string localizer.</param>
        /// <param name="logger">Default logger.</param>
        public ContentTransformerBase(
            ISharedResourcesLocalizer? localizer,
            ILogger<IContentTransformer<TPublish>>? logger)
        {
            _localizer = localizer;
            _logger = logger;
            _typeName = this.GetType().Name;
        }

        /// <inheritdoc />
        public async Task<TPublish?> ExecuteAsync(TPublish itemToTransform, CancellationToken cancel)
        {
            var ret = await TransformAsync(itemToTransform, cancel).ConfigureAwait(false);

            if ((_logger is not null) && (_localizer is not null))
                _logger.LogDebug(_localizer[SharedResourceKeys.ContentTransformerBaseDebugMessage], _typeName, itemToTransform.ToStringForLog());

            return ret;
        }

        /// <summary>
        /// Executes the transformation.
        /// </summary>
        /// <param name="itemToTransform">The input context from the migration engine or previous hook.</param>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>
        /// A task to await containing the context, 
        /// potentially modified to pass on to the next hook or migration engine, 
        /// or null to continue passing the same context as <paramref name="itemToTransform"/>.
        /// </returns>
        public abstract Task<TPublish?> TransformAsync(TPublish itemToTransform, CancellationToken cancel);
    }
}

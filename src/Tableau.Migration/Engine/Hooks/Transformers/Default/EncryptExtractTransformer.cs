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
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks.Transformers.Default
{
    /// <summary>
    /// Transformer that encrypts extracts based on the site's encryption mode.
    /// </summary>
    /// <typeparam name="TIExtractContent"></typeparam>
    public class EncryptExtractTransformer<TIExtractContent> : ContentTransformerBase<TIExtractContent> where TIExtractContent : IContentReference, IExtractContent
    {
        private readonly ILogger _logger;
        private readonly ISharedResourcesLocalizer _localizer;
        private readonly IMigration _migration;
        private string? _siteExtractEncryptionMode = null;

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Creates a new EncryptExtractTransformer
        /// </summary>
        /// <param name="localizer"></param>
        /// <param name="logger"></param>
        /// <param name="migration"></param>
        public EncryptExtractTransformer(
            ISharedResourcesLocalizer localizer,
            ILogger<IContentTransformer<TIExtractContent>> logger,
            IMigration migration)
                : base(localizer, logger)
        {
            _migration = migration;
            _logger = logger;
            _localizer = localizer;
        }

        /// <inheritdoc />
        public override async Task<TIExtractContent?> TransformAsync(TIExtractContent itemToTransform, CancellationToken cancel)
        {
            // This transformer is registered with DI as scoped.
            // This means a new transformer instance is created per batch per content type.
            // To limit the number of _migration.Destination.GetSessionAsync calls, we cache the site's encryption mode
            // after the first call and make it thread safe by wrapping it in a semaphore of count 1 which is async-compatible.
            await _semaphore.WaitAsync(cancel).ConfigureAwait(false);
            try
            {
                if (_siteExtractEncryptionMode is null)
                {
                    var session = await _migration.Destination.GetSessionAsync(cancel).ConfigureAwait(false);

                    if (!session.Success || session.Value?.Settings?.ExtractEncryptionMode is null)
                    {
                        throw new System.Exception("Unable to determine site data source encryption mode.");
                    }

                    _siteExtractEncryptionMode = session.Value.Settings.ExtractEncryptionMode;
                }
            }
            finally
            {
                _semaphore.Release();
            }

            if (ExtractEncryptionModes.IsAMatch(_siteExtractEncryptionMode, ExtractEncryptionModes.Enforced))
            {
                itemToTransform.EncryptExtracts = true;
            }
            else if (ExtractEncryptionModes.IsAMatch(_siteExtractEncryptionMode, ExtractEncryptionModes.Disabled))
            {
                itemToTransform.EncryptExtracts = false;
            }

            return itemToTransform;
        }
    }
}

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

using System.Threading;
using System.Threading.Tasks;
using System.Text.Json.Nodes;
using Tableau.Migration.Content;

namespace Tableau.Migration.Engine.Hooks.Transformers
{
    /// <summary>
    /// Interface for a content transformer that manipulates the Tableau JSON file of a content item. 
    /// </summary>
    /// <typeparam name="TPublish"><inheritdoc /></typeparam>
    public interface IJsonContentTransformer<TPublish> : IContentTransformer<TPublish>
        where TPublish : IFileContent
    {
        /// <summary>
        /// Finds whether the content item needs any JSON changes, 
        /// returning false prevents file IO from occurring.
        /// </summary>
        /// <param name="ctx">The content item to inspect.</param>
        /// <returns>Whether or not the content item needs JSON changes.</returns>
        bool NeedsJsonTransforming(TPublish ctx);

        /// <summary>
        /// Transforms the JSON of the content item.
        /// </summary>
        /// <param name="ctx">The content item being transformed.</param>
        /// <param name="json">
        /// The mutable JSON of the content item to transform.
        /// Any changes made to the JSON are persisted back to the file before publishing.
        /// </param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>A task to await.</returns>
        Task TransformAsync(TPublish ctx, JsonNode json, CancellationToken cancel);

        /// <inheritdoc />
        async Task<TPublish?> IMigrationHook<TPublish>.ExecuteAsync(TPublish ctx, CancellationToken cancel)
        {
            if (!NeedsJsonTransforming(ctx))
            {
                return ctx;
            }

            //We expect the item preparer to finalize/dispose the file stream.
            var jsonStream = await ctx.File.GetJsonStreamAsync(cancel).ConfigureAwait(false);
            var json = await jsonStream.GetJsonAsync(cancel).ConfigureAwait(false);

            await TransformAsync(ctx, json, cancel).ConfigureAwait(false);

            return ctx;
        }
    }
}


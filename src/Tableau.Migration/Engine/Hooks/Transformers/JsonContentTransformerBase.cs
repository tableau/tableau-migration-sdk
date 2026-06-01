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
    /// Abstract base class for transformers that manipulate the Tableau JSON file of a content item. 
    /// </summary>
    /// <typeparam name="TPublish"><inheritdoc /></typeparam>
    public abstract class JsonContentTransformerBase<TPublish> : IJsonContentTransformer<TPublish>
        where TPublish : IFileContent
    {
        /// <summary>
        /// Finds whether the content item needs any JSON changes, 
        /// returning false prevents file IO from occurring.
        /// </summary>
        /// <param name="ctx">The content item to inspect.</param>
        /// <returns>Whether or not the content item needs JSON changes.</returns>
        protected virtual bool NeedsJsonTransforming(TPublish ctx) => true;

        bool IJsonContentTransformer<TPublish>.NeedsJsonTransforming(TPublish ctx) => NeedsJsonTransforming(ctx);

        /// <inheritdoc />
        public abstract Task TransformAsync(TPublish ctx, JsonNode json, CancellationToken cancel);
    }
}


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
using Tableau.Migration.Content;

namespace Tableau.Migration.Engine.Hooks.Transformers.Default
{
    /// <summary>
    /// Transformer that maps the user for a content item owner.
    /// </summary>
    public class OwnershipTransformer<TContent> : IContentTransformer<TContent>
        where TContent : IWithOwner
    {
        private readonly IMappedUserTransformer _userTransformer;

        /// <summary>
        /// Creates a new <see cref="OwnershipTransformer{TContent}"/> object.
        /// </summary>
        /// <param name="userTransformer">The user transformer.</param>
        public OwnershipTransformer(IMappedUserTransformer userTransformer)
        {
            _userTransformer = userTransformer;
        }

        /// <inheritdoc/>
        public async Task<TContent?> ExecuteAsync(TContent ctx, CancellationToken cancel)
        {
            var mapped = await _userTransformer.ExecuteAsync(ctx.Owner, cancel).ConfigureAwait(false);

            if (mapped is not null)
                ctx.Owner = mapped;

            return ctx;
        }
    }
}

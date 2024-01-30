// Copyright (c) 2023, Salesforce, Inc.
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

using Tableau.Migration.Engine.Hooks.PostPublish;

namespace Tableau.Migration.Interop.Hooks.PostPublish
{
    /// <summary>
    /// Interface representing a synchronous hook called when a migration attempt for bulk content items completes.
    /// </summary>
    /// <typeparam name="TSource"><inheritdoc/></typeparam>
    public interface ISyncBulkPostPublishHook<TSource>
        : ISyncMigrationHook<BulkPostPublishContext<TSource>>, IBulkPostPublishHook<TSource>
    {
        /// <summary>
        /// Executes a hook callback.
        /// </summary>
        /// <param name="ctx">The input context from the migration engine or previous hook.</param>
        /// <returns>
        /// The context, 
        /// potentially modified to pass on to the next hook or migration engine, 
        /// or null to continue passing the same context as <paramref name="ctx"/>.
        /// </returns>
        new BulkPostPublishContext<TSource>? Execute(BulkPostPublishContext<TSource> ctx);
    }
}

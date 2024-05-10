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

using Tableau.Migration.Engine.Hooks.Mappings;

namespace Tableau.Migration.Interop.Hooks.Mappings
{
    /// <summary>
    /// Interface for an object that can synchronously map content of a specific content type.
    /// </summary>
    /// <typeparam name="TContent"><inheritdoc/></typeparam>
    public interface ISyncContentMapping<TContent>
        : ISyncMigrationHook<ContentMappingContext<TContent>>, IContentMapping<TContent>
        where TContent : IContentReference
    {
        /// <summary>
        /// Executes a mapping callback.
        /// </summary>
        /// <param name="ctx">The input context from the migration engine or previous mapping.</param>
        /// <returns>
        /// The context, 
        /// potentially modified to pass on to the next mapping or migration engine, 
        /// or null to continue passing the same context as <paramref name="ctx"/>.
        /// </returns>
        new ContentMappingContext<TContent>? Execute(ContentMappingContext<TContent> ctx);
    }
}

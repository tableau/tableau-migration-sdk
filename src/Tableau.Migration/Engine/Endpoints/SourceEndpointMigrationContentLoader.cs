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

using Tableau.Migration.Paging;

namespace Tableau.Migration.Engine.Endpoints
{
    /// <summary>
    /// Default <see cref="IMigrationContentLoader{TContent}"/> implementation that gets all items from the source endpoint.
    /// </summary>
    /// <typeparam name="TContent"><inheritdoc /></typeparam>
    public class SourceEndpointMigrationContentLoader<TContent> : IMigrationContentLoader<TContent>
    {
        private readonly ISourceEndpoint _source;

        /// <summary>
        /// Creates a new <see cref="SourceEndpointMigrationContentLoader{TContent}"/> object.
        /// </summary>
        /// <param name="source">The source endpoint.</param>
        public SourceEndpointMigrationContentLoader(ISourceEndpoint source)
        {
            _source = source;
        }

        /// <inheritdoc />
        public IPager<TContent> GetMigrationContentPager(int pageSize)
            => _source.GetPager<TContent>(pageSize);
    }
}

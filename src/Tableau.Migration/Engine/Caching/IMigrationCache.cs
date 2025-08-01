﻿//
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

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Engine.Caching
{
    /// <summary>
    /// Interface for a cache containing data relevant to an ongoing migration.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    public interface IMigrationCache<TKey, TValue> : IDisposable
        where TKey : notnull
        where TValue : class
    {
        /// <summary>
        /// Adds a value to the cache.
        /// </summary>
        /// <param name="key">The key to add the value for.</param>
        /// <param name="value">The value to add.</param>
        void Add(TKey key, TValue value);

        /// <summary>
        /// Gets or retrieves a value from the cache.
        /// </summary>
        /// <param name="key">The key to get or retrive a value for.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>A result with the cached or retrieved value, or any errors that occurred.</returns>
        Task<IResult<TValue>> GetOrAddAsync(TKey key, CancellationToken cancel);
    }
}

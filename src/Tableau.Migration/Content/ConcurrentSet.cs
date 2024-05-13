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

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Tableau.Migration.Content
{
    /// <summary>
    /// Class representing a thread-safe unordered <see cref="ICollection{T}"/> implementation.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    public sealed class ConcurrentSet<T> : ICollection<T>
        where T : notnull
    {
        private readonly ConcurrentDictionary<T, byte> _inner = new();

        /// <summary>
        /// Gets the number of elements contained in the set.
        /// </summary>
        public int Count => _inner.Count;

        /// <summary>
        ///  Gets a value indicating whether the set is read-only.
        /// </summary>
        public bool IsReadOnly { get; } = false;

        /// <summary>
        /// Adds an item to the set.
        /// </summary>
        /// <param name="item">The object to add to the set.</param>
        public void Add(T item) => _inner.TryAdd(item, 0);

        /// <summary>
        /// Removes all items from the set.
        /// </summary>
        public void Clear() => _inner.Clear();

        /// <summary>
        /// Determines whether the set contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the set.</param>
        /// <returns>true if item is found in the set; otherwise, false.</returns>
        public bool Contains(T item) => _inner.ContainsKey(item);

        /// <summary>
        /// Copies the elements of the set to an <see cref="Array"/>, starting at a particular <see cref="Array"/> index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination of the elements copied
        /// from the set. The <see cref="Array"/> must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex) => _inner.Keys.CopyTo(array, arrayIndex);

        /// <summary>
        /// Removes the first occurrence of a specific object from the set.
        /// </summary>
        /// <param name="item">The object to remove from the set.</param>
        /// <returns>
        /// True if item was successfully removed from the set;
        /// otherwise, false. This method also returns false if item is not found in the
        /// original set.
        /// </returns>
        public bool Remove(T item) => _inner.TryRemove(item, out var _);

        /// <summary>
        /// Returns an enumerator that iterates through the set.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the set.</returns>
        public IEnumerator<T> GetEnumerator() => _inner.Keys.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _inner.Keys.GetEnumerator();
    }
}

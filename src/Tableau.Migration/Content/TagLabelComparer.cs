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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Tableau.Migration.Content
{
    /// <summary>
    /// <see cref="IEqualityComparer{ITag}"/> implementation that compares tags 
    /// by their string value with case sensitivity.
    /// </summary>
    public class TagLabelComparer : IEqualityComparer<ITag>
    {
        /// <summary>
        /// A singleton instance of the comparer.
        /// </summary>
        public static readonly TagLabelComparer Instance = new();

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>True if the specified objects are equal; otherwise, false.</returns>
        public bool Equals(ITag? x, ITag? y)
            => string.Equals(x?.Label, y?.Label, StringComparison.Ordinal);

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <param name="obj">The object for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object.</returns>
        public int GetHashCode([DisallowNull] ITag obj)
            => obj.Label.GetHashCode(StringComparison.Ordinal);
    }
}

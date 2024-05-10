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

namespace Tableau.Migration
{
    /// <summary>
    /// An <see cref="IEqualityComparer{Type}"/> class that considers derived types and their base types equal.
    /// </summary>
    internal class InheritedTypeComparer : IEqualityComparer<Type>
    {
        public static readonly InheritedTypeComparer Instance = new();

        public bool Equals(Type? x, Type? y)
        {
            if (x is null && y is null)
                return true;

            if (x is null || y is null)
                return false;

            if (x.Equals(y))
                return true;

            if (x.IsAssignableFrom(y) || y.IsAssignableFrom(x))
                return true;

            return false;
        }

        /// <summary>
        /// We always return zero here because otherwise any type (inherited or not) will not match any other.
        /// Dictionaries internally will check GetHashCode and Equals, so our comparison logic is in the Equals method.
        /// Because this relies on external implementation details, ensure that usage is tested thoroughly with the class
        /// doing the comparisons (i.e. Dictionary, HashSet, etc.).
        /// Do not use this method in your code!
        /// </summary>
        [Obsolete("Do not call this method in your code. See explanation in method comments.")]
        public int GetHashCode([DisallowNull] Type obj) => 0;
    }
}

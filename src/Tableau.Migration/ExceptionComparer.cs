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
using System.Collections.Generic;
using System.Reflection;

namespace Tableau.Migration
{
    /// <summary>
    /// Provides methods for comparing exceptions
    /// </summary>
    public class ExceptionComparer : IEqualityComparer<Exception>
    {
        ///<inheritdoc/>
        public bool Equals(Exception? x, Exception? y)
        {
            if (x is null && y is null) return true;
            if (x is null || y is null) return false;

            // Check if x and y are of the same type
            if (x.GetType() != y.GetType())
            {
                return false;
            }

            // Check if they implement IEquatable<T> for their specific type
            Type equatableType = typeof(IEquatable<>).MakeGenericType(x.GetType());
            if (equatableType.IsAssignableFrom(x.GetType()))
            {
                return (bool?)equatableType.GetMethod("Equals")?.Invoke(x, new object?[] { y }) ?? false;
            }
            else
            {
                return x.Message == y.Message;
            }
        }

        ///<inheritdoc/>
        public int GetHashCode(Exception obj)
        {
            // Check if the object's type overrides GetHashCode
            MethodInfo? getHashCodeMethod = obj.GetType().GetMethod("GetHashCode", Type.EmptyTypes);
            if (getHashCodeMethod != null && getHashCodeMethod.DeclaringType != typeof(object))
            {
                return obj.GetHashCode();
            }
            else if (obj is IEquatable<Exception>)
            {
                return obj.GetHashCode();
            }
            else
            {
                return obj.Message?.GetHashCode() ?? 0;
            }
        }

    }
}
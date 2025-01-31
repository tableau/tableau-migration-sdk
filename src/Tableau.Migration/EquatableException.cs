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

using System;

namespace Tableau.Migration
{
    /// <summary>
    /// A base class for custom exceptions that implements IEquatable to allow for equality comparison.
    /// </summary>
    /// <typeparam name="T">The type of the derived exception class.</typeparam>
    public abstract class EquatableException<T> : Exception, IEquatable<T> where T : EquatableException<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EquatableException{T}"/> class.
        /// </summary>
        protected EquatableException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EquatableException{T}"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        protected EquatableException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EquatableException{T}"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public EquatableException(string message, Exception innerException) : base(message, innerException) { }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return Equals(obj as T);
        }

        /// <inheritdoc/>
        public bool Equals(T? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return GetType() == other.GetType() && Message == other.Message && EqualsCore(other);
        }

        /// <summary>
        /// Determines whether the specified exception is equal to the current exception.
        /// Derived classes can override this method to add additional comparison logic.
        /// </summary>
        /// <param name="other">The exception to compare with the current exception.</param>
        /// <returns>true if the specified exception is equal to the current exception; otherwise, false.</returns>
        protected virtual bool EqualsCore(T other)
        {
            return true; // Default implementation, can be overridden by derived classes
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(GetType(), Message);
        }
    }
}

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

namespace Tableau.Migration.JsonConverters.Exceptions
{
    /// <summary>
    /// The exception that is thrown when a <see cref="IMigrationManifest"/> that was serialized and then deserialized was not able to create an exception of the correct type.
    /// </summary>
    /// <remarks>This means that the either the serializer or deserializer has a bug.</remarks>
    public class UnknownException : EquatableException<UnknownException>
    {
        /// <summary>
        /// The exception that is thrown when a <see cref="IMigrationManifest"/> that was serialized and then deserialized was not able to create an exception of the correct type.
        /// </summary>
        public UnknownException(string message) : base(message)
        { }

        /// <summary>
        /// The exception that is thrown when a <see cref="IMigrationManifest"/> that was serialized and then deserialized was not able to create an exception of the correct type.
        /// </summary>
        /// <param name="originalExceptionType">The type of the exception that was expected.</param>
        /// <param name="message">The message that describes the error.</param>
        public UnknownException(string originalExceptionType, string message) : base(message)
        {
            Data.Add("OriginalExceptionType", originalExceptionType);
        }

        /// <summary>
        /// The exception that is thrown when a <see cref="IMigrationManifest"/> that was serialized and then deserialized was not able to create an exception of the correct type.
        /// </summary>
        /// <param name="originalExceptionType">The type of the exception that was expected.</param>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public UnknownException(string originalExceptionType, string message, Exception innerException)
            : base(message, innerException)
        {
            Data.Add("OriginalExceptionType", originalExceptionType);
        }
    }
}
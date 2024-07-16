﻿//
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

namespace Tableau.Migration.JsonConverters.Exceptions
{
    /// <summary>
    /// The exception that is thrown when a <see cref="IMigrationManifest"/> that was serialized and then deserialized but did not match the initial Manifest.
    /// </summary>
    /// <remarks>This means that the either the serializer or deserializer has a bug.</remarks>
    public class MismatchException : Exception
    {
        /// <summary>
        /// The exception that is thrown when a <see cref="IMigrationManifest"/> that was serialized and then deserialized but did not match the initial Manifest.
        /// </summary>
        /// <remarks>This means that the either the serializer or deserializer has a bug.</remarks>
        public MismatchException()
        {
        }

        /// <summary>
        /// The exception that is thrown when a <see cref="IMigrationManifest"/> that was serialized and then deserialized but did not match the initial Manifest.
        /// </summary>
        /// <remarks>This means that the either the serializer or deserializer has a bug.</remarks>
        public MismatchException(string message) : base(message)
        {
        }

        /// <summary>
        /// The exception that is thrown when a <see cref="IMigrationManifest"/> that was serialized and then deserialized but did not match the initial Manifest.
        /// </summary>
        /// <remarks>This means that the either the serializer or deserializer has a bug.</remarks>
        public MismatchException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
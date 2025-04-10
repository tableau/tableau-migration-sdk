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

namespace Tableau.Migration.Api.Models
{
    /// <summary>
    /// Default <see cref="IKeychainUserMapping"/> implementation.
    /// </summary>
    public class KeychainUserMapping : IKeychainUserMapping
    {
        /// <summary>
        /// Creates a new <see cref="KeychainUserMapping"/> object.
        /// </summary>
        /// <param name="sourceUserId">The source user ID.</param>
        /// <param name="destinationUserId">The destination user ID.</param>
        public KeychainUserMapping(Guid sourceUserId, Guid destinationUserId)
        {
            SourceUserId = sourceUserId;
            DestinationUserId = destinationUserId;
        }

        /// <inheritdoc />
        public Guid SourceUserId { get; }

        /// <inheritdoc />
        public Guid DestinationUserId { get; }
    }
}

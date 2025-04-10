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

using System.Collections.Generic;

namespace Tableau.Migration.Api.Models
{
    /// <summary>
    /// Default <see cref="IApplyKeychainOptions"/> implementation.
    /// </summary>
    public class ApplyKeychainOptions : IApplyKeychainOptions
    {
        /// <summary>
        /// Creates a new <see cref="ApplyKeychainOptions"/> object.
        /// </summary>
        /// <param name="encryptedKeychains">The encrypted keychains to apply to the content item.</param>
        /// <param name="keychainUserMapping">The user mapping to use when applying the keychain.</param>
        public ApplyKeychainOptions(IEnumerable<string> encryptedKeychains, IEnumerable<IKeychainUserMapping> keychainUserMapping)
        { 
            EncryptedKeychains = encryptedKeychains;
            KeychainUserMapping = keychainUserMapping;
        }

        /// <inheritdoc />
        public IEnumerable<string> EncryptedKeychains { get; }

        /// <inheritdoc />
        public IEnumerable<IKeychainUserMapping> KeychainUserMapping { get; }
    }
}

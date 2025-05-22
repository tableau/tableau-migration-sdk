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
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Tableau.Migration.Api.Models;

namespace Tableau.Migration.Api.Rest.Models.Requests
{
    /// <summary>
    /// Class representing an apply keychain request.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class ApplyKeychainRequest : TableauServerRequest
    {
        /// <summary>
        /// Create a new <see cref="ApplyKeychainRequest"/> object.
        /// </summary>
        public ApplyKeychainRequest()
        { }

        /// <summary>
        /// Create a new <see cref="ApplyKeychainRequest"/> object.
        /// </summary>
        /// <param name="encryptedKeychains">The encrypted keychains to include in the request.</param>
        /// <param name="keychainUserMapping">The keychain user mapping.</param>
        public ApplyKeychainRequest(IEnumerable<string> encryptedKeychains, IEnumerable<IKeychainUserMapping>? keychainUserMapping)
        {
            EncryptedKeychains = encryptedKeychains.ToArray();

            if (keychainUserMapping.IsNullOrEmpty())
            {
                AssociatedUserLuidMapping = null;
            }
            else
            {
                AssociatedUserLuidMapping = keychainUserMapping.Select(m => new UserLuidPairType(m)).ToArray();
            }
        }

        /// <summary>
        /// Create a new <see cref="ApplyKeychainRequest"/> object.
        /// </summary>
        /// <param name="options">The request options.</param>
        public ApplyKeychainRequest(IApplyKeychainOptions options)
            : this(options.EncryptedKeychains, options.KeychainUserMapping)
        { }

        /// <summary>
        /// Gets or sets the array of keychains to apply.
        /// </summary>
        [XmlArray("encryptedKeychainList")]
        [XmlArrayItem("encryptedKeychain")]
        public string[] EncryptedKeychains { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Gets or sets the array of user LUID mapping pairs.
        /// </summary>
        [XmlArray("associatedUserLuidMapping")]
        [XmlArrayItem("userLuidPair")]
        public UserLuidPairType[]? AssociatedUserLuidMapping { get; set; }

        /// <summary>
        /// Class representing a user LUID pair in the apply keychain request.
        /// </summary>
        public class UserLuidPairType
        {
            /// <summary>
            /// Creates a new <see cref="UserLuidPairType"/> object.
            /// </summary>
            public UserLuidPairType()
            { }

            /// <summary>
            /// Creates a new <see cref="UserLuidPairType"/> object.
            /// </summary>
            /// <param name="keychainUserMapping">The keychain user mapping pair</param>
            public UserLuidPairType(IKeychainUserMapping keychainUserMapping)
            {
                SourceSiteUserLuid = keychainUserMapping.SourceUserId;
                DestinationSiteUserLuid = keychainUserMapping.DestinationUserId;
            }

            /// <summary>
            /// Gets or sets the source site user LUID.
            /// </summary>
            [XmlAttribute("sourceSiteUserLuid")]
            public Guid SourceSiteUserLuid { get; set; }

            /// <summary>
            /// Gets or sets the destination site user LUID.
            /// </summary>
            [XmlAttribute("destinationSiteUserLuid")]
            public Guid DestinationSiteUserLuid { get; set; }
        }
    }
}

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

namespace Tableau.Migration.Api.Rest.Models.Responses
{
    /// <summary>
    /// Class representing an embedded credentials retrieve keychain response.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class RetrieveKeychainResponse : TableauServerResponse
    {
        /// <summary>
        /// Gets or sets the associated user IDs for the response.
        /// </summary>
        [XmlArray("associatedUserLuidList")]
        [XmlArrayItem("associatedUserLuid")]
        public Guid[] AssociatedUserLuidList { get; set; } = [];

        /// <summary>
        /// Gets or sets the encrypted key chains for the response.
        /// </summary>
        [XmlArray("encryptedKeychainList")]
        [XmlArrayItem("encryptedKeychain")]
        public string[] EncryptedKeychainList { get; set; } = [];

        /// <summary>
        /// The default parameterless constructor.
        /// </summary>
        public RetrieveKeychainResponse()
        { }

        /// <summary>
        /// Constructor to build from encrypted keychain list and associated user LUID list.
        /// </summary>
        /// <param name="encryptedKeychainList">The list of encrypted keychains for this response.</param>
        /// <param name="associatedUserLuidList">The list of associated user LUIDs for this response.</param>
        public RetrieveKeychainResponse(IEnumerable<string> encryptedKeychainList, IEnumerable<Guid>? associatedUserLuidList)
        {
            EncryptedKeychainList = encryptedKeychainList.ToArray();
            AssociatedUserLuidList = associatedUserLuidList?.ToArray() ?? [];
        }
    }
}

﻿//
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

namespace Tableau.Migration.Api.Rest.Models.Requests
{
    /// <summary>
    /// Class representing an upload saved credentials request.
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/">Tableau API Reference</see> 
    /// for documentation.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class UploadUserSavedCredentialsRequest : TableauServerRequest
    {
        /// <summary>
        /// The default parameterless constructor.
        /// </summary>
        public UploadUserSavedCredentialsRequest() { }

        /// <summary>
        /// Creates a new <see cref="UploadUserSavedCredentialsRequest"/> object.
        /// </summary>
        public UploadUserSavedCredentialsRequest(IEnumerable<string> encryptedKeychains)
        {
            EncryptedKeychains = encryptedKeychains.ToArray();
        }

        /// <summary>
        /// Gets or sets the encrypted keychains for the request.
        /// </summary>
        [XmlArray("encryptedKeychainList")]
        [XmlArrayItem("encryptedKeychain", typeof(string))]
        public string[] EncryptedKeychains { get; set; } = Array.Empty<string>();
    }
}
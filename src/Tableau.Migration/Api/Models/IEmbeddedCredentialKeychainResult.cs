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
using System.Collections.Immutable;

namespace Tableau.Migration.Api.Models
{
    /// <summary>
    /// Interface for retrieve saved credentials result for content items.
    /// </summary>
    public interface IEmbeddedCredentialKeychainResult
    {
        /// <summary>
        /// The list of encrypted Keychains for the content item.
        /// </summary>
        IImmutableList<string> EncryptedKeychains { get; }

        /// <summary>
        /// The list of associated user IDs for the embedded credentials of the content item.
        /// </summary>
        IImmutableList<Guid> AssociatedUserIds { get; }
    }
}
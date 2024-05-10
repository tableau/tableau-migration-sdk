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
using Tableau.Migration.Api.Rest.Models;

namespace Tableau.Migration.Content.Permissions
{
    /// <summary>
    /// The interface for content permissions responses.
    /// </summary>
    public interface IPermissions
    {
        /// <summary>
        /// The collection of Grantee Capabilities for this content item.
        /// </summary>
        IGranteeCapability[] GranteeCapabilities { get; set; }

        /// <summary>
        /// The ID of the parent content item 
        /// The parent content can be one of the types in 
        /// <see cref="ParentContentTypeNames"/>.
        /// </summary>
        public Guid? ParentId { get; set; }
    }
}

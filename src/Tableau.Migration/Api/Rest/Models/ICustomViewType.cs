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

namespace Tableau.Migration.Api.Rest.Models
{
    /// <summary>
    /// Interface for a custom view REST response.
    /// </summary>
    public interface ICustomViewType :
        IRestIdentifiable,
        INamedContent,
        IWithWorkbookReferenceType,
        IWithOwnerType
    {
        /// <summary>
        /// The created timestamp for the response.
        /// </summary>        
        public string? CreatedAt { get; }

        /// <summary>
        /// The updated timestamp for the response.
        /// </summary>
        public string? UpdatedAt { get; }

        /// <summary>
        /// The lastAccessed timestamp for the response.
        /// </summary>
        public string? LastAccessedAt { get; }

        /// <summary>
        /// The shared flag for the response.
        /// </summary>
        public bool Shared { get; }

        /// <summary>
        /// The view ID for the response.
        /// </summary>
        public Guid? ViewId { get; }

        /// <summary>
        /// The view name for the response.
        /// </summary>
        public string? ViewName { get; }
    }
}

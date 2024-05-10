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
using Tableau.Migration.Api.Rest;

namespace Tableau.Migration.Content
{
    /// <summary>
    /// Interface for a content item's label.
    /// </summary>
    public interface ILabel : IRestIdentifiable
    {
        /// <summary>
        /// Gets the site ID.
        /// </summary>        
        Guid SiteId { get; }

        /// <summary>
        /// Gets the owner ID.
        /// </summary>
        Guid OwnerId { get; }

        /// <summary>
        /// Gets the user display name.
        /// </summary>
        string? UserDisplayName { get; }

        /// <summary>
        /// Gets the ID for the label's content item.
        /// </summary>
        Guid ContentId { get; }

        /// <summary>
        /// Gets the type for the label's content item.
        /// </summary>
        string? ContentType { get; }

        /// <summary>
        /// Gets the message.
        /// </summary>
        string? Message { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        string? Value { get; }

        /// <summary>
        /// Gets the category.
        /// </summary>
        string? Category { get; }

        /// <summary>
        /// Gets the active flag.
        /// </summary>
        bool Active { get; }

        /// <summary>
        /// Gets the active flag.
        /// </summary>
        bool Elevated { get; }

        /// <summary>
        /// Gets the create timestamp.
        /// </summary>        
        string? CreatedAt { get; }

        /// <summary>
        /// Gets the update timestamp.
        /// </summary>
        string? UpdatedAt { get; }
    }
}

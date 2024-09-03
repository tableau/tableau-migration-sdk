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

namespace Tableau.Migration.Content
{
    /// <summary>
    /// The interface for a custom view content item.
    /// </summary>
    public interface ICustomView :
        IContentReference,
        IWithOwner,
        IWithWorkbook
    {
        /// <summary>
        /// Gets the created timestamp.
        /// </summary>
        string CreatedAt { get; }

        /// <summary>
        /// Gets the updated timestamp.
        /// </summary>
        string? UpdatedAt { get; }

        /// <summary>
        /// Gets the last accessed timestamp.
        /// </summary>
        string? LastAccessedAt { get; }

        /// <summary>
        /// Gets or sets whether the custom view is shared with all users (true) or private (false).
        /// </summary>
        bool Shared { get; set; }

        /// <summary>
        /// Gets the ID of the view that this custom view is based on.
        /// </summary>
        Guid BaseViewId { get; }

        /// <summary>
        /// Gets the name of the view that this custom view is based on.
        /// </summary>
        string BaseViewName { get; }
    }
}
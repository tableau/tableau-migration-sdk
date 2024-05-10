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
    /// Interface for a workbook REST response.
    /// </summary>
    public interface IWorkbookType : IRestIdentifiable, INamedContent, IWithProjectType, IWithOwnerType, IWithTagTypes
    {
        /// <summary>
        /// The description for the response.
        /// </summary>
        public string? Description { get; }

        /// <summary>
        /// The content URL for the response.
        /// </summary>
        public string? ContentUrl { get; }

        /// <summary>
        /// whether tabs are shown for the response.
        /// </summary>
        public bool ShowTabs { get; }

        /// <summary>
        /// The size for the response.
        /// </summary>        
        public long Size { get; }

        /// <summary>
        /// The webpage URL for the response.
        /// </summary>       
        public string? WebpageUrl { get; }

        /// <summary>
        /// The created timestamp for the response.
        /// </summary>        
        public string? CreatedAt { get; }

        /// <summary>
        /// The updated timestamp for the response.
        /// </summary>
        public string? UpdatedAt { get; }

        /// <summary>
        /// Gets the encrypted extracts flag for the response.
        /// </summary>
        bool EncryptExtracts { get; }

        /// <summary>
        /// The Default View Id for the response.
        /// </summary>
        Guid DefaultViewId { get; }

        /// <summary>
        /// The location for the response.
        /// </summary>
        ILocationType? Location { get; }
    }
}

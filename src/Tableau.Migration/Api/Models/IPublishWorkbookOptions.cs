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
using System.Collections.Generic;

namespace Tableau.Migration.Api.Models
{
    /// <summary>
    /// Interface for API client workbook publish options. 
    /// </summary>
    public interface IPublishWorkbookOptions : IPublishFileOptions
    {
        /// <summary>
        /// Gets the name of the workbook.
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Gets the description of the workbook.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets whether to show tabs for the workbook.
        /// </summary>
        bool ShowTabs { get; }

        /// <summary>
        /// Gets whether to encrypt extracts for the workbook.
        /// </summary>
        bool EncryptExtracts { get; }

        /// <summary>
        /// Gets whether or not to skip the data source connection check.
        /// </summary>
        bool SkipConnectionCheck { get; }

        /// <summary>
        /// Gets whether or not to overwrite any existing workbook.
        /// </summary>
        bool Overwrite { get; }

        /// <summary>
        /// Gets the ID of the user to generate thumbnails as.
        /// </summary>
        Guid? ThumbnailsUserId { get; }

        /// <summary>
        /// Gets the ID of the project to publish to.
        /// </summary>
        Guid ProjectId { get; }

        /// <summary>
        /// Gets the names of the views that should be hidden.
        /// </summary>
        IEnumerable<string> HiddenViewNames { get; }
    }
}

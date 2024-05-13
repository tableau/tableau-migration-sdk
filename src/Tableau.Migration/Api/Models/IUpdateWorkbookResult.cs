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

namespace Tableau.Migration.Api.Models
{
    /// <summary>
    /// Interface to represent the response returned by the Update method in <see cref="IWorkbooksApiClient"/>.
    /// </summary>
    public interface IUpdateWorkbookResult
    {
        /// <summary>
        /// Gets the unique identifier of the workbook.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the name of the workbook.
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Gets the description of the workbook.
        /// </summary>
        string? Description { get; }

        /// <summary>
        /// Gets the content URL of the workbook.
        /// </summary>
        string? ContentUrl { get; }

        /// <summary>
        /// Gets the show tabs option for the workbook.
        /// </summary>
        bool ShowTabs { get; }

        /// <summary>
        /// Gets the creation date/time of the workbook.
        /// </summary>
        DateTime CreatedAtUtc { get; }

        /// <summary>
        /// Gets the update date/time of the workbook.
        /// </summary>
        DateTime UpdatedAtUtc { get; }

        /// <summary>
        /// Gets the encrypt extracts flag for the workbook.
        /// </summary>
        bool EncryptExtracts { get; }
    }
}

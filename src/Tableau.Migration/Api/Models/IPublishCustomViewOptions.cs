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
    /// Interface for API client Custom View publish options. 
    /// </summary>
    public interface IPublishCustomViewOptions : IPublishFileOptions
    {
        /// <summary>
        /// The name of the Custom View.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Flag indicating if the Custom View is shared.
        /// </summary>
        bool Shared { get; set; }

        /// <summary>
        /// The ID of the Custom View's workbook.
        /// </summary>
        Guid WorkbookId { get; set; }

        /// <summary>
        /// The Owner ID for the Custom View.
        /// </summary>
        Guid OwnerId { get; set; }
    }
}

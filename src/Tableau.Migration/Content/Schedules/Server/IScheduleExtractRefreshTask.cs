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

namespace Tableau.Migration.Content.Schedules.Server
{
    /// <summary>
    /// The interface for an extract.
    /// </summary>
    public interface IScheduleExtractRefreshTask
    {
        /// <summary>
        /// The extract ID.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// The extract priority.
        /// </summary>
        int Priority { get; set; }
        /// <summary>
        /// The extract type. This is either full or incremental.
        /// </summary>
        string Type { get; set; }

        /// <summary>
        /// The ID of the workbook this extract is linked to.
        /// </summary>
        Guid? WorkbookId { get; set; }

        /// <summary>
        /// The ID of the Data Source this extract is linked to.
        /// </summary>
        Guid? DatasourceId { get; set; }
    }
}
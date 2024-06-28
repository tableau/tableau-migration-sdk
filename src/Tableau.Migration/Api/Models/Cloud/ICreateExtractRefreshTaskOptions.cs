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
using Tableau.Migration.Content.Schedules;
using Tableau.Migration.Content.Schedules.Cloud;

namespace Tableau.Migration.Api.Models.Cloud
{
    /// <summary>
    /// Interface for an API client extract refresh task creation model.
    /// </summary>
    public interface ICreateExtractRefreshTaskOptions
    {
        /// <summary>
        /// Gets the type of extract refresh. FullRefresh or IncrementalRefresh.
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Gets the extract refresh task's content type.
        /// </summary>
        ExtractRefreshContentType ContentType { get; }

        /// <summary>
        /// Gets the extract refresh task's content ID.
        /// </summary>
        Guid ContentId { get; }

        /// <summary>
        /// Gets the extract refresh task's schedule.
        /// </summary>
        ICloudSchedule Schedule { get; }
    }
}

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
    /// Class for Tableau Cloud API client extract refresh task creation options. 
    /// </summary>
    public class CreateExtractRefreshTaskOptions : ICreateExtractRefreshTaskOptions
    {
        /// <inheritdoc/>
        public string Type { get; }

        /// <inheritdoc/>
        public ExtractRefreshContentType ContentType { get; }

        /// <inheritdoc/>
        public Guid ContentId { get; }

        /// <inheritdoc/>
        public ICloudSchedule Schedule { get; }

        /// <summary>
        /// Creates a new <see cref="CreateExtractRefreshTaskOptions"/> instance.
        /// </summary>
        /// <param name="type">The extract refresh type.</param>
        /// <param name="contentType">The extract refresh task's content type.</param>
        /// <param name="contentId">The extract refresh task's content ID.</param>
        /// <param name="schedule">The extract refresh task's schedule.</param>
        public CreateExtractRefreshTaskOptions(
            string type,
            ExtractRefreshContentType contentType,
            Guid contentId,
            ICloudSchedule schedule)
        {
            Type = type;
            ContentType = contentType;
            ContentId = contentId;
            Schedule = schedule;
        }
    }
}

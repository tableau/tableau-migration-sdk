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

namespace Tableau.Migration.Content.Schedules
{
    /// <summary>
    /// Interface for an extract refresh task content item.
    /// </summary>
    public interface IExtractRefreshTask<TSchedule> : IWithSchedule<TSchedule>
        where TSchedule : ISchedule
    {
        /// <summary>
        /// Gets the extract refresh type.
        /// </summary>
        string Type { get; set; }

        /// <summary>
        /// Gets the extract refresh task's content type.
        /// </summary>
        ExtractRefreshContentType ContentType { get; set; }

        /// <summary>
        /// Gets the extract refresh task's content.
        /// </summary>
        IContentReference Content { get; set; }
    }
}

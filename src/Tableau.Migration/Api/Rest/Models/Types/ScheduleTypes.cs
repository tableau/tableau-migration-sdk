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

namespace Tableau.Migration.Api.Rest.Models.Types
{
    /// <summary>
    /// <para>
    /// Class containing schedule type constants.
    /// </para>
    /// <para>
    /// See https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_jobs_tasks_and_schedules.htm#create_schedule for documentation.
    /// </para>
    /// </summary>
    public class ScheduleTypes : StringEnum<ScheduleTypes>
    {
        /// <summary>
        /// Gets the name of the extract schedule type.
        /// </summary>
        public const string Extract = "Extract";

        /// <summary>
        /// Gets the name of the flow schedule type.
        /// </summary>
        public const string Flow = "Flow ";

        /// <summary>
        /// Gets the name of the subscription schedule type.
        /// </summary>
        public const string Subscription = "Subscription";
    }
}

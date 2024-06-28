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
    /// Class containing schedule frequency constants.
    /// </para>
    /// <para>
    /// See https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_extract_and_encryption.htm#create_cloud_extract_refresh_task for documentation.
    /// </para>
    /// </summary>
    public class ScheduleFrequencies : StringEnum<ScheduleFrequencies>
    {
        /// <summary>
        /// Gets the name of the hourly schedule frequency.
        /// </summary>
        public const string Hourly = "Hourly";

        /// <summary>
        /// Gets the name of the daily schedule frequency.
        /// </summary>
        public const string Daily = "Daily";

        /// <summary>
        /// Gets the name of the weekly schedule frequency.
        /// </summary>
        public const string Weekly = "Weekly";

        /// <summary>
        /// Gets the name of the monthly schedule frequency.
        /// </summary>
        public const string Monthly = "Monthly";
    }
}

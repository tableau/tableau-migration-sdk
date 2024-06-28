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

namespace Tableau.Migration.Api.Rest.Models.Responses
{
    /// <summary>
    /// Interface for a schedule interval response.
    /// </summary>
    public interface IScheduleIntervalType
    {
        /// <summary>
        /// Gets the interval's hours.
        /// </summary>
        string? Hours { get; set; }

        /// <summary>
        /// Gets the interval's minutes.
        /// </summary>
        string? Minutes { get; set; }

        /// <summary>
        /// Gets the interval's month/day.
        /// </summary>
        string? MonthDay { get; set; }

        /// <summary>
        /// Gets the interval's weekday.
        /// </summary>
        string? WeekDay { get; set; }
    }
}
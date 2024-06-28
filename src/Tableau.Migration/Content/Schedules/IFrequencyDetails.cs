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

namespace Tableau.Migration.Content.Schedules
{
    /// <summary>
    /// Interface for a schedule's frequency details.
    /// </summary>
    public interface IFrequencyDetails
    {
        /// <summary>
        /// Gets the schedule's start time.
        /// </summary>
        TimeOnly? StartAt { get; set; }

        /// <summary>
        /// Gets the schedule's end time.
        /// </summary>
        TimeOnly? EndAt { get; set; }

        /// <summary>
        /// Gets the schedule's intervals.
        /// </summary>
        IList<IInterval> Intervals { get; set; }
    }
}
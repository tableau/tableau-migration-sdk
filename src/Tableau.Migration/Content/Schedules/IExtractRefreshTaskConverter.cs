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

namespace Tableau.Migration.Content.Schedules
{
    /// <summary>
    /// Interface for converting extract refresh tasks from one type to another.
    /// </summary>
    /// <typeparam name="TSourceTask">The type of the source extract refresh task.</typeparam>
    /// <typeparam name="TSourceSchedule">The type of the source extract refresh task.</typeparam>
    /// <typeparam name="TTargetTask">The type of the target extract refresh task.</typeparam>
    /// <typeparam name="TTargetSchedule">The type of the source extract refresh task.</typeparam>
    public interface IExtractRefreshTaskConverter<TSourceTask, TSourceSchedule, TTargetTask, TTargetSchedule>
        where TSourceTask : IExtractRefreshTask<TSourceSchedule>
        where TSourceSchedule : ISchedule
        where TTargetTask : IExtractRefreshTask<TTargetSchedule>
        where TTargetSchedule : ISchedule
    {
        /// <summary>
        /// Converts a source extract refresh task to a target extract refresh task.
        /// </summary>
        /// <param name="source">The source extract refresh task to convert.</param>
        /// <returns>The converted target extract refresh task.</returns>
        TTargetTask Convert(TSourceTask source);
    }
}

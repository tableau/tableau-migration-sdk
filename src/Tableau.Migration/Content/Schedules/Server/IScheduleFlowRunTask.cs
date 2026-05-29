//
//  Copyright (c) 2025, Salesforce, Inc.
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

namespace Tableau.Migration.Content.Schedules.Server
{
    /// <summary>
    /// Interface for a schedule flow run task.
    /// </summary>
    public interface IScheduleFlowRunTask
    {
        /// <summary>
        /// Gets the flow run task ID.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the flow run task priority.
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// Gets the consecutive failed count.
        /// </summary>
        int ConsecutiveFailedCount { get; }

        /// <summary>
        /// Gets the flow run task type.
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Gets the schedule ID.
        /// </summary>
        Guid? ScheduleId { get; }

        /// <summary>
        /// Gets the flow ID.
        /// </summary>
        Guid? FlowId { get; }

        /// <summary>
        /// Gets the flow name.
        /// </summary>
        string? FlowName { get; }

        /// <summary>
        /// Gets the flow parameter runs.
        /// </summary>
        IReadOnlyList<IFlowParameterRun> FlowParameterRuns { get; }
    }

    /// <summary>
    /// Interface for a flow parameter run.
    /// </summary>
    public interface IFlowParameterRun
    {
        /// <summary>
        /// Gets the parameter ID.
        /// </summary>
        Guid ParameterId { get; }

        /// <summary>
        /// Gets the parameter name.
        /// </summary>
        string? Name { get; }

        /// <summary>
        /// Gets the parameter description.
        /// </summary>
        string? Description { get; }

        /// <summary>
        /// Gets the override value.
        /// </summary>
        string? OverrideValue { get; }
    }
}


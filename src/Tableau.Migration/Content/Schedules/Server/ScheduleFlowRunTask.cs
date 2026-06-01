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
using System.Linq;
using Tableau.Migration;
using Tableau.Migration.Api.Rest.Models.Responses.Server;

namespace Tableau.Migration.Content.Schedules.Server
{
    /// <summary>
    /// Class for a schedule flow run task.
    /// </summary>
    public class ScheduleFlowRunTask : IScheduleFlowRunTask
    {
        /// <summary>
        /// Constructor to build from a <see cref="AddFlowTaskToScheduleResponse.TaskType.FlowRunType"/>.
        /// </summary>
        /// <param name="response">The response to build from.</param>
        public ScheduleFlowRunTask(AddFlowTaskToScheduleResponse.TaskType.FlowRunType response)
        {
            Guard.AgainstNull(response, nameof(response));
            Id = Guard.AgainstDefaultValue(response.Id, () => response.Id);
            Priority = response.Priority;
            ConsecutiveFailedCount = response.ConsecutiveFailedCount;
            Type = Guard.AgainstNull(response.Type, () => response.Type);
            ScheduleId = response.Schedule?.Id;
            FlowId = response.Flow?.Id;
            FlowName = response.Flow?.Name;
            FlowParameterRuns = response.FlowParametersRuns
                .Select(r => new FlowParameterRun(r))
                .Cast<IFlowParameterRun>()
                .ToList()
                .AsReadOnly();
        }

        /// <summary>
        /// Constructor to build from a <see cref="FlowRunTasksResponse.TaskType.FlowRunType"/>.
        /// </summary>
        /// <param name="response">The response to build from.</param>
        public ScheduleFlowRunTask(FlowRunTasksResponse.TaskType.FlowRunType response)
        {
            Guard.AgainstNull(response, nameof(response));
            Id = Guard.AgainstDefaultValue(response.Id, () => response.Id);
            Priority = response.Priority;
            ConsecutiveFailedCount = response.ConsecutiveFailedCount;
            Type = Guard.AgainstNull(response.Type, () => response.Type);
            ScheduleId = response.Schedule?.Id;
            FlowId = response.Flow?.Id;
            FlowName = response.Flow?.Name;
            FlowParameterRuns = (response.Parameters ?? Array.Empty<FlowRunTasksResponse.TaskType.FlowRunType.ParameterType>())
                .Select(p => new FlowParameterRun(p))
                .Cast<IFlowParameterRun>()
                .ToList()
                .AsReadOnly();
        }

        /// <inheritdoc/>
        public Guid Id { get; }

        /// <inheritdoc/>
        public int Priority { get; }

        /// <inheritdoc/>
        public int ConsecutiveFailedCount { get; }

        /// <inheritdoc/>
        public string Type { get; }

        /// <inheritdoc/>
        public Guid? ScheduleId { get; }

        /// <inheritdoc/>
        public Guid? FlowId { get; }

        /// <inheritdoc/>
        public string? FlowName { get; }

        /// <inheritdoc/>
        public IReadOnlyList<IFlowParameterRun> FlowParameterRuns { get; }
    }

    /// <summary>
    /// Class for a flow parameter run.
    /// </summary>
    public class FlowParameterRun : IFlowParameterRun
    {
        /// <summary>
        /// Constructor to build from a <see cref="AddFlowTaskToScheduleResponse.TaskType.FlowRunType.ParameterRunsType"/>.
        /// </summary>
        /// <param name="response">The response to build from.</param>
        public FlowParameterRun(AddFlowTaskToScheduleResponse.TaskType.FlowRunType.ParameterRunsType response)
        {
            Guard.AgainstNull(response, nameof(response));
            ParameterId = Guard.AgainstDefaultValue(response.ParameterId, () => response.ParameterId);
            Name = response.Name;
            Description = response.Description;
            OverrideValue = response.OverrideValue;
        }

        /// <summary>
        /// Constructor to build from a <see cref="FlowRunTasksResponse.TaskType.FlowRunType.ParameterType"/>.
        /// </summary>
        /// <param name="response">The response to build from.</param>
        public FlowParameterRun(FlowRunTasksResponse.TaskType.FlowRunType.ParameterType response)
        {
            Guard.AgainstNull(response, nameof(response));
            ParameterId = Guard.AgainstDefaultValue(response.Id, () => response.Id);
            Name = response.Name;
            Description = response.Description;
            OverrideValue = response.Value;
        }

        /// <inheritdoc/>
        public Guid ParameterId { get; }

        /// <inheritdoc/>
        public string? Name { get; }

        /// <inheritdoc/>
        public string? Description { get; }

        /// <inheritdoc/>
        public string? OverrideValue { get; }
    }
}


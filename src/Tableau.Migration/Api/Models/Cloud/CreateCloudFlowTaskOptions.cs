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
using Tableau.Migration.Content.Schedules.Cloud;
using Tableau.Migration.Api.Rest.Models.Requests;

namespace Tableau.Migration.Api.Models.Cloud
{
    /// <summary>
    /// Class for Tableau Cloud API client flow task creation options. 
    /// </summary>
    public class CreateCloudFlowTaskOptions : ICreateCloudFlowTaskOptions
    {
        /// <inheritdoc/>
        public Guid FlowId { get; }

        /// <inheritdoc/>
        public ICloudSchedule Schedule { get; }

        /// <inheritdoc/>
        public IEnumerable<FlowParameterSpec>? FlowParameterSpecs { get; }

        /// <inheritdoc/>
        public IEnumerable<Guid>? FlowOutputStepIds { get; }

        /// <summary>
        /// Creates a new <see cref="CreateCloudFlowTaskOptions"/> instance.
        /// </summary>
        /// <param name="flowId">The flow ID.</param>
        /// <param name="schedule">The flow task's schedule.</param>
        /// <param name="flowParameterSpecs">Optional flow parameter specifications.</param>
        /// <param name="flowOutputStepIds">Optional flow output step IDs.</param>
        public CreateCloudFlowTaskOptions(
            Guid flowId,
            ICloudSchedule schedule,
            IEnumerable<FlowParameterSpec>? flowParameterSpecs = null,
            IEnumerable<Guid>? flowOutputStepIds = null)
        {
            FlowId = flowId;
            Schedule = schedule;
            FlowParameterSpecs = flowParameterSpecs;
            FlowOutputStepIds = flowOutputStepIds;
        }
    }
}


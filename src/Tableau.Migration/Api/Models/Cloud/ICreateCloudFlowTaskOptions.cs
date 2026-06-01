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
    /// Interface for an API client cloud flow task creation model.
    /// </summary>
    public interface ICreateCloudFlowTaskOptions
    {
        /// <summary>
        /// Gets the flow ID.
        /// </summary>
        Guid FlowId { get; }

        /// <summary>
        /// Gets the flow task's schedule.
        /// </summary>
        ICloudSchedule Schedule { get; }

        /// <summary>
        /// Gets the optional flow parameter specifications.
        /// </summary>
        IEnumerable<FlowParameterSpec>? FlowParameterSpecs { get; }

        /// <summary>
        /// Gets the optional flow output step IDs.
        /// </summary>
        IEnumerable<Guid>? FlowOutputStepIds { get; }
    }
}


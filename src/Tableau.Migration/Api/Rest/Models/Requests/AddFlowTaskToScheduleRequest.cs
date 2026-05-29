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
using System.Xml.Serialization;

namespace Tableau.Migration.Api.Rest.Models.Requests
{
    /// <summary>
    /// Class representing an add flow task to schedule request.
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_flow.htm#add_flow_task_to_schedule">Tableau API Reference</see> for documentation.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class AddFlowTaskToScheduleRequest : TableauServerRequest
    {
        /// <summary>
        /// Gets or sets the task for the request.
        /// </summary>
        [XmlElement("task")]
        public TaskType? Task { get; set; }

        /// <summary>
        /// Creates a new <see cref="AddFlowTaskToScheduleRequest"/> instance.
        /// </summary>
        public AddFlowTaskToScheduleRequest()
        { }

        /// <summary>
        /// Creates a new <see cref="AddFlowTaskToScheduleRequest"/> instance.
        /// </summary>
        /// <param name="flowId">The ID of the flow to add to the schedule.</param>
        /// <param name="flowParameterSpecs">Optional flow parameter specifications.</param>
        public AddFlowTaskToScheduleRequest(
            Guid flowId,
            IEnumerable<FlowParameterSpec>? flowParameterSpecs = null)
        {
            Task = new TaskType(flowId, flowParameterSpecs);
        }

        /// <summary>
        /// Class representing a request task item.
        /// </summary>
        public class TaskType
        {
            /// <summary>
            /// Gets or sets the flow run for the request.
            /// </summary>
            [XmlElement("flowRun")]
            public FlowRunType? FlowRun { get; set; }

            /// <summary>
            /// Creates a new <see cref="TaskType"/> instance.
            /// </summary>
            public TaskType()
            { }

            /// <summary>
            /// Creates a new <see cref="TaskType"/> instance.
            /// </summary>
            /// <param name="flowId">The ID of the flow to add to the schedule.</param>
            /// <param name="flowParameterSpecs">Optional flow parameter specifications.</param>
            public TaskType(
                Guid flowId,
                IEnumerable<FlowParameterSpec>? flowParameterSpecs = null)
            {
                FlowRun = new FlowRunType(flowId, flowParameterSpecs);
            }

            /// <summary>
            /// Class representing a request flow run item.
            /// </summary>
            public class FlowRunType
            {
                /// <summary>
                /// Gets or sets the flow for the request.
                /// </summary>
                [XmlElement("flow")]
                public FlowType? Flow { get; set; }

                /// <summary>
                /// Gets or sets the flow run spec for the request.
                /// </summary>
                [XmlElement("flowRunSpec")]
                public FlowRunSpecType? FlowRunSpec { get; set; }

                /// <summary>
                /// Creates a new <see cref="FlowRunType"/> instance.
                /// </summary>
                public FlowRunType()
                { }

                /// <summary>
                /// Creates a new <see cref="FlowRunType"/> instance.
                /// </summary>
                /// <param name="flowId">The ID of the flow to add to the schedule.</param>
                /// <param name="flowParameterSpecs">Optional flow parameter specifications.</param>
                public FlowRunType(
                    Guid flowId,
                    IEnumerable<FlowParameterSpec>? flowParameterSpecs = null)
                {
                    Flow = new FlowType(flowId);
                    if (flowParameterSpecs != null && flowParameterSpecs.Any())
                    {
                        FlowRunSpec = new FlowRunSpecType(flowParameterSpecs);
                    }
                }

                /// <summary>
                /// Class representing a request flow item.
                /// </summary>
                public class FlowType
                {
                    /// <summary>
                    /// Gets or sets the ID for the request.
                    /// </summary>
                    [XmlAttribute("id")]
                    public Guid Id { get; set; }

                    /// <summary>
                    /// Creates a new <see cref="FlowType"/> instance.
                    /// </summary>
                    public FlowType()
                    { }

                    /// <summary>
                    /// Creates a new <see cref="FlowType"/> instance.
                    /// </summary>
                    /// <param name="flowId">The ID of the flow.</param>
                    public FlowType(Guid flowId)
                    {
                        Id = flowId;
                    }
                }

                /// <summary>
                /// Class representing a request flow run spec item.
                /// </summary>
                public class FlowRunSpecType
                {
                    /// <summary>
                    /// Gets or sets the flow parameter specs for the request.
                    /// </summary>
                    [XmlArray("flowParameterSpecs")]
                    [XmlArrayItem("flowParameterSpec")]
                    public FlowParameterSpecType[] FlowParameterSpecs { get; set; } = Array.Empty<FlowParameterSpecType>();

                    /// <summary>
                    /// Creates a new <see cref="FlowRunSpecType"/> instance.
                    /// </summary>
                    public FlowRunSpecType()
                    { }

                    /// <summary>
                    /// Creates a new <see cref="FlowRunSpecType"/> instance.
                    /// </summary>
                    /// <param name="flowParameterSpecs">The flow parameter specifications.</param>
                    public FlowRunSpecType(IEnumerable<FlowParameterSpec> flowParameterSpecs)
                    {
                        FlowParameterSpecs = flowParameterSpecs.Select(spec => new FlowParameterSpecType(spec)).ToArray();
                    }

                    /// <summary>
                    /// Class representing a request flow parameter spec item.
                    /// </summary>
                    public class FlowParameterSpecType
                    {
                        /// <summary>
                        /// Gets or sets the parameter ID for the request.
                        /// </summary>
                        [XmlAttribute("parameterId")]
                        public Guid ParameterId { get; set; }

                        /// <summary>
                        /// Gets or sets the override value for the request.
                        /// </summary>
                        [XmlAttribute("overrideValue")]
                        public string? OverrideValue { get; set; }

                        /// <summary>
                        /// Creates a new <see cref="FlowParameterSpecType"/> instance.
                        /// </summary>
                        public FlowParameterSpecType()
                        { }

                        /// <summary>
                        /// Creates a new <see cref="FlowParameterSpecType"/> instance.
                        /// </summary>
                        /// <param name="spec">The flow parameter spec to copy from.</param>
                        public FlowParameterSpecType(FlowParameterSpec spec)
                        {
                            ParameterId = spec.ParameterId;
                            OverrideValue = spec.OverrideValue;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Class representing a flow parameter specification.
    /// </summary>
    public class FlowParameterSpec
    {
        /// <summary>
        /// Gets or sets the parameter ID.
        /// </summary>
        public Guid ParameterId { get; set; }

        /// <summary>
        /// Gets or sets the override value.
        /// </summary>
        public string? OverrideValue { get; set; }

        /// <summary>
        /// Creates a new <see cref="FlowParameterSpec"/> instance.
        /// </summary>
        public FlowParameterSpec()
        { }

        /// <summary>
        /// Creates a new <see cref="FlowParameterSpec"/> instance.
        /// </summary>
        /// <param name="parameterId">The parameter ID.</param>
        /// <param name="overrideValue">The override value.</param>
        public FlowParameterSpec(Guid parameterId, string? overrideValue = null)
        {
            ParameterId = parameterId;
            OverrideValue = overrideValue;
        }
    }
}


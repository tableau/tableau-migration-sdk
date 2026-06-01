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
using System.Xml.Serialization;

namespace Tableau.Migration.Api.Rest.Models.Responses.Server
{
    /// <summary>
    /// Class representing an add flow task to schedule response.
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_flow.htm#add_flow_task_to_schedule">Tableau API Reference</see> for documentation.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class AddFlowTaskToScheduleResponse : TableauServerResponse<AddFlowTaskToScheduleResponse.TaskType>
    {
        /// <summary>
        /// Gets or sets the task for the response.
        /// </summary>
        [XmlElement("task")]
        public override TaskType? Item { get; set; }

        /// <summary>
        /// Class representing a response task item.
        /// </summary>
        public class TaskType
        {
            /// <summary>
            /// Gets or sets the flow run for the response.
            /// </summary>
            [XmlElement("flowRun")]
            public FlowRunType? FlowRun { get; set; }

            /// <summary>
            /// Class representing a response flow run item.
            /// </summary>
            public class FlowRunType
            {
                /// <summary>
                /// Gets or sets the ID for the response.
                /// </summary>
                [XmlAttribute("id")]
                public Guid Id { get; set; }

                /// <summary>
                /// Gets or sets the priority for the response.
                /// </summary>
                [XmlAttribute("priority")]
                public int Priority { get; set; }

                /// <summary>
                /// Gets or sets the consecutive failed count for the response.
                /// </summary>
                [XmlAttribute("consecutiveFailedCount")]
                public int ConsecutiveFailedCount { get; set; }

                /// <summary>
                /// Gets or sets the type for the response.
                /// </summary>
                [XmlAttribute("type")]
                public string? Type { get; set; }

                /// <summary>
                /// Gets or sets the schedule for the response.
                /// </summary>
                [XmlElement("schedule")]
                public ScheduleType? Schedule { get; set; }

                /// <summary>
                /// Gets or sets the flow for the response.
                /// </summary>
                [XmlElement("flow")]
                public FlowType? Flow { get; set; }

                /// <summary>
                /// Gets or sets the flow parameters runs for the response.
                /// </summary>
                [XmlArray("flowParametersRuns")]
                [XmlArrayItem("parameterRuns")]
                public ParameterRunsType[] FlowParametersRuns { get; set; } = Array.Empty<ParameterRunsType>();

                /// <summary>
                /// Class representing a response schedule item.
                /// </summary>
                public class ScheduleType : IScheduleReferenceType
                {
                    /// <summary>
                    /// Gets or sets the ID for the response.
                    /// </summary>
                    [XmlAttribute("id")]
                    public Guid Id { get; set; }

                    /// <summary>
                    /// Gets or sets the name for the response.
                    /// </summary>
                    [XmlAttribute("name")]
                    public string? Name { get; set; }

                    /// <summary>
                    /// Gets or sets the state for the response.
                    /// </summary>
                    [XmlAttribute("state")]
                    public string? State { get; set; }

                    /// <summary>
                    /// Gets or sets the priority for the response.
                    /// </summary>
                    [XmlAttribute("priority")]
                    public int Priority { get; set; }

                    /// <summary>
                    /// Gets or sets the created time for the response.
                    /// </summary>
                    [XmlAttribute("createdAt")]
                    public string? CreatedAt { get; set; }

                    /// <summary>
                    /// Gets or sets the updated time for the response.
                    /// </summary>
                    [XmlAttribute("updatedAt")]
                    public string? UpdatedAt { get; set; }

                    /// <summary>
                    /// Gets or sets the type for the response.
                    /// </summary>
                    [XmlAttribute("type")]
                    public string? Type { get; set; }

                    /// <summary>
                    /// Gets or sets the frequency for the response.
                    /// </summary>
                    [XmlAttribute("frequency")]
                    public string? Frequency { get; set; }

                    /// <summary>
                    /// Gets or sets the next run time for the response.
                    /// </summary>
                    [XmlAttribute("nextRunAt")]
                    public string? NextRunAt { get; set; }
                }

                /// <summary>
                /// Class representing a response flow item.
                /// </summary>
                public class FlowType
                {
                    /// <summary>
                    /// Gets or sets the ID for the response.
                    /// </summary>
                    [XmlAttribute("id")]
                    public Guid Id { get; set; }

                    /// <summary>
                    /// Gets or sets the name for the response.
                    /// </summary>
                    [XmlAttribute("name")]
                    public string? Name { get; set; }
                }

                /// <summary>
                /// Class representing a response parameter runs item.
                /// </summary>
                public class ParameterRunsType
                {
                    /// <summary>
                    /// Gets or sets the parameter ID for the response.
                    /// </summary>
                    [XmlAttribute("parameterId")]
                    public Guid ParameterId { get; set; }

                    /// <summary>
                    /// Gets or sets the name for the response.
                    /// </summary>
                    [XmlAttribute("name")]
                    public string? Name { get; set; }

                    /// <summary>
                    /// Gets or sets the description for the response.
                    /// </summary>
                    [XmlAttribute("description")]
                    public string? Description { get; set; }

                    /// <summary>
                    /// Gets or sets the override value for the response.
                    /// </summary>
                    [XmlAttribute("overrideValue")]
                    public string? OverrideValue { get; set; }
                }
            }
        }
    }
}


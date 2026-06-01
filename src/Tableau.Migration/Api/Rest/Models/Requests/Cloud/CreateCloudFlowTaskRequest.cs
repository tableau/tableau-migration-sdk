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
using Tableau.Migration.Content.Schedules;
using Tableau.Migration.Content.Schedules.Cloud;
using CloudModels = Tableau.Migration.Api.Models.Cloud;

namespace Tableau.Migration.Api.Rest.Models.Requests.Cloud
{
    /// <summary>   
    /// Class representing a create cloud flow task request.
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_flow.htm#create_cloud_flow_task">REST API Reference</see> for documentation.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class CreateCloudFlowTaskRequest : TableauServerRequest
    {
        /// <summary>
        /// Gets or sets the task for the request.
        /// </summary>
        [XmlElement("task")]
        public TaskType? Task { get; set; }

        /// <summary>
        /// Gets or sets the schedule for the request.
        /// </summary>
        [XmlElement("schedule")]
        public ScheduleType? Schedule { get; set; }

        /// <summary>
        /// Creates a new <see cref="CreateCloudFlowTaskRequest"/> instance.
        /// </summary>
        public CreateCloudFlowTaskRequest()
        { }

        /// <summary>
        /// Creates a new <see cref="CreateCloudFlowTaskRequest"/> instance.
        /// </summary>
        /// <param name="flowId">The flow ID.</param>
        /// <param name="schedule">The flow task's schedule.</param>
        /// <param name="flowParameterSpecs">Optional flow parameter specifications.</param>
        /// <param name="flowOutputStepIds">Optional flow output step IDs.</param>
        public CreateCloudFlowTaskRequest(
            Guid flowId,
            ICloudSchedule schedule,
            IEnumerable<FlowParameterSpec>? flowParameterSpecs = null,
            IEnumerable<Guid>? flowOutputStepIds = null)
        {
            Task = new TaskType(flowId, flowParameterSpecs, flowOutputStepIds);
            Schedule = new ScheduleType(schedule);
        }

        /// <summary>
        /// Creates a new <see cref="CreateCloudFlowTaskRequest"/> instance.
        /// </summary>
        /// <param name="options">The flow task creation options.</param>
        public CreateCloudFlowTaskRequest(
            CloudModels.ICreateCloudFlowTaskOptions options)
            : this(
                  options.FlowId,
                  options.Schedule,
                  options.FlowParameterSpecs,
                  options.FlowOutputStepIds)
        { }

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
            /// <param name="flowId">The flow ID.</param>
            /// <param name="flowParameterSpecs">Optional flow parameter specifications.</param>
            /// <param name="flowOutputStepIds">Optional flow output step IDs.</param>
            public TaskType(
                Guid flowId,
                IEnumerable<FlowParameterSpec>? flowParameterSpecs = null,
                IEnumerable<Guid>? flowOutputStepIds = null)
            {
                FlowRun = new FlowRunType(flowId, flowParameterSpecs, flowOutputStepIds);
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
                /// <param name="flowId">The flow ID.</param>
                /// <param name="flowParameterSpecs">Optional flow parameter specifications.</param>
                /// <param name="flowOutputStepIds">Optional flow output step IDs.</param>
                public FlowRunType(
                    Guid flowId,
                    IEnumerable<FlowParameterSpec>? flowParameterSpecs = null,
                    IEnumerable<Guid>? flowOutputStepIds = null)
                {
                    Flow = new FlowType(flowId);
                    if ((flowParameterSpecs != null && flowParameterSpecs.Any()) || 
                        (flowOutputStepIds != null && flowOutputStepIds.Any()))
                    {
                        FlowRunSpec = new FlowRunSpecType(flowId, flowParameterSpecs, flowOutputStepIds);
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
                    /// <param name="flowId">The flow ID.</param>
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
                    /// Gets or sets the flow ID for the request.
                    /// </summary>
                    [XmlAttribute("flowId")]
                    public Guid FlowId { get; set; }

                    /// <summary>
                    /// Gets or sets the flow output steps for the request.
                    /// </summary>
                    [XmlArray("flowOutputSteps")]
                    [XmlArrayItem("flowOutputStep")]
                    public FlowOutputStepType[] FlowOutputSteps { get; set; } = Array.Empty<FlowOutputStepType>();

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
                /// <param name="flowId">The flow ID.</param>
                /// <param name="flowParameterSpecs">Optional flow parameter specifications.</param>
                /// <param name="flowOutputStepIds">Optional flow output step IDs.</param>
                public FlowRunSpecType(
                    Guid flowId,
                    IEnumerable<FlowParameterSpec>? flowParameterSpecs = null,
                    IEnumerable<Guid>? flowOutputStepIds = null)
                {
                    FlowId = flowId;
                    if (flowOutputStepIds != null && flowOutputStepIds.Any())
                    {
                        FlowOutputSteps = flowOutputStepIds.Select(id => new FlowOutputStepType(id)).ToArray();
                    }
                    if (flowParameterSpecs != null && flowParameterSpecs.Any())
                    {
                        FlowParameterSpecs = flowParameterSpecs.Select(spec => new FlowParameterSpecType(spec)).ToArray();
                    }
                }

                    /// <summary>
                    /// Class representing a request flow output step item.
                    /// </summary>
                    public class FlowOutputStepType
                    {
                        /// <summary>
                        /// Gets or sets the ID for the request.
                        /// </summary>
                        [XmlAttribute("id")]
                        public Guid Id { get; set; }

                        /// <summary>
                        /// Creates a new <see cref="FlowOutputStepType"/> instance.
                        /// </summary>
                        public FlowOutputStepType()
                        { }

                        /// <summary>
                        /// Creates a new <see cref="FlowOutputStepType"/> instance.
                        /// </summary>
                        /// <param name="id">The flow output step ID.</param>
                        public FlowOutputStepType(Guid id)
                        {
                            Id = id;
                        }
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

        /// <summary>
        /// Class representing a request schedule item.
        /// </summary>
        public class ScheduleType
        {
            /// <summary>
            /// Gets or sets the frequency for the request.
            /// </summary>
            [XmlAttribute("frequency")]
            public string? Frequency { get; set; }

            /// <summary>
            /// Gets or sets the frequency details for the request.
            /// </summary>
            [XmlElement("frequencyDetails")]
            public FrequencyDetailsType? FrequencyDetails { get; set; }

            /// <summary>
            /// Creates a new <see cref="ScheduleType"/> instance.
            /// </summary>
            public ScheduleType()
            { }

            /// <summary>
            /// Creates a new <see cref="ScheduleType"/> instance.
            /// </summary>
            /// <param name="schedule">The schedule to copy from.</param>
            public ScheduleType(ICloudSchedule schedule)
            {
                Frequency = schedule.Frequency;
                FrequencyDetails = new(schedule.FrequencyDetails);
            }

            /// <summary>
            /// Class representing a request frequency details item.
            /// </summary>
            public class FrequencyDetailsType
            {
                /// <summary>
                /// Gets or sets the start time for the request.
                /// </summary>
                [XmlAttribute("start")]
                public string? Start { get; set; }

                /// <summary>
                /// Gets or sets the end time for the request.
                /// </summary>
                [XmlAttribute("end")]
                public string? End { get; set; }

                /// <summary>
                /// Gets or sets the intervals for the request.
                /// </summary>
                [XmlArray("intervals")]
                [XmlArrayItem("interval")]
                public IntervalType[] Intervals { get; set; } = Array.Empty<IntervalType>();

                /// <summary>
                /// Creates a new <see cref="FrequencyDetailsType"/> instance.
                /// </summary>
                public FrequencyDetailsType()
                { }

                /// <summary>
                /// Creates a new <see cref="FrequencyDetailsType"/> instance.
                /// </summary>
                /// <param name="frequencyDetails">The frequency details to copy from.</param>
                public FrequencyDetailsType(IFrequencyDetails frequencyDetails)
                {
                    Start = frequencyDetails.StartAt?.ToString(Constants.FrequencyTimeFormat);
                    End = frequencyDetails.EndAt?.ToString(Constants.FrequencyTimeFormat);
                    Intervals = frequencyDetails.Intervals.Select(i => new IntervalType(i)).ToArray();
                }

                /// <summary>
                /// Class representing a request interval item.
                /// </summary>
                public class IntervalType
                {
                    /// <summary>
                    /// Gets or sets the hours for the request.
                    /// </summary>
                    [XmlAttribute("hours")]
                    public string? Hours { get; set; }

                    /// <summary>
                    /// Gets or sets the minutes for the request.
                    /// </summary>
                    [XmlAttribute("minutes")]
                    public string? Minutes { get; set; }

                    /// <summary>
                    /// Gets or sets the weekday for the request.
                    /// </summary>
                    [XmlAttribute("weekDay")]
                    public string? WeekDay { get; set; }

                    /// <summary>
                    /// Gets or sets the month/day for the request.
                    /// </summary>
                    [XmlAttribute("monthDay")]
                    public string? MonthDay { get; set; }

                    /// <summary>
                    /// Creates a new <see cref="IntervalType"/> instance.
                    /// </summary>
                    public IntervalType()
                    { }

                    /// <summary>
                    /// Creates a new <see cref="IntervalType"/> instance.
                    /// </summary>
                    /// <param name="interval">The interval to copy from.</param>
                    public IntervalType(IInterval interval)
                    {
                        Hours = interval.Hours?.ToString();
                        Minutes = interval.Minutes?.ToString();
                        WeekDay = interval.WeekDay;
                        MonthDay = interval.MonthDay;
                    }
                }
            }
        }
    }
}


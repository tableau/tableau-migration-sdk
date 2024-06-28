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
using System.Xml.Serialization;

namespace Tableau.Migration.Api.Rest.Models.Responses.Server
{
    /// <summary>
    /// Class representing a schedule response.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class ScheduleResponse : TableauServerResponse<ScheduleResponse.ScheduleType>
    {
        /// <summary>
        /// Gets or sets the schedule for the response.
        /// </summary>
        [XmlElement("schedule")]
        public override ScheduleType? Item { get; set; }

        /// <summary>
        /// Class representing a schedule response.
        /// </summary>
        public class ScheduleType : IServerScheduleType
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
            /// Gets or sets the created timestamp for the response.
            /// </summary>
            [XmlAttribute("createdAt")]
            public string? CreatedAt { get; set; }

            /// <summary>
            /// Gets or sets the updated timestamp for the response.
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
            /// Gets or sets the next run at for the response.
            /// </summary>
            [XmlAttribute("nextRunAt")]
            public string? NextRunAt { get; set; }

            /// <summary>
            /// Gets or sets the execution order for the response.
            /// </summary>
            [XmlAttribute("executionOrder")]
            public string? ExecutionOrder { get; set; }

            /// <summary>
            /// Gets or sets the frequency details for the response.
            /// </summary>
            [XmlElement("frequencyDetails")]
            public FrequencyDetailsType FrequencyDetails { get; set; } = new();

            IScheduleFrequencyDetailsType? IScheduleType.FrequencyDetails => FrequencyDetails;

            /// <summary>
            /// Class representing a REST API frequency details response.
            /// </summary>
            public class FrequencyDetailsType : IScheduleFrequencyDetailsType
            {
                /// <summary>
                /// Gets or sets the start time for the response.
                /// </summary>
                [XmlAttribute("start")]
                public string? Start { get; set; }

                /// <summary>
                /// Gets or sets the end time for the response.
                /// </summary>
                [XmlAttribute("end")]
                public string? End { get; set; }

                /// <summary>
                /// Gets or sets the intervals for the response.
                /// </summary>
                [XmlArray("intervals")]
                [XmlArrayItem("interval")]
                public IntervalType[] Intervals { get; set; } = Array.Empty<IntervalType>();

                IScheduleIntervalType[] IScheduleFrequencyDetailsType.Intervals => Intervals;

                /// <summary>
                /// Class representing a REST API interval response.
                /// </summary>
                public class IntervalType : IScheduleIntervalType
                {
                    /// <summary>
                    /// Gets or sets the hours for the response.
                    /// </summary>
                    [XmlAttribute("hours")]
                    public string? Hours { get; set; }

                    /// <summary>
                    /// Gets or sets the minutes for the response.
                    /// </summary>
                    [XmlAttribute("minutes")]
                    public string? Minutes { get; set; }

                    /// <summary>
                    /// Gets or sets the week day for the response.
                    /// </summary>
                    [XmlAttribute("weekDay")]
                    public string? WeekDay { get; set; }

                    /// <summary>
                    /// Gets or sets the week day for the response.
                    /// </summary>
                    [XmlAttribute("monthDay")]
                    public string? MonthDay { get; set; }
                }
            }
        }
    }
}

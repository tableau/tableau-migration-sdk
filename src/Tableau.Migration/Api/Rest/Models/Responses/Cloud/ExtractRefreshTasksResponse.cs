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

namespace Tableau.Migration.Api.Rest.Models.Responses.Cloud
{
    /// <summary>
    /// Class representing a cloud extract refresh tasks response.
    /// https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref.htm#tableau-cloud-request3
    /// </summary>
    [XmlType(XmlTypeName)]
    public class ExtractRefreshTasksResponse : TableauServerListResponse<ExtractRefreshTasksResponse.TaskType>
    {
        /// <summary>
        /// Gets or sets the extract refresh tasks for the response.
        /// </summary>
        [XmlArray("tasks")]
        [XmlArrayItem("task")]
        public override TaskType[] Items { get; set; } = Array.Empty<TaskType>();

        /// <summary>
        /// Class representing a response task item.
        /// </summary>
        public class TaskType
        {
            /// <summary>
            /// Gets or sets the extract refresh for the response.
            /// </summary>
            [XmlElement("extractRefresh")]
            public ExtractRefreshType? ExtractRefresh { get; set; }

            /// <summary>
            /// Class representing a response extract refresh item.
            /// </summary>
            public class ExtractRefreshType : ICloudExtractRefreshType<ExtractRefreshType.WorkbookType, ExtractRefreshType.DataSourceType>
            {
                /// <inheritdoc/>
                [XmlAttribute("id")]
                public Guid Id { get; set; }

                /// <inheritdoc/>
                [XmlAttribute("priority")]
                public int Priority { get; set; }

                /// <inheritdoc/>
                [XmlAttribute("type")]
                public string? Type { get; set; }

                /// <inheritdoc/>
                [XmlAttribute("consecutiveFailedCount")]
                public int ConsecutiveFailedCount { get; set; }

                /// <summary>
                /// Gets or sets the schedule for the response.
                /// </summary>
                [XmlElement("schedule")]
                public ScheduleType Schedule { get; set; } = new();

                /// <summary>
                /// Gets or sets the data source for the response.
                /// </summary>
                [XmlElement("datasource")]
                public DataSourceType? DataSource { get; set; }

                /// <summary>
                /// Gets or sets the workbook for the response.
                /// </summary>
                [XmlElement("workbook")]
                public WorkbookType? Workbook { get; set; }

                IRestIdentifiable? IWithWorkbookReferenceType.Workbook => Workbook;
                IRestIdentifiable? IWithDataSourceReferenceType.DataSource => DataSource;

                /// <summary>
                /// Class representing a response schedule item.
                /// </summary>
                public class ScheduleType : ICloudScheduleType
                {
                    /// <summary>
                    /// Gets or sets the frequency for the response.
                    /// </summary>
                    [XmlAttribute("frequency")]
                    public string? Frequency { get; set; }

                    /// <summary>
                    /// Gets or sets the frequency details for the response.
                    /// </summary>
                    [XmlElement("frequencyDetails")]
                    public FrequencyDetailsType? FrequencyDetails { get; set; }

                    /// <summary>
                    /// Gets or sets the next run at for the response.
                    /// </summary>
                    [XmlAttribute("nextRunAt")]
                    public string? NextRunAt { get; set; }

                    IScheduleFrequencyDetailsType? IScheduleType.FrequencyDetails => FrequencyDetails;

                    /// <summary>
                    /// Class representing a response frequency details item.
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

                        /// <inheritdoc/>
                        IScheduleIntervalType[] IScheduleFrequencyDetailsType.Intervals => Intervals;

                        /// <summary>
                        /// Class representing a response interval item.
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
                            /// Gets or sets the weekday for the response.
                            /// </summary>
                            [XmlAttribute("weekDay")]
                            public string? WeekDay { get; set; }

                            /// <summary>
                            /// Gets or sets the month/day for the response.
                            /// </summary>
                            [XmlAttribute("monthDay")]
                            public string? MonthDay { get; set; }
                        }
                    }
                }

                /// <summary>
                /// Class representing a response data source item.
                /// </summary>
                public class DataSourceType : IRestIdentifiable
                {
                    /// <summary>
                    /// Gets or sets the ID for the response.
                    /// </summary>
                    [XmlAttribute("id")]
                    public Guid Id { get; set; }
                }

                /// <summary>
                /// Class representing a response workbook item.
                /// </summary>
                public class WorkbookType : IRestIdentifiable
                {
                    /// <summary>
                    /// Gets or sets the ID for the response.
                    /// </summary>
                    [XmlAttribute("id")]
                    public Guid Id { get; set; }
                }
            }
        }
    }
}

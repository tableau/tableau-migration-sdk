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
using System.Linq;
using System.Xml.Serialization;
using Tableau.Migration.Content.Schedules;
using Tableau.Migration.Content.Schedules.Cloud;
using CloudModels = Tableau.Migration.Api.Models.Cloud;

namespace Tableau.Migration.Api.Rest.Models.Requests.Cloud
{
    /// <summary>   
    /// <para>
    /// Class representing a create extract refresh task request.
    /// </para>
    /// <para>
    /// See https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref.htm#create_cloud_extract_refresh_task for documentation.
    /// </para>
    /// </summary>
    [XmlType(XmlTypeName)]
    public class CreateExtractRefreshTaskRequest : TableauServerRequest
    {
        /// <summary>
        /// Gets or sets the extract refresh for the request.
        /// </summary>
        [XmlElement("extractRefresh")]
        public ExtractRefreshType? ExtractRefresh { get; set; }

        /// <summary>
        /// Gets or sets the schedule for the request.
        /// </summary>
        [XmlElement("schedule")]
        public ScheduleType? Schedule { get; set; }

        /// <summary>
        /// Creates a new <see cref="CreateExtractRefreshTaskRequest"/> instance.
        /// </summary>
        public CreateExtractRefreshTaskRequest()
        { }

        /// <summary>
        /// Creates a new <see cref="CreateExtractRefreshTaskRequest"/> instance.
        /// </summary>
        /// <param name="type">The extract refresh type.</param>
        /// <param name="contentType">The extract refresh task's content type.</param>
        /// <param name="contentId">The extract refresh task's content ID.</param>
        /// <param name="schedule">The extract refresh task's schedule.</param>
        public CreateExtractRefreshTaskRequest(
            string type,
            ExtractRefreshContentType contentType,
            Guid contentId, 
            ICloudSchedule schedule)
        {
            ExtractRefresh = new ExtractRefreshType(
                type,
                contentType,
                contentId);
            Schedule = new ScheduleType(
                schedule);
        }

        /// <summary>
        /// Creates a new <see cref="CreateExtractRefreshTaskRequest"/> instance.
        /// </summary>
        /// <param name="options">The extract refresh task creation options.</param>
        public CreateExtractRefreshTaskRequest(
            CloudModels.ICreateExtractRefreshTaskOptions options)
            : this(
                  options.Type,
                  options.ContentType, 
                  options.ContentId, 
                  options.Schedule)
        { }

        /// <summary>
        /// Class representing a request extract refresh item.
        /// </summary>
        public class ExtractRefreshType
        {
            /// <summary>
            /// Gets or sets the type of extract refresh to the request.
            /// </summary>
            [XmlAttribute("type")]
            public string? Type { get; set; }

            /// <summary>
            /// Gets or sets the data source for the request.
            /// </summary>
            [XmlElement("datasource")]
            public DataSourceType? DataSource { get; set; }

            /// <summary>
            /// Gets or sets the workbook for the request.
            /// </summary>
            [XmlElement("workbook")]
            public WorkbookType? Workbook { get; set; }

            /// <summary>
            /// Creates a new <see cref="ExtractRefreshType"/> instance.
            /// </summary>
            public ExtractRefreshType()
            { }

            /// <summary>
            /// Creates a new <see cref="ExtractRefreshType"/> instance.
            /// </summary>
            /// <param name="type">The extract refresh type.</param>
            /// <param name="contentType">The extract refresh task's content type.</param>
            /// <param name="contentId">The extract refresh task's content ID.</param>
            public ExtractRefreshType(
                string type,
                ExtractRefreshContentType contentType,
                Guid contentId)
            {
                Type = type;
                switch (contentType)
                {
                    case ExtractRefreshContentType.Workbook:
                        Workbook = new(contentId);
                        break;

                    case ExtractRefreshContentType.DataSource:
                        DataSource = new(contentId);
                        break;
                }
            }

            /// <summary>
            /// Class representing a request data source item.
            /// </summary>
            public class DataSourceType
            {
                /// <summary>
                /// Gets or sets the ID for the request.
                /// </summary>
                [XmlAttribute("id")]
                public Guid Id { get; set; }

                /// <summary>
                /// Creates a new <see cref="DataSourceType"/> instance.
                /// </summary>
                public DataSourceType()
                { }

                /// <summary>
                /// Creates a new <see cref="DataSourceType"/> instance.
                /// </summary>
                public DataSourceType(Guid dataSourceId)
                {
                    Id = dataSourceId;
                }
            }

            /// <summary>
            /// Class representing a request workbook item.
            /// </summary>
            public class WorkbookType
            {
                /// <summary>
                /// Gets or sets the ID for the request.
                /// </summary>
                [XmlAttribute("id")]
                public Guid Id { get; set; }

                /// <summary>
                /// Creates a new <see cref="WorkbookType"/> instance.
                /// </summary>
                public WorkbookType()
                { }

                /// <summary>
                /// Creates a new <see cref="WorkbookType"/> instance.
                /// </summary>
                public WorkbookType(Guid workbookId)
                {
                    Id = workbookId;
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
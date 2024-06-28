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
    /// Class representing a server extract refresh tasks response.
    /// https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref.htm#tableau-server-request3
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
            public class ExtractRefreshType : IServerExtractRefreshType<ExtractRefreshType.WorkbookType, ExtractRefreshType.DataSourceType>
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
                public ScheduleType? Schedule { get; set; }

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
                IScheduleReferenceType? IWithScheduleReferenceType.Schedule => Schedule;

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

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
    public class ScheduleExtractRefreshTasksResponse : PagedTableauServerResponse<ScheduleExtractRefreshTasksResponse.ExtractType>
    {
        /// <summary>
        /// Gets or sets the items for the response.
        /// </summary>
        [XmlArray("extracts")]
        [XmlArrayItem("extract")]
        public override ExtractType[] Items { get; set; } = Array.Empty<ExtractType>();

        /// <summary>
        /// Class representing the extract type in the response.
        /// </summary>
        public class ExtractType
        {            
            /// <summary>
            /// Gets or sets the id for the extract.
            /// </summary>
            [XmlAttribute("id")]
            public Guid Id { get; set; }

            /// <summary>
            /// Gets or sets the priority for the extract.
            /// </summary>
            [XmlAttribute("priority")]
            public int Priority { get; set; }

            /// <summary>
            /// Gets or sets the type for the extract.
            /// </summary>
            [XmlAttribute("type")]
            public string? Type { get; set; }

            /// <summary>
            /// Gets or sets the workbook for the extract.
            /// </summary>
            [XmlElement("workbook")]
            public WorkbookType? Workbook { get; set; }

            /// <summary>
            /// Gets or sets the datasource for the extract.
            /// </summary>
            [XmlElement("datasource")]
            public DataSourceType? DataSource { get; set; }

            /// <summary>
            /// Class representing the workbook in the response.
            /// </summary>
            public class WorkbookType
            {
                /// <summary>
                /// Gets or sets the id for the workbook.
                /// </summary>
                [XmlAttribute("id")]
                public Guid Id { get; set; }
            }

            /// <summary>
            /// Class representing the data source in the response.
            /// </summary>
            public class DataSourceType
            {
                /// <summary>
                /// Gets or sets the id for the datasource.
                /// </summary>
                [XmlAttribute("id")]
                public Guid Id { get; set; }
            }
        }
    }
}
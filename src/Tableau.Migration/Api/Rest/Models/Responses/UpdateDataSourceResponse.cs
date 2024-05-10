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

namespace Tableau.Migration.Api.Rest.Models.Responses
{
    /// <summary>
    /// Class representing a data source update response.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class UpdateDataSourceResponse : TableauServerResponse<UpdateDataSourceResponse.DataSourceType>
    {
        /// <summary>
        /// Get or sets the data source object.
        /// </summary>
        [XmlElement("datasource")]
        public override DataSourceType? Item { get; set; }

        /// <summary>
        /// Type for the data source object.
        /// </summary>
        public class DataSourceType : IRestIdentifiable
        {
            /// <summary>
            /// Class representing a project response.
            /// </summary>
            public class ProjectType : IRestIdentifiable
            {
                /// <summary>
                /// Gets or sets the id for the response.
                /// </summary>
                [XmlAttribute("id")]
                public Guid Id { get; set; }
            }

            /// <summary>
            /// Class representing a owner response.
            /// </summary>
            public class OwnerType : IRestIdentifiable
            {
                /// <summary>
                /// Gets or sets the id for the response.
                /// </summary>
                [XmlAttribute("id")]
                public Guid Id { get; set; }
            }

            /// <summary>
            /// Class representing a job response.
            /// </summary>
            public class JobType : IRestIdentifiable
            {
                /// <summary>
                /// Gets or sets the id for the response.
                /// </summary>
                [XmlAttribute("id")]
                public Guid Id { get; set; }
            }

            /// <summary>
            /// Gets or sets the id for the response.
            /// </summary>
            [XmlAttribute("id")]
            public Guid Id { get; set; }

            /// <summary>
            /// Gets or sets the name for the response.
            /// </summary>
            [XmlAttribute("name")]
            public string? Name { get; set; }

            /// <summary>
            /// Gets or sets the content URL for the response.
            /// </summary>
            [XmlAttribute("contentUrl")]
            public string? ContentUrl { get; set; }

            /// <summary>
            /// Gets or sets the type for the response.
            /// </summary>
            [XmlAttribute("type")]
            public string? Type { get; set; }

            /// <summary>
            /// Gets or sets the creation date/time for the response.
            /// </summary>
            [XmlAttribute("createdAt")]
            public string? CreatedAt { get; set; }

            /// <summary>
            /// Gets or sets the update date/time for the response.
            /// </summary>
            [XmlAttribute("updatedAt")]
            public string? UpdatedAt { get; set; }

            /// <summary>
            /// Gets or sets the certification status for the response.
            /// </summary>
            [XmlAttribute("isCertified")]
            public bool IsCertified { get; set; }

            /// <summary>
            /// Gets or sets the certification note for the response.
            /// </summary>
            [XmlAttribute("certificationNote")]
            public string? CertificationNote { get; set; }

            /// <summary>
            /// Gets or sets the encrypt extracts flag for the response.
            /// </summary>
            [XmlAttribute("encryptExtracts")]
            public bool EncryptExtracts { get; set; }

            /// <summary>
            /// Gets or sets the project for the response.
            /// </summary>
            [XmlElement("project")]
            public ProjectType? Project { get; set; }

            /// <summary>
            /// Gets or sets the owner for the response.
            /// </summary>
            [XmlElement("owner")]
            public OwnerType? Owner { get; set; }

            /// <summary>
            /// Gets or sets the job for the response.
            /// </summary>
            [XmlElement("job")]
            public JobType? Job { get; set; }
        }
    }
}

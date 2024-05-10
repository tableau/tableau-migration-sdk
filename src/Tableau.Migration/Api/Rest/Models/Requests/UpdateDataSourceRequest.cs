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

namespace Tableau.Migration.Api.Rest.Models.Requests
{
    /// <summary>
    /// <para>
    /// Class representing an update data source request.
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_data_sources.htm#update_data_source">Tableau API Reference</see> for documentation.
    /// </para>
    /// </summary>
    [XmlType(XmlTypeName)]
    public class UpdateDataSourceRequest : TableauServerRequest
    {
        /// <summary>
        /// The default parameterless constructor.
        /// </summary>
        public UpdateDataSourceRequest() { }

        /// <summary>
        /// Builds the Update request for a data source.
        /// </summary>
        /// <param name="newName">(Optional) The new name of a the data source.</param>
        /// <param name="newProjectId">(Optional) The LUID of a project to add the data source to.</param>
        /// <param name="newOwnerId">(Optional) The LUID of a user to assign the data source to as owner.</param>
        /// <param name="newIsCertified">(Optional) A Boolean value that indicates whether the data source is certified.</param>
        /// <param name="newCertificationNote">(Optional) A note that provides more information on the certification of the data source, if applicable.</param>
        /// <param name="newEncryptExtracts">(Optional) true to encrypt the extracts or false to not encrypt extracts.</param>
        public UpdateDataSourceRequest(
            string? newName = null,
            Guid? newProjectId = null,
            Guid? newOwnerId = null,
            bool? newIsCertified = null,
            string? newCertificationNote = null,
            bool? newEncryptExtracts = null)
        {
            DataSource = new(newIsCertified, newEncryptExtracts);

            if (newName is not null)
                DataSource.Name = newName;

            if (newProjectId is not null)
                DataSource.Project = new() { Id = newProjectId.Value };

            if (newOwnerId is not null)
                DataSource.Owner = new() { Id = newOwnerId.Value };

            if (newCertificationNote is not null)
                DataSource.CertificationNote = newCertificationNote;
        }

        /// <summary>
        /// Gets or sets the data source for the request.
        /// </summary>
        [XmlElement("datasource")]
        public DataSourceType? DataSource { get; set; }

        /// <summary>
        /// The data source type in the API request body.
        /// </summary>
        public class DataSourceType
        {
            private bool? _isCertified;
            private bool? _encryptExtracts;

            /// <summary>
            /// Default parameterless constructor.
            /// </summary>
            public DataSourceType() { }

            /// <summary>
            /// Creates a new <see cref="DataSourceType"/> instance.
            /// </summary>
            /// <param name="isCertified">(Optional) A Boolean value that indicates whether the data source is certified.</param>
            /// <param name="encryptExtracts">(Optional) true to encrypt the extracts or false to not encrypt extracts.</param>
            public DataSourceType(
                bool? isCertified,
                bool? encryptExtracts)
            {
                _isCertified = isCertified;
                _encryptExtracts = encryptExtracts;
            }

            /// <summary>
            /// Class representing a project request.
            /// </summary>
            public class ProjectType
            {
                /// <summary>
                /// Gets or sets the id for the request.
                /// </summary>
                [XmlAttribute("id")]
                public Guid Id { get; set; }
            }

            /// <summary>
            /// Class representing a owner request.
            /// </summary>
            public class OwnerType
            {
                /// <summary>
                /// Gets or sets the id for the request.
                /// </summary>
                [XmlAttribute("id")]
                public Guid Id { get; set; }
            }

            /// <summary>
            /// Gets or sets the name for the request.
            /// </summary>
            [XmlAttribute("name")]
            public string? Name { get; set; }

            /// <summary>
            /// Gets or sets the project for the request.
            /// </summary>
            [XmlElement("project")]
            public ProjectType? Project { get; set; }

            /// <summary>
            /// Gets or sets the owner for the request.
            /// </summary>
            [XmlElement("owner")]
            public OwnerType? Owner { get; set; }

            /// <summary>
            /// Gets the certification status for the request.
            /// </summary>
            [XmlAttribute("isCertified")]
            public bool IsCertified
            {
                get => _isCertified!.Value;
                set => _isCertified = value;
            }

            /// <summary>
            /// Defines the serialization for the property <see cref="IsCertified"/>.
            /// </summary>
            [XmlIgnore]
            public bool IsCertifiedSpecified => _isCertified.HasValue;

            /// <summary>
            /// Gets or sets the certification note for the request.
            /// </summary>
            [XmlAttribute("certificationNote")]
            public string? CertificationNote { get; set; }

            /// <summary>
            /// Gets the encrypt extracts flag for the request.
            /// </summary>
            [XmlAttribute("encryptExtracts")]
            public bool EncryptExtracts
            {
                get => _encryptExtracts!.Value;
                set => _encryptExtracts = value;
            }

            /// <summary>
            /// Defines the serialization for the property <see cref="EncryptExtracts"/>.
            /// </summary>
            [XmlIgnore]
            public bool EncryptExtractsSpecified => _encryptExtracts.HasValue;
        }
    }
}

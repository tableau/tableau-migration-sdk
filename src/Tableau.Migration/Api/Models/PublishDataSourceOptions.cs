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
using System.IO;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content;

namespace Tableau.Migration.Api.Models
{
    /// <summary>
    /// Class for API client data source publish options. 
    /// </summary>
    public class PublishDataSourceOptions : PublishContentWithFileOptions, IPublishDataSourceOptions
    {
        ///<inheritdoc/>
        public string Name { get; }

        ///<inheritdoc/>
        public string Description { get; }

        ///<inheritdoc/>
        public bool UseRemoteQueryAgent { get; }

        ///<inheritdoc/>
        public bool EncryptExtracts { get; }

        ///<inheritdoc/>
        public bool Overwrite { get; } = true;

        ///<inheritdoc/>
        public Guid ProjectId { get; }

        /// <summary>
        /// Creates a new <see cref="PublishDataSourceOptions"/> instance.
        /// </summary>
        /// <param name="dataSource">The publishable data source information.</param>
        /// <param name="file">The data source file as a <see cref="Stream"/></param>
        /// <param name="fileType">The type of data source file.</param>
        public PublishDataSourceOptions(
            IPublishableDataSource dataSource,
            Stream file,
            string fileType = DataSourceFileTypes.Tdsx)
             : base(
                  file,
                  dataSource.File.OriginalFileName,
                  fileType)
        {
            Name = dataSource.Name;
            Description = dataSource.Description;
            UseRemoteQueryAgent = dataSource.UseRemoteQueryAgent;
            EncryptExtracts = dataSource.EncryptExtracts;
            ProjectId = ((IContainerContent)dataSource).Container.Id;
        }
    }
}

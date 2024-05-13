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
using System.Collections.Generic;
using System.Linq;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal class RestCommitDataSourceUploadResponseBuilder : RestCommitUploadResponseBuilder<DataSourceResponse, DataSourceResponse.DataSourceType, CommitDataSourcePublishRequest>
    {
        public RestCommitDataSourceUploadResponseBuilder(
            TableauData data,
            IHttpContentSerializer serializer)
            : base(
                  data,
                  serializer,
                  data => data.DataSources,
                  data => data.DataSourceFiles,
                  (data, ds, file) => data.AddDataSource(ds, file))
        { }

        protected override DataSourceResponse.DataSourceType? GetExistingContentItem(CommitDataSourcePublishRequest commitRequest)
        {
            var commitDataSource = Guard.AgainstNull(commitRequest.DataSource, () => commitRequest.DataSource);

            var targetDataSource = Data.DataSources
                .SingleOrDefault(ds =>
                    ds.Project?.Id == commitDataSource.Project?.Id &&
                    string.Equals(ds.Name, commitDataSource.Name, DataSource.NameComparison));

            return targetDataSource;
        }

        protected override DataSourceResponse.DataSourceType BuildContent(
            CommitDataSourcePublishRequest commitRequest,
            ref byte[] commitFileData,
            DataSourceResponse.DataSourceType? existingContent,
            UsersResponse.UserType currentUser,
            bool overwrite)
        {
            var commitDataSource = Guard.AgainstNull(commitRequest.DataSource, () => commitRequest.DataSource);

            var targetDataSource = existingContent ?? new DataSourceResponse.DataSourceType
            {
                Id = Guid.NewGuid(),
                ContentUrl = Guid.NewGuid().ToString(), // Auto-generated to simplify.
                CreatedAt = DateTime.UtcNow.ToString(),
                EncryptExtracts = false,
                IsCertified = false,
            };

            targetDataSource.Name = commitDataSource.Name;
            targetDataSource.Description = commitDataSource.Description;
            targetDataSource.UpdatedAt = DateTime.UtcNow.ToString();
            targetDataSource.UseRemoteQueryAgent = commitDataSource.UseRemoteQueryAgent;

            targetDataSource.Owner = new()
            {
                Id = currentUser.Id
            };

            targetDataSource.Project = new()
            {
                Id = commitDataSource.Project?.Id ?? Data.DefaultProject.Id
            };

            targetDataSource.Tags = [];

            return targetDataSource;
        }

        protected override DataSourceResponse.DataSourceType BuildResponse(DataSourceResponse.DataSourceType dataSource)
        {
            DataSourceResponse.DataSourceType dataSourceType = dataSource;

            if (dataSource.Owner is not null)
            {
                dataSourceType.Owner = new()
                {
                    Id = dataSource.Owner.Id
                };
            }

            if (dataSource.Project is not null)
            {
                dataSourceType.Project = new()
                {
                    Id = dataSource.Project.Id,
                    Name = dataSource.Project.Name,
                };
            }

            var tags = new List<DataSourceResponse.DataSourceType.TagType>();

            if (dataSource.Tags is not null)
            {
                foreach (var dataSourceTag in dataSource.Tags)
                {
                    tags.Add(new()
                    {
                        Label = dataSourceTag.Label
                    });
                }
            }

            dataSourceType.Tags = tags.ToArray();

            return dataSourceType;
        }
    }
}

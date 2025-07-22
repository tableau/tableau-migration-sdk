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

using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoFixture;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Simulation;
using Tableau.Migration.Net;
using Xunit;

namespace Tableau.Migration.Tests.Simulation.DataPreparation
{
    /// <summary>
    /// Static class responsible for preparing data sources data for migration tests.
    /// </summary>
    public static class DataSourcesDataPreparation
    {
        /// <summary>
        /// Prepares the source data for migration tests.
        /// </summary>
        /// <param name="sourceApi">The source API simulator.</param>
        /// <param name="fixture">The fixture for creating test data.</param>
        /// <returns>The list of prepared data sources.</returns>
        public static List<DataSourceResponse.DataSourceType> PrepareServerSource(
            TableauApiSimulator sourceApi,
            IFixture fixture)
        {
            var dataSources = new List<DataSourceResponse.DataSourceType>();

            // Get all users that are not support users, and all groups
            var users = CommonDataPreparation.GetNonSupportUsers(sourceApi);
            var groups = sourceApi.Data.Groups;

            int counter = 0;
            foreach (var project in sourceApi.Data.Projects)
            { // loop over all the projects and add a data source to each
                var dataSource = fixture.Create<DataSourceResponse.DataSourceType>();
                dataSource.Project = CommonDataPreparation.CreateProjectReference<DataSourceResponse.DataSourceType.ProjectType>(project);

                var dataSourceFileData = new SimulatedDataSourceData();

                var owner = users[counter % users.Count];
                dataSource.Owner = CommonDataPreparation.CreateOwnerReference<DataSourceResponse.DataSourceType.OwnerType>(owner);

                sourceApi.Data.AddDataSourcePermissions(dataSource, CommonDataPreparation.CreatePermissions(users, groups, fixture));

                // Assert infra
                Assert.NotNull(dataSource.Tags);
                Assert.NotEmpty(dataSource.Tags);

                var keychains = CommonDataPreparation.CreateEmbeddedCredentials(sourceApi.Data.DataSourceKeychains, users, dataSource.Id, counter, fixture);
                CommonDataPreparation.CreateConnections(dataSourceFileData, embed: keychains.EncryptedKeychainList.Any(), fixture);

                // Our data source data will just be a guid as a string, encoded to a byte array
                byte[] dataSourceData = Encoding.UTF8.GetBytes(dataSourceFileData.ToXml());
                sourceApi.Data.AddDataSource(dataSource, dataSourceData);
                dataSources.Add(dataSource);
                counter++;
            }

            return dataSources;
        }
    }
}
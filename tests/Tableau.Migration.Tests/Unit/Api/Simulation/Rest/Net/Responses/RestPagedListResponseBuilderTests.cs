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

using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Simulation;
using Tableau.Migration.Api.Simulation.Rest.Net.Responses;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Simulation.Rest.Net.Responses
{
    public class RestPagedListResponseBuilderTests
    {
        public abstract class RestPagedListResponseBuilderTest : ResponseBuilderTestBase
        { }

        public class RespondAsync : RestPagedListResponseBuilderTest
        {
            [Fact]
            public async Task Creates_response()
            {
                var data = new TableauData(Create<UsersResponse.UserType>());

                const int WORKBOOK_COUNT = 10;

                for (var i = 0; i != WORKBOOK_COUNT; i++)
                {
                    data.Workbooks.Add(Create<WorkbookResponse.WorkbookType>());
                }

                var builder = new RestPagedListResponseBuilder<WorkbooksResponse, WorkbooksResponse.WorkbookType>(
                    data,
                    Serializer,
                    (data, _) => data.Workbooks.Select(workbook => new WorkbooksResponse.WorkbookType(workbook)).ToList(),
                    false);

                var response = await builder.RespondAsync(new HttpRequestMessage(), Cancel);

                Assert.NotNull(response.Content);

                var deserialized = await Serializer.DeserializeAsync<WorkbooksResponse>(response.Content, Cancel);

                Assert.NotNull(deserialized);

                Assert.Equal(1, deserialized.Pagination.PageNumber);
                Assert.Equal(100, deserialized.Pagination.PageSize);
                Assert.Equal(WORKBOOK_COUNT, deserialized.Pagination.TotalAvailable);

                Assert.Null(deserialized.Error);
                Assert.Equal(WORKBOOK_COUNT, deserialized.Items.Length);

                Assert.All(deserialized.Items, i => _ = data.Workbooks.Single(wb => wb.Id == i.Id));
            }
        }
    }
}

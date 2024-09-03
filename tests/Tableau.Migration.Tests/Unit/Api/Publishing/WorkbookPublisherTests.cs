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

using System.Threading.Tasks;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Publishing;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Publishing
{
    public class WorkbookPublisherTests
    {
        public abstract class WorkbookPublisherTest : FilePublisherTestBase<IWorkbookPublisher, IPublishWorkbookOptions, IWorkbookDetails>
        {
            internal readonly WorkbookPublisher WorkbookPublisher;

            protected override IWorkbookPublisher Publisher => WorkbookPublisher;

            public WorkbookPublisherTest()
                : base(RestUrlPrefixes.Workbooks)
            {
                WorkbookPublisher = CreateService<WorkbookPublisher>();
            }
        }

        public class PublishAsync : WorkbookPublisherTest
        {
            [Fact]
            public async Task Publishes()
            {
                var initiateResponse = SetupSuccessResponse<FileUploadResponse, FileUploadResponse.FileUploadType>();
                var getWorkbookResponse = SetupSuccessResponse<WorkbookResponse, WorkbookResponse.WorkbookType>();

                var publishOptions = Create<IPublishWorkbookOptions>();

                var result = await Publisher.PublishAsync(publishOptions, Cancel);

                Assert.True(result.Success);

                AssertRequests(
                    initiateResponse.Item,
                    r =>
                    {
                        r.AssertQuery("workbookType", publishOptions.FileType);
                        r.AssertQuery("overwrite", publishOptions.Overwrite.ToString().ToLower());
                        r.AssertQuery("skipConnectionCheck", publishOptions.SkipConnectionCheck.ToString().ToLower());
                    });

                Assert.Equal(getWorkbookResponse.Item.Id, result.Value.Id);
            }
        }
    }
}

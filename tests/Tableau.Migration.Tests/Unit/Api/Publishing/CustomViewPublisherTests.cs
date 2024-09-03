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
using Tableau.Migration.Api;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Publishing;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Publishing
{
    public class CustomViewPublisherTests
    {
        public abstract class CustomViewPublisherTest :
            FilePublisherTestBase<ICustomViewPublisher, IPublishCustomViewOptions, ICustomView>
        {
            internal readonly CustomViewPublisher CustomViewPublisher;

            protected override ICustomViewPublisher Publisher => CustomViewPublisher;

            public CustomViewPublisherTest() : base(RestUrlPrefixes.CustomViews)
            {
                CustomViewPublisher = CreateService<CustomViewPublisher>();
            }
        }

        public class PublishAsync : CustomViewPublisherTest
        {
            [Fact]
            public async Task Publishes()
            {
                var initiateResponse = SetupSuccessResponse<FileUploadResponse, FileUploadResponse.FileUploadType>();
                var getCustomViewResponse = SetupSuccessResponse<CustomViewResponse, CustomViewResponse.CustomViewType>();

                var publishOptions = Create<IPublishCustomViewOptions>();

                var result = await CustomViewPublisher.PublishAsync(publishOptions, Cancel);

                Assert.True(result.Success);

                Assert.Equal(getCustomViewResponse.Item.Id, result.Value.Id);
            }
        }
    }
}

﻿//
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

using System.Threading.Tasks;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Publishing;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Publishing
{
    public class FlowPublisherTests
    {
        public abstract class FlowPublisherTest : FilePublisherTestBase<IFlowPublisher, IPublishFlowOptions, IFlow>
        {
            internal readonly FlowPublisher FlowPublisher;

            protected override IFlowPublisher Publisher => FlowPublisher;

            public FlowPublisherTest()
                : base(RestUrlKeywords.Flows)
            {
                FlowPublisher = CreateService<FlowPublisher>();
            }
        }

        public class PublishAsync : FlowPublisherTest
        {
            [Fact]
            public async Task Publishes()
            {
                var initiateResponse = SetupSuccessResponse<FileUploadResponse, FileUploadResponse.FileUploadType>();
                var getFlowResponse = SetupSuccessResponse<FlowResponse, FlowResponse.FlowType>();

                var publishOptions = Create<IPublishFlowOptions>();

                var result = await FlowPublisher.PublishAsync(publishOptions, Cancel);

                Assert.True(result.Success);

                AssertRequests(
                    initiateResponse.Item,
                    r =>
                    {
                        r.AssertQuery("flowType", publishOptions.FileType);
                        r.AssertQuery("overwrite", publishOptions.Overwrite.ToString().ToLower());
                    });

                Assert.Equal(getFlowResponse.Item.Id, result.Value.Id);
            }
        }
    }
}

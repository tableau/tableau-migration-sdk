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
using System.Threading.Tasks;
using AutoFixture;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Labels;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Tableau.Migration.Net;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Labels
{
    public class LabelsApiClientTests : ApiTestBase
    {
        public class LabelsApiClientTest : LabelsApiClientTests
        {
            internal LabelsApiClient<IDataSource> TestLabelsApiClient;
            internal readonly string LabelsRelativeUri;

            public LabelsApiClientTest()
            {
                TestLabelsApiClient = new LabelsApiClient<IDataSource>(
                    RestRequestBuilderFactory,
                    MockLoggerFactory.Object,
                    MockSharedResourcesLocalizer.Object);
                LabelsRelativeUri = $"/api/{TableauServerVersion.RestApiVersion}/labels";
            }
        }
        public class GetLabelsAsync : LabelsApiClientTest
        {
            [Fact]
            public async Task Returns_success()
            {
                var response = AutoFixture.CreateResponse<LabelsResponse>();

                var mockResponse = new MockHttpResponseMessage<LabelsResponse>(response);

                MockHttpClient.SetupResponse(mockResponse);

                var result = await TestLabelsApiClient.GetLabelsAsync(Guid.NewGuid(), Cancel);

                Assert.True(result.Success);
                Assert.NotNull(result.Value);

                var labels = result.Value;
                Assert.NotNull(labels);
                Assert.NotEmpty(labels);

                MockHttpClient
                    .AssertSingleRequest()
                    .AssertRelativeUri(LabelsRelativeUri);
            }

            [Fact]
            public async Task Returns_success_with_category_input()
            {
                var response = AutoFixture.CreateResponse<LabelsResponse>();

                var mockResponse = new MockHttpResponseMessage<LabelsResponse>(response);

                MockHttpClient.SetupResponse(mockResponse);

                var categories = LabelCategories.GetAll();

                var result = await TestLabelsApiClient.GetLabelsAsync(Guid.NewGuid(), Cancel, categories);

                Assert.True(result.Success);
                Assert.NotNull(result.Value);

                var labels = result.Value;
                Assert.NotNull(labels);
                Assert.NotEmpty(labels);

                var categoryList = LabelsApiClient<IDataSource>.GetCategoryListString(categories).UrlEncode();
                MockHttpClient
                    .AssertSingleRequest()
                    .AssertRelativeUri($"{LabelsRelativeUri}?categories={categoryList}");
            }

            [Fact]
            public async Task Returns_success_on_empty_labels()
            {
                var response = AutoFixture.CreateResponse<LabelsResponse>();
                response.Items = Array.Empty<LabelsResponse.LabelType>();

                var mockResponse = new MockHttpResponseMessage<LabelsResponse>(response);

                MockHttpClient.SetupResponse(mockResponse);

                var result = await TestLabelsApiClient.GetLabelsAsync(Guid.NewGuid(), Cancel);

                Assert.True(result.Success);
                Assert.NotNull(result.Value);

                var labels = result.Value;
                Assert.NotNull(labels);
                Assert.Empty(labels);
            }
        }
        public class UpdateLabelsAsync : LabelsApiClientTest
        {
            [Fact]
            public async Task Returns_success()
            {
                var labels = CreateMany<LabelsResponse.LabelType>().ToList();

                foreach (var label in labels)
                {
                    // Although the return type of an update label call is a list of labels,
                    // in practice, the list contains only 1 element.
                    var response = AutoFixture.Build<LabelsResponse>()
                        .With(r => r.Items, new[] { label })
                        .Without(r => r.Error)
                        .Create();

                    MockHttpClient.SetupResponse(new MockHttpResponseMessage<LabelsResponse>(response));
                }

                var result = await TestLabelsApiClient.UpdateLabelsAsync(
                    Guid.NewGuid(),
                    labels.Select(l => (ILabelUpdateOptions)new LabelUpdateOptions(new Label(l))),
                    Cancel);

                Assert.True(result.Success);
                Assert.NotNull(result.Value);

                var resultLabels = result.Value;
                Assert.NotNull(labels);
                Assert.NotEmpty(labels);
                Assert.Equal(labels.Count, resultLabels.Count);

                Assert.All(
                    MockHttpClient.AssertRequestCount(labels.Count),
                    request => request.AssertRelativeUri(LabelsRelativeUri));
            }

        }

        public class GetCategoryList : LabelsApiClientTest
        {
            [Fact]
            public void Parses_multiple()
            {
                var test = LabelCategories.GetAll().ToList();

                var result = LabelsApiClient<IDataSource>.GetCategoryListString(test);

                Assert.NotNull(result);
                Assert.False(string.IsNullOrEmpty(result));
            }

            [Fact]
            public void Parses_single()
            {
                var test = CreateMany<string>(1).ToList();

                var result = LabelsApiClient<IDataSource>.GetCategoryListString(test);

                Assert.NotNull(result);
                Assert.False(string.IsNullOrEmpty(result));
                Assert.DoesNotContain(result, ",");
            }

            [Fact]
            public void Parses_empty_list()
            {
                var result = LabelsApiClient<IDataSource>.GetCategoryListString(new List<string>());

                Assert.NotNull(result);
                Assert.True(string.IsNullOrEmpty(result));
            }
        }
    }
}

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

using System;
using AutoFixture;
using Tableau.Migration.Api.Rest.Models.Responses;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models.Responses
{
    public class UpdateFlowResponseTests
    {
        public class Serialization : SerializationTestBase
        {
            [Fact]
            public void DeserializesSuccess()
            {
                var (testXml, expectedFlow) = GetTestDataMinimal();
                var deserialized = Serializer.DeserializeFromXml<UpdateFlowResponse>(testXml);

                Assert.NotNull(deserialized);
                Assert.Null(deserialized.Error);
                var flow = deserialized.Item;
                Assert.NotNull(flow);

                // Only check non-optional fields for minimal test
                Assert.Equal(expectedFlow.Id, flow.Id);
                Assert.Equal(expectedFlow.Name, flow.Name);
                Assert.Equal(expectedFlow.FileType, flow.FileType);
                Assert.Equal(expectedFlow.CreatedAt, flow.CreatedAt);
                AssertProject(expectedFlow.Project, flow.Project);
                AssertOwner(expectedFlow.Owner, flow.Owner);
            }

            [Fact]
            public void DeserializesWithOptionalFields()
            {
                var (testXml, expectedFlow) = GetTestDataWithAllFields();
                var deserialized = Serializer.DeserializeFromXml<UpdateFlowResponse>(testXml);

                Assert.NotNull(deserialized);
                Assert.Null(deserialized.Error);
                var flow = deserialized.Item;
                Assert.NotNull(flow);

                AssertFlow(expectedFlow, flow);
                Assert.Equal(expectedFlow.Description, flow.Description);
                Assert.Equal(expectedFlow.WebpageUrl, flow.WebpageUrl);
                Assert.Equal(expectedFlow.UpdatedAt, flow.UpdatedAt);
            }

            [Fact]
            public void DeserializesWithoutOptionalFields()
            {
                var flowId = Guid.NewGuid();
                var projectId = Guid.NewGuid();
                var ownerId = Guid.NewGuid();

                var testXml = $@"
<tsResponse xmlns=""http://tableau.com/api"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
    <flow id=""{flowId}"" name=""Test Flow"" fileType=""tflx"" createdAt=""2023-08-28T17:55:20Z"">
        <project id=""{projectId}"" name=""Default""/>
        <owner id=""{ownerId}""/>
    </flow>
</tsResponse>";

                var deserialized = Serializer.DeserializeFromXml<UpdateFlowResponse>(testXml);

                Assert.NotNull(deserialized);
                Assert.Null(deserialized.Error);
                var flow = deserialized.Item;
                Assert.NotNull(flow);

                Assert.Equal(flowId, flow.Id);
                Assert.Equal("Test Flow", flow.Name);
                Assert.Null(flow.Description);
                Assert.Null(flow.WebpageUrl);
                Assert.Null(flow.UpdatedAt);
                Assert.Equal("tflx", flow.FileType);
                Assert.Equal("2023-08-28T17:55:20Z", flow.CreatedAt);

                Assert.NotNull(flow.Project);
                Assert.Equal(projectId, flow.Project.Id);
                Assert.Equal("Default", flow.Project.Name);

                Assert.NotNull(flow.Owner);
                Assert.Equal(ownerId, flow.Owner.Id);
            }

            private static void AssertFlow(
                UpdateFlowResponse.FlowType? expected,
                UpdateFlowResponse.FlowType? actual)
            {
                Assert.NotNull(expected);
                Assert.NotNull(actual);
                Assert.Equal(expected.Id, actual.Id);
                Assert.Equal(expected.Name, actual.Name);
                Assert.Equal(expected.Description, actual.Description);
                Assert.Equal(expected.WebpageUrl, actual.WebpageUrl);
                Assert.Equal(expected.FileType, actual.FileType);
                Assert.Equal(expected.CreatedAt, actual.CreatedAt);
                Assert.Equal(expected.UpdatedAt, actual.UpdatedAt);
            }

            private static void AssertProject(
                UpdateFlowResponse.FlowType.ProjectType? expected,
                UpdateFlowResponse.FlowType.ProjectType? actual)
            {
                Assert.NotNull(expected);
                Assert.NotNull(actual);
                Assert.Equal(expected.Id, actual.Id);
                Assert.Equal(expected.Name, actual.Name);
            }

            private static void AssertOwner(
                UpdateFlowResponse.FlowType.OwnerType? expected,
                UpdateFlowResponse.FlowType.OwnerType? actual)
            {
                Assert.NotNull(expected);
                Assert.NotNull(actual);
                Assert.Equal(expected.Id, actual.Id);
            }

            #region - Test Data -

            private (string TestXML, UpdateFlowResponse.FlowType ExpectedFlow) GetTestDataMinimal()
            {
                var flow = AutoFixture.Create<UpdateFlowResponse.FlowType>();

                Assert.NotNull(flow);
                Assert.NotNull(flow.Project);
                Assert.NotNull(flow.Owner);

                var testXml = $@"
<tsResponse xmlns=""http://tableau.com/api"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
    <flow id=""{flow.Id}"" name=""{flow.Name}"" fileType=""{flow.FileType}"" createdAt=""{flow.CreatedAt}"">
        <project id=""{flow.Project.Id}"" name=""{flow.Project.Name}""/>
        <owner id=""{flow.Owner.Id}""/>
    </flow>
</tsResponse>";

                return (testXml, flow);
            }

            private (string TestXML, UpdateFlowResponse.FlowType ExpectedFlow) GetTestDataWithAllFields()
            {
                var flow = AutoFixture.Create<UpdateFlowResponse.FlowType>();

                Assert.NotNull(flow);
                Assert.NotNull(flow.Project);
                Assert.NotNull(flow.Owner);

                var testXml = $@"
<tsResponse xmlns=""http://tableau.com/api"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
    <flow id=""{flow.Id}"" name=""{flow.Name}"" description=""{flow.Description}"" webpageUrl=""{flow.WebpageUrl}"" fileType=""{flow.FileType}"" createdAt=""{flow.CreatedAt}"" updatedAt=""{flow.UpdatedAt}"">
        <project id=""{flow.Project.Id}"" name=""{flow.Project.Name}""/>
        <owner id=""{flow.Owner.Id}""/>
    </flow>
</tsResponse>";

                return (testXml, flow);
            }

            #endregion
        }
    }
}


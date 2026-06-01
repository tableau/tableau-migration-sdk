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
using System.Linq;
using AutoFixture;
using Tableau.Migration.Api.Rest.Models.Responses;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models.Responses
{
    public class FlowResponseTests
    {
        public class Serialization : SerializationTestBase
        {
            [Fact]
            public void DeserializesWithFlowOutputSteps()
            {
                var (testXml, expectedFlow, expectedSteps) = GetTestDataWithOutputSteps();
                var deserialized = Serializer.DeserializeFromXml<FlowResponse>(testXml);

                Assert.NotNull(deserialized);
                Assert.Null(deserialized.Error);
                var flow = deserialized.Item;
                Assert.NotNull(flow);

                AssertFlow(expectedFlow, flow);
                AssertProject(expectedFlow.Project, flow.Project);
                AssertOwner(expectedFlow.Owner, flow.Owner);
                AssertTags(expectedFlow.Tags!, flow.Tags!);
                AssertFlowOutputSteps(expectedSteps, deserialized.FlowOutputSteps);
            }

            [Fact]
            public void DeserializesWithoutFlowOutputSteps()
            {
                var (testXml, expectedFlow) = GetTestDataWithoutOutputSteps();
                var deserialized = Serializer.DeserializeFromXml<FlowResponse>(testXml);

                Assert.NotNull(deserialized);
                Assert.Null(deserialized.Error);
                var flow = deserialized.Item;
                Assert.NotNull(flow);

                AssertFlow(expectedFlow, flow);
                AssertProject(expectedFlow.Project, flow.Project);
                AssertOwner(expectedFlow.Owner, flow.Owner);
                AssertTags(expectedFlow.Tags!, flow.Tags!);
                Assert.Empty(deserialized.FlowOutputSteps);
            }

            [Fact]
            public void DeserializesWithMultipleFlowOutputSteps()
            {
                var (testXml, expectedFlow, expectedSteps) = GetTestDataWithMultipleOutputSteps();
                var deserialized = Serializer.DeserializeFromXml<FlowResponse>(testXml);

                Assert.NotNull(deserialized);
                Assert.Null(deserialized.Error);
                var flow = deserialized.Item;
                Assert.NotNull(flow);

                AssertFlow(expectedFlow, flow);
                Assert.Equal(3, deserialized.FlowOutputSteps.Length);
                AssertFlowOutputSteps(expectedSteps, deserialized.FlowOutputSteps);
            }

            private static void AssertFlow(
                FlowResponse.FlowType? expected,
                FlowResponse.FlowType? actual)
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
                FlowResponse.FlowType.ProjectType? expected,
                FlowResponse.FlowType.ProjectType? actual)
            {
                Assert.NotNull(expected);
                Assert.NotNull(actual);
                Assert.Equal(expected.Id, actual.Id);
                Assert.Equal(expected.Name, actual.Name);
            }

            private static void AssertOwner(
                FlowResponse.FlowType.OwnerType? expected,
                FlowResponse.FlowType.OwnerType? actual)
            {
                Assert.NotNull(expected);
                Assert.NotNull(actual);
                Assert.Equal(expected.Id, actual.Id);
            }

            private static void AssertTags(
                FlowResponse.FlowType.TagType[] expected,
                FlowResponse.FlowType.TagType[] actual)
            {
                Assert.Equal(expected.Length, actual.Length);
                Assert.All(actual, tag => Assert.Contains(tag, expected, ITagTypeComparer.Instance));
            }

            private static void AssertFlowOutputSteps(
                FlowResponse.FlowOutputStepType[] expected,
                FlowResponse.FlowOutputStepType[] actual)
            {
                Assert.Equal(expected.Length, actual.Length);
                for (int i = 0; i < expected.Length; i++)
                {
                    Assert.Equal(expected[i].Id, actual[i].Id);
                    Assert.Equal(expected[i].Name, actual[i].Name);
                }
            }

            #region - Test Data -

            private (string TestXML, FlowResponse.FlowType ExpectedFlow, FlowResponse.FlowOutputStepType[] ExpectedSteps) GetTestDataWithOutputSteps()
            {
                var flowTags = CreateMany<FlowResponse.FlowType.TagType>(2).ToArray();

                var flow = AutoFixture
                    .Build<FlowResponse.FlowType>()
                    .With(f => f.Tags, flowTags)
                    .Create();

                Assert.NotNull(flow);
                Assert.NotNull(flow.Project);
                Assert.NotNull(flow.Owner);

                var outputStep1 = new FlowResponse.FlowOutputStepType
                {
                    Id = Guid.NewGuid(),
                    Name = "Output Step 1"
                };

                var outputStep2 = new FlowResponse.FlowOutputStepType
                {
                    Id = Guid.NewGuid(),
                    Name = "Output Step 2"
                };

                var outputSteps = new[] { outputStep1, outputStep2 };

                var testXml = $@"
<tsResponse xmlns=""http://tableau.com/api"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""http://tableau.com/api https://help.tableau.com/samples/en-us/rest_api/ts-api_3_18.xsd"">
    <flow id=""{flow.Id}"" name=""{flow.Name}"" description=""{flow.Description}"" webpageUrl=""{flow.WebpageUrl}"" fileType=""{flow.FileType}"" createdAt=""{flow.CreatedAt}"" updatedAt=""{flow.UpdatedAt}"">
        <project id=""{flow.Project.Id}"" name=""{flow.Project.Name}""/>
        <owner id=""{flow.Owner.Id}"" name=""{Create<string>()}""/>
        <tags>
            <tag label=""{flow.Tags![0].Label}""/>
            <tag label=""{flow.Tags[1].Label}""/>
        </tags>
    </flow>
    <flowOutputSteps>
        <flowOutputStep id=""{outputStep1.Id}"" name=""{outputStep1.Name}""/>
        <flowOutputStep id=""{outputStep2.Id}"" name=""{outputStep2.Name}""/>
    </flowOutputSteps>
</tsResponse>";

                return (testXml, flow, outputSteps);
            }

            private (string TestXML, FlowResponse.FlowType ExpectedFlow) GetTestDataWithoutOutputSteps()
            {
                var flowTags = CreateMany<FlowResponse.FlowType.TagType>(2).ToArray();

                var flow = AutoFixture
                    .Build<FlowResponse.FlowType>()
                    .With(f => f.Tags, flowTags)
                    .Create();

                Assert.NotNull(flow);
                Assert.NotNull(flow.Project);
                Assert.NotNull(flow.Owner);

                var testXml = $@"
<tsResponse xmlns=""http://tableau.com/api"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""http://tableau.com/api https://help.tableau.com/samples/en-us/rest_api/ts-api_3_18.xsd"">
    <flow id=""{flow.Id}"" name=""{flow.Name}"" description=""{flow.Description}"" webpageUrl=""{flow.WebpageUrl}"" fileType=""{flow.FileType}"" createdAt=""{flow.CreatedAt}"" updatedAt=""{flow.UpdatedAt}"">
        <project id=""{flow.Project.Id}"" name=""{flow.Project.Name}""/>
        <owner id=""{flow.Owner.Id}"" name=""{Create<string>()}""/>
        <tags>
            <tag label=""{flow.Tags![0].Label}""/>
            <tag label=""{flow.Tags[1].Label}""/>
        </tags>
    </flow>
</tsResponse>";

                return (testXml, flow);
            }

            private (string TestXML, FlowResponse.FlowType ExpectedFlow, FlowResponse.FlowOutputStepType[] ExpectedSteps) GetTestDataWithMultipleOutputSteps()
            {
                var flowTags = CreateMany<FlowResponse.FlowType.TagType>(1).ToArray();

                var flow = AutoFixture
                    .Build<FlowResponse.FlowType>()
                    .With(f => f.Tags, flowTags)
                    .Create();

                Assert.NotNull(flow);
                Assert.NotNull(flow.Project);
                Assert.NotNull(flow.Owner);

                var outputStep1 = new FlowResponse.FlowOutputStepType
                {
                    Id = Guid.NewGuid(),
                    Name = "CSV Output"
                };

                var outputStep2 = new FlowResponse.FlowOutputStepType
                {
                    Id = Guid.NewGuid(),
                    Name = "Database Output"
                };

                var outputStep3 = new FlowResponse.FlowOutputStepType
                {
                    Id = Guid.NewGuid(),
                    Name = "Published Datasource"
                };

                var outputSteps = new[] { outputStep1, outputStep2, outputStep3 };

                var testXml = $@"
<tsResponse xmlns=""http://tableau.com/api"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""http://tableau.com/api https://help.tableau.com/samples/en-us/rest_api/ts-api_3_18.xsd"">
    <flow id=""{flow.Id}"" name=""{flow.Name}"" description=""{flow.Description}"" webpageUrl=""{flow.WebpageUrl}"" fileType=""{flow.FileType}"" createdAt=""{flow.CreatedAt}"" updatedAt=""{flow.UpdatedAt}"">
        <project id=""{flow.Project.Id}"" name=""{flow.Project.Name}""/>
        <owner id=""{flow.Owner.Id}"" name=""{Create<string>()}""/>
        <tags>
            <tag label=""{flow.Tags![0].Label}""/>
        </tags>
    </flow>
    <flowOutputSteps>
        <flowOutputStep id=""{outputStep1.Id}"" name=""{outputStep1.Name}""/>
        <flowOutputStep id=""{outputStep2.Id}"" name=""{outputStep2.Name}""/>
        <flowOutputStep id=""{outputStep3.Id}"" name=""{outputStep3.Name}""/>
    </flowOutputSteps>
</tsResponse>";

                return (testXml, flow, outputSteps);
            }

            #endregion
        }
    }
}


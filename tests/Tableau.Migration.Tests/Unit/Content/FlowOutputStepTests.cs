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
using Moq;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content
{
    public class FlowOutputStepTests
    {
        public class Ctor : AutoFixtureTestBase
        {
            [Fact]
            public void EmptyId()
            {
                var mockResponse = Create<Mock<IFlowOutputStepType>>();
                mockResponse.SetupGet(x => x.Id).Returns(Guid.Empty);

                Assert.Throws<ArgumentException>(() => new FlowOutputStep(mockResponse.Object));
            }

            [Theory]
            [NullEmptyWhiteSpaceData]
            public void NameRequired(string? s)
            {
                var mockResponse = Create<Mock<IFlowOutputStepType>>();
                mockResponse.SetupGet(x => x.Name).Returns(s);

                Assert.Throws<ArgumentException>(() => new FlowOutputStep(mockResponse.Object));
            }

            [Fact]
            public void Initializes()
            {
                var mockResponse = Create<Mock<IFlowOutputStepType>>();
                var expectedId = Guid.NewGuid();
                var expectedName = "Test Output Step";
                
                mockResponse.SetupGet(x => x.Id).Returns(expectedId);
                mockResponse.SetupGet(x => x.Name).Returns(expectedName);

                var result = new FlowOutputStep(mockResponse.Object);

                Assert.Equal(expectedId, result.Id);
                Assert.Equal(expectedName, result.Name);
            }
        }
    }
}


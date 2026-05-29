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
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content
{
    public class FlowDetailsTests
    {
        public abstract class FlowDetailsTest : AutoFixtureTestBase
        {
            public IFlowDetails CreateFlow() => Create<IFlowDetails>();
            public IFlowDetailsType CreateResponse() => Create<IFlowDetailsType>();

            public IContentReference CreateProjectReference() => Create<IContentReference>();
            public IContentReference CreateOwnerReference() => Create<IContentReference>();
        }

        public class Ctor
        {
            public class FromResponse : FlowDetailsTest
            {
                [Fact]
                public void Initializes()
                {
                    var response = CreateResponse();
                    var project = CreateProjectReference();
                    var owner = CreateOwnerReference();

                    var result = new FlowDetails(response, project, owner);

                    result.Assert(response, project, owner, flow =>
                    {
                        foreach (var responseStep in response.FlowOutputSteps)
                        {
                            Assert.Single(flow.FlowOutputSteps, s =>
                                s.Id == responseStep.Id &&
                                s.Name == responseStep.Name);
                        }
                    });
                }
            }

            public class FromFlow : FlowDetailsTest
            {
                [Fact]
                public void Initializes()
                {
                    var flowDetails = CreateFlow();

                    var result = new FlowDetails(flowDetails);

                    result.Assert(flowDetails, flow => Assert.Same(flowDetails.FlowOutputSteps, flow.FlowOutputSteps));
                }
            }
        }
    }
}


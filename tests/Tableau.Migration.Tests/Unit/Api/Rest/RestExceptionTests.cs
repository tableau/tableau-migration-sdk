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
using System.Net.Http;
using Microsoft.Extensions.Localization;
using Moq;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest
{
    public class RestExceptionTests
    {
        public abstract class RestExceptionTest : AutoFixtureTestBase
        { }

        public class Ctor : RestExceptionTest
        {
            [Fact]
            public void Initializes()
            {
                var mockLocalizer = new Mock<ISharedResourcesLocalizer>();
                mockLocalizer
                    .Setup(x => x[It.IsAny<string>()])
                    .Returns(new LocalizedString("Error", "Error"));
                var error = Create<Error>();

                var exception = new RestException(
                    HttpMethod.Get,
                    new Uri("http://localhost"),
                    error,
                    mockLocalizer.Object);

                Assert.Equal(error.Code, exception.Code);
                Assert.Equal(error.Detail, exception.Detail);
                Assert.Equal(error.Summary, exception.Summary);
            }
        }
    }
}

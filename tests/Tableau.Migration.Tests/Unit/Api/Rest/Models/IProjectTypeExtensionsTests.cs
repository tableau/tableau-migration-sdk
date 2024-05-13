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
using Moq;
using Tableau.Migration.Api.Rest.Models;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models
{
    public class IProjectTypeExtensionsTests
    {
        public class GetControllingPermissionsProjectId
        {
            [Theory]
            [NullableGuidParseData]
            public void ParsesNullableGuid(string? s, Guid? expected)
            {
                var mockProject = new Mock<IProjectType>();
                mockProject.SetupGet(p => p.ControllingPermissionsProjectId).Returns(s);

                Assert.Equal(expected, mockProject.Object.GetControllingPermissionsProjectId());
            }
        }

        public class GetParentProjectId
        {
            [Theory]
            [NullableGuidParseData]
            public void ParsesNullableGuid(string? s, Guid? expected)
            {
                var mockProject = new Mock<IProjectType>();
                mockProject.SetupGet(p => p.ParentProjectId).Returns(s);

                Assert.Equal(expected, mockProject.Object.GetParentProjectId());
            }
        }
    }
}

//
//  Copyright (c) 2026, Salesforce, Inc.
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
using Tableau.Migration.Api;
using Tableau.Migration.Api.Rest.Models.Responses;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api
{
    public class WorkbookViewTests
    {
        public class Constructor : AutoFixtureTestBase
        {
            [Fact]
            public void MapsAllProperties()
            {
                // Arrange
                var viewResponse = Create<WorkbookViewsResponse.ViewType>();
                var expectedTags = CreateMany<WorkbookViewsResponse.ViewType.TagType>(3).ToArray();
                viewResponse.Tags = expectedTags;

                // Act
                var workbookView = new WorkbookView(viewResponse);

                // Assert
                Assert.Equal(viewResponse.Id, workbookView.Id);
                Assert.Equal(viewResponse.Name, workbookView.Name);
                Assert.Equal(viewResponse.ContentUrl, workbookView.ContentUrl);
                Assert.Equal(viewResponse.ViewUrlName, workbookView.ViewUrlName);
                Assert.Equal(viewResponse.CreatedAt, workbookView.CreatedAt);
                Assert.Equal(viewResponse.UpdatedAt, workbookView.UpdatedAt);

                Assert.Equal(expectedTags.Length, workbookView.Tags.Count);
                for (int i = 0; i < expectedTags.Length; i++)
                {
                    Assert.Equal(expectedTags[i].Label, workbookView.Tags[i].Label);
                }
            }

            [Theory]
            [InlineData(" ")]
            [InlineData("")]
            [InlineData(null)]
            public void HandlesInvalidName(string? name)
            {
                // Arrange
                var viewResponse = Create<WorkbookViewsResponse.ViewType>();
                viewResponse.Name = name;

                // Act & Assert
                var exception = Assert.Throws<ArgumentException>(() => new WorkbookView(viewResponse));
                Assert.Contains("Name", exception.Message);
            }
            
            [Fact]
            public void HandlesNullOptionalProperties()
            {
                // Arrange
                var viewResponse = Create<WorkbookViewsResponse.ViewType>();
                viewResponse.ContentUrl = null;
                viewResponse.ViewUrlName = null;
                viewResponse.CreatedAt = null;
                viewResponse.UpdatedAt = null;

                // Act
                var workbookView = new WorkbookView(viewResponse);

                // Assert
                Assert.Null(workbookView.ContentUrl);
                Assert.Null(workbookView.ViewUrlName);
                Assert.Null(workbookView.CreatedAt);
                Assert.Null(workbookView.UpdatedAt);
            }
        }
    }
} 
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
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content
{
    public class ContentBaseTests
    {
        public class EqualsTests : AutoFixtureTestBase
        {
            Guid BaseGuid;
            ContentLocation BaseContentLocation;

            public EqualsTests() : base()
            {
                BaseGuid = Guid.NewGuid();
                BaseContentLocation = Create<ContentLocation>();
            }

            static ContentBase CreateContentBase(Guid id, string name, string contentUrl, ContentLocation location)
            {
                var mockContentBase = new Mock<ContentBase>();
                mockContentBase.SetupGet(x => x.Id).Returns(id);
                mockContentBase.SetupGet(x => x.Name).Returns(name);
                mockContentBase.SetupGet(x => x.ContentUrl).Returns(contentUrl);
                mockContentBase.SetupGet(x => x.Location).Returns(location);

                return mockContentBase.Object;
            }

            [Fact]
            public void Equal()
            {
                var cb1 = EqualsTests.CreateContentBase(BaseGuid, "Name", "ContentUrl", BaseContentLocation);
                var cb2 = EqualsTests.CreateContentBase(BaseGuid, "Name", "ContentUrl", BaseContentLocation);

                Assert.NotNull(cb1);
                Assert.NotNull(cb2);

                // Verify value equals
                Assert.True(cb1.Equals(cb1));
                Assert.True(cb1.Equals(cb2));
                Assert.True(cb2.Equals(cb1));

                Assert.True(cb1 == cb2);
                Assert.True(cb2 == cb1);

                Assert.False(cb1 != cb2);
                Assert.False(cb2 != cb1);

                Assert.False(cb1 == null);
                Assert.False(null == cb1);

                Assert.True(cb1 != null);
                Assert.True(null != cb1);
            }

            [Fact]
            public void DifferentId()
            {
                var cb1 = EqualsTests.CreateContentBase(BaseGuid, "Name", "ContentUrl", BaseContentLocation);
                var cb2 = EqualsTests.CreateContentBase(Guid.NewGuid(), "Name", "ContentUrl", BaseContentLocation);

                Assert.NotNull(cb1);
                Assert.NotNull(cb2);

                // Verify value equals
                Assert.True(cb1.Equals(cb1));
                Assert.False(cb1.Equals(cb2));
                Assert.False(cb2.Equals(cb1));

                Assert.False(cb1 == cb2);
                Assert.False(cb2 == cb1);

                Assert.True(cb1 != cb2);
                Assert.True(cb2 != cb1);
            }

            [Fact]
            public void DifferentName()
            {
                var cb1 = EqualsTests.CreateContentBase(BaseGuid, "Name1", "ContentUrl", BaseContentLocation);
                var cb2 = EqualsTests.CreateContentBase(BaseGuid, "Name2", "ContentUrl", BaseContentLocation);

                Assert.NotNull(cb1);
                Assert.NotNull(cb2);

                // Verify value equals
                Assert.True(cb1.Equals(cb1));
                Assert.False(cb1.Equals(cb2));
                Assert.False(cb2.Equals(cb1));

                Assert.False(cb1 == cb2);
                Assert.False(cb2 == cb1);

                Assert.True(cb1 != cb2);
                Assert.True(cb2 != cb1);
            }

            [Fact]
            public void DifferentContentUrl()
            {
                var cb1 = EqualsTests.CreateContentBase(BaseGuid, "Name", "ContentUrl1", BaseContentLocation);
                var cb2 = EqualsTests.CreateContentBase(BaseGuid, "Name", "ContentUrl2", BaseContentLocation);

                Assert.NotNull(cb1);
                Assert.NotNull(cb2);

                // Verify value equals
                Assert.True(cb1.Equals(cb1));
                Assert.False(cb1.Equals(cb2));
                Assert.False(cb2.Equals(cb1));

                Assert.False(cb1 == cb2);
                Assert.False(cb2 == cb1);

                Assert.True(cb1 != cb2);
                Assert.True(cb2 != cb1);
            }

            [Fact]
            public void DifferentContentLocation()
            {
                var cb1 = EqualsTests.CreateContentBase(BaseGuid, "Name", "ContentUrl1", BaseContentLocation);
                var cb2 = EqualsTests.CreateContentBase(BaseGuid, "Name", "ContentUrl2", Create<ContentLocation>());

                Assert.NotNull(cb1);
                Assert.NotNull(cb2);

                // Verify value equals
                Assert.True(cb1.Equals(cb1));
                Assert.False(cb1.Equals(cb2));
                Assert.False(cb2.Equals(cb1));

                Assert.False(cb1 == cb2);
                Assert.False(cb2 == cb1);

                Assert.True(cb1 != cb2);
                Assert.True(cb2 != cb1);
            }
        }
    }
}
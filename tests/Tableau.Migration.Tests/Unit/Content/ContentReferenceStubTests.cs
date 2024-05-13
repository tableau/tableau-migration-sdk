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
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content
{
    public class ContentReferenceStubTests
    {
        public class Ctor : AutoFixtureTestBase
        {
            [Fact]
            public void CopiesAllValues()
            {
                var contentRef = Create<IContentReference>();

                var stub = new ContentReferenceStub(contentRef);

                Assert.Equal(contentRef.Id, stub.Id);
                Assert.Equal(contentRef.Location, stub.Location);
                Assert.Equal(contentRef.ContentUrl, stub.ContentUrl);
            }
        }

        public class EqualsTests : AutoFixtureTestBase
        {
            Guid BaseGuid;
            ContentLocation BaseContentLocation;

            public EqualsTests() : base()
            {
                BaseGuid = Guid.NewGuid();
                BaseContentLocation = Create<ContentLocation>();
            }

            [Fact]
            public void Equal()
            {
                var crs1 = new ContentReferenceStub(id: BaseGuid, contentUrl: "ContentUrl", location: BaseContentLocation, name: "Name");
                var crs2 = new ContentReferenceStub(id: BaseGuid, contentUrl: "ContentUrl", location: BaseContentLocation, name: "Name");

                Assert.NotNull(crs1);
                Assert.NotNull(crs2);

                // Verify value equals
                Assert.True(crs1.Equals(crs1));
                Assert.True(crs1.Equals(crs2));
                Assert.True(crs2.Equals(crs1));

                Assert.True(crs1 == crs2);
                Assert.True(crs2 == crs1);
            }

            [Fact]
            public void DifferentId()
            {
                var crs1 = new ContentReferenceStub(id: BaseGuid, contentUrl: "ContentUrl", location: BaseContentLocation, name: "Name");
                var crs2 = new ContentReferenceStub(id: Guid.NewGuid(), contentUrl: "ContentUrl", location: BaseContentLocation, name: "Name");

                Assert.NotNull(crs1);
                Assert.NotNull(crs2);

                // Verify value equals
                Assert.True(crs1.Equals(crs1));
                Assert.False(crs1.Equals(crs2));
                Assert.False(crs2.Equals(crs1));

                Assert.True(crs1 != crs2);
                Assert.True(crs2 != crs1);
            }

            [Fact]
            public void DifferentName()
            {
                var crs1 = new ContentReferenceStub(id: BaseGuid, contentUrl: "ContentUrl", location: BaseContentLocation, name: "Name1");
                var crs2 = new ContentReferenceStub(id: BaseGuid, contentUrl: "ContentUrl", location: BaseContentLocation, name: "Name2");

                Assert.NotNull(crs1);
                Assert.NotNull(crs2);

                // Verify value equals
                Assert.True(crs1.Equals(crs1));
                Assert.False(crs1.Equals(crs2));
                Assert.False(crs2.Equals(crs1));

                Assert.True(crs1 != crs2);
                Assert.True(crs2 != crs1);
            }

            [Fact]
            public void DifferentContentUrl()
            {
                var crs1 = new ContentReferenceStub(id: BaseGuid, contentUrl: "ContentUrl1", location: BaseContentLocation, name: "Name");
                var crs2 = new ContentReferenceStub(id: BaseGuid, contentUrl: "ContentUrl2", location: BaseContentLocation, name: "Name");

                Assert.NotNull(crs1);
                Assert.NotNull(crs2);

                // Verify value equals
                Assert.True(crs1.Equals(crs1));
                Assert.False(crs1.Equals(crs2));
                Assert.False(crs2.Equals(crs1));

                Assert.True(crs1 != crs2);
                Assert.True(crs2 != crs1);
            }

            [Fact]
            public void DifferentContentLocation()
            {
                var crs1 = new ContentReferenceStub(id: BaseGuid, contentUrl: "ContentUrl", location: BaseContentLocation, name: "Name");
                var crs2 = new ContentReferenceStub(id: BaseGuid, contentUrl: "ContentUrl", location: Create<ContentLocation>(), name: "Name");

                Assert.NotNull(crs1);
                Assert.NotNull(crs2);

                // Verify value equals
                Assert.True(crs1.Equals(crs1));
                Assert.False(crs1.Equals(crs2));
                Assert.False(crs2.Equals(crs1));

                Assert.True(crs1 != crs2);
                Assert.True(crs2 != crs1);
            }
        }
    }
}

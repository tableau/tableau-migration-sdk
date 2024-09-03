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
using Tableau.Migration.Api.Rest.Models.Requests;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models.Requests
{
    public class UpdateCustomViewRequestTests
    {
        public class Serialization : SerializationTestBase
        {
            [Fact]
            public void Serializes_DefaultCtor()
            {
                // Arrange
                var request = new UpdateCustomViewRequest();
                var expected = $@"
<tsRequest>
  <customView>
	  <owner />
  </customView>
</tsRequest>";

                // Act
                var serialized = Serializer.SerializeToXml(request);

                // Assert
                Assert.NotNull(serialized);
                AssertXmlEqual(expected, serialized);
            }

            [Fact]
            public void Serializes_NameOnly()
            {
                // Arrange
                var request = new UpdateCustomViewRequest();
                request.CustomView.Name = Guid.NewGuid().ToString();
                var expected = $@"
<tsRequest>
  <customView name=""{request.CustomView.Name}"" >
	  <owner />
  </customView>
</tsRequest>";

                // Act
                var serialized = Serializer.SerializeToXml(request);

                // Assert
                Assert.NotNull(serialized);
                AssertXmlEqual(expected, serialized);
            }

            [Fact]
            public void Serializes_OwnerIdOnly()
            {
                // Arrange
                var request = new UpdateCustomViewRequest();
                request.CustomView.Owner.Id = Guid.NewGuid();
                var expected = $@"
<tsRequest>
  <customView >
	  <owner id=""{request.CustomView.Owner.Id}"" />
  </customView>
</tsRequest>";

                // Act
                var serialized = Serializer.SerializeToXml(request);

                // Assert
                Assert.NotNull(serialized);
                AssertXmlEqual(expected, serialized);
            }

            [Fact]
            public void Serializes_NameAndOwnerId()
            {
                // Arrange
                var request = new UpdateCustomViewRequest();
                request.CustomView.Name = Guid.NewGuid().ToString();
                request.CustomView.Owner.Id = Guid.NewGuid();
                var expected = $@"
<tsRequest>
  <customView name=""{request.CustomView.Name}"">
	  <owner id=""{request.CustomView.Owner.Id}"" />
  </customView>
</tsRequest>";

                // Act
                var serialized = Serializer.SerializeToXml(request);

                // Assert
                Assert.NotNull(serialized);
                AssertXmlEqual(expected, serialized);
            }
        }

        public class Ctor : AutoFixtureTestBase
        {
            [Fact]
            public void Initializes()
            {
                var req = new UpdateCustomViewRequest();

                Assert.Null(req.CustomView.Name);
                Assert.False(req.CustomView.Owner.IdSpecified);
            }
            
            [Fact]
            public void InitializesWithNulls()
            {
                var req = new UpdateCustomViewRequest(null, null);

                Assert.Null(req.CustomView.Name);
                Assert.False(req.CustomView.Owner.IdSpecified);
            }
            
            [Fact]
            public void InitializesWithEmpty()
            {
                var req = new UpdateCustomViewRequest(Guid.Empty, string.Empty);

                Assert.Null(req.CustomView.Name);
                Assert.False(req.CustomView.Owner.IdSpecified);
            }
            
            [Fact]
            public void InitializesWithValues()
            {
                var ownerId = Guid.NewGuid();
                var viewName = Guid.NewGuid().ToString();
                var req = new UpdateCustomViewRequest(ownerId, viewName);

                Assert.Equal(viewName, req.CustomView.Name);
                Assert.True(req.CustomView.Owner.IdSpecified);
                Assert.Equal(ownerId, req.CustomView.Owner.Id);
            }
        }
    }
}

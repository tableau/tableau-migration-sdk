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
using Tableau.Migration.JsonConverters.SerializableObjects;
using Xunit;

namespace Tableau.Migration.Tests.Unit.JsonConverter.SerializableObjects
{
    public class TestSerializableContentReference : AutoFixtureTestBase
    {
        [Fact]
        public void AsContentReferenceStub()
        {
            var input = Create<SerializableContentReference>();

            var output = input.AsContentReferenceStub();

            Assert.NotNull(output);

            Assert.Equal(Guid.Parse(input.Id!), output.Id);
            Assert.Equal(input.ContentUrl, output.ContentUrl);
            Assert.Equal(input.Location!.AsContentLocation(), output.Location);
            Assert.Equal(input.Name, output.Name);
        }

        [Fact]
        public void BadDeserialization_NullId()
        {
            var input = Create<SerializableContentReference>();
            input.Id = null;

            Assert.Throws<ArgumentNullException>(() => input.AsContentReferenceStub());
        }

        [Fact]
        public void BadDeserialization_NullContentUrl()
        {
            var input = Create<SerializableContentReference>();
            input.ContentUrl = null;

            Assert.Throws<ArgumentNullException>(() => input.AsContentReferenceStub());
        }

        [Fact]
        public void BadDeserialization_NullLocation()
        {
            var input = Create<SerializableContentReference>();
            input.Location = null;

            Assert.Throws<ArgumentNullException>(() => input.AsContentReferenceStub());
        }

        [Fact]
        public void BadDeserialization_NullName()
        {
            var input = Create<SerializableContentReference>();
            input.Name = null;

            Assert.Throws<ArgumentNullException>(() => input.AsContentReferenceStub());
        }
    }
}

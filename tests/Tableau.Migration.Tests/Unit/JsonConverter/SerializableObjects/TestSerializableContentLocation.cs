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
using Tableau.Migration.JsonConverters.Exceptions;
using Tableau.Migration.JsonConverters.SerializableObjects;
using Xunit;

namespace Tableau.Migration.Tests.Unit.JsonConverter.SerializableObjects
{
    public class TestSerializableContentLocation : AutoFixtureTestBase
    {
        [Fact]
        public void AsContentLocation()
        {
            var input = Create<SerializableContentLocation>();

            var contentLocation = input.AsContentLocation();

            Assert.Equal(input.Path, contentLocation.Path);
            Assert.Equal(input.PathSegments, contentLocation.PathSegments);
            Assert.Equal(input.PathSeparator, contentLocation.PathSeparator);
            Assert.Equal(input.IsEmpty, contentLocation.IsEmpty);
            Assert.Equal(input.Name, contentLocation.Name);
        }

        [Fact]
        public void BadDeserialization_NullPath()
        {
            var input = Create<SerializableContentLocation>();
            input.Path = null;

            Assert.Throws<ArgumentNullException>(() => input.AsContentLocation());
        }

        [Fact]
        public void BadDeserialization_NullPathSegments()
        {
            var input = Create<SerializableContentLocation>();
            input.PathSegments = null;

            Assert.Throws<ArgumentNullException>(() => input.AsContentLocation());
        }

        [Fact]
        public void BadDeserialization_NullPathSeperator()
        {
            var input = Create<SerializableContentLocation>();
            input.PathSeparator = null;

            Assert.Throws<ArgumentNullException>(() => input.AsContentLocation());
        }

        [Fact]
        public void BadDeserialization_NullName()
        {
            var input = Create<SerializableContentLocation>();
            input.Name = null;

            Assert.Throws<ArgumentNullException>(() => input.AsContentLocation());
        }

        [Fact]
        public void BadDeserialization_PathDoesNotMatchSegments()
        {
            var input = Create<SerializableContentLocation>();
            input.Path = "Path";

            Assert.Throws<MismatchException>(() => input.AsContentLocation());
        }

        [Fact]
        public void BadDeserialization_NameDoesNotMatchSegments()
        {
            var input = Create<SerializableContentLocation>();
            input.Name = "Name";

            Assert.Throws<MismatchException>(() => input.AsContentLocation());
        }

        [Fact]
        public void BadDeserialization_IsEmptyDoesNotMatchSegments()
        {
            var input = Create<SerializableContentLocation>();
            input.IsEmpty = !input.IsEmpty;

            Assert.Throws<MismatchException>(() => input.AsContentLocation());
        }
    }
}

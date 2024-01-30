// Copyright (c) 2023, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the ""License"") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an ""AS IS"" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using Tableau.Migration.TestComponents.JsonConverters.Exceptions;
using Tableau.Migration.TestComponents.JsonConverters.JsonObjects;

namespace Tableau.Migration.TestComponents.Tests.JsonConverter.JsonObjects
{
    public class TestJsonContentLocation : AutoFixtureTestBase
    {
        [Fact]
        public void AsContentLocation()
        {
            var input = Create<JsonContentLocation>();

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
            var input = Create<JsonContentLocation>();
            input.Path = null;

            Assert.Throws<ArgumentNullException>(() => input.AsContentLocation());
        }

        [Fact]
        public void BadDeserialization_NullPathSegments()
        {
            var input = Create<JsonContentLocation>();
            input.PathSegments = null;

            Assert.Throws<ArgumentNullException>(() => input.AsContentLocation());
        }

        [Fact]
        public void BadDeserialization_NullPathSeperator()
        {
            var input = Create<JsonContentLocation>();
            input.PathSeparator = null;

            Assert.Throws<ArgumentNullException>(() => input.AsContentLocation());
        }

        [Fact]
        public void BadDeserialization_NullName()
        {
            var input = Create<JsonContentLocation>();
            input.Name = null;

            Assert.Throws<ArgumentNullException>(() => input.AsContentLocation());
        }

        [Fact]
        public void BadDeserialization_PathDoesNotMatchSegments()
        {
            var input = Create<JsonContentLocation>();
            input.Path = "Path";

            Assert.Throws<MismatchException>(() => input.AsContentLocation());
        }

        [Fact]
        public void BadDeserialization_NameDoesNotMatchSegments()
        {
            var input = Create<JsonContentLocation>();
            input.Name = "Name";

            Assert.Throws<MismatchException>(() => input.AsContentLocation());
        }

        [Fact]
        public void BadDeserialization_IsEmptyDoesNotMatchSegments()
        {
            var input = Create<JsonContentLocation>();
            input.IsEmpty = !input.IsEmpty;

            Assert.Throws<MismatchException>(() => input.AsContentLocation());
        }
    }
}

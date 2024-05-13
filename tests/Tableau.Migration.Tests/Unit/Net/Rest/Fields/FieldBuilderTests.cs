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

using System.Linq;
using Moq;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Rest.Fields;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net.Rest.Fields
{
    public class FieldBuilderTests
    {
        public abstract class FieldBuilderTest : AutoFixtureTestBase
        {
            internal readonly FieldBuilder Builder = new();
        }

        public class IsEmpty : FieldBuilderTest
        {
            [Fact]
            public void True()
            {
                Assert.True(Builder.IsEmpty);
            }

            [Fact]
            public void False()
            {
                var field = Create<Field>();

                Builder.AddField(field);

                Assert.False(Builder.IsEmpty);
            }
        }

        public class Build : FieldBuilderTest
        {
            [Fact]
            public void Builds_single_field()
            {
                var field = Create<Field>();

                Builder.AddField(field);

                Assert.Equal($"fields={field.Expression}", Builder.Build());
            }

            [Fact]
            public void Builds_multiple_fields()
            {
                var fields = CreateMany<Field>(2).ToList();

                Builder.AddFields(fields.ToArray());

                Assert.Equal($"fields={fields[0].Expression},{fields[1].Expression}", Builder.Build());
            }
        }

        public class AppendQueryString : FieldBuilderTest
        {
            protected readonly Mock<IQueryStringBuilder> MockQuery = new();

            [Fact]
            public void Skips_when_empty()
            {
                Assert.True(Builder.IsEmpty);

                Builder.AppendQueryString(MockQuery.Object);

                MockQuery.VerifyNoOtherCalls();
            }

            [Fact]
            public void Appends()
            {
                var field = Create<Field>();

                Builder.AddField(field);

                Assert.False(Builder.IsEmpty);

                Builder.AppendQueryString(MockQuery.Object);

                MockQuery.Verify(q => q.AddOrUpdate("fields", field.Expression), Times.Once);
            }
        }
    }
}

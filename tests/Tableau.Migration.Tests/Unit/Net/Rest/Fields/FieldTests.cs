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

using Tableau.Migration.Net.Rest.Fields;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net.Rest.Fields
{
    public class FieldTests
    {
        public abstract class FieldTest : AutoFixtureTestBase
        {
            protected static void AssertValue(Field field, string expectedName)
            {
                Assert.Equal(expectedName, field.Name);
            }
        }

        public class Ctor : FieldTest
        {
            [Fact]
            public void Sets_Name()
            {
                var name = Create<string>();

                var field = new Field(name);

                Assert.Equal(name, field.Name);
            }

            [Fact]
            public void Sets_Expression()
            {
                var name = Create<string>();

                var field = new Field(name);

                Assert.Equal(name, field.Expression);
            }
        }

        public class DefinedFields : FieldTest
        {
            [Fact]
            public void Returns_expected_value()
            {
                AssertValue(Field.All, "_all_");
                AssertValue(Field.Default, "_default_");
            }
        }
    }
}

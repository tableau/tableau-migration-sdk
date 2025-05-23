//
//  Copyright (c) 2025, Salesforce, Inc.
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

using Xunit;

namespace Tableau.Migration.Tests.Unit
{
    public class TypeExtensionsTests
    {
        public class GetAllPublicStringValues : TypeExtensionsTests
        {
            private class TestClass
            {
                public const string StringConstant1 = "value1";
                public const string StringConstant2 = "value2";
                public const int IntConstant = 42;
                public static string NonConstant = "non-constant";
                public string InstanceField = "instance";
            }

            private class EmptyClass { }

            [Fact]
            public void Returns_all_string_constants()
            {
                var values = typeof(TestClass).GetAllPublicStringValues();

                Assert.Equal(2, values.Count);
                Assert.Contains("value1", values);
                Assert.Contains("value2", values);
            }

            [Fact]
            public void Excludes_non_string_constants()
            {
                var values = typeof(TestClass).GetAllPublicStringValues();

                Assert.DoesNotContain("42", values);
            }

            [Fact]
            public void Excludes_non_constants()
            {
                var values = typeof(TestClass).GetAllPublicStringValues();

                Assert.DoesNotContain("non-constant", values);
                Assert.DoesNotContain("instance", values);
            }

            [Fact]
            public void Empty_class_returns_empty_list()
            {
                var values = typeof(EmptyClass).GetAllPublicStringValues();

                Assert.Empty(values);
            }
        }
    }
}
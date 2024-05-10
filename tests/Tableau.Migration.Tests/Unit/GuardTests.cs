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
using System.Collections;
using Xunit;

namespace Tableau.Migration.Tests.Unit
{
    public class GuardTests
    {
        #region - AgainstNull -

        public class AgainstNull
        {
            [Fact]
            public void NotNullValue()
            {
                Guard.AgainstNull(new object(), "param");
                Assert.Equal("value", Guard.AgainstNull("value", "param"));
            }

            [Fact]
            public void NullValue()
            {
                Assert.Throws<ArgumentNullException>(() => Guard.AgainstNull(null, "param"));
                Assert.Throws<ArgumentNullException>(() => Guard.AgainstNull((string?)null, "param"));
            }
        }

        #endregion

        #region - AgainstNullOrEmpty -

        public class AgainstNullOrEmpty
        {
            public class String_Overloads
            {
                [Fact]
                public void TestGuardAgainstNullOrEmpty()
                {
                    Assert.Equal("test", Guard.AgainstNullOrEmpty("test", "param"));
                }

                [Fact]
                public void TestGuardAgainstNullOrEmptyNull()
                {
                    Assert.Throws<ArgumentException>(() => Guard.AgainstNullOrEmpty(null, "param"));
                }

                [Fact]
                public void TestGuardAgainstNullOrEmptyEmpty()
                {
                    Assert.Throws<ArgumentException>(() => Guard.AgainstNullOrEmpty(String.Empty, "param"));
                }
            }

            public class IEnumerable_Overloads
            {
                [Fact]
                public void NotEmpty()
                {
                    var input = new int[] { 1 };
                    Assert.Same(input, Guard.AgainstNullOrEmpty(input, "param"));
                }

                [Fact]
                public void Null()
                {
                    Assert.Throws<ArgumentException>(() => Guard.AgainstNullOrEmpty((IEnumerable?)null, "param"));
                }

                [Fact]
                public void Empty()
                {
                    Assert.Throws<ArgumentException>(() => Guard.AgainstNullOrEmpty(Array.Empty<int>(), "param"));
                }
            }
        }

        #endregion

        #region - AgainstNullOrWhiteSpace -

        public class AgainstNullOrWhiteSpace
        {
            [Fact]
            public void NotNullOrWhiteSpace()
            {
                Assert.Equal("test", Guard.AgainstNullOrWhiteSpace("test", "param"));
            }

            [Theory]
            [InlineData(null)]
            [InlineData(" ")]
            public void NullOrWhiteSpace(string? value)
            {
                Assert.Throws<ArgumentException>(() => Guard.AgainstNullOrWhiteSpace(value, "param"));
            }
        }

        #endregion

        #region - AgainstNullEmptyOrWhiteSpace -

        public class AgainstNullEmptyOrWhiteSpace
        {
            [Fact]
            public void NotNullEmptyOrWhiteSpace()
            {
                Assert.Equal("test", Guard.AgainstNullEmptyOrWhiteSpace("test", "param"));
            }

            [Theory]
            [NullEmptyWhiteSpaceData]
            public void NullEmptyOrWhiteSpace(string? value)
            {
                Assert.Throws<ArgumentException>(() => Guard.AgainstNullEmptyOrWhiteSpace(value, "param"));
            }
        }

        #endregion

        #region - AgainstDefaultValue -

        public class AgainstDefaultValue
        {
            [Fact]
            public void NonDefaultValue()
            {
                Assert.Equal(10, Guard.AgainstDefaultValue(10, "param"));
            }

            [Fact]
            public void DefaultValue()
            {
                Assert.Throws<ArgumentException>(() => Guard.AgainstDefaultValue(0, "param"));
            }
        }

        #endregion
    }
}

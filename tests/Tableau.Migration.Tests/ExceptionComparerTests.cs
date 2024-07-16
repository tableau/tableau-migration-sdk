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
using Xunit;
using Tableau.Migration;

namespace Tableau.Migration.Tests.Unit
{
    public class ExceptionComparerTests
    {
        // Custom exception class for testing IEquatable<T>
        private class EquatableException : Exception, IEquatable<EquatableException>
        {
            public EquatableException(string message) : base(message)
            { }

            public bool Equals(EquatableException? other)
            {
                return other != null && Message == other.Message;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        private readonly ExceptionComparer _comparer = new ExceptionComparer();

        [Fact]
        public void TestEquals_BothNull_ReturnsTrue()
        {
            Assert.True(_comparer.Equals(null, null));
        }

        [Fact]
        public void TestEquals_OneNull_ReturnsFalse()
        {
            var ex = new Exception();
            Assert.False(_comparer.Equals(null, ex));
            Assert.False(_comparer.Equals(ex, null));
        }

        [Fact]
        public void TestEquals_DifferentTypes_ReturnsFalse()
        {
            var ex1 = new Exception();
            var ex2 = new InvalidOperationException();
            Assert.False(_comparer.Equals(ex1, ex2));
        }

        [Fact]
        public void TestEquals_SameTypeDifferentMessages_ReturnsFalse()
        {
            var ex1 = new Exception("Message 1");
            var ex2 = new Exception("Message 2");
            Assert.False(_comparer.Equals(ex1, ex2));
        }

        [Fact]
        public void TestEquals_SameTypeSameMessage_ReturnsTrue()
        {
            var ex1 = new Exception("Message");
            var ex2 = new Exception("Message");
            Assert.True(_comparer.Equals(ex1, ex2));
        }

        [Fact]
        public void TestEquals_ImplementsIEquatable_ReturnsTrue()
        {
            var ex1 = new EquatableException("Message");
            var ex2 = new EquatableException("Message");
            Assert.True(_comparer.Equals(ex1, ex2));
        }

        [Fact]
        public void TestEquals_ExceptionVsEquatableException_ReturnsFalse()
        {
            var standardException = new Exception("Message");
            var equatableException = new EquatableException("Message");
            Assert.False(_comparer.Equals(standardException, equatableException));
        }


        [Fact]
        public void TestGetHashCode_DifferentMessages_DifferentHashCodes()
        {
            var ex1 = new Exception("Message 1");
            var ex2 = new Exception("Message 2");
            Assert.NotEqual(_comparer.GetHashCode(ex1), _comparer.GetHashCode(ex2));
        }

        [Fact]
        public void TestGetHashCode_SameMessage_SameHashCode()
        {
            var ex1 = new Exception("Message");
            var ex2 = new Exception("Message");
            Assert.Equal(_comparer.GetHashCode(ex1), _comparer.GetHashCode(ex2));
        }

        [Fact]
        public void TestGetHashCode_ImplementsIEquatable_ConsistentHashCode()
        {
            var ex = new EquatableException("Message");
            Assert.Equal(ex.GetHashCode(), _comparer.GetHashCode(ex));
        }
    }
}

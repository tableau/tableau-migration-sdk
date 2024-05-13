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
using System.Threading.Tasks;
using Xunit;

namespace Tableau.Migration.Tests.Unit
{
    public class ExceptionExtensionsTests
    {
        public class IsCancellationException
        {
            [Fact]
            public void OperationCanceledException()
            {
                Assert.True(new OperationCanceledException().IsCancellationException());
            }

            [Fact]
            public void TaskCanceledException()
            {
                Assert.True(new TaskCanceledException().IsCancellationException());
            }

            [Fact]
            public void WrappedTaskCanceledException()
            {
                Assert.True(new AggregateException(new TaskCanceledException()).IsCancellationException());
            }

            [Fact]
            public void MixedAggregateException()
            {
                Assert.False(new AggregateException(new TaskCanceledException(), new Exception()).IsCancellationException());
            }

            [Fact]
            public void NonCanceledException()
            {
                Assert.False(new Exception().IsCancellationException());
            }
        }
    }
}

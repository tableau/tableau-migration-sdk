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
    public class TaskExtensionsTests
    {
        #region - AwaitResult -

        public class AwaitResult
        {
            private static async Task<int> AsyncMethodAsync(bool throwException = false)
            {
                var i = await Task.Run(() => 47);

                if (throwException)
                {
                    throw new Exception("break");
                }

                return i;
            }

            [Fact]
            public void AwaitsResult()
            {
                var i = AwaitResult.AsyncMethodAsync().AwaitResult();

                Assert.Equal(47, i);
            }

            [Fact]
            public void FaultedTaskExceptionPropagated()
            {
                Exception? ex = null;
                try
                {
                    var i = AwaitResult.AsyncMethodAsync(throwException: true).AwaitResult();
                }
                catch (Exception e)
                {
                    ex = e;
                }

                Assert.NotNull(ex);
                Assert.IsNotType<AggregateException>(ex);
            }
        }

        #endregion
    }
}

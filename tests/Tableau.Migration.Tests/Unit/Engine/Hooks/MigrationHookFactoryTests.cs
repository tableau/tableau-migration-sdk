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
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Engine.Hooks;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks
{
    public class MigrationHookFactoryTests
    {
        #region - Test Types -

        private interface ITestHook : IMigrationHook<int> { }

        private interface IDifferentHook : IMigrationHook<Guid> { }

        private class TestHook : ITestHook
        {
            public Task<int> ExecuteAsync(int ctx, CancellationToken cancel)
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region - Create -

        public class Create
        {
            [Fact]
            public void CreatesFromFactory()
            {
                var h = new TestHook();
                var f = new MigrationHookFactory(s => h);

                var result = f.Create<ITestHook>(new Mock<IServiceProvider>().Object);

                Assert.Same(h, result);
            }

            [Fact]
            public void ThrowsOnInvalidHookType()
            {
                var h = new TestHook();
                var f = new MigrationHookFactory(s => h);

                Assert.Throws<InvalidCastException>(() => f.Create<IDifferentHook>(new Mock<IServiceProvider>().Object));
            }
        }

        #endregion
    }
}

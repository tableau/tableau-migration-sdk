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

using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Engine.Migrators.Batch;
using Tableau.Migration.Interop.Hooks;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Interop.Hooks
{
    public class ISyncContentBatchMigrationCompletedHookTests
    {
        public class ExecuteAsync : AutoFixtureTestBase
        {
            public class TestImplementation : ISyncContentBatchMigrationCompletedHook<IUser>
            {
                public virtual IContentBatchMigrationResult<IUser>? Execute(IContentBatchMigrationResult<IUser> ctx) => ctx;
            }

            [Fact]
            public async Task CallsExecuteAsync()
            {
                var mockTransformer = new Mock<TestImplementation>()
                {
                    CallBase = true
                };

                var ctx = Create<IContentBatchMigrationResult<IUser>>();

                var result = await ((IMigrationHook<IContentBatchMigrationResult<IUser>>)mockTransformer.Object).ExecuteAsync(ctx, Cancel);

                Assert.Same(ctx, result);

                mockTransformer.Verify(x => x.Execute(ctx), Times.Once);
            }
        }
    }
}

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
using System.Threading.Tasks;
using Tableau.Migration.Engine.Hooks.PostPublish;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.PostPublish
{
    public class BulkPostPublishContextTests
    {
        public abstract class BulkPostPublishContextTest : AutoFixtureTestBase
        { }

        public class Ctor : BulkPostPublishContextTest
        {
            [Fact]
            public void Initializes()
            {
                var sourceItems = CreateMany<TestContentType>(5);

                var ctx = new BulkPostPublishContext<TestContentType>(sourceItems);

                Assert.True(sourceItems.SequenceEqual(ctx.PublishedItems));
            }
        }

        public class ToTask : BulkPostPublishContextTest
        {
            [Fact]
            public async Task CreatesCompletedTask()
            {
                var sourceItems = CreateMany<TestContentType>(5);

                var ctx = new BulkPostPublishContext<TestContentType>(sourceItems);

                var task = ctx.ToTask();

                var result = await task;

                Assert.True(task.IsCompleted);
                Assert.Same(ctx, result);
            }
        }
    }
}

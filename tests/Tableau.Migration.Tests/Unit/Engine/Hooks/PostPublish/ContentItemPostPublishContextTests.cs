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
using Tableau.Migration.Engine.Hooks.PostPublish;
using Tableau.Migration.Engine.Manifest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.PostPublish
{
    public class ContentItemPostPublishContextTests
    {
        public abstract class ContentItemPostPublishContextTest : AutoFixtureTestBase
        { }

        public class SameSourceAndDestinationType
        {
            public class Ctor : ContentItemPostPublishContextTest
            {
                [Fact]
                public void Initializes()
                {
                    var manifestEntry = Create<IMigrationManifestEntryEditor>();
                    var publish = Create<TestPublishType>();
                    var result = Create<TestContentType>();

                    var ctx = new ContentItemPostPublishContext<TestPublishType, TestContentType>(manifestEntry, publish, result);

                    Assert.Same(manifestEntry, ctx.ManifestEntry);
                    Assert.Same(publish, ctx.PublishedItem);
                    Assert.Same(result, ctx.DestinationItem);
                }
            }

            public class ToTask : ContentItemPostPublishContextTest
            {
                [Fact]
                public async Task CreatesCompletedTask()
                {
                    var ctx = Create<ContentItemPostPublishContext<TestPublishType, TestContentType>>();

                    var task = ctx.ToTask();

                    var result = await task;

                    Assert.True(task.IsCompleted);
                    Assert.Same(ctx, result);
                }
            }
        }

        public class DifferentSourceAndDestinationTypes
        {
            private class SourceContentType : TestContentType
            { }

            private class DestinationContentType : TestContentType
            { }

            public class Ctor : ContentItemPostPublishContextTest
            {
                [Fact]
                public void Initializes()
                {
                    var manifestEntry = Create<IMigrationManifestEntryEditor>();
                    var source = Create<SourceContentType>();
                    var destination = Create<DestinationContentType>();

                    var ctx = new ContentItemPostPublishContext<SourceContentType, DestinationContentType>(manifestEntry, source, destination);

                    Assert.Same(manifestEntry, ctx.ManifestEntry);
                    Assert.Same(source, ctx.PublishedItem);
                    Assert.Same(destination, ctx.DestinationItem);
                }
            }

            public class ToTask : ContentItemPostPublishContextTest
            {
                [Fact]
                public async Task CreatesCompletedTask()
                {
                    var ctx = Create<ContentItemPostPublishContext<SourceContentType, DestinationContentType>>();

                    var task = ctx.ToTask();

                    var result = await task;

                    Assert.True(task.IsCompleted);
                    Assert.Same(ctx, result);
                }
            }
        }
    }
}

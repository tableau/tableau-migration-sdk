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

using System;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Content.Files;
using Tableau.Migration.Engine.Hooks.Transformers;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Transformers
{
    public class IJsonContentTransformerTests
    {
        public class ExecuteAsync : AutoFixtureTestBase
        {
            public class TestImplementation : IJsonContentTransformer<TestFileContentType>
            {
                public virtual Task TransformAsync(TestFileContentType ctx, JsonNode json, CancellationToken cancel)
                {
                    return Task.CompletedTask;
                }

                public Func<TestFileContentType, bool> NeedsJsonTransformingFilter = _ => true;

                public bool NeedsJsonTransforming(TestFileContentType ctx) => NeedsJsonTransformingFilter(ctx);
            }

            [Fact]
            public async Task FiltersItemsAsync()
            {
                var mockFile = Freeze<Mock<IContentFileHandle>>();
                var ctx = Create<TestFileContentType>();

                var transformer = new TestImplementation();
                transformer.NeedsJsonTransformingFilter = _ => false;

                var result = await ((IJsonContentTransformer<TestFileContentType>)transformer).ExecuteAsync(ctx, Cancel);

                Assert.Same(ctx, result);
                mockFile.Verify(x => x.GetJsonStreamAsync(Cancel), Times.Never);
            }

            [Fact]
            public async Task GetsOrReadsJsonAsync()
            {
                var mockFile = Freeze<Mock<IContentFileHandle>>();
                var ctx = Create<TestFileContentType>();

                var mockJsonStream = Freeze<Mock<ITableauFileJsonStream>>();
                var json = JsonNode.Parse("{\"test\": \"value\"}");

                mockJsonStream.Setup(x => x.GetJsonAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(json!);

                var mockTransformer = new Mock<TestImplementation> { CallBase = true };

                var result = await ((IJsonContentTransformer<TestFileContentType>)mockTransformer.Object).ExecuteAsync(ctx, Cancel);

                Assert.Same(ctx, result);

                mockFile.Verify(x => x.GetJsonStreamAsync(Cancel), Times.Once);
                mockJsonStream.Verify(x => x.GetJsonAsync(Cancel), Times.Once);

                mockTransformer.Verify(x => x.TransformAsync(ctx, It.IsAny<JsonNode>(), Cancel), Times.Once);
            }
        }
    }
}


//
//  Copyright (c) 2026, Salesforce, Inc.
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
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Hooks.Transformers.Default;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Transformers.Default
{
    public class WorkbookReferenceTransformerTests
    {
        public abstract class WorkbookReferenceTransformerTest : AutoFixtureTestBase
        {
            protected readonly Mock<IDestinationContentReferenceFinderFactory> MockDestinationFinderFactory = new();
            protected readonly Mock<ILogger<WorkbookReferenceTransformer<IWithWorkbook>>> MockLogger;
            protected readonly MockSharedResourcesLocalizer MockSharedResourcesLocalizer = new();
            protected readonly Mock<IDestinationContentReferenceFinder<IWorkbook>> MockWorkbookContentFinder = new();

            protected readonly WorkbookReferenceTransformer<IWithWorkbook> Transformer;

            public WorkbookReferenceTransformerTest()
            {
                MockLogger = Create<Mock<ILogger<WorkbookReferenceTransformer<IWithWorkbook>>>>();

                MockDestinationFinderFactory
                    .Setup(p => p.ForDestinationContentType<IWorkbook>())
                    .Returns(MockWorkbookContentFinder.Object);

                Transformer = new(
                    MockDestinationFinderFactory.Object,
                    MockLogger.Object,
                    MockSharedResourcesLocalizer.Object);
            }
        }

        public class ExecuteAsync : WorkbookReferenceTransformerTest
        {
            [Fact]
            public async Task ThrowsOnMappedWorkbookNotFoundAsync()
            {
                var sourceContentType = Create<IWithWorkbook>();
                await Assert.ThrowsAsync<Exception>(() => Transformer.TransformAsync(sourceContentType, Cancel));
            }

            [Fact]
            public async Task ReturnsDestinationWorkbookWhenFoundAsync()
            {
                var sourceContentType = Create<IWithWorkbook>();
                var destinationWorkbook = Create<IWorkbook>();

                MockWorkbookContentFinder.Setup(
                    f => f.FindBySourceLocationAsync(sourceContentType.Workbook.Location, Cancel))
                    .ReturnsAsync(destinationWorkbook);

                var result = await Transformer.TransformAsync(sourceContentType, Cancel);

                Assert.NotNull(result);
                Assert.Same(destinationWorkbook, result.Workbook);
                Assert.Equal(destinationWorkbook.Id, result.Workbook.Id);
                MockLogger.VerifyDebug(Times.Never());
            }
        }
    }
}

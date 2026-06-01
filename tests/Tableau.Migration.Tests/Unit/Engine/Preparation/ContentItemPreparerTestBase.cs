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
using System.Threading;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Files;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Conversion;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Engine.Hooks.Pulled;
using Tableau.Migration.Engine.Hooks.Transformers;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Engine.Pipelines;

namespace Tableau.Migration.Tests.Unit.Engine.Preparation
{
    public class ContentItemPreparerTestBase<TContent, TPrepare, TPublish> : AutoFixtureTestBase
        where TContent : class, IContentReference
        where TPrepare : class, IContentReference
        where TPublish : class
    {
        protected readonly Mock<IMigrationPipeline> MockPipeline;
        protected readonly Mock<IMigrationHookRunner> MockHookRunner;
        protected readonly Mock<IContentTransformerRunner> MockTransformerRunner;
        protected readonly Mock<IMigrationManifestEntryEditor> MockManifestEntry;
        protected readonly Mock<IDestinationContentReferenceFinder<IProject>> MockProjectFinder;
        protected readonly Mock<IContentFileStore> MockFileStore;
        protected readonly ContentMigrationItem<TContent> Item;

        protected ContentLocation MappedLocation { get; set; }

        public ContentItemPreparerTestBase()
        {
            MockPipeline = Freeze<Mock<IMigrationPipeline>>();
            MockPipeline.Setup(x => x.GetItemConverter<It.IsAnyType, It.IsAnyType>())
                .Returns(new InvocationFunc(invocation =>
                {
                    var genericArgs = invocation.Method.GetGenericArguments();
                    return Activator.CreateInstance(typeof(DirectContentItemConverter<,>).MakeGenericType(genericArgs))!;
                }));

            MockHookRunner = Freeze<Mock<IMigrationHookRunner>>();
            MockHookRunner.Setup(x => x.ExecuteAsync<IContentItemPulledHook<TPrepare>, ContentItemPulledContext<TPrepare>>(
                It.IsAny<ContentItemPulledContext<TPrepare>>(),
                It.IsAny<Action<string, ContentItemPulledContext<TPrepare>, ContentItemPulledContext<TPrepare>>>(),
                It.IsAny<CancellationToken>())
                )
                .ReturnsAsync((
                    ContentItemPulledContext<TPrepare> ctx, 
                    Action<string, ContentItemPulledContext<TPrepare>, ContentItemPulledContext<TPrepare>> afterAction, 
                    CancellationToken c) =>
                {
                    afterAction?.Invoke("test hook", ctx, ctx);
                    return ctx;
                });

            MockTransformerRunner = Freeze<Mock<IContentTransformerRunner>>();
            MockTransformerRunner.Setup(x => x.ExecuteAsync(It.IsAny<TPublish>(), Cancel))
                .ReturnsAsync((TPublish item, CancellationToken cancel) => item);

            MappedLocation = Create<ContentLocation>();

            MockManifestEntry = Freeze<Mock<IMigrationManifestEntryEditor>>();
            MockManifestEntry.SetupGet(x => x.MappedLocation).Returns(() => MappedLocation);

            MockProjectFinder = Freeze<Mock<IDestinationContentReferenceFinder<IProject>>>();
            MockProjectFinder.Setup(x => x.FindBySourceLocationAsync(It.IsAny<ContentLocation>(), Cancel))
                    .ReturnsAsync((IContentReference?)null);

            var mockDestinationFinderFactory = Freeze<Mock<IDestinationContentReferenceFinderFactory>>();
            mockDestinationFinderFactory.Setup(x => x.ForDestinationContentType<IProject>())
                .Returns(MockProjectFinder.Object);

            MockFileStore = Freeze<Mock<IContentFileStore>>();

            Item = Create<ContentMigrationItem<TContent>>();
        }
    }

    public class ContentItemPreparerTestBase<TPrepare, TPublish> : ContentItemPreparerTestBase<TPrepare, TPrepare, TPublish>
        where TPrepare : class, IContentReference
        where TPublish : class
    { }

    public class ContentItemPreparerTestBase<TPrepare> : ContentItemPreparerTestBase<TPrepare, TPrepare, TPrepare>
        where TPrepare : class, IContentReference
    { }
}

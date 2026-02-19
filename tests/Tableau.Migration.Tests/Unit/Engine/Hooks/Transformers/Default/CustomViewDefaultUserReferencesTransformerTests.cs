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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Hooks.Transformers.Default;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Transformers.Default
{
    public class CustomViewDefaultUserReferencesTransformerTests
    {
        public abstract class CustomViewDefaultUserReferencesTransformerTest : AutoFixtureTestBase
        {
            protected readonly Mock<IDestinationContentReferenceFinder<IUser>> MockUserFinder = new();
            protected readonly Mock<ILogger<CustomViewDefaultUserReferencesTransformer>> MockLogger;
            protected readonly MockSharedResourcesLocalizer MockSharedResourcesLocalizer = new();
            protected readonly CustomViewDefaultUserReferencesTransformer Transformer;

            public CustomViewDefaultUserReferencesTransformerTest()
            {
                MockLogger = Create<Mock<ILogger<CustomViewDefaultUserReferencesTransformer>>>();

                MockUserFinder
                    .Setup(p => p.FindBySourceLocationAsync(It.IsAny<ContentLocation>(), Cancel))
                    .Returns(Task.FromResult((IContentReference?)Create<IContentReference>()));

                var mockFinderFactory = new Mock<IDestinationContentReferenceFinderFactory>();
                mockFinderFactory.Setup(x => x.ForDestinationContentType<IUser>()).Returns(MockUserFinder.Object);

                Transformer = new(mockFinderFactory.Object, MockSharedResourcesLocalizer.Object, MockLogger.Object);
            }
        }

        public class ExecuteAsync : CustomViewDefaultUserReferencesTransformerTest
        {
            [Fact]
            public async Task ThrowsWhenUsersNotFoundAsync()
            {
                var sourceCustomView = Create<IPublishableCustomView>();

                var defaultUsers = new List<IContentReference>();
                foreach (var item in sourceCustomView.DefaultUsers)
                {
                    defaultUsers.Add(item);
                }

                MockUserFinder
                   .Setup(p => p.FindBySourceLocationAsync(It.IsAny<ContentLocation>(), Cancel))
                   .Returns(Task.FromResult((IContentReference?)null));

                await Assert.ThrowsAsync<Exception>(() => Transformer.TransformAsync(sourceCustomView, Cancel));
            }

            [Fact]
            public async Task UpdatesDestinationUsersWhenFoundAsync()
            {
                var sourceCustomView = Create<IPublishableCustomView>();

                var sourceUsers = new List<IContentReference>();
                foreach (var item in sourceCustomView.DefaultUsers)
                {
                    sourceUsers.Add(item);
                }

                var result = await Transformer.TransformAsync(sourceCustomView, Cancel);

                Assert.NotNull(result);
                Assert.NotEmpty(result.DefaultUsers);
                Assert.Equal(sourceUsers.Count, sourceCustomView.DefaultUsers.Count);
                Assert.NotEqual(sourceUsers, result.DefaultUsers);

                MockLogger.VerifyDebug(Times.Never());
            }

            [Fact]
            public async Task ThrowsWhenUsersPartiallyFoundAsync()
            {
                var sourceCustomView = Create<IPublishableCustomView>();

                var sourceUsers = new List<IContentReference>();
                foreach (var item in sourceCustomView.DefaultUsers)
                {
                    sourceUsers.Add(item);
                }

                var userNotOnDestination = sourceCustomView.DefaultUsers.First();
                MockUserFinder
                   .Setup(p => p.FindBySourceLocationAsync(userNotOnDestination.Location, Cancel))
                   .Returns(Task.FromResult((IContentReference?)null));

                await Assert.ThrowsAsync<Exception>(() => Transformer.TransformAsync(sourceCustomView, Cancel));
            }
        }
    }
}

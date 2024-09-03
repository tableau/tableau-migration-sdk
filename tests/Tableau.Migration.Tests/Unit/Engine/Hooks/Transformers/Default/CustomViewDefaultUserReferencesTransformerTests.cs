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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks.Transformers.Default;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Transformers.Default
{
    public class CustomViewDefaultUserReferencesTransformerTests
    {
        public abstract class CustomViewDefaultUserReferencesTransformerTest : AutoFixtureTestBase
        {
            protected readonly Mock<IMappedUserTransformer> MockUserTransformer = new();
            protected readonly Mock<ILogger<CustomViewDefaultUserReferencesTransformer>> MockLogger = new();
            protected readonly MockSharedResourcesLocalizer MockSharedResourcesLocalizer = new();
            protected readonly CustomViewDefaultUserReferencesTransformer Transformer;

            public CustomViewDefaultUserReferencesTransformerTest()
            {
                MockUserTransformer
                    .Setup(p => p.ExecuteAsync(It.IsAny<IContentReference>(), Cancel))
                    .Returns(Task.FromResult((IContentReference?)Create<IContentReference>()));

                Transformer = new(
                    MockUserTransformer.Object,
                    MockLogger.Object,
                    MockSharedResourcesLocalizer.Object);
            }
        }

        public class ExecuteAsync : CustomViewDefaultUserReferencesTransformerTest
        {
            [Fact]
            public async Task Returns_same_when_users_not_found()
            {
                var sourceCustomView = Create<IPublishableCustomView>();

                var defaultUsers = new List<IContentReference>();
                foreach (var item in sourceCustomView.DefaultUsers)
                {
                    defaultUsers.Add(item);
                };

                MockUserTransformer
                   .Setup(p => p.ExecuteAsync(It.IsAny<IContentReference>(), Cancel))
                   .Returns(Task.FromResult((IContentReference?)null));

                var result = await Transformer.TransformAsync(sourceCustomView, Cancel);

                Assert.NotNull(result);
                
                MockLogger.VerifyDebug(Times.Once());

                Assert.NotEmpty(result.DefaultUsers);
                Assert.Equal(defaultUsers, result.DefaultUsers);
            }

            [Fact]
            public async Task Updates_destination_users_when_found()
            {
                var sourceCustomView = Create<IPublishableCustomView>();

                var sourceUsers = new List<IContentReference>();
                foreach (var item in sourceCustomView.DefaultUsers)
                {
                    sourceUsers.Add(item);
                };

                var result = await Transformer.TransformAsync(sourceCustomView, Cancel);

                Assert.NotNull(result);
                Assert.NotEmpty(result.DefaultUsers);
                Assert.Equal(sourceUsers.Count, sourceCustomView.DefaultUsers.Count);
                Assert.NotEqual(sourceUsers, result.DefaultUsers);

                MockLogger.VerifyDebug(Times.Never());
            }

            [Fact]
            public async Task Updates_to_destination_users_when_partially_found()
            {
                var sourceCustomView = Create<IPublishableCustomView>();

                var sourceUsers = new List<IContentReference>();
                foreach (var item in sourceCustomView.DefaultUsers)
                {
                    sourceUsers.Add(item);
                };

                var userNotOnDestination = sourceCustomView.DefaultUsers.First();
                MockUserTransformer
                   .Setup(p => p.ExecuteAsync(userNotOnDestination, Cancel))
                   .Returns(Task.FromResult((IContentReference?)null));

                var result = await Transformer.TransformAsync(sourceCustomView, Cancel);

                Assert.NotNull(result);
                Assert.NotEmpty(result.DefaultUsers);
                Assert.Equal(sourceUsers.Count, sourceCustomView.DefaultUsers.Count);
                Assert.NotEqual(sourceUsers, result.DefaultUsers);
                Assert.Same(userNotOnDestination, result.DefaultUsers.First());
                
                MockLogger.VerifyDebug(Times.Once());
            }
        }
    }
}


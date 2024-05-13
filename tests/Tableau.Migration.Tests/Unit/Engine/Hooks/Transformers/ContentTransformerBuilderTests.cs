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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Engine.Hooks.Transformers;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Transformers
{
    public class ContentTransformerBuilderTests
    {
        public class TestUserTransformer : ContentTransformerBase<IUser>
        {
            public TestUserTransformer(
                ISharedResourcesLocalizer localizer,
                ILogger<TestUserTransformer> logger) 
                    : base(localizer, logger) { }

            public override Task<IUser?> TransformAsync(IUser itemToTransform, CancellationToken cancel)
            {
                return Task.FromResult<IUser?>(itemToTransform);
            }
        }

        public class GenericTransformer<TContent>
            : ContentTransformerBase<TContent>
            where TContent : IContentReference
        {
            public GenericTransformer(
                ISharedResourcesLocalizer localizer, 
                ILogger<GenericTransformer<TContent>> logger) : base(localizer, logger) { }

            public override Task<TContent?> TransformAsync(TContent itemToTransform, CancellationToken cancel)
            {
                return Task.FromResult<TContent?>(itemToTransform);
            }
        }

        public class Clear
        {
            [Fact]
            public void ClearsFactories()
            {
                var builder = new ContentTransformerBuilder().Add<TestUserTransformer, IUser>()
                    .Clear();

                var result = builder.Build();
                Assert.Empty(result.GetHooks<IContentTransformer<IUser>>());
            }
        }

        public class Add
        {
            [Fact]
            public void AddFromType()
            {
                var builder = new ContentTransformerBuilder().Add<TestUserTransformer, IUser>();

                var result = builder.Build();
                Assert.NotNull(result);

                var transformers = result.GetHooks<IContentTransformer<IUser>>();
                Assert.Single(transformers);
            }
        }

        public class AddMappingGenericInstance
        {
            [Fact]
            public void AddFromType()
            {
                var mockLocalizer = new MockSharedResourcesLocalizer();
                var mockLogger = new Mock<ILogger<GenericTransformer<IUser>>>();

                var genericTransformer = new GenericTransformer<IUser>(mockLocalizer.Object, mockLogger.Object);
                var builder = new ContentTransformerBuilder().Add(genericTransformer);

                var result = builder.Build();
                Assert.NotNull(result);

                var transformers = result.GetHooks<IContentTransformer<IUser>>();
                Assert.Single(transformers);
            }
        }

        public class AddGenericFactory
        {
            [Fact]
            public void AddFromType()
            {
                var builder = new ContentTransformerBuilder().Add<GenericTransformer<IUser>, IUser>();

                var result = builder.Build();
                Assert.NotNull(result);

                var transformers = result.GetHooks<IContentTransformer<IUser>>();
                Assert.Single(transformers);
            }
        }

        public class AddGenericFactoryAsInterface
        {
            [Fact]
            public void AddFromType()
            {
                var builder = new ContentTransformerBuilder().Add<IContentTransformer<IUser>, IUser>();

                var result = builder.Build();
                Assert.NotNull(result);

                var transformers = result.GetHooks<IContentTransformer<IUser>>();
                Assert.Single(transformers);
            }
        }

        public class AddConstructed
        {
            [Fact]
            public void AddFromType()
            {
                var builder = new ContentTransformerBuilder().Add(typeof(IContentTransformer<>), new[] { new[] { typeof(IUser) }, new[] { typeof(IProject) } });

                var result = builder.Build();
                Assert.NotNull(result);

                var transformers = result.GetHooks<IContentTransformer<IUser>>();
                Assert.Single(transformers);

                transformers = result.GetHooks<IContentTransformer<IProject>>();
                Assert.Single(transformers);
            }
        }

        public class AddCallback
        {
            [Fact]
            public void AddFromType()
            {
                var builder = new ContentTransformerBuilder().Add(
                    (IUser context,
                    CancellationToken cancel)
                    => Task.FromResult(null as IUser));

                var result = builder.Build();
                Assert.NotNull(result);

                var transformers = result.GetHooks<IContentTransformer<IUser>>();
                Assert.Single(transformers);
            }
        }

        public class AddRawFilterInterface : AutoFixtureTestBase
        {
            private class RawHook : IContentTransformer<IUser>
            {
                public Task<IUser?> ExecuteAsync(IUser ctx, CancellationToken cancel)
                {
                    throw new NotImplementedException();
                }
            }

            [Fact]
            public async Task IgnoresRawHookInterface()
            {
                // Arrange
                var mockedContext = Create<IUser>();
                var collection = new ServiceCollection()
                    .AddSingleton<RawHook>();
                var builder = new ContentTransformerBuilder()
                        .Add<RawHook, IUser>();
                var factories = builder.Build().GetHooks<IContentTransformer<IUser>>();
                Assert.Single(factories);
                var hook = factories[0].Create<IContentTransformer<IUser>>(collection.BuildServiceProvider());

                // Act/Assert
                await Assert.ThrowsAsync<NotImplementedException>(async () =>
                {
                    await hook.ExecuteAsync(mockedContext, CancellationToken.None);
                });
            }
        }

        #region - Lifetime -

        public class Lifetime
        {
            MockSharedResourcesLocalizer _mockLocalizer = new();
            Mock<ILogger<TestUserTransformer>> _mockLogger = new();

            [Fact]
            public void BuildAndCreateObject()
            {
                // Arrange
                var serviceProvider = new ServiceCollection().BuildServiceProvider();
                var hook = new TestUserTransformer(_mockLocalizer.Object, _mockLogger.Object);
                var hookFactory = new ContentTransformerBuilder()
                    .Add(hook)
                    .Build()
                    .GetHooks<IContentTransformer<IUser>>();
                Assert.Single(hookFactory);
                IContentTransformer<IUser>? firstScopeHook1;
                IContentTransformer<IUser>? firstScopeHook2;
                IContentTransformer<IUser>? lastScopeHook;

                // Act
                using (var scope1 = serviceProvider.CreateScope())
                {
                    firstScopeHook1 = hookFactory[0].Create<IContentTransformer<IUser>>(scope1.ServiceProvider);
                    firstScopeHook2 = hookFactory[0].Create<IContentTransformer<IUser>>(scope1.ServiceProvider);
                }

                using (var scope2 = serviceProvider.CreateScope())
                {
                    lastScopeHook = hookFactory[0].Create<IContentTransformer<IUser>>(scope2.ServiceProvider);
                }

                // Assert
                Assert.Same(hook, firstScopeHook1);
                Assert.Same(hook, firstScopeHook2);
                Assert.Same(hook, lastScopeHook);
            }

            [Fact]
            public void BuildAndCreateNoInitializerSingleton()
            {
                // Arrange
                var serviceProvider = new ServiceCollection()
                    .AddSharedResourcesLocalization()
                    .AddSingleton<TestUserTransformer>()
                    .BuildServiceProvider();
                var hookFactory = new ContentTransformerBuilder()
                    .Add<TestUserTransformer, IUser>()
                    .Build()
                    .GetHooks<IContentTransformer<IUser>>();
                Assert.Single(hookFactory);
                IContentTransformer<IUser>? firstScopeHook1;
                IContentTransformer<IUser>? firstScopeHook2;
                IContentTransformer<IUser>? lastScopeHook;

                // Act
                using (var scope1 = serviceProvider.CreateScope())
                {
                    firstScopeHook1 = hookFactory[0].Create<IContentTransformer<IUser>>(scope1.ServiceProvider);
                    firstScopeHook2 = hookFactory[0].Create<IContentTransformer<IUser>>(scope1.ServiceProvider);
                }

                using (var scope2 = serviceProvider.CreateScope())
                {
                    lastScopeHook = hookFactory[0].Create<IContentTransformer<IUser>>(scope2.ServiceProvider);
                }

                // Assert
                Assert.NotNull(firstScopeHook1);
                Assert.Same(firstScopeHook1, firstScopeHook2);
                Assert.Same(firstScopeHook1, lastScopeHook);
            }

            [Fact]
            public void BuildAndCreateNoInitializerScoped()
            {
                // Arrange
                var serviceProvider = new ServiceCollection()
                    .AddSharedResourcesLocalization()
                    .AddScoped<TestUserTransformer>()
                    .BuildServiceProvider();
                var hookFactory = new ContentTransformerBuilder()
                    .Add<TestUserTransformer, IUser>()
                    .Build()
                    .GetHooks<IContentTransformer<IUser>>();
                Assert.Single(hookFactory);
                IContentTransformer<IUser>? firstScopeHook1;
                IContentTransformer<IUser>? firstScopeHook2;
                IContentTransformer<IUser>? lastScopeHook;

                // Act
                using (var scope1 = serviceProvider.CreateScope())
                {
                    firstScopeHook1 = hookFactory[0].Create<IContentTransformer<IUser>>(scope1.ServiceProvider);
                    firstScopeHook2 = hookFactory[0].Create<IContentTransformer<IUser>>(scope1.ServiceProvider);
                }

                using (var scope2 = serviceProvider.CreateScope())
                {
                    lastScopeHook = hookFactory[0].Create<IContentTransformer<IUser>>(scope2.ServiceProvider);
                }

                // Assert
                Assert.NotNull(firstScopeHook1);
                Assert.Same(firstScopeHook1, firstScopeHook2);
                Assert.NotSame(firstScopeHook1, lastScopeHook);
            }

            [Fact]
            public void BuildAndCreateNoInitializerTransient()
            {
                // Arrange
                var serviceProvider = new ServiceCollection()
                    .AddSharedResourcesLocalization()
                    .AddTransient<TestUserTransformer>()
                    .BuildServiceProvider();
                var hookFactory = new ContentTransformerBuilder()
                    .Add<TestUserTransformer, IUser>()
                    .Build()
                    .GetHooks<IContentTransformer<IUser>>();
                Assert.Single(hookFactory);
                IContentTransformer<IUser>? firstScopeHook1;
                IContentTransformer<IUser>? firstScopeHook2;
                IContentTransformer<IUser>? lastScopeHook;

                // Act
                using (var scope1 = serviceProvider.CreateScope())
                {
                    firstScopeHook1 = hookFactory[0].Create<IContentTransformer<IUser>>(scope1.ServiceProvider);
                    firstScopeHook2 = hookFactory[0].Create<IContentTransformer<IUser>>(scope1.ServiceProvider);
                }

                using (var scope2 = serviceProvider.CreateScope())
                {
                    lastScopeHook = hookFactory[0].Create<IContentTransformer<IUser>>(scope2.ServiceProvider);
                }

                // Assert
                Assert.NotNull(firstScopeHook1);
                Assert.NotSame(firstScopeHook1, firstScopeHook2);
                Assert.NotSame(firstScopeHook1, lastScopeHook);
            }

            [Fact]
            public void BuildAndCreateWithInitializerObjectReference()
            {
                // Arrange
                var hook = new TestUserTransformer(_mockLocalizer.Object, _mockLogger.Object);
                var serviceProvider = new ServiceCollection().BuildServiceProvider();
                var hookFactory = new ContentTransformerBuilder()
                    .Add<IContentTransformer<IUser>, IUser>(provider => hook)
                    .Build()
                    .GetHooks<IContentTransformer<IUser>>();
                Assert.Single(hookFactory);
                IContentTransformer<IUser>? firstScopeHook1;
                IContentTransformer<IUser>? firstScopeHook2;
                IContentTransformer<IUser>? lastScopeHook;

                // Act
                using (var scope1 = serviceProvider.CreateScope())
                {
                    firstScopeHook1 = hookFactory[0].Create<IContentTransformer<IUser>>(scope1.ServiceProvider);
                    firstScopeHook2 = hookFactory[0].Create<IContentTransformer<IUser>>(scope1.ServiceProvider);
                }

                using (var scope2 = serviceProvider.CreateScope())
                {
                    lastScopeHook = hookFactory[0].Create<IContentTransformer<IUser>>(scope2.ServiceProvider);
                }

                // Assert
                Assert.NotNull(firstScopeHook1);
                Assert.Same(hook, firstScopeHook1);
                Assert.Same(firstScopeHook1, firstScopeHook2);
                Assert.Same(firstScopeHook1, lastScopeHook);
            }

            [Fact]
            public void BuildAndCreateWithInitializerNewObject()
            {
                // Arrange
                var serviceProvider = new ServiceCollection().BuildServiceProvider();
                var hookFactory = new ContentTransformerBuilder()
                    .Add<IContentTransformer<IUser>, IUser>(provider => new TestUserTransformer(_mockLocalizer.Object, _mockLogger.Object))
                    .Build()
                    .GetHooks<IContentTransformer<IUser>>();
                Assert.Single(hookFactory);
                IContentTransformer<IUser>? firstScopeHook1;
                IContentTransformer<IUser>? firstScopeHook2;
                IContentTransformer<IUser>? lastScopeHook;

                // Act
                using (var scope1 = serviceProvider.CreateScope())
                {
                    firstScopeHook1 = hookFactory[0].Create<IContentTransformer<IUser>>(scope1.ServiceProvider);
                    firstScopeHook2 = hookFactory[0].Create<IContentTransformer<IUser>>(scope1.ServiceProvider);
                }

                using (var scope2 = serviceProvider.CreateScope())
                {
                    lastScopeHook = hookFactory[0].Create<IContentTransformer<IUser>>(scope2.ServiceProvider);
                }

                // Assert
                Assert.NotNull(firstScopeHook1);
                Assert.NotSame(firstScopeHook1, firstScopeHook2);
                Assert.NotSame(firstScopeHook1, lastScopeHook);
            }

            [Fact]
            public void BuildAndCreateWithInitializerSingleton()
            {
                // Arrange
                var serviceProvider = new ServiceCollection()
                    .AddSharedResourcesLocalization()
                    .AddSingleton<TestUserTransformer>()
                    .BuildServiceProvider();
                var hookFactory = new ContentTransformerBuilder()
                    .Add<IContentTransformer<IUser>, IUser>(provider => provider.GetRequiredService<TestUserTransformer>())
                    .Build()
                    .GetHooks<IContentTransformer<IUser>>();
                Assert.Single(hookFactory);
                IContentTransformer<IUser>? firstScopeHook1;
                IContentTransformer<IUser>? firstScopeHook2;
                IContentTransformer<IUser>? lastScopeHook;

                // Act
                using (var scope1 = serviceProvider.CreateScope())
                {
                    firstScopeHook1 = hookFactory[0].Create<IContentTransformer<IUser>>(scope1.ServiceProvider);
                    firstScopeHook2 = hookFactory[0].Create<IContentTransformer<IUser>>(scope1.ServiceProvider);
                }

                using (var scope2 = serviceProvider.CreateScope())
                {
                    lastScopeHook = hookFactory[0].Create<IContentTransformer<IUser>>(scope2.ServiceProvider);
                }

                // Assert
                Assert.NotNull(firstScopeHook1);
                Assert.Same(firstScopeHook1, firstScopeHook2);
                Assert.Same(firstScopeHook1, lastScopeHook);
            }

            [Fact]
            public void BuildAndCreateWithInitializerScoped()
            {
                // Arrange
                var serviceProvider = new ServiceCollection()
                    .AddSharedResourcesLocalization()
                    .AddScoped<TestUserTransformer>()
                    .BuildServiceProvider();
                var hookFactory = new ContentTransformerBuilder()
                    .Add<IContentTransformer<IUser>, IUser>(provider => provider.GetRequiredService<TestUserTransformer>())
                    .Build()
                    .GetHooks<IContentTransformer<IUser>>();
                Assert.Single(hookFactory);
                IContentTransformer<IUser>? firstScopeHook1;
                IContentTransformer<IUser>? firstScopeHook2;
                IContentTransformer<IUser>? lastScopeHook;

                // Act
                using (var scope1 = serviceProvider.CreateScope())
                {
                    firstScopeHook1 = hookFactory[0].Create<IContentTransformer<IUser>>(scope1.ServiceProvider);
                    firstScopeHook2 = hookFactory[0].Create<IContentTransformer<IUser>>(scope1.ServiceProvider);
                }

                using (var scope2 = serviceProvider.CreateScope())
                {
                    lastScopeHook = hookFactory[0].Create<IContentTransformer<IUser>>(scope2.ServiceProvider);
                }

                // Assert
                Assert.NotNull(firstScopeHook1);
                Assert.Same(firstScopeHook1, firstScopeHook2);
                Assert.NotSame(firstScopeHook1, lastScopeHook);
            }

            [Fact]
            public void BuildAndCreateWithInitializerTransient()
            {
                // Arrange
                var serviceProvider = new ServiceCollection()
                    .AddSharedResourcesLocalization()
                    .AddTransient<TestUserTransformer>()
                    .BuildServiceProvider();
                var hookFactory = new ContentTransformerBuilder()
                    .Add<IContentTransformer<IUser>, IUser>(provider => provider.GetRequiredService<TestUserTransformer>())
                    .Build()
                    .GetHooks<IContentTransformer<IUser>>();
                Assert.Single(hookFactory);
                IContentTransformer<IUser>? firstScopeHook1;
                IContentTransformer<IUser>? firstScopeHook2;
                IContentTransformer<IUser>? lastScopeHook;

                // Act
                using (var scope1 = serviceProvider.CreateScope())
                {
                    firstScopeHook1 = hookFactory[0].Create<IContentTransformer<IUser>>(scope1.ServiceProvider);
                    firstScopeHook2 = hookFactory[0].Create<IContentTransformer<IUser>>(scope1.ServiceProvider);
                }

                using (var scope2 = serviceProvider.CreateScope())
                {
                    lastScopeHook = hookFactory[0].Create<IContentTransformer<IUser>>(scope2.ServiceProvider);
                }

                // Assert
                Assert.NotNull(firstScopeHook1);
                Assert.NotSame(firstScopeHook1, firstScopeHook2);
                Assert.NotSame(firstScopeHook1, lastScopeHook);
            }

            [Fact]
            public void BuildAndCreateCallback()
            {
                // Arrange
                static Task<IUser?> callback(IUser context,
                    CancellationToken cancel)
                    => Task.FromResult(null as IUser);
                var serviceProvider = new ServiceCollection().BuildServiceProvider();
                var hookFactory = new ContentTransformerBuilder()
                    .Add<IUser>(callback)
                    .Build()
                    .GetHooks<IContentTransformer<IUser>>();
                Assert.Single(hookFactory);
                IMigrationHook<IUser>? firstScopeHook1;
                IMigrationHook<IUser>? firstScopeHook2;
                IMigrationHook<IUser>? lastScopeHook;

                // Act
                using (var scope1 = serviceProvider.CreateScope())
                {
                    firstScopeHook1 = hookFactory[0].Create<IMigrationHook<IUser>>(scope1.ServiceProvider);
                    firstScopeHook2 = hookFactory[0].Create<IMigrationHook<IUser>>(scope1.ServiceProvider);
                }

                using (var scope2 = serviceProvider.CreateScope())
                {
                    lastScopeHook = hookFactory[0].Create<IMigrationHook<IUser>>(scope2.ServiceProvider);
                }

                // Assert
                Assert.NotNull(firstScopeHook1);
                Assert.NotSame(firstScopeHook1, firstScopeHook2);
                Assert.NotSame(firstScopeHook1, lastScopeHook);
            }
        }

        #endregion
    }
}

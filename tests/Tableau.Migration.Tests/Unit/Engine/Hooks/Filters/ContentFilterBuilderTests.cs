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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Content;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Engine.Hooks.Filters;
using Moq;
using Xunit;
using Polly;
using Tableau.Migration.Resources;
using Microsoft.Extensions.Localization;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Filters
{
    public class ContentFilterBuilderTests
    {
        private class UserFilter : ContentFilterBase<IUser>
        {
            public UserFilter(
                ISharedResourcesLocalizer localizer,
                ILogger<IContentFilter<IUser>> logger) : base(localizer, logger) { }

            public override bool ShouldMigrate(ContentMigrationItem<IUser> item) => true;
        }

        private class GenericFilter<TContent> : ContentFilterBase<TContent>
            where TContent : IContentReference
        {
            public GenericFilter(ISharedResourcesLocalizer localizer, ILogger<IContentFilter<TContent>> logger) 
                : base (localizer, logger) { }

            public override bool ShouldMigrate(ContentMigrationItem<TContent> item) => true;
        }

        public class Clear
        {
            [Fact]
            public void ClearsFactories()
            {
                var builder = new ContentFilterBuilder().Add<UserFilter, IUser>()
                    .Clear();

                var result = builder.Build();
                Assert.Empty(result.GetHooks<IContentFilter<IUser>>());
            }
        }

        public class Add
        {
            [Fact]
            public void AddFromType()
            {
                var builder = new ContentFilterBuilder().Add<UserFilter, IUser>();

                var result = builder.Build();
                Assert.NotNull(result);

                var filters = result.GetHooks<IContentFilter<IUser>>();
                Assert.Single(filters);
            }
        }

        public class AddMappingGenericInstance
        {
            protected readonly MockSharedResourcesLocalizer _mockLocalizer = new();
            protected readonly Mock<ILogger<IContentFilter<IUser>>> _mockLogger = new();

            [Fact]
            public void AddFromType()
            {
                var genericFilter = new GenericFilter<IUser>(_mockLocalizer.Object, _mockLogger.Object);
                var builder = new ContentFilterBuilder().Add(genericFilter);

                var result = builder.Build();
                Assert.NotNull(result);

                var filters = result.GetHooks<IContentFilter<IUser>>();
                Assert.Single(filters);
            }
        }

        public class AddGenericFactory
        {
            [Fact]
            public void AddFromType()
            {
                var builder = new ContentFilterBuilder().Add<GenericFilter<IUser>, IUser>();

                var result = builder.Build();
                Assert.NotNull(result);

                var filters = result.GetHooks<IContentFilter<IUser>>();
                Assert.Single(filters);
            }
        }

        public class AddGenericFactoryAsInterface
        {
            [Fact]
            public void AddFromType()
            {
                var builder = new ContentFilterBuilder().Add<IContentFilter<IUser>, IUser>();

                var result = builder.Build();
                Assert.NotNull(result);

                var filters = result.GetHooks<IContentFilter<IUser>>();
                Assert.Single(filters);
            }
        }

        public class AddCallback
        {
            [Fact]
            public void AddFromType()
            {
                var builder = new ContentFilterBuilder().Add(
                    (IEnumerable<ContentMigrationItem<IUser>> context,
                    CancellationToken cancel)
                    => Task.FromResult(null as IEnumerable<ContentMigrationItem<IUser>>));

                var result = builder.Build();
                Assert.NotNull(result);

                var mappings = result.GetHooks<IContentFilter<IUser>>();
                Assert.Single(mappings);
            }
        }

        public class AddRawFilterInterface
        {
            private class RawHook : IContentFilter<IUser>
            {
                public Task<IEnumerable<ContentMigrationItem<IUser>>?> ExecuteAsync(IEnumerable<ContentMigrationItem<IUser>> ctx, CancellationToken cancel)
                {
                    throw new NotImplementedException();
                }
            }

            [Fact]
            public async Task IgnoresRawHookInterface()
            {
                // Arrange
                var users = new List<ContentMigrationItem<IUser>>();
                var collection = new ServiceCollection()
                    .AddSingleton<RawHook>();
                var builder = new ContentFilterBuilder()
                        .Add<RawHook, IUser>();
                var factories = builder.Build().GetHooks<IContentFilter<IUser>>();
                Assert.Single(factories);
                var hook = factories[0].Create<IContentFilter<IUser>>(collection.BuildServiceProvider());

                // Act/Assert
                await Assert.ThrowsAsync<NotImplementedException>(async () =>
                {
                    await hook.ExecuteAsync(users, CancellationToken.None);
                });
            }
        }

        #region - Lifetime -

        public class Lifetime
        {
            private readonly MockSharedResourcesLocalizer _mockLocalizer = new();
            private readonly Mock<ILogger<IContentFilter<IUser>>> _mockLogger = new();

            [Fact]
            public void BuildAndCreateObject()
            {
                // Arrange
                var serviceProvider = new ServiceCollection().BuildServiceProvider();
                var hook = new UserFilter(_mockLocalizer.Object, _mockLogger.Object);
                var hookFactory = new ContentFilterBuilder()
                    .Add(hook)
                    .Build()
                    .GetHooks<IContentFilter<IUser>>();
                Assert.Single(hookFactory);
                IContentFilter<IUser>? firstScopeHook1;
                IContentFilter<IUser>? firstScopeHook2;
                IContentFilter<IUser>? lastScopeHook;

                // Act
                using (var scope1 = serviceProvider.CreateScope())
                {
                    firstScopeHook1 = hookFactory[0].Create<IContentFilter<IUser>>(scope1.ServiceProvider);
                    firstScopeHook2 = hookFactory[0].Create<IContentFilter<IUser>>(scope1.ServiceProvider);
                }

                using (var scope2 = serviceProvider.CreateScope())
                {
                    lastScopeHook = hookFactory[0].Create<IContentFilter<IUser>>(scope2.ServiceProvider);
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
                    .AddSingleton<UserFilter>()
                    .BuildServiceProvider();
                var hookFactory = new ContentFilterBuilder()
                    .Add<UserFilter, IUser>()
                    .Build()
                    .GetHooks<IContentFilter<IUser>>();
                Assert.Single(hookFactory);
                IContentFilter<IUser>? firstScopeHook1;
                IContentFilter<IUser>? firstScopeHook2;
                IContentFilter<IUser>? lastScopeHook;

                // Act
                using (var scope1 = serviceProvider.CreateScope())
                {
                    firstScopeHook1 = hookFactory[0].Create<IContentFilter<IUser>>(scope1.ServiceProvider);
                    firstScopeHook2 = hookFactory[0].Create<IContentFilter<IUser>>(scope1.ServiceProvider);
                }

                using (var scope2 = serviceProvider.CreateScope())
                {
                    lastScopeHook = hookFactory[0].Create<IContentFilter<IUser>>(scope2.ServiceProvider);
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
                    .AddScoped<UserFilter>()
                    .BuildServiceProvider();
                var hookFactory = new ContentFilterBuilder()
                    .Add<UserFilter, IUser>()
                    .Build()
                    .GetHooks<IContentFilter<IUser>>();
                Assert.Single(hookFactory);
                IContentFilter<IUser>? firstScopeHook1;
                IContentFilter<IUser>? firstScopeHook2;
                IContentFilter<IUser>? lastScopeHook;

                // Act
                using (var scope1 = serviceProvider.CreateScope())
                {
                    firstScopeHook1 = hookFactory[0].Create<IContentFilter<IUser>>(scope1.ServiceProvider);
                    firstScopeHook2 = hookFactory[0].Create<IContentFilter<IUser>>(scope1.ServiceProvider);
                }

                using (var scope2 = serviceProvider.CreateScope())
                {
                    lastScopeHook = hookFactory[0].Create<IContentFilter<IUser>>(scope2.ServiceProvider);
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
                    .AddTransient<UserFilter>()
                    .BuildServiceProvider();
                var hookFactory = new ContentFilterBuilder()
                    .Add<UserFilter, IUser>()
                    .Build()
                    .GetHooks<IContentFilter<IUser>>();
                Assert.Single(hookFactory);
                IContentFilter<IUser>? firstScopeHook1;
                IContentFilter<IUser>? firstScopeHook2;
                IContentFilter<IUser>? lastScopeHook;

                // Act
                using (var scope1 = serviceProvider.CreateScope())
                {
                    firstScopeHook1 = hookFactory[0].Create<IContentFilter<IUser>>(scope1.ServiceProvider);
                    firstScopeHook2 = hookFactory[0].Create<IContentFilter<IUser>>(scope1.ServiceProvider);
                }

                using (var scope2 = serviceProvider.CreateScope())
                {
                    lastScopeHook = hookFactory[0].Create<IContentFilter<IUser>>(scope2.ServiceProvider);
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
                var hook = new UserFilter(_mockLocalizer.Object, _mockLogger.Object);
                var serviceProvider = new ServiceCollection().BuildServiceProvider();
                var hookFactory = new ContentFilterBuilder()
                    .Add<IContentFilter<IUser>, IUser>(provider => hook)
                    .Build()
                    .GetHooks<IContentFilter<IUser>>();
                Assert.Single(hookFactory);
                IContentFilter<IUser>? firstScopeHook1;
                IContentFilter<IUser>? firstScopeHook2;
                IContentFilter<IUser>? lastScopeHook;

                // Act
                using (var scope1 = serviceProvider.CreateScope())
                {
                    firstScopeHook1 = hookFactory[0].Create<IContentFilter<IUser>>(scope1.ServiceProvider);
                    firstScopeHook2 = hookFactory[0].Create<IContentFilter<IUser>>(scope1.ServiceProvider);
                }

                using (var scope2 = serviceProvider.CreateScope())
                {
                    lastScopeHook = hookFactory[0].Create<IContentFilter<IUser>>(scope2.ServiceProvider);
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
                var hookFactory = new ContentFilterBuilder()
                    .Add<IContentFilter<IUser>, IUser>(provider => new UserFilter(_mockLocalizer.Object, _mockLogger.Object))
                    .Build()
                    .GetHooks<IContentFilter<IUser>>();
                Assert.Single(hookFactory);
                IContentFilter<IUser>? firstScopeHook1;
                IContentFilter<IUser>? firstScopeHook2;
                IContentFilter<IUser>? lastScopeHook;

                // Act
                using (var scope1 = serviceProvider.CreateScope())
                {
                    firstScopeHook1 = hookFactory[0].Create<IContentFilter<IUser>>(scope1.ServiceProvider);
                    firstScopeHook2 = hookFactory[0].Create<IContentFilter<IUser>>(scope1.ServiceProvider);
                }

                using (var scope2 = serviceProvider.CreateScope())
                {
                    lastScopeHook = hookFactory[0].Create<IContentFilter<IUser>>(scope2.ServiceProvider);
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
                    .AddSingleton<UserFilter>()
                    .BuildServiceProvider();
                var hookFactory = new ContentFilterBuilder()
                    .Add<IContentFilter<IUser>, IUser>(provider => provider.GetRequiredService<UserFilter>())
                    .Build()
                    .GetHooks<IContentFilter<IUser>>();
                Assert.Single(hookFactory);
                IContentFilter<IUser>? firstScopeHook1;
                IContentFilter<IUser>? firstScopeHook2;
                IContentFilter<IUser>? lastScopeHook;

                // Act
                using (var scope1 = serviceProvider.CreateScope())
                {
                    firstScopeHook1 = hookFactory[0].Create<IContentFilter<IUser>>(scope1.ServiceProvider);
                    firstScopeHook2 = hookFactory[0].Create<IContentFilter<IUser>>(scope1.ServiceProvider);
                }

                using (var scope2 = serviceProvider.CreateScope())
                {
                    lastScopeHook = hookFactory[0].Create<IContentFilter<IUser>>(scope2.ServiceProvider);
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
                    .AddScoped<UserFilter>()
                    .BuildServiceProvider();
                var hookFactory = new ContentFilterBuilder()
                    .Add<IContentFilter<IUser>, IUser>(provider => provider.GetRequiredService<UserFilter>())
                    .Build()
                    .GetHooks<IContentFilter<IUser>>();
                Assert.Single(hookFactory);
                IContentFilter<IUser>? firstScopeHook1;
                IContentFilter<IUser>? firstScopeHook2;
                IContentFilter<IUser>? lastScopeHook;

                // Act
                using (var scope1 = serviceProvider.CreateScope())
                {
                    firstScopeHook1 = hookFactory[0].Create<IContentFilter<IUser>>(scope1.ServiceProvider);
                    firstScopeHook2 = hookFactory[0].Create<IContentFilter<IUser>>(scope1.ServiceProvider);
                }

                using (var scope2 = serviceProvider.CreateScope())
                {
                    lastScopeHook = hookFactory[0].Create<IContentFilter<IUser>>(scope2.ServiceProvider);
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
                    .AddTransient<UserFilter>()
                    .BuildServiceProvider();
                var hookFactory = new ContentFilterBuilder()
                    .Add<IContentFilter<IUser>, IUser>(provider => provider.GetRequiredService<UserFilter>())
                    .Build()
                    .GetHooks<IContentFilter<IUser>>();
                Assert.Single(hookFactory);
                IContentFilter<IUser>? firstScopeHook1;
                IContentFilter<IUser>? firstScopeHook2;
                IContentFilter<IUser>? lastScopeHook;

                // Act
                using (var scope1 = serviceProvider.CreateScope())
                {
                    firstScopeHook1 = hookFactory[0].Create<IContentFilter<IUser>>(scope1.ServiceProvider);
                    firstScopeHook2 = hookFactory[0].Create<IContentFilter<IUser>>(scope1.ServiceProvider);
                }

                using (var scope2 = serviceProvider.CreateScope())
                {
                    lastScopeHook = hookFactory[0].Create<IContentFilter<IUser>>(scope2.ServiceProvider);
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
                static Task<IEnumerable<ContentMigrationItem<IUser>>?> callback(IEnumerable<ContentMigrationItem<IUser>> context,
                    CancellationToken cancel)
                    => Task.FromResult(null as IEnumerable<ContentMigrationItem<IUser>>);
                var serviceProvider = new ServiceCollection().BuildServiceProvider();
                var hookFactory = new ContentFilterBuilder()
                    .Add<IUser>(callback)
                    .Build()
                    .GetHooks<IContentFilter<IUser>>();
                Assert.Single(hookFactory);
                IMigrationHook<IEnumerable<ContentMigrationItem<IUser>>>? firstScopeHook1;
                IMigrationHook<IEnumerable<ContentMigrationItem<IUser>>>? firstScopeHook2;
                IMigrationHook<IEnumerable<ContentMigrationItem<IUser>>>? lastScopeHook;

                // Act
                using (var scope1 = serviceProvider.CreateScope())
                {
                    firstScopeHook1 = hookFactory[0].Create<IMigrationHook<IEnumerable<ContentMigrationItem<IUser>>>>(scope1.ServiceProvider);
                    firstScopeHook2 = hookFactory[0].Create<IMigrationHook<IEnumerable<ContentMigrationItem<IUser>>>>(scope1.ServiceProvider);
                }

                using (var scope2 = serviceProvider.CreateScope())
                {
                    lastScopeHook = hookFactory[0].Create<IMigrationHook<IEnumerable<ContentMigrationItem<IUser>>>>(scope2.ServiceProvider);
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

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
using Tableau.Migration.Engine.Hooks;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks
{
    #region - Test Types -

    public interface ITestHook : IMigrationHook<int> { }

    public class TestHook : ITestHook
    {
        public Task<int> ExecuteAsync(int ctx, CancellationToken cancel)
        {
            throw new NotImplementedException();
        }
    }

    #endregion

    public class MigrationHookBuilderTests
    {
        #region - Clear -

        public class Clear
        {
            [Fact]
            public void ClearsFactories()
            {
                var b = new MigrationHookBuilder();
                var result = b
                        .Add(new TestHook())
                        .Clear();

                Assert.Same(b, result);

                Assert.Empty(b.Build().GetHooks<ITestHook>());
            }
        }

        #endregion

        #region - Add -

        public class Add
        {
            [Fact]
            public void AddObject()
            {
                var h = new TestHook();

                var b = new MigrationHookBuilder();
                var result = b.Add(h);

                Assert.Same(b, result);
            }

            [Fact]
            public void AddTypeNoInitializer()
            {
                var b = new MigrationHookBuilder();
                var result = b.Add<TestHook>();

                Assert.Same(b, result);
            }

            [Fact]
            public void AddTypeWithInitializer()
            {
                var b = new MigrationHookBuilder();
                var result = b.Add(services => new TestHook());

                Assert.Same(b, result);
            }

            [Fact]
            public void Callback()
            {
                static Task<int> callback(int ctx, CancellationToken cancel) => Task.FromResult(0);

                var b = new MigrationHookBuilder();
                var result = b.Add<ITestHook, int>(callback);

                Assert.Same(b, result);
            }

            private class ContentClass1 : TestContentType
            { }

            private class ContentClass2 : TestContentType
            { }

            private interface IContentHook<TContent> : IMigrationHook<TContent>
            { }

            public class ContentHook<TContent> : IContentHook<TContent>
            {
                public Task<TContent?> ExecuteAsync(TContent ctx, CancellationToken cancel)
                {
                    throw new NotImplementedException();
                }
            }

            [Fact]
            public void AddConstructed()
            {
                var b = new MigrationHookBuilder();
                var result = b.Add(typeof(ContentHook<>), new[] { new[] { typeof(ContentClass1) }, new[] { typeof(ContentClass2) } });

                Assert.Same(b, result);
            }

            private class RawHook : IMigrationHook<int>
            {
                public Task<int> ExecuteAsync(int ctx, CancellationToken cancel)
                {
                    throw new NotImplementedException();
                }
            }

            [Fact]
            public void IgnoresRawHookInterface()
            {
                Assert.Throws<ArgumentException>(() =>
                {
                    new MigrationHookBuilder()
                        .Add<RawHook>();
                });
            }
        }

        #endregion

        #region - Build -

        public class Build
        {
            [Fact]
            public void DetectsHookInterfaceFromConcrete()
            {
                var hooks = new MigrationHookBuilder()
                    .Add(new TestHook())
                    .Build();

                Assert.Single(hooks.GetHooks<ITestHook>());
                Assert.Empty(hooks.GetHooks<IMigrationHook<int>>());
            }

            [Fact]
            public void DetectsHookInterface()
            {
                var hooks = new MigrationHookBuilder()
                    .Add<ITestHook>()
                    .Build();

                Assert.Single(hooks.GetHooks<ITestHook>());
                Assert.Empty(hooks.GetHooks<IMigrationHook<int>>());
            }

            private class TestHookChild : TestHook
            { }

            [Fact]
            public void DetectsBaseClassInterface()
            {
                var hooks = new MigrationHookBuilder()
                    .Add<TestHookChild>()
                    .Build();

                Assert.Single(hooks.GetHooks<ITestHook>());
                Assert.Empty(hooks.GetHooks<IMigrationHook<int>>());
            }
        }

        #endregion

        #region - Lifetime -

        public class Lifetime
        {
            [Fact]
            public void BuildAndCreateObject()
            {
                // Arrange
                var serviceProvider = new ServiceCollection().BuildServiceProvider();
                var hook = new TestHook();
                var hookFactory = new MigrationHookBuilder()
                    .Add(hook)
                    .Build()
                    .GetHooks<ITestHook>();
                Assert.Single(hookFactory);
                IMigrationHook<int>? firstScopeHook1;
                IMigrationHook<int>? firstScopeHook2;
                IMigrationHook<int>? lastScopeHook;

                // Act
                using (var scope1 = serviceProvider.CreateScope())
                {
                    firstScopeHook1 = hookFactory[0].Create<IMigrationHook<int>>(scope1.ServiceProvider);
                    firstScopeHook2 = hookFactory[0].Create<IMigrationHook<int>>(scope1.ServiceProvider);
                }

                using (var scope2 = serviceProvider.CreateScope())
                {
                    lastScopeHook = hookFactory[0].Create<IMigrationHook<int>>(scope2.ServiceProvider);
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
                    .AddSingleton<TestHook>()
                    .BuildServiceProvider();
                var hookFactory = new MigrationHookBuilder()
                    .Add<TestHook>()
                    .Build()
                    .GetHooks<ITestHook>();
                Assert.Single(hookFactory);
                IMigrationHook<int>? firstScopeHook1;
                IMigrationHook<int>? firstScopeHook2;
                IMigrationHook<int>? lastScopeHook;

                // Act
                using (var scope1 = serviceProvider.CreateScope())
                {
                    firstScopeHook1 = hookFactory[0].Create<IMigrationHook<int>>(scope1.ServiceProvider);
                    firstScopeHook2 = hookFactory[0].Create<IMigrationHook<int>>(scope1.ServiceProvider);
                }

                using (var scope2 = serviceProvider.CreateScope())
                {
                    lastScopeHook = hookFactory[0].Create<IMigrationHook<int>>(scope2.ServiceProvider);
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
                    .AddScoped<TestHook>()
                    .BuildServiceProvider();
                var hookFactory = new MigrationHookBuilder()
                    .Add<TestHook>()
                    .Build()
                    .GetHooks<ITestHook>();
                Assert.Single(hookFactory);
                IMigrationHook<int>? firstScopeHook1;
                IMigrationHook<int>? firstScopeHook2;
                IMigrationHook<int>? lastScopeHook;

                // Act
                using (var scope1 = serviceProvider.CreateScope())
                {
                    firstScopeHook1 = hookFactory[0].Create<IMigrationHook<int>>(scope1.ServiceProvider);
                    firstScopeHook2 = hookFactory[0].Create<IMigrationHook<int>>(scope1.ServiceProvider);
                }

                using (var scope2 = serviceProvider.CreateScope())
                {
                    lastScopeHook = hookFactory[0].Create<IMigrationHook<int>>(scope2.ServiceProvider);
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
                    .AddTransient<TestHook>()
                    .BuildServiceProvider();
                var hookFactory = new MigrationHookBuilder()
                    .Add<TestHook>()
                    .Build()
                    .GetHooks<ITestHook>();
                Assert.Single(hookFactory);
                IMigrationHook<int>? firstScopeHook1;
                IMigrationHook<int>? firstScopeHook2;
                IMigrationHook<int>? lastScopeHook;

                // Act
                using (var scope1 = serviceProvider.CreateScope())
                {
                    firstScopeHook1 = hookFactory[0].Create<IMigrationHook<int>>(scope1.ServiceProvider);
                    firstScopeHook2 = hookFactory[0].Create<IMigrationHook<int>>(scope1.ServiceProvider);
                }

                using (var scope2 = serviceProvider.CreateScope())
                {
                    lastScopeHook = hookFactory[0].Create<IMigrationHook<int>>(scope2.ServiceProvider);
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
                var hook = new TestHook();
                var serviceProvider = new ServiceCollection().BuildServiceProvider();
                var hookFactory = new MigrationHookBuilder()
                    .Add(provider => hook)
                    .Build()
                    .GetHooks<ITestHook>();
                Assert.Single(hookFactory);
                IMigrationHook<int>? firstScopeHook1;
                IMigrationHook<int>? firstScopeHook2;
                IMigrationHook<int>? lastScopeHook;

                // Act
                using (var scope1 = serviceProvider.CreateScope())
                {
                    firstScopeHook1 = hookFactory[0].Create<IMigrationHook<int>>(scope1.ServiceProvider);
                    firstScopeHook2 = hookFactory[0].Create<IMigrationHook<int>>(scope1.ServiceProvider);
                }

                using (var scope2 = serviceProvider.CreateScope())
                {
                    lastScopeHook = hookFactory[0].Create<IMigrationHook<int>>(scope2.ServiceProvider);
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
                var hookFactory = new MigrationHookBuilder()
                    .Add(provider => new TestHook())
                    .Build()
                    .GetHooks<ITestHook>();
                Assert.Single(hookFactory);
                IMigrationHook<int>? firstScopeHook1;
                IMigrationHook<int>? firstScopeHook2;
                IMigrationHook<int>? lastScopeHook;

                // Act
                using (var scope1 = serviceProvider.CreateScope())
                {
                    firstScopeHook1 = hookFactory[0].Create<IMigrationHook<int>>(scope1.ServiceProvider);
                    firstScopeHook2 = hookFactory[0].Create<IMigrationHook<int>>(scope1.ServiceProvider);
                }

                using (var scope2 = serviceProvider.CreateScope())
                {
                    lastScopeHook = hookFactory[0].Create<IMigrationHook<int>>(scope2.ServiceProvider);
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
                    .AddSingleton<TestHook>()
                    .BuildServiceProvider();
                var hookFactory = new MigrationHookBuilder()
                    .Add(provider => provider.GetRequiredService<TestHook>())
                    .Build()
                    .GetHooks<ITestHook>();
                Assert.Single(hookFactory);
                IMigrationHook<int>? firstScopeHook1;
                IMigrationHook<int>? firstScopeHook2;
                IMigrationHook<int>? lastScopeHook;

                // Act
                using (var scope1 = serviceProvider.CreateScope())
                {
                    firstScopeHook1 = hookFactory[0].Create<IMigrationHook<int>>(scope1.ServiceProvider);
                    firstScopeHook2 = hookFactory[0].Create<IMigrationHook<int>>(scope1.ServiceProvider);
                }

                using (var scope2 = serviceProvider.CreateScope())
                {
                    lastScopeHook = hookFactory[0].Create<IMigrationHook<int>>(scope2.ServiceProvider);
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
                    .AddScoped<TestHook>()
                    .BuildServiceProvider();
                var hookFactory = new MigrationHookBuilder()
                    .Add(provider => provider.GetRequiredService<TestHook>())
                    .Build()
                    .GetHooks<ITestHook>();
                Assert.Single(hookFactory);
                IMigrationHook<int>? firstScopeHook1;
                IMigrationHook<int>? firstScopeHook2;
                IMigrationHook<int>? lastScopeHook;

                // Act
                using (var scope1 = serviceProvider.CreateScope())
                {
                    firstScopeHook1 = hookFactory[0].Create<IMigrationHook<int>>(scope1.ServiceProvider);
                    firstScopeHook2 = hookFactory[0].Create<IMigrationHook<int>>(scope1.ServiceProvider);
                }

                using (var scope2 = serviceProvider.CreateScope())
                {
                    lastScopeHook = hookFactory[0].Create<IMigrationHook<int>>(scope2.ServiceProvider);
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
                    .AddTransient<TestHook>()
                    .BuildServiceProvider();
                var hookFactory = new MigrationHookBuilder()
                    .Add(provider => provider.GetRequiredService<TestHook>())
                    .Build()
                    .GetHooks<ITestHook>();
                Assert.Single(hookFactory);
                IMigrationHook<int>? firstScopeHook1;
                IMigrationHook<int>? firstScopeHook2;
                IMigrationHook<int>? lastScopeHook;

                // Act
                using (var scope1 = serviceProvider.CreateScope())
                {
                    firstScopeHook1 = hookFactory[0].Create<IMigrationHook<int>>(scope1.ServiceProvider);
                    firstScopeHook2 = hookFactory[0].Create<IMigrationHook<int>>(scope1.ServiceProvider);
                }

                using (var scope2 = serviceProvider.CreateScope())
                {
                    lastScopeHook = hookFactory[0].Create<IMigrationHook<int>>(scope2.ServiceProvider);
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
                static Task<int> callback(int ctx, CancellationToken cancel) => Task.FromResult(0);
                var serviceProvider = new ServiceCollection().BuildServiceProvider();
                var hookFactory = new MigrationHookBuilder()
                    .Add<ITestHook, int>(callback)
                    .Build()
                    .GetHooks<ITestHook>();
                Assert.Single(hookFactory);
                IMigrationHook<int>? firstScopeHook1;
                IMigrationHook<int>? firstScopeHook2;
                IMigrationHook<int>? lastScopeHook;

                // Act
                using (var scope1 = serviceProvider.CreateScope())
                {
                    firstScopeHook1 = hookFactory[0].Create<IMigrationHook<int>>(scope1.ServiceProvider);
                    firstScopeHook2 = hookFactory[0].Create<IMigrationHook<int>>(scope1.ServiceProvider);
                }

                using (var scope2 = serviceProvider.CreateScope())
                {
                    lastScopeHook = hookFactory[0].Create<IMigrationHook<int>>(scope2.ServiceProvider);
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

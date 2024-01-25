using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Engine.Hooks.Transformers;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Transformers
{
    public class ContentTransformerBuilderTests
    {
        private class TestUserTransformer : ContentTransformerBase<IUser>
        {
            public override Task<IUser?> ExecuteAsync(IUser itemToTransform, CancellationToken cancel)
            {
                return Task.FromResult<IUser?>(itemToTransform);
            }
        }

        private class GenericTransformer<TContent>
            : ContentTransformerBase<TContent>
            where TContent : IContentReference
        {
            public override Task<TContent?> ExecuteAsync(TContent itemToTransform, CancellationToken cancel)
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
                var genericTransformer = new GenericTransformer<IUser>();
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
            [Fact]
            public void BuildAndCreateObject()
            {
                // Arrange
                var serviceProvider = new ServiceCollection().BuildServiceProvider();
                var hook = new TestUserTransformer();
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
                var hook = new TestUserTransformer();
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
                    .Add<IContentTransformer<IUser>, IUser>(provider => new TestUserTransformer())
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

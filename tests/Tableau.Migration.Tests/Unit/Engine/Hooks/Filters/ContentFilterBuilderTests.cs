using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.Content;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Engine.Hooks.Filters;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Filters
{
    public class ContentFilterBuilderTests
    {
        private class UserFilter : ContentFilterBase<IUser>
        {
            public override bool ShouldMigrate(ContentMigrationItem<IUser> item) => true;
        }

        private class GenericFilter<TContent> : ContentFilterBase<TContent>
            where TContent : IContentReference
        {
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
            [Fact]
            public void AddFromType()
            {
                var genericFilter = new GenericFilter<IUser>();
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
            [Fact]
            public void BuildAndCreateObject()
            {
                // Arrange
                var serviceProvider = new ServiceCollection().BuildServiceProvider();
                var hook = new UserFilter();
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
                var hook = new UserFilter();
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
                    .Add<IContentFilter<IUser>, IUser>(provider => new UserFilter())
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

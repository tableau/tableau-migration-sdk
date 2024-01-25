using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Engine.Hooks.Mappings;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Mappings
{
    public class ContentMappingBuilderTests
    {
        private class UserMapping : ContentMappingBase<IUser>
        {
            public override Task<ContentMappingContext<IUser>?> ExecuteAsync(ContentMappingContext<IUser> ctx, CancellationToken cancel)
                => ctx.ToTask();
        }

        private class GenericMapping<TContent> : ContentMappingBase<TContent>
            where TContent : IContentReference
        {
            public override Task<ContentMappingContext<TContent>?> ExecuteAsync(ContentMappingContext<TContent> ctx, CancellationToken cancel)
                => ctx.ToTask();
        }

        public class Clear
        {
            [Fact]
            public void ClearsFactories()
            {
                var builder = new ContentMappingBuilder().Add<UserMapping, IUser>()
                    .Clear();

                var result = builder.Build();
                Assert.Empty(result.GetHooks<IContentMapping<IUser>>());
            }
        }

        public class Add
        {
            [Fact]
            public void AddFromType()
            {
                var builder = new ContentMappingBuilder().Add<UserMapping, IUser>();

                var result = builder.Build();
                Assert.NotNull(result);

                var mappings = result.GetHooks<IContentMapping<IUser>>();
                Assert.Single(mappings);
            }
        }

        public class AddMappingGenericInstance
        {
            [Fact]
            public void AddFromType()
            {
                var genericMapping = new GenericMapping<IUser>();
                var builder = new ContentMappingBuilder().Add(genericMapping);

                var result = builder.Build();
                Assert.NotNull(result);

                var mappings = result.GetHooks<IContentMapping<IUser>>();
                Assert.Single(mappings);
            }
        }

        public class AddGenericFactory
        {
            [Fact]
            public void AddFromType()
            {
                var builder = new ContentMappingBuilder().Add<GenericMapping<IUser>, IUser>();

                var result = builder.Build();
                Assert.NotNull(result);

                var mappings = result.GetHooks<IContentMapping<IUser>>();
                Assert.Single(mappings);
            }
        }

        public class AddGenericFactoryAsInterface
        {
            [Fact]
            public void AddFromType()
            {
                var builder = new ContentMappingBuilder().Add<IContentMapping<IUser>, IUser>();

                var result = builder.Build();
                Assert.NotNull(result);

                var mappings = result.GetHooks<IContentMapping<IUser>>();
                Assert.Single(mappings);
            }
        }

        public class AddCallback
        {
            [Fact]
            public void AddFromType()
            {
                var builder = new ContentMappingBuilder().Add<IUser>(
                    (ContentMappingContext<IUser> context, CancellationToken cancel)
                    => context.MapTo(new ContentLocation()).ToTask());

                var result = builder.Build();
                Assert.NotNull(result);

                var mappings = result.GetHooks<IContentMapping<IUser>>();
                Assert.Single(mappings);
            }
        }

        public class AddRawFilterInterface : AutoFixtureTestBase
        {
            private class RawHook : IContentMapping<IUser>
            {
                public Task<ContentMappingContext<IUser>?> ExecuteAsync(ContentMappingContext<IUser> ctx, CancellationToken cancel)
                {
                    throw new NotImplementedException();
                }
            }

            [Fact]
            public async Task IgnoresRawHookInterface()
            {
                // Arrange
                var mockedContext = Create<ContentMappingContext<IUser>>();
                var collection = new ServiceCollection()
                    .AddSingleton<RawHook>();
                var builder = new ContentMappingBuilder()
                        .Add<RawHook, IUser>();
                var factories = builder.Build().GetHooks<IContentMapping<IUser>>();
                Assert.Single(factories);
                var hook = factories[0].Create<IContentMapping<IUser>>(collection.BuildServiceProvider());

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
                var hook = new UserMapping();
                var hookFactory = new ContentMappingBuilder()
                    .Add(hook)
                    .Build()
                    .GetHooks<IContentMapping<IUser>>();
                Assert.Single(hookFactory);
                IContentMapping<IUser>? firstScopeHook1;
                IContentMapping<IUser>? firstScopeHook2;
                IContentMapping<IUser>? lastScopeHook;

                // Act
                using (var scope1 = serviceProvider.CreateScope())
                {
                    firstScopeHook1 = hookFactory[0].Create<IContentMapping<IUser>>(scope1.ServiceProvider);
                    firstScopeHook2 = hookFactory[0].Create<IContentMapping<IUser>>(scope1.ServiceProvider);
                }

                using (var scope2 = serviceProvider.CreateScope())
                {
                    lastScopeHook = hookFactory[0].Create<IContentMapping<IUser>>(scope2.ServiceProvider);
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
                    .AddSingleton<UserMapping>()
                    .BuildServiceProvider();
                var hookFactory = new ContentMappingBuilder()
                    .Add<UserMapping, IUser>()
                    .Build()
                    .GetHooks<IContentMapping<IUser>>();
                Assert.Single(hookFactory);
                IContentMapping<IUser>? firstScopeHook1;
                IContentMapping<IUser>? firstScopeHook2;
                IContentMapping<IUser>? lastScopeHook;

                // Act
                using (var scope1 = serviceProvider.CreateScope())
                {
                    firstScopeHook1 = hookFactory[0].Create<IContentMapping<IUser>>(scope1.ServiceProvider);
                    firstScopeHook2 = hookFactory[0].Create<IContentMapping<IUser>>(scope1.ServiceProvider);
                }

                using (var scope2 = serviceProvider.CreateScope())
                {
                    lastScopeHook = hookFactory[0].Create<IContentMapping<IUser>>(scope2.ServiceProvider);
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
                    .AddScoped<UserMapping>()
                    .BuildServiceProvider();
                var hookFactory = new ContentMappingBuilder()
                    .Add<UserMapping, IUser>()
                    .Build()
                    .GetHooks<IContentMapping<IUser>>();
                Assert.Single(hookFactory);
                IContentMapping<IUser>? firstScopeHook1;
                IContentMapping<IUser>? firstScopeHook2;
                IContentMapping<IUser>? lastScopeHook;

                // Act
                using (var scope1 = serviceProvider.CreateScope())
                {
                    firstScopeHook1 = hookFactory[0].Create<IContentMapping<IUser>>(scope1.ServiceProvider);
                    firstScopeHook2 = hookFactory[0].Create<IContentMapping<IUser>>(scope1.ServiceProvider);
                }

                using (var scope2 = serviceProvider.CreateScope())
                {
                    lastScopeHook = hookFactory[0].Create<IContentMapping<IUser>>(scope2.ServiceProvider);
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
                    .AddTransient<UserMapping>()
                    .BuildServiceProvider();
                var hookFactory = new ContentMappingBuilder()
                    .Add<UserMapping, IUser>()
                    .Build()
                    .GetHooks<IContentMapping<IUser>>();
                Assert.Single(hookFactory);
                IContentMapping<IUser>? firstScopeHook1;
                IContentMapping<IUser>? firstScopeHook2;
                IContentMapping<IUser>? lastScopeHook;

                // Act
                using (var scope1 = serviceProvider.CreateScope())
                {
                    firstScopeHook1 = hookFactory[0].Create<IContentMapping<IUser>>(scope1.ServiceProvider);
                    firstScopeHook2 = hookFactory[0].Create<IContentMapping<IUser>>(scope1.ServiceProvider);
                }

                using (var scope2 = serviceProvider.CreateScope())
                {
                    lastScopeHook = hookFactory[0].Create<IContentMapping<IUser>>(scope2.ServiceProvider);
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
                var hook = new UserMapping();
                var serviceProvider = new ServiceCollection().BuildServiceProvider();
                var hookFactory = new ContentMappingBuilder()
                    .Add<IContentMapping<IUser>, IUser>(provider => hook)
                    .Build()
                    .GetHooks<IContentMapping<IUser>>();
                Assert.Single(hookFactory);
                IContentMapping<IUser>? firstScopeHook1;
                IContentMapping<IUser>? firstScopeHook2;
                IContentMapping<IUser>? lastScopeHook;

                // Act
                using (var scope1 = serviceProvider.CreateScope())
                {
                    firstScopeHook1 = hookFactory[0].Create<IContentMapping<IUser>>(scope1.ServiceProvider);
                    firstScopeHook2 = hookFactory[0].Create<IContentMapping<IUser>>(scope1.ServiceProvider);
                }

                using (var scope2 = serviceProvider.CreateScope())
                {
                    lastScopeHook = hookFactory[0].Create<IContentMapping<IUser>>(scope2.ServiceProvider);
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
                var hookFactory = new ContentMappingBuilder()
                    .Add<IContentMapping<IUser>, IUser>(provider => new UserMapping())
                    .Build()
                    .GetHooks<IContentMapping<IUser>>();
                Assert.Single(hookFactory);
                IContentMapping<IUser>? firstScopeHook1;
                IContentMapping<IUser>? firstScopeHook2;
                IContentMapping<IUser>? lastScopeHook;

                // Act
                using (var scope1 = serviceProvider.CreateScope())
                {
                    firstScopeHook1 = hookFactory[0].Create<IContentMapping<IUser>>(scope1.ServiceProvider);
                    firstScopeHook2 = hookFactory[0].Create<IContentMapping<IUser>>(scope1.ServiceProvider);
                }

                using (var scope2 = serviceProvider.CreateScope())
                {
                    lastScopeHook = hookFactory[0].Create<IContentMapping<IUser>>(scope2.ServiceProvider);
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
                    .AddSingleton<UserMapping>()
                    .BuildServiceProvider();
                var hookFactory = new ContentMappingBuilder()
                    .Add<IContentMapping<IUser>, IUser>(provider => provider.GetRequiredService<UserMapping>())
                    .Build()
                    .GetHooks<IContentMapping<IUser>>();
                Assert.Single(hookFactory);
                IContentMapping<IUser>? firstScopeHook1;
                IContentMapping<IUser>? firstScopeHook2;
                IContentMapping<IUser>? lastScopeHook;

                // Act
                using (var scope1 = serviceProvider.CreateScope())
                {
                    firstScopeHook1 = hookFactory[0].Create<IContentMapping<IUser>>(scope1.ServiceProvider);
                    firstScopeHook2 = hookFactory[0].Create<IContentMapping<IUser>>(scope1.ServiceProvider);
                }

                using (var scope2 = serviceProvider.CreateScope())
                {
                    lastScopeHook = hookFactory[0].Create<IContentMapping<IUser>>(scope2.ServiceProvider);
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
                    .AddScoped<UserMapping>()
                    .BuildServiceProvider();
                var hookFactory = new ContentMappingBuilder()
                    .Add<IContentMapping<IUser>, IUser>(provider => provider.GetRequiredService<UserMapping>())
                    .Build()
                    .GetHooks<IContentMapping<IUser>>();
                Assert.Single(hookFactory);
                IContentMapping<IUser>? firstScopeHook1;
                IContentMapping<IUser>? firstScopeHook2;
                IContentMapping<IUser>? lastScopeHook;

                // Act
                using (var scope1 = serviceProvider.CreateScope())
                {
                    firstScopeHook1 = hookFactory[0].Create<IContentMapping<IUser>>(scope1.ServiceProvider);
                    firstScopeHook2 = hookFactory[0].Create<IContentMapping<IUser>>(scope1.ServiceProvider);
                }

                using (var scope2 = serviceProvider.CreateScope())
                {
                    lastScopeHook = hookFactory[0].Create<IContentMapping<IUser>>(scope2.ServiceProvider);
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
                    .AddTransient<UserMapping>()
                    .BuildServiceProvider();
                var hookFactory = new ContentMappingBuilder()
                    .Add<IContentMapping<IUser>, IUser>(provider => provider.GetRequiredService<UserMapping>())
                    .Build()
                    .GetHooks<IContentMapping<IUser>>();
                Assert.Single(hookFactory);
                IContentMapping<IUser>? firstScopeHook1;
                IContentMapping<IUser>? firstScopeHook2;
                IContentMapping<IUser>? lastScopeHook;

                // Act
                using (var scope1 = serviceProvider.CreateScope())
                {
                    firstScopeHook1 = hookFactory[0].Create<IContentMapping<IUser>>(scope1.ServiceProvider);
                    firstScopeHook2 = hookFactory[0].Create<IContentMapping<IUser>>(scope1.ServiceProvider);
                }

                using (var scope2 = serviceProvider.CreateScope())
                {
                    lastScopeHook = hookFactory[0].Create<IContentMapping<IUser>>(scope2.ServiceProvider);
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
                static Task<ContentMappingContext<IUser>?> callback(ContentMappingContext<IUser> context,
                    CancellationToken cancel)
                    => context.MapTo(new ContentLocation()).ToTask();
                var serviceProvider = new ServiceCollection().BuildServiceProvider();
                var hookFactory = new ContentMappingBuilder()
                    .Add<IUser>(callback)
                    .Build()
                    .GetHooks<IContentMapping<IUser>>();
                Assert.Single(hookFactory);
                IMigrationHook<ContentMappingContext<IUser>>? firstScopeHook1;
                IMigrationHook<ContentMappingContext<IUser>>? firstScopeHook2;
                IMigrationHook<ContentMappingContext<IUser>>? lastScopeHook;

                // Act
                using (var scope1 = serviceProvider.CreateScope())
                {
                    firstScopeHook1 = hookFactory[0].Create<IMigrationHook<ContentMappingContext<IUser>>>(scope1.ServiceProvider);
                    firstScopeHook2 = hookFactory[0].Create<IMigrationHook<ContentMappingContext<IUser>>>(scope1.ServiceProvider);
                }

                using (var scope2 = serviceProvider.CreateScope())
                {
                    lastScopeHook = hookFactory[0].Create<IMigrationHook<ContentMappingContext<IUser>>>(scope2.ServiceProvider);
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

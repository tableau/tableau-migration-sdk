using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Resources
{
    public class SharedResourceTests
    {
        public class LocalizedClass
        {
            private readonly ISharedResourcesLocalizer _localizer;

            public LocalizedClass(ISharedResourcesLocalizer localizer)
            {
                _localizer = localizer;
            }

            public string GetLocalizedString() => _localizer[SharedResourceKeys.ProductName];

            public string GetLocalizedString(string key) => _localizer[key];
        }

        private static ServiceCollection CreateLocalizationTestServices()
        {
            var services = new ServiceCollection();
            services.AddTableauMigrationSdk();
            services.AddTransient<LocalizedClass>();

            return services;
        }

        public class ISharedResourcesLocalizerTests
        {
            [Fact]
            public void Default()
            {
                var services = CreateLocalizationTestServices();
                var container = services.BuildServiceProvider();

                var loc = container.GetRequiredService<LocalizedClass>();

                Assert.Equal("Tableau Migration SDK", loc.GetLocalizedString());
            }

            [Fact]
            public void CustomResourcePath()
            {
                var services = CreateLocalizationTestServices();
                services.AddLocalization(opts => opts.ResourcesPath = "SpecialPath");

                var container = services.BuildServiceProvider();

                var loc = container.GetRequiredService<LocalizedClass>();

                Assert.Equal("Tableau Migration SDK", loc.GetLocalizedString());
            }
        }

        public class SharedResourceKeysTests
        {

            [Fact]
            public void Error_on_bad_input()
            {
                var services = CreateLocalizationTestServices();
                var container = services.BuildServiceProvider();

                var localizer = container.GetRequiredService<ISharedResourcesLocalizer>();

                var bad = localizer["ThisStringDoesNotExist"];
                Assert.True(bad.ResourceNotFound);
            }

            [Fact]
            public void All_key_valid()
            {
                var services = CreateLocalizationTestServices();
                var container = services.BuildServiceProvider();

                var localizer = container.GetRequiredService<ISharedResourcesLocalizer>();

                var constants = typeof(SharedResourceKeys)
                    .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .Where(f => f.IsLiteral && !f.IsInitOnly)
                    .ToList();

                Assert.All(constants, (constant) =>
                {
                    // If this fails, it means the key is not in the SharedResource.rex file

                    var localized = localizer[constant.Name];
                    Assert.False(localized.ResourceNotFound);
                });
            }
        }
    }

    
}

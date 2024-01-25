using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Tests
{
    /// <summary>
    /// An <see cref="ISharedResourcesLocalizer"/> instance with with default behavior.
    /// </summary>
    public sealed class TestSharedResourcesLocalizer : ISharedResourcesLocalizer, IDisposable
    {
        public static readonly TestSharedResourcesLocalizer Instance = new();

        private readonly ServiceProvider _serviceProvider;
        private readonly ISharedResourcesLocalizer _localizer;

        public TestSharedResourcesLocalizer()
        {
            _serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddLocalization()
                .AddSharedResourcesLocalization()
                .BuildServiceProvider();

            _localizer = _serviceProvider.GetRequiredService<ISharedResourcesLocalizer>();
        }

        public void Dispose() => _serviceProvider.Dispose();

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => _localizer.GetAllStrings(includeParentCultures);

        public LocalizedString this[string name] => _localizer[name];
        public LocalizedString this[string name, params object[] arguments] => _localizer[name, arguments];
    }
}

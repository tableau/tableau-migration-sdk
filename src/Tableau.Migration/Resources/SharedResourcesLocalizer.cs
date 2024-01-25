using System.Collections.Generic;
using Microsoft.Extensions.Localization;

namespace Tableau.Migration.Resources
{
    internal sealed class SharedResourcesLocalizer : ISharedResourcesLocalizer
    {
        private readonly IStringLocalizer _innerLocalizer;

        internal SharedResourcesLocalizer(IStringLocalizer innerLocalizer)
        {
            _innerLocalizer = innerLocalizer;
        }

        public LocalizedString this[string name] => _innerLocalizer[name];

        public LocalizedString this[string name, params object[] arguments] => _innerLocalizer[name, arguments];

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => _innerLocalizer.GetAllStrings(includeParentCultures);
    }
}

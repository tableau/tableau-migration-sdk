using Moq;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Tests
{
    /// <summary>
    /// A mocked <see cref="ISharedResourcesLocalizer"/> instance that is setup with with default behavior.
    /// </summary>
    public sealed class MockSharedResourcesLocalizer : Mock<ISharedResourcesLocalizer>
    {
        private readonly ISharedResourcesLocalizer _localizer = TestSharedResourcesLocalizer.Instance;

        public MockSharedResourcesLocalizer()
        {
            Setup(l => l.GetAllStrings(It.IsAny<bool>()))
                .Returns(_localizer.GetAllStrings);

            Setup(l => l[It.IsAny<string>()])
                .Returns<string>(value => _localizer[value]);
        }
    }
}

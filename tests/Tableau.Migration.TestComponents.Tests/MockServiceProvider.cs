using AutoFixture;
using AutoFixture.Kernel;
using Moq;

namespace Tableau.Migration.TestComponents.Tests
{
    public sealed class MockServiceProvider : Mock<IServiceProvider>
    {
        public MockServiceProvider(IFixture fixture)
        {
            Setup(x => x.GetService(It.IsAny<Type>())).Returns((Type t) => fixture.Create(t, new SpecimenContext(fixture)));
        }
    }
}

using Tableau.Migration.Net;

namespace Tableau.Migration.Tests.Unit.Api.Simulation.Rest.Net.Responses
{
    public abstract class ResponseBuilderTestBase : AutoFixtureTestBase
    {
        internal readonly HttpContentSerializer Serializer = HttpContentSerializer.Instance;
    }
}

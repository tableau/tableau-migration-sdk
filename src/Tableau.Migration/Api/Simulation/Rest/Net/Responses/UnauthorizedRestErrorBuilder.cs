using System.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal record UnauthorizedRestErrorBuilder : StaticRestErrorBuilder
    {
        public UnauthorizedRestErrorBuilder()
            : base(HttpStatusCode.Unauthorized, 0, "Unauthorized", "Unauthorized")
        { }
    }
}

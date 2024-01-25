using System.Net;
using Tableau.Migration.Api.Rest.Models;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal record StaticRestErrorBuilder(HttpStatusCode StatusCode, int SubCode, string Summary, string Detail) : IRestErrorBuilder
    {
        public Error BuildError(out HttpStatusCode statusCode)
        {
            statusCode = StatusCode;

            return new()
            {
                Code = StatusCode.ToRestErrorCode(SubCode),
                Summary = Summary,
                Detail = Detail
            };
        }
    }
}

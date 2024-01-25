using System.Net;
using Tableau.Migration.Api.Rest.Models;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal interface IRestErrorBuilder
    {
        Error BuildError(out HttpStatusCode statusCode);
    }
}

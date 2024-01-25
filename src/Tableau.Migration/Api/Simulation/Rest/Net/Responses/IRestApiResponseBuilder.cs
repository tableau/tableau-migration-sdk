using Tableau.Migration.Net.Simulation.Responses;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal interface IRestApiResponseBuilder : IResponseBuilder
    {
        IRestErrorBuilder? ErrorOverride { get; set; }
    }
}

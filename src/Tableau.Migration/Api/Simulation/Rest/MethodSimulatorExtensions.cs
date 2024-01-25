using System;
using System.Net;
using Tableau.Migration.Api.Simulation.Rest.Net.Responses;
using Tableau.Migration.Net.Simulation;

namespace Tableau.Migration.Api.Simulation.Rest
{
    internal static class MethodSimulatorExtensions
    {
        public static MethodSimulator RespondWithError(this MethodSimulator method, HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
            int subCode = 0,
            string summary = "Error Summary",
            string detail = "Error Detail")
        {
            return method.RespondWithError(new StaticRestErrorBuilder(statusCode, subCode, summary, detail));
        }

        public static MethodSimulator RespondWithError(this MethodSimulator method, StaticRestErrorBuilder errorBuilder)
        {
            if (method.ResponseBuilder is not IRestApiResponseBuilder restResponseBuilder)
            {
                throw new ArgumentException("Method simulator must have a REST API response builder to generate a REST API error.");
            }

            restResponseBuilder.ErrorOverride = errorBuilder;
            return method;
        }
    }
}

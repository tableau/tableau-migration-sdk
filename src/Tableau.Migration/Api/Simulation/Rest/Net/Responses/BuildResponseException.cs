using System;
using System.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal class BuildResponseException : Exception
    {
        public BuildResponseException(HttpStatusCode statusCode, int subCode, string summary, string detail)
        {
            StatusCode = statusCode;
            SubCode = subCode;
            Summary = summary;
            Detail = detail;
        }

        public HttpStatusCode StatusCode { get; set; }
        public int SubCode { get; set; }
        public string Summary { get; set; }
        public string Detail { get; set; }

    }
}

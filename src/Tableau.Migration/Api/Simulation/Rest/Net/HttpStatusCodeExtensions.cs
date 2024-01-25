using System.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net
{
    internal static class HttpStatusCodeExtensions
    {
        public static string ToRestErrorCode(this HttpStatusCode httpStatusCode, int subCode = 0) => httpStatusCode.ToRestErrorCode(subCode.ToString().PadLeft(3, '0'));

        public static string ToRestErrorCode(this HttpStatusCode httpStatusCode, string subCode = "000") => ((int)httpStatusCode).ToString() + subCode;
    }
}

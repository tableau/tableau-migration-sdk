using System;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Api.Rest
{
    /// <summary>
    /// Class representing an error from a Tableau REST API
    /// </summary>
    public class RestException : Exception
    {
        /// <summary>
        /// Gets the request URI from Tableau API.
        /// </summary>
        public readonly Uri? RequestUri;

        /// <summary>
        /// Gets the error code from Tableau API.
        /// </summary>
        public readonly string? Code;

        /// <summary>
        /// Gets the error detail from Tableau API.
        /// </summary>
        public readonly string? Detail;

        /// <summary>
        /// Gets the error summary from Tableau API.
        /// </summary>
        public readonly string? Summary;

        /// <summary>
        /// Creates a new <see cref="RestException"/> instance.
        /// </summary>
        /// <param name="requestUri">The request URI that generated the current error.</param>
        /// <param name="error">The <see cref="Error"/> returned from the Tableau API.</param>
        /// <param name="sharedResourcesLocalizer">A string localizer.</param>
        public RestException(
            Uri? requestUri,
            Error error,
            ISharedResourcesLocalizer sharedResourcesLocalizer)
            : base(FormatError(requestUri, error, sharedResourcesLocalizer))
        {
            RequestUri = requestUri;
            Code = error.Code;
            Detail = error.Detail;
            Summary = error.Summary;
        }

        private static string FormatError(
            Uri? requestUri,
            Error error,
            ISharedResourcesLocalizer sharedResourcesLocalizer)
        {
            var nullValue = $"<{sharedResourcesLocalizer[SharedResourceKeys.NullValue]}>";

            return string.Format(
                sharedResourcesLocalizer[SharedResourceKeys.RestExceptionContent],
                requestUri,
                error.Code ?? nullValue,
                error.Summary ?? nullValue,
                error.Detail ?? nullValue);
        }
    }
}

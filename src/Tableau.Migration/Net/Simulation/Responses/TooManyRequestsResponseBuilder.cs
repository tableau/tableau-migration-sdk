using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Net.Simulation.Responses
{
    /// <summary>
    /// <see cref="EmptyResponseBuilder"/> that simulates a 429 - Too Many Requests response.
    /// </summary>
    public class TooManyRequestsResponseBuilder : EmptyResponseBuilder
    {
        private readonly RetryConditionHeaderValue? _retryAfter;

        /// <summary>
        /// Creates a new <see cref="TooManyRequestsResponseBuilder"/> object.
        /// </summary>
        /// <param name="retryAfter">A time span to supply for the "Retry-After" header, or null to not include a "Retry-After" header.</param>
        /// <param name="requiresAuthentication">Whether the response requires an authenticated request.</param>
        public TooManyRequestsResponseBuilder(TimeSpan? retryAfter = null, bool requiresAuthentication = true)
            : this(retryAfter is null ? null : new RetryConditionHeaderValue(retryAfter.Value), requiresAuthentication)
        { }

        /// <summary>
        /// Creates a new <see cref="TooManyRequestsResponseBuilder"/> object.
        /// </summary>
        /// <param name="retryAfter">A target date to supply for the "Retry-After" header, or null to not include a "Retry-After" header.</param>
        /// <param name="requiresAuthentication">Whether the response requires an authenticated request.</param>
        public TooManyRequestsResponseBuilder(DateTimeOffset? retryAfter = null, bool requiresAuthentication = true)
            : this(retryAfter is null ? null : new RetryConditionHeaderValue(retryAfter.Value), requiresAuthentication)
        { }

        private TooManyRequestsResponseBuilder(RetryConditionHeaderValue? retryAfter, bool requiresAuthentication = true)
            : base(HttpStatusCode.TooManyRequests, requiresAuthentication)
        {
            _retryAfter = retryAfter;
        }

        /// <inheritdoc />
        public override async Task<HttpResponseMessage> RespondAsync(HttpRequestMessage request, CancellationToken cancel)
        {
            var response = await base.RespondAsync(request, cancel).ConfigureAwait(false);

            if (_retryAfter is not null)
            {
                response.Headers.RetryAfter = _retryAfter;
            }

            return response;
        }
    }
}

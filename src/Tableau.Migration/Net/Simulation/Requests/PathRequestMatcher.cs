using System;
using System.Net.Http;

namespace Tableau.Migration.Net.Simulation.Requests
{
    /// <summary>
    /// Default <see cref="IPathRequestMatcher"/> implementation.
    /// </summary>
    /// <param name="Method"><inheritdoc /></param>
    /// <param name="RequestUrl"><inheritdoc /></param>
    public record PathRequestMatcher(HttpMethod Method, Uri RequestUrl) : IPathRequestMatcher
    {
        /// <inheritdoc />
        public bool Matches(HttpRequestMessage request)
        {
            if (request.Method != Method)
            {
                return false;
            }

            if (request.RequestUri is null)
            {
                return false;
            }

            if (!BaseUrlComparer.Instance.Equals(request.RequestUri, RequestUrl))
            {
                return false;
            }

            if (!string.Equals(request.RequestUri.TrimmedPath(), RequestUrl.TrimmedPath()))
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc />
        public bool Equals(IRequestMatcher? other)
        {
            if (other is null || other is not IPathRequestMatcher pathMatcher)
            {
                return false;
            }

            return Equals(new(pathMatcher.Method, pathMatcher.RequestUrl));
        }

        /// <summary>
        /// Returns a string which represents the object instance.
        /// </summary>
        public override string ToString() => $"{nameof(RequestUrl)}: {RequestUrl}, {nameof(Method)}: {Method}";
    }
}

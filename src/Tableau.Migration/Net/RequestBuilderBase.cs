using System;
using System.Collections.Generic;
using System.Linq;

namespace Tableau.Migration.Net
{
    internal abstract class RequestBuilderBase<TBuilder> : IRequestBuilder<TBuilder>
        where TBuilder : RequestBuilderBase<TBuilder>
    {
        private readonly IHttpRequestBuilderFactory _requestBuilderFactory;
        private readonly IQueryStringBuilder _query;
        private readonly Uri _baseUri;
        private readonly string? _path;

        public RequestBuilderBase(Uri baseUri, string path, IHttpRequestBuilderFactory requestBuilderFactory, IQueryStringBuilder query)
        {
            _baseUri = baseUri;
            _path = path;
            _requestBuilderFactory = requestBuilderFactory;
            _query = query;
        }

        public RequestBuilderBase(Uri baseUri, string path, IHttpRequestBuilderFactory requestBuilderFactory)
            : this(baseUri, path, requestBuilderFactory, new QueryStringBuilder())
        { }

        public TBuilder WithQuery(Action<IQueryStringBuilder> query)
        {
            query(_query);
            return (TBuilder)this;
        }

        public TBuilder WithQuery(string key, string value)
        {
            _query.AddOrUpdate(key, value);
            return (TBuilder)this;
        }

        internal virtual Uri BuildUri()
        {
            var uriBuilder = new UriBuilder(_baseUri);

            var segments = _path?.Split('/', StringSplitOptions.RemoveEmptyEntries)?.ToList() ?? new List<string>();

            BuildPath(segments);

            if (segments.Any())
            {
                uriBuilder.Path = String.Join("/", segments).TrimStartPath();
            }

            BuildQuery(_query);

            if (!_query.IsEmpty)
            {
                uriBuilder.Query = _query.Build();
            }

            return uriBuilder.Uri;
        }

        protected virtual void BuildPath(List<string> segments)
        { }

        protected virtual void BuildQuery(IQueryStringBuilder query) { }

        public IHttpDeleteRequestBuilder ForDeleteRequest() => _requestBuilderFactory.CreateDeleteRequest(BuildUri());

        public IHttpGetRequestBuilder ForGetRequest() => _requestBuilderFactory.CreateGetRequest(BuildUri());

        public IHttpPatchRequestBuilder ForPatchRequest() => _requestBuilderFactory.CreatePatchRequest(BuildUri());

        public IHttpPostRequestBuilder ForPostRequest() => _requestBuilderFactory.CreatePostRequest(BuildUri());

        public IHttpPutRequestBuilder ForPutRequest() => _requestBuilderFactory.CreatePutRequest(BuildUri());
    }
}

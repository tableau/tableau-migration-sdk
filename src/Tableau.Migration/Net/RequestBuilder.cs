using System;

namespace Tableau.Migration.Net
{
    internal class RequestBuilder : RequestBuilderBase<RequestBuilder>
    {
        public RequestBuilder(Uri baseUri, string path, IHttpRequestBuilderFactory requestBuilderFactory, IQueryStringBuilder query)
            : base(baseUri, path, requestBuilderFactory, query)
        { }

        public RequestBuilder(Uri baseUri, string path, IHttpRequestBuilderFactory requestBuilderFactory)
            : base(baseUri, path, requestBuilderFactory)
        { }
    }
}

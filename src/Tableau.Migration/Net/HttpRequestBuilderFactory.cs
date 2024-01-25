using System;

namespace Tableau.Migration.Net
{
    internal class HttpRequestBuilderFactory : IHttpRequestBuilderFactory
    {
        private readonly IHttpClient _httpClient;
        private readonly IHttpContentSerializer _serializer;

        public HttpRequestBuilderFactory(IHttpClient httpClient, IHttpContentSerializer serializer)
        {
            _httpClient = httpClient;
            _serializer = serializer;
        }

        public IHttpDeleteRequestBuilder CreateDeleteRequest(Uri uri) => new HttpDeleteRequestBuilder(uri, _httpClient);

        public IHttpGetRequestBuilder CreateGetRequest(Uri uri) => new HttpGetRequestBuilder(uri, _httpClient);

        public IHttpPatchRequestBuilder CreatePatchRequest(Uri uri) => new HttpPatchRequestBuilder(uri, _httpClient, _serializer);

        public IHttpPostRequestBuilder CreatePostRequest(Uri uri) => new HttpPostRequestBuilder(uri, _httpClient, _serializer);

        public IHttpPutRequestBuilder CreatePutRequest(Uri uri) => new HttpPutRequestBuilder(uri, _httpClient, _serializer);
    }
}

using System.Net.Http;
using Tableau.Migration.Config;

namespace Tableau.Migration.Net.Policies
{
    internal class SimpleCachedRequestTimeoutPolicyBuilder
        : SimpleCachedHttpPolicyBuilder, IHttpPolicyBuilder
    {
        private const string RequestKey = "request";
        private const string FileTransferRequestKey = "fileTransferRequest";

        private readonly IConfigReader _configReader;

        public SimpleCachedRequestTimeoutPolicyBuilder(
            RequestTimeoutPolicyBuilder policyBuilder,
            IConfigReader configReader)
            : base(policyBuilder)
        {
            _configReader = configReader;
        }

        protected override string GetRequestKey(
            HttpRequestMessage httpRequest)
        {
            // Double Key - Shared for every request/file-transfer request
            return RequestTimeoutPolicyBuilder.IsFileTransferRequest(httpRequest) ? RequestKey : FileTransferRequestKey;
        }

        protected override string GetCachedConfigurationKey()
        {
            var resilienceOptions = _configReader
                .Get()
                .Network
                .Resilience;

            return $"{resilienceOptions.PerRequestTimeout}_{resilienceOptions.PerFileTransferRequestTimeout}";
        }
    }
}

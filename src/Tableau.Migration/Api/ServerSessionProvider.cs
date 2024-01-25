using System;
using Tableau.Migration.Api.Models;

namespace Tableau.Migration.Api
{
    internal sealed class ServerSessionProvider : IServerSessionProvider
    {
        private readonly ITableauServerVersionProvider _versionProvider;
        private readonly IAuthenticationTokenProvider _tokenProvider;

        public TableauServerVersion? Version => _versionProvider.Version;
        public string? AuthenticationToken => _tokenProvider.Token;

        public Guid? SiteId { get; private set; }
        public string? SiteContentUrl { get; private set; }

        public Guid? UserId { get; private set; }

        public ServerSessionProvider(
            ITableauServerVersionProvider versionProvider,
            IAuthenticationTokenProvider tokenProvider)
        {
            _versionProvider = versionProvider;
            _tokenProvider = tokenProvider;
        }

        public void SetCurrentUserAndSite(ISignInResult signInResult)
            => SetCurrentUserAndSite(signInResult.UserId, signInResult.SiteId, signInResult.SiteContentUrl, signInResult.Token);

        public void SetCurrentUserAndSite(Guid userId, Guid siteId, string siteContentUrl, string authenticationToken)
        {
            SiteId = siteId;
            SiteContentUrl = siteContentUrl;
            UserId = userId;

            _tokenProvider.Set(authenticationToken);
        }

        public void ClearCurrentUserAndSite()
        {
            SiteId = null;
            SiteContentUrl = null;
            UserId = null;

            _tokenProvider.Clear();
        }

        public void SetVersion(TableauServerVersion version) => _versionProvider.Set(version);
    }
}

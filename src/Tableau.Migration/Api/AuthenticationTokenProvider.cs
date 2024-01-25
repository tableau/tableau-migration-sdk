using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Api
{
    internal sealed class AuthenticationTokenProvider : IAuthenticationTokenProvider
    {
        public event AsyncEventHandler? RefreshRequestedAsync;

        public string? Token { get; private set; }

        public void Set(string token) => Token = token;

        public void Clear() => Token = null;

        public async Task RequestRefreshAsync(CancellationToken cancel)
        {
            if (RefreshRequestedAsync is not null)
                await RefreshRequestedAsync.Invoke(cancel).ConfigureAwait(false);
        }
    }
}

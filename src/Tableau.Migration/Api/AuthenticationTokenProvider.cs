//
//  Copyright (c) 2024, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the "License") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Api
{
    internal sealed class AuthenticationTokenProvider : IAuthenticationTokenProvider
    {
        private readonly SemaphoreSlim _tokenSemaphore = new(1, 1);

        private string? _token;

        /// <inheritdoc />
        public event RefreshAuthenticationTokenDelegate? RefreshRequestedAsync;

        /// <inheritdoc />
        public async Task<string?> GetAsync(CancellationToken cancel)
        {
            await _tokenSemaphore.WaitAsync(cancel).ConfigureAwait(false);
            try
            {
                return _token;
            }
            finally
            {
                _tokenSemaphore.Release();
            }
        }

        /// <inheritdoc />
        public async Task SetAsync(string token, CancellationToken cancel)
        {
            await _tokenSemaphore.WaitAsync(cancel).ConfigureAwait(false);
            try
            {
                _token = token;
            }
            finally
            {
                _tokenSemaphore.Release();
            }
        }

        /// <inheritdoc />
        public async Task ClearAsync(CancellationToken cancel)
        {
            await _tokenSemaphore.WaitAsync(cancel).ConfigureAwait(false);
            try
            {
                _token = null;
            }
            finally
            {
                _tokenSemaphore.Release();
            }
        }

        /// <inheritdoc />
        public async Task RequestRefreshAsync(string? previousToken, CancellationToken cancel)
        {
            if (RefreshRequestedAsync is null)
            {
                return;
            }

            await _tokenSemaphore.WaitAsync(cancel).ConfigureAwait(false);
            try
            {
                // Another thread refreshed the token while we waited for the refresh lock.
                if(!string.Equals(previousToken, _token, StringComparison.Ordinal))
                {
                    return;
                }

                var newTokenResult = await RefreshRequestedAsync.Invoke(cancel).ConfigureAwait(false);
                
                if(newTokenResult.Success)
                {
                    _token = newTokenResult.Value;
                }
            }
            finally
            {
                _tokenSemaphore.Release();
            }
        }
    }
}

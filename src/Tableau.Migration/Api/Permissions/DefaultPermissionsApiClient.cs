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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Config;
using Tableau.Migration.Content.Permissions;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Api.Permissions
{
    internal class DefaultPermissionsApiClient : IDefaultPermissionsApiClient
    {
        private readonly ConcurrentDictionary<string, IPermissionsApiClient> _contentTypeClients = new(StringComparer.OrdinalIgnoreCase);

        private readonly IPermissionsApiClientFactory _permissionsClientFactory;
        private readonly ILogger<DefaultPermissionsApiClient> _logger;
        private readonly ISharedResourcesLocalizer _localizer;

        public DefaultPermissionsApiClient(
            IPermissionsApiClientFactory permissionsClientFactory,
            DefaultPermissionsContentTypeOptions options,
            ILoggerFactory loggerFactory,
            ISharedResourcesLocalizer localizer)
        {
            _permissionsClientFactory = permissionsClientFactory;
            _logger = loggerFactory.CreateLogger<DefaultPermissionsApiClient>();
            _localizer = localizer;
            foreach (var contentTypeUrlSegment in options.UrlSegments)
                EnsurePermissionsClient(contentTypeUrlSegment);
        }

        private IPermissionsApiClient EnsurePermissionsClient(string contentTypeUrlSegment)
        {
            return _contentTypeClients.GetOrAdd(contentTypeUrlSegment, s =>
            {
                var uriBuilder = new PermissionsUriBuilder("projects", $"default-permissions/{s}");
                var client = _permissionsClientFactory.Create(uriBuilder);
                _contentTypeClients[s] = client;
                return client;
            });
        }

        public Task<IResult<IPermissions>> GetPermissionsAsync(
            string contentTypeUrlSegment,
            Guid projectId,
            CancellationToken cancel)
            => EnsurePermissionsClient(contentTypeUrlSegment).GetPermissionsAsync(projectId, cancel);

        public Task<IResult<IPermissions>> CreatePermissionsAsync(
            string contentTypeUrlSegment,
            Guid projectId,
            IPermissions permissions,
            CancellationToken cancel)
            => EnsurePermissionsClient(contentTypeUrlSegment).CreatePermissionsAsync(projectId, permissions, cancel);

        public Task<IResult> DeleteCapabilityAsync(
            string contentTypeUrlSegment,
            Guid projectId,
            Guid granteeId,
            GranteeType granteeType,
            ICapability capability,
            CancellationToken cancel)
            => EnsurePermissionsClient(contentTypeUrlSegment).DeleteCapabilityAsync(projectId, granteeId, granteeType, capability, cancel);

        public Task<IResult> DeleteAllPermissionsAsync(
            string contentTypeUrlSegment,
            Guid projectId,
            IPermissions permissions,
            CancellationToken cancel)
            => EnsurePermissionsClient(contentTypeUrlSegment).DeleteAllPermissionsAsync(projectId, permissions, cancel);

        public Task<IResult<IPermissions>> UpdatePermissionsAsync(
            string contentTypeUrlSegment,
            Guid projectId,
            IPermissions permissions,
            CancellationToken cancel)
            => EnsurePermissionsClient(contentTypeUrlSegment).UpdatePermissionsAsync(projectId, permissions, cancel);

        public async Task<IResult<IImmutableDictionary<string, IPermissions>>> GetAllPermissionsAsync(
            Guid projectId,
            CancellationToken cancel)
        {
            var resultBuilder = new ResultBuilder();

            var defaultPermissions = ImmutableDictionary.CreateBuilder<string, IPermissions>(StringComparer.OrdinalIgnoreCase);

            var getPermissionsTasks = _contentTypeClients
                .ToDictionary(c => c.Key, c => GetPermissionsAsync(c.Key, projectId, cancel));

            var getPermissionsResults = await Task.WhenAll(getPermissionsTasks.Values).ConfigureAwait(false);

            if (getPermissionsResults is null)
            {
                resultBuilder.Add(
                    Result.Failed(
                        new Exception($"Failed to get default project permissions for one or more content types for {projectId}.")));
            }
            else
            {
                foreach (var getPermissionsTask in getPermissionsTasks)
                {
                    var getPermissionsResult = getPermissionsTask.Value.Result;

                    if (!getPermissionsResult.Success)
                    {
                        resultBuilder.Add(getPermissionsResult);
                        _logger.LogWarning(
                            new AggregateException(getPermissionsResult.Errors),
                            _localizer[SharedResourceKeys.FailedToGetDefaultPermissionsMessage],
                            getPermissionsTask.Key,
                            projectId);
                        continue;
                    }
                    
                    defaultPermissions.Add(getPermissionsTask.Key, getPermissionsResult.Value);
                }
            }

            if (defaultPermissions.Any())
            {
                return Result<IImmutableDictionary<string, IPermissions>>.Succeeded(defaultPermissions.ToImmutable());
            }

            return Result<IImmutableDictionary<string, IPermissions>>.Failed(
                resultBuilder.Build().Errors);
        }

        public async Task<IResult<IImmutableDictionary<string, IPermissions>>> UpdateAllPermissionsAsync(
            Guid projectId,
            IReadOnlyDictionary<string, IPermissions> permissions,
            CancellationToken cancel)
        {
            var resultBuilder = new ResultBuilder();

            var updatedPermissions = ImmutableDictionary.CreateBuilder<string, IPermissions>(StringComparer.OrdinalIgnoreCase);

            var updatePermissionsTasks = new Dictionary<string, Task<IResult<IPermissions>>>(StringComparer.OrdinalIgnoreCase);

            foreach (var permission in permissions)
            {
                updatePermissionsTasks.Add(permission.Key, UpdatePermissionsAsync(permission.Key, projectId, permission.Value, cancel));
            }

            var updatePermissionsResults = await Task.WhenAll(updatePermissionsTasks.Values).ConfigureAwait(false);

            if (updatePermissionsResults is null)
            {
                resultBuilder.Add(
                    Result.Failed(
                        new Exception($"Failed to update default project permissions for one or more content types for {projectId}.")));
            }
            else
            {
                foreach (var updatePermissionsTask in updatePermissionsTasks)
                {
                    var updatePermissionsResult = updatePermissionsTask.Value.Result;

                    if (!updatePermissionsResult.Success)
                        resultBuilder.Add(updatePermissionsResult);
                    else
                        updatedPermissions.Add(updatePermissionsTask.Key, updatePermissionsResult.Value);
                }
            }

            var result = resultBuilder.Build();

            if (!result.Success)
                return Result<IImmutableDictionary<string, IPermissions>>.Failed(result.Errors);

            return Result<IImmutableDictionary<string, IPermissions>>.Succeeded(updatedPermissions.ToImmutable());
        }
    }
}

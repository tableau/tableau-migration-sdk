//
//  Copyright (c) 2025, Salesforce, Inc.
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
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Tableau.Migration.Api.Rest
{
    internal static class RestUrlPrefixes
    {
        private static readonly IImmutableDictionary<Type, string> _urlPrefixesByType = new Dictionary<Type, string>(InheritedTypeComparer.Instance)
        {
            [typeof(IDataSourcesApiClient)] = RestUrlKeywords.DataSources,
            [typeof(IFlowsApiClient)] = RestUrlKeywords.Flows,
            [typeof(IGroupsApiClient)] = RestUrlKeywords.Groups,
            [typeof(IJobsApiClient)] = RestUrlKeywords.Jobs,
            [typeof(ISchedulesApiClient)] = RestUrlKeywords.Schedules,
            [typeof(IProjectsApiClient)] = RestUrlKeywords.Projects,
            [typeof(ISitesApiClient)] = RestUrlKeywords.Sites,
            [typeof(IUsersApiClient)] = RestUrlKeywords.Users,
            [typeof(IViewsApiClient)] = RestUrlKeywords.Views,
            [typeof(IWorkbooksApiClient)] = RestUrlKeywords.Workbooks,
            [typeof(ITasksApiClient)] = RestUrlKeywords.Tasks,
            [typeof(ICustomViewsApiClient)] = RestUrlKeywords.CustomViews,
            [typeof(ISubscriptionsApiClient)] = RestUrlKeywords.Subscriptions
        }
        .ToImmutableDictionary(InheritedTypeComparer.Instance);

        public static string GetUrlPrefix<TApiClient>()
            where TApiClient : IContentApiClient
            => GetUrlPrefix(typeof(TApiClient));

        public static string GetUrlPrefix(Type apiClientType)
        {
            if (_urlPrefixesByType.TryGetValue(apiClientType, out var urlPrefix))
                return urlPrefix;

            throw new ArgumentException($"No REST URL prefix was found for type {apiClientType.Name}", nameof(apiClientType));
        }
    }
}
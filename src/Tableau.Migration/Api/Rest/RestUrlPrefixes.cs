using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Tableau.Migration.Api.Rest
{
    internal static class RestUrlPrefixes
    {
        private static readonly IImmutableDictionary<Type, string> _urlPrefixesByType = new Dictionary<Type, string>(InheritedTypeComparer.Instance)
        {
            [typeof(IDataSourcesApiClient)] = DataSources,
            [typeof(IGroupsApiClient)] = Groups,
            [typeof(IJobsApiClient)] = Jobs,
            [typeof(IProjectsApiClient)] = Projects,
            [typeof(ISitesApiClient)] = Sites,
            [typeof(IUsersApiClient)] = Users,
            [typeof(IWorkbooksApiClient)] = Workbooks,
            [typeof(IViewsApiClient)] = Views,
        }
        .ToImmutableDictionary(InheritedTypeComparer.Instance);

        public const string Sites = "sites";
        public const string Projects = "projects";
        public const string Users = "users";
        public const string Groups = "groups";
        public const string DataSources = "datasources";
        public const string Workbooks = "workbooks";
        public const string Jobs = "jobs";
        public const string Views = "views";
        public const string FileUploads = "fileUploads";
        public const string Content = "content";

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

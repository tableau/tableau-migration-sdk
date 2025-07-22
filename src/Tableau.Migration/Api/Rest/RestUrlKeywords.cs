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

using System.Collections.Generic;

namespace Tableau.Migration.Api.Rest
{
    internal static class RestUrlKeywords
    {
        public const string ApplyKeychain = "applykeychain";
        public const string Auth = "auth";
        public const string Connections = "connections";
        public const string Content = "content";
        public const string Current = "current";
        public const string CustomViews = "customviews";
        public const string DataSources = "datasources";
        public const string Databases = "databases";
        public const string Default = "default";
        public const string DefaultPermissions = "default-permissions";
        public const string ExtractRefreshes = "extractRefreshes";
        public const string Extracts = "extracts";
        public const string Favorites = "favorites";
        public const string FileUploads = "fileUploads";
        public const string Flows = "flows";
        public const string Groups = "groups";
        public const string GroupSets = "groupsets";
        public const string Import = "import";
        public const string Jobs = "jobs";
        public const string Labels = "labels";
        public const string Permissions = "permissions";
        public const string Projects = "projects";
        public const string RetrieveKeychain = "retrievekeychain";
        public const string RetrieveSavedCreds = "retrieveSavedCreds";
        public const string Schedules = "schedules";
        public const string ServerInfo = "serverinfo";
        public const string Sessions = "sessions";
        public const string SignIn = "signin";
        public const string SignOut = "signout";
        public const string Sites = "sites";
        public const string SiteAuthConfigs = "site-auth-configurations";
        public const string Subscriptions = "subscriptions";
        public const string Tables = "tables";
        public const string Tags = "tags";
        public const string Tasks = "tasks";
        public const string UploadSavedCreds = "uploadSavedCreds";
        public const string Users = "users";
        public const string VirtualConnections = "virtualconnections";
        public const string Views = "views";
        public const string Workbooks = "workbooks";

        public static HashSet<string> All => TypeExtensions.GetAllPublicStringValues(typeof(RestUrlKeywords));
    }
}

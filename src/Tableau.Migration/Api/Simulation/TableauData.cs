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
using System.Linq;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Permissions;
using Tableau.Migration.Net.Rest;
using CloudResponse = Tableau.Migration.Api.Rest.Models.Responses.Cloud;
using ServerResponse = Tableau.Migration.Api.Rest.Models.Responses.Server;

namespace Tableau.Migration.Api.Simulation
{
    /// <summary>
    /// Object that represents data stored by a Tableau Server/Cloud site to be accessible to APIs through a <see cref="TableauApiSimulator"/>.
    /// </summary>
    public sealed class TableauData
    {
        /// <summary>
        /// Gets the default domain of the site.
        /// </summary>
        public string DefaultDomain { get; set; }

        /// <summary>
        /// Gets the flag indicating whether the current Tableau Data is for Tableau Server (true) or Tableau Cloud (false).
        /// </summary>
        public bool IsTableauServer { get; set; }

        /// <summary>
        /// Gets or sets the "All Users" group.
        /// </summary>
        public GroupsResponse.GroupType AllUsersGroup { get; set; }

        /// <summary>
        /// Gets or sets the "Default" project.
        /// </summary>
        public ProjectsResponse.ProjectType DefaultProject { get; set; }

        /// <summary>
        /// Gets or sets the groups.
        /// </summary>
        public ConcurrentSet<GroupsResponse.GroupType> Groups { get; set; } = new();

        /// <summary>
        /// Gets or sets the projects.
        /// </summary>
        public ConcurrentSet<ProjectsResponse.ProjectType> Projects { get; set; } = new();

        /// <summary>
        /// Gets or sets the server information.
        /// </summary>
        public ServerInfoResponse.ServerInfoType ServerInfo { get; set; }

        /// <summary>
        /// Gets or sets the sign in result.
        /// </summary>
        public SignInResponse.CredentialsType? SignIn { get; set; }

        /// <summary>
        /// Gets or sets the sites.
        /// </summary>
        public ConcurrentSet<SiteResponse.SiteType> Sites { get; set; } = new();

        /// <summary>
        /// Gets or sets the workbooks.
        /// </summary>
        public ConcurrentSet<WorkbookResponse.WorkbookType> Workbooks { get; set; } = new();

        /// <summary>
        /// Gets or sets the workbook fileData contents, by ID.
        /// </summary>
        public ConcurrentDictionary<Guid, byte[]> WorkbookFiles { get; set; } = new();

        /// <summary>
        /// Gets or sets the jobs.
        /// </summary>
        public ConcurrentSet<JobResponse.JobType> Jobs { get; set; } = new();

        /// <summary>
        /// Gets or sets the schedules.
        /// </summary>
        public ConcurrentSet<ServerResponse.ScheduleResponse.ScheduleType> Schedules { get; set; } = new();

        /// <summary>
        /// Gets or sets the schedules extract refresh tasks.
        /// </summary>
        public ConcurrentSet<ServerResponse.ScheduleExtractRefreshTasksResponse.ExtractType> ScheduleExtractRefreshTasks { get; set; } = new();

        /// <summary>
        /// Gets or sets the Tableau Server extract refresh tasks.
        /// </summary>
        public ConcurrentSet<ServerResponse.ExtractRefreshTasksResponse.TaskType> ServerExtractRefreshTasks { get; set; } = new();

        /// <summary>
        /// Gets or sets the Tableau Cloud extract refresh tasks.
        /// </summary>
        public ConcurrentSet<CloudResponse.ExtractRefreshTasksResponse.TaskType> CloudExtractRefreshTasks { get; set; } = new();

        /// <summary>
        /// Gets or sets the jobs.
        /// </summary>
        public ConcurrentSet<ImportJobResponse.ImportJobType> UserImportJobs { get; set; } = new();

        /// <summary>
        /// Gets or sets the users.
        /// </summary>
        public ConcurrentSet<UsersResponse.UserType> Users { get; set; } = new();

        /// <summary>
        /// Gets or sets the users.
        /// </summary>
        public ConcurrentSet<WorkbookResponse.WorkbookType.ViewReferenceType> Views { get; set; } = new();

        /// <summary>
        /// Gets or sets the data source permissions.
        /// </summary>
        public ConcurrentDictionary<Guid, PermissionsType> DataSourcePermissions { get; set; } = new();

        /// <summary>
        /// Gets or sets the project permissions.
        /// </summary>
        public ConcurrentDictionary<Guid, PermissionsType> ProjectPermissions { get; set; } = new();

        /// <summary>
        /// Gets or sets the workbook permissions.
        /// </summary>
        public ConcurrentDictionary<Guid, PermissionsType> WorkbookPermissions { get; set; } = new();

        /// <summary>
        /// Gets or sets the view permissions.
        /// </summary>
        public ConcurrentDictionary<Guid, PermissionsType> ViewPermissions { get; set; } = new();

        /// <summary>
        /// Gets or sets the project default permissions.
        /// </summary>
        public ConcurrentDictionary<Guid, IDictionary<string, PermissionsType>> DefaultProjectPermissions { get; set; } = new();

        /// <summary>
        /// Gets or sets the data sources.
        /// </summary>
        public ConcurrentSet<DataSourceResponse.DataSourceType> DataSources { get; set; } = new();

        /// <summary>
        /// Gets or sets the data source fileData contents, by ID.
        /// </summary>
        public ConcurrentDictionary<Guid, byte[]> DataSourceFiles { get; set; } = new();

        /// <summary>
        /// Gets or sets the uploaded files, by session Id.
        /// </summary>
        public ConcurrentDictionary<string, IEnumerable<byte>> Files { get; set; } = new();

        /// <summary>
        /// Gets or sets the custom views.
        /// </summary>
        public ConcurrentSet<CustomViewResponse.CustomViewType> CustomViews { get; set; } = new();


        /// <summary>
        /// Gets or sets the custom view fileData contents, by ID.
        /// </summary>
        public ConcurrentDictionary<Guid, byte[]> CustomViewFiles { get; set; } = new();



        /// <summary>
        /// Gets or sets the custom view default users contents, by ID.
        /// </summary>
        public ConcurrentDictionary<Guid, List<UsersWithCustomViewAsDefaultViewResponse.UserType>> CustomViewDefaultUsers { get; set; } = new();

        #region - Relationships -

        /// <summary>
        /// Gets the users that belong to groups, by ID.
        /// </summary>
        public ConcurrentDictionary<Guid, ConcurrentSet<Guid>> GroupUsers { get; set; } = new();

        /// <summary>
        /// Gets the groups that users belong to, by ID.
        /// </summary>
        public ConcurrentDictionary<Guid, ConcurrentSet<Guid>> UserGroups { get; set; } = new();

        /// <summary>
        /// Gets the extract refresh tasks that users belong to, by ID.
        /// </summary>
        public ConcurrentDictionary<Guid, ConcurrentSet<Guid>> ScheduleExtracts { get; set; } = new();

        #endregion

        /// <summary>
        /// Creates a new <see cref="TableauData"/> object.
        /// </summary>
        public TableauData(UsersResponse.UserType? defaultSignedInUser, string defaultDomain = Constants.LocalDomain)
        {
            DefaultDomain = defaultDomain;

            var signInSiteId = Guid.NewGuid();

            // Set some default values for common requests so we don't have to create them for every call.
            // The corresponding properties can be set to null or other values if needed for testing.
            ServerInfo = new ServerInfoResponse.ServerInfoType("3.18", "2023.2", "20232.23.0224.1230");

            // Default "All Users" group
            AllUsersGroup = CreateAllUsersGroup();

            // Add "All Users" to group list responses.
            AddGroup(AllUsersGroup);

            if (defaultSignedInUser is not null)
            {
                SignIn = new SignInResponse.CredentialsType(
                    siteId: signInSiteId,
                    contentUrl: "",
                    userId: defaultSignedInUser.Id,
                    token: Guid.NewGuid().ToString());

                AddUser(defaultSignedInUser);
                AddUserToGroup(defaultSignedInUser.Id, AllUsersGroup.Id);
            }

            // Create and add the sign-in site.
            AddSignInSite(signInSiteId);

            DefaultProject = CreateDefaultProject(defaultSignedInUser?.Id ?? Guid.NewGuid());

            AddProject(DefaultProject);
        }

        /// <summary>
        /// Adds a group.
        /// </summary>
        /// <param name="group">The group to add.</param>
        public void AddGroup(GroupsResponse.GroupType group)
        {
            if (GroupUsers.ContainsKey(group.Id))
            {
                return;
            }

            Groups.Add(group);
            GroupUsers.TryAdd(group.Id, new());
        }

        /// <summary>
        /// Adds an user.
        /// </summary>
        /// <param name="user">The user to add.</param>
        public UsersResponse.UserType AddUser(UsersResponse.UserType user)
        {
            if (UserGroups.ContainsKey(user.Id))
            {
                return Users.Single(u => u.Id == user.Id);
            }

            user.Domain ??= GetUserDomain(user);

            Users.Add(user);
            UserGroups.TryAdd(user.Id, new());

            return user;
        }

        /// <summary>
        /// Adds a job.
        /// </summary>
        /// <param name="job">The job to add.</param>
        public void AddJob(JobResponse.JobType job)
        {
            var jobId = job.Id.ToUrlSegment();

            var existing = Jobs.SingleOrDefault(j => j.Id.ToUrlSegment() == jobId);

            if (existing is not null)
                Jobs.Remove(existing);

            Jobs.Add(job);
        }

        /// <summary>
        /// Adds a project.
        /// </summary>
        /// <param name="project">The project to add.</param>
        /// <param name="parentProject">A parent project to add and link the project to.</param>
        /// <param name="useSignInOwner">
        /// Whether or not to assign the signed in user as the owner of the project.</param>
        public ProjectsResponse.ProjectType AddProject(ProjectsResponse.ProjectType project,
            ProjectsResponse.ProjectType? parentProject = null,
            bool useSignInOwner = false)
        {
            if (parentProject is not null)
            {
                project.ParentProjectId = parentProject.Id.ToString();
                TryAddProject(parentProject);
            }

            TryAddProject(project);

            return project;

            void TryAddProject(ProjectsResponse.ProjectType p)
            {
                if (useSignInOwner && SignIn?.User is not null)
                {
                    p.Owner = new() { Id = SignIn.User.Id };
                }

                var projectId = p.Id;

                if (Projects.Any(p => p.Id == projectId))
                {
                    return;
                }

                Projects.Add(p);
            }
        }

        internal static UsersResponse.UserType.DomainType? GetUserDomain(UsersResponse.UserType user)
            => GetUserDomain(user?.Name);

        internal static UsersResponse.UserType.DomainType? GetUserDomain(string? username)
        {
            if (username is null)
                return null;

            var userNameSplit = username.Split(Constants.DomainNameSeparator);

            if (userNameSplit.Length == 2)
            {
                return new()
                {
                    Name = userNameSplit[0],
                };
            }

            return null;
        }

        /// <summary>
        /// Adds a user to a group.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="groupId">The group ID.</param>
        public void AddUserToGroup(Guid userId, Guid groupId)
        {
            UserGroups[userId].Add(groupId);
            GroupUsers[groupId].Add(userId);
        }

        /// <summary>
        /// Removes a user from a group.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="groupId">The group ID.</param>
        public void RemoveUserFromGroup(Guid userId, Guid groupId)
        {
            UserGroups.TryGetValue(userId, out ConcurrentSet<Guid>? groups);

            if (groups is not null)
            {
                UserGroups[userId].Remove(groupId);
            }

            GroupUsers.TryGetValue(groupId, out ConcurrentSet<Guid>? users);

            if (users is not null)
            {
                GroupUsers[groupId].Remove(userId);
            }
        }

        /// <summary>
        /// Adds a user to a group.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="group">The group.</param>
        public void AddUserToGroup(UsersResponse.UserType user, GroupsResponse.GroupType group)
        {
            AddUser(user);
            AddGroup(group);

            AddUserToGroup(user.Id, group.Id);
        }

        /// <summary>
        /// Adds a schedule.
        /// </summary>
        /// <param name="schedule">The schedule to add.</param>
        public ServerResponse.ScheduleResponse.ScheduleType AddSchedule(
            ServerResponse.ScheduleResponse.ScheduleType schedule)
        {
            if (ScheduleExtracts.ContainsKey(schedule.Id))
            {
                return Schedules.Single(s => s.Id == schedule.Id);
            }

            Schedules.Add(schedule);
            ScheduleExtracts.TryAdd(schedule.Id, new());

            return schedule;
        }

        /// <summary>
        /// Add a schedule extract refresh task to a schedule.
        /// </summary>
        /// <param name="extract">The schedule extract refresh task.</param>
        /// <param name="schedule">The schedule.</param>
        public void AddExtractToSchedule(
            ServerResponse.ScheduleExtractRefreshTasksResponse.ExtractType extract,
            ServerResponse.ScheduleResponse.ScheduleType schedule)
        {
            schedule = AddSchedule(schedule);
            ScheduleExtractRefreshTasks.Add(extract);

            ScheduleExtracts[schedule.Id].Add(extract.Id);
        }

        internal void UpdateFile(string sessionId, byte[] chunk)
        {
            Files.AddOrUpdate(sessionId, chunk, (_, __) => __.Concat(chunk));
        }

        private void AddSignInSite(Guid id)
            => Sites.Add(
                new()
                {
                    Id = id,
                    ContentUrl = "",
                    Name = "Default",
                    ExtractEncryptionMode = ExtractEncryptionModes.Disabled
                });

        private static GroupsResponse.GroupType CreateAllUsersGroup()
            => new()
            {
                Id = Guid.NewGuid(),
                Name = "All Users",
                Domain = new GroupsResponse.GroupType.DomainType
                {
                    Name = "local"
                }
            };

        private static ProjectsResponse.ProjectType CreateDefaultProject(Guid ownerId)
            => new()
            {
                Id = Guid.NewGuid(),
                Name = "Default",
                ParentProjectId = null,
                ContentPermissions = ContentPermissions.ManagedByOwner,
                Owner = new() { Id = ownerId }
            };

        /// <summary>
        /// Adds a data source to simulated dataset.
        /// </summary>
        /// <param name="dataSource">The <see cref="DataSourceResponse.DataSourceType"/> metadata</param>
        /// <param name="fileData">A byte array representing the data source. If null, empty array is used</param>
        internal void AddDataSource(
            DataSourceResponse.DataSourceType dataSource,
            byte[]? fileData)
        {
            DataSources.Add(dataSource);
            DataSourceFiles[dataSource.Id] = fileData ?? Array.Empty<byte>();
        }

        /// <summary>
        /// Adds a data source to simulated dataset.
        /// </summary>
        /// <param name="dataSource">The <see cref="DataSourceResponse.DataSourceType"/> metadata</param>
        /// <param name="fileData">A byte array representing the data source. If null, empty array is used</param>
        /// <param name="certificationNote">Certification note to use</param>
        internal void AddDataSource(
            DataSourcesResponse.DataSourceType dataSource,
            byte[]? fileData,
            string certificationNote = ""
            )
            => AddDataSource(
                new DataSourceResponse.DataSourceType(dataSource)
                {
                    CertificationNote = certificationNote
                },
                fileData);

        /// <summary>
        /// Adds a workbook to simulated dataset.
        /// </summary>
        /// <param name="workbook">The <see cref="WorkbookResponse.WorkbookType"/> metadata</param>
        /// <param name="fileData">A byte array representing the workbook. If null, empty array is used</param>
        internal void AddWorkbook(
            WorkbookResponse.WorkbookType workbook,
            byte[]? fileData)
        {
            Workbooks.Add(workbook);
            WorkbookFiles[workbook.Id] = fileData ?? Array.Empty<byte>();
        }

        /// <summary>
        /// Adds a view to simulated dataset.
        /// </summary>
        /// <param name="view">The <see cref="WorkbookResponse.WorkbookType.ViewReferenceType"/> metadata</param>
        internal void AddView(
            WorkbookResponse.WorkbookType.ViewReferenceType view)
        {
            Views.Add(view);
        }

        /// <summary>
        /// Adds a custom view to simulated dataset.
        /// </summary>
        /// <param name="customView">The <see cref="CustomViewResponse.CustomViewType"/> metadata</param>
        /// <param name="fileData">A byte array representing the custom view. If null, empty array is used</param>
        internal void AddCustomView(
            CustomViewResponse.CustomViewType customView,
            byte[]? fileData)
        {
            CustomViews.Add(customView);
            CustomViewFiles[customView.Id] = fileData ?? [];
        }
        internal void AddDefaultProjectPermissions(Guid projectId, string contentTypeUrlSegment, PermissionsType permissions)
        {
            DefaultProjectPermissions.AddOrUpdate(
                projectId,
                id =>
                {
                    DefaultProjectPermissions[id] = new Dictionary<string, PermissionsType>(StringComparer.OrdinalIgnoreCase)
                    {
                        [contentTypeUrlSegment] = permissions
                    };
                    return DefaultProjectPermissions[id];
                },
                (id, value) =>
                {
                    value[contentTypeUrlSegment] = permissions;
                    return value;
                });
        }

        internal void AddDefaultProjectPermissions(Guid projectId, string contentTypeUrlSegment, IPermissions permissions)
            => AddDefaultProjectPermissions(projectId, contentTypeUrlSegment, new PermissionsType
            {
                GranteeCapabilities = permissions.GranteeCapabilities.ToGranteeCapabilityTypes()
            });

        internal void AddProjectPermissions(IProjectType project, PermissionsType permission)
            => AddContentTypePermissions(RestUrlPrefixes.Projects, project.Id, permission);

        internal void AddDataSourcePermissions(IDataSourceType dataSource, PermissionsType permission)
            => AddContentTypePermissions(RestUrlPrefixes.DataSources, dataSource.Id, permission);

        internal void AddWorkbookPermissions(IWorkbookType workbook, PermissionsType permission)
            => AddContentTypePermissions(RestUrlPrefixes.Workbooks, workbook.Id, permission);

        internal void AddViewPermissions(IViewReferenceType view, PermissionsType permission)
            => AddContentTypePermissions(RestUrlPrefixes.Views, view.Id, permission);

        internal void AddViewPermissions(Guid viewId, PermissionsType permission)
            => AddContentTypePermissions(RestUrlPrefixes.Views, viewId, permission);

        internal ConcurrentDictionary<Guid, PermissionsType> GetContentTypePermissions(string contentTypeUrlPrefix)
        {
            return contentTypeUrlPrefix.ToLower() switch
            {
                RestUrlPrefixes.DataSources => DataSourcePermissions,
                RestUrlPrefixes.Projects => ProjectPermissions,
                RestUrlPrefixes.Workbooks => WorkbookPermissions,
                RestUrlPrefixes.Views => ViewPermissions,
                _ => throw new ArgumentException($"No permissions are set up for content type {contentTypeUrlPrefix}.", nameof(contentTypeUrlPrefix)),
            };
        }

        internal TContent AddContentTypePermissions<TContent>(string contentTypeUrlPrefix, Func<ICollection<TContent>> getContent, Func<TContent> createContent, PermissionsType permission)
            where TContent : IRestIdentifiable, INamedContent
        {
            var allContent = getContent();
            var content = createContent();

            allContent.Add(content);

            permission.ContentItem = new PermissionsContentItemType
            {
                Id = content.Id,
                Name = content.Name
            };

            AddContentTypePermissions(contentTypeUrlPrefix, content.Id, permission);

            return content;
        }

        internal void AddContentTypePermissions(string contentTypeUrlPrefix, Guid contentId, PermissionsType permission)
        {
            var contentTypePermissions = GetContentTypePermissions(contentTypeUrlPrefix);

            contentTypePermissions.AddOrUpdate(contentId, _ => permission, (_, __) => permission);
        }
    }
}

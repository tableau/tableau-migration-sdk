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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Api.Simulation;
using Tableau.Migration.Config;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Permissions;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Net;
using Tableau.Migration.Tests.Content.Permissions;
using Tableau.Migration.Tests.Unit.Content.Permissions;
using Xunit;
using Server = Tableau.Migration.Api.Rest.Models.Responses.Server;

namespace Tableau.Migration.Tests.Simulation
{
    /// <summary>
    /// <see cref="SimulationTestBase"/> implementation for test classes that require source and cloud destination servers (i.e. for migrations testing).
    /// </summary>
    public abstract class ServerToCloudSimulationTestBase : SimulationTestBase
    {
        protected TableauApiSimulator SourceApi { get; }

        protected TableauSiteConnectionConfiguration SourceSiteConfig { get; }

        public TableauApiEndpointConfiguration SourceEndpointConfig { get; }

        protected TableauApiSimulator CloudDestinationApi { get; }

        protected TableauSiteConnectionConfiguration CloudDestinationSiteConfig { get; }

        public TableauApiEndpointConfiguration CloudDestinationEndpointConfig { get; }

        public ServerToCloudSimulationTestBase(string sourceUrl = "https://source", string destinationUrl = "https://destination")
        {
            UsersResponse.UserType CreateDefaultUser()
            {
                var defaultUser = AutoFixture.Build<UsersResponse.UserType>()
                    .With(u => u.SiteRole, SiteRoles.ServerAdministrator)
                    .With(u => u.Name, $"SignedInUserName{Guid.NewGuid()}")
                    .With(u => u.FullName, $"SignedInUserFullName{Guid.NewGuid()}")
                    .Create();

                // Wrong - Work item in in backlog
                defaultUser.Name = $"{defaultUser.Domain!.Name}\\{defaultUser.Name}";

                return defaultUser;
            }

            SourceApi = RegisterTableauServerApiSimulator(sourceUrl, CreateDefaultUser());
            SourceSiteConfig = BuildSiteConnectionConfiguration(SourceApi);
            SourceEndpointConfig = new(SourceSiteConfig);

            CloudDestinationApi = RegisterTableauCloudApiSimulator(destinationUrl, CreateDefaultUser());
            CloudDestinationSiteConfig = BuildSiteConnectionConfiguration(CloudDestinationApi);
            CloudDestinationEndpointConfig = new(CloudDestinationSiteConfig);
        }

        protected virtual bool UsersBatchImportEnabled { get; } = true;

        protected override IServiceCollection ConfigureServices(IServiceCollection services)
        {
            var mockedConfigReader = Freeze<Mock<IConfigReader>>();
            mockedConfigReader.Setup(x => x.Get<IUser>())
                .Returns(new ContentTypesOptions
                {
                    BatchPublishingEnabled = UsersBatchImportEnabled
                });
            mockedConfigReader.Setup(x => x.Get())
                .Returns(new MigrationSdkOptions());

            return services.AddTableauMigrationSdk()
                .AddSingleton(mockedConfigReader.Object);
        }

        #region - Asserts -

        protected static IContentReference? MapReference<TContent>(IMigrationManifest manifest, Guid sourceId)
        {
            var entries = manifest.Entries.ForContentType<TContent>();
            var entry = entries.Single(e => e.Source.Id == sourceId);
            return entry.Destination;
        }

        protected static GranteeCapabilityType MapGrantee(IMigrationManifest manifest, GranteeCapabilityType capability)
            => capability.GranteeType switch
            {
                GranteeType.User => new()
                {
                    User = new()
                    {
                        Id = MapReference<IUser>(manifest, capability.GranteeId)!.Id
                    },
                    Capabilities = capability.Capabilities
                },
                GranteeType.Group => new()
                {
                    Group = new()
                    {
                        Id = MapReference<IGroup>(manifest, capability.GranteeId)!.Id
                    },
                    Capabilities = capability.Capabilities
                },
                _ => throw new ArgumentException($"Grantee Type {capability.GranteeType} is invalid.", nameof(capability)),
            };

        protected static void AssertPermissionsMigrated(IMigrationManifest manifest, PermissionsType? sourcePermissions, PermissionsType? destinationPermissions)
        {
            Assert.NotNull(sourcePermissions);
            Assert.NotNull(destinationPermissions);

            var sourceGranteeCapabilities = sourcePermissions.GranteeCapabilities;
            var destinationGranteeCapabilities = destinationPermissions.GranteeCapabilities;

            Assert.NotNull(sourceGranteeCapabilities);
            Assert.NotNull(destinationGranteeCapabilities);

            var mappedGranteeCapabilities = sourceGranteeCapabilities.Select(g => ServerToCloudSimulationTestBase.MapGrantee(manifest, g));

            var comparer = new IGranteeCapabilityComparer(false);

            Assert.Equal(mappedGranteeCapabilities.ToIGranteeCapabilities(), destinationGranteeCapabilities.ToIGranteeCapabilities(), comparer);
        }

        #endregion

        #region - Prepare Source Data (Users) -

        protected (List<UsersResponse.UserType> NonSupportUsers, List<UsersResponse.UserType> SupportUsers) PrepareSourceUsersData(int? count = null)
        {
            var allSiteRoles = SiteRoles.GetAll();
            var numSourceUsers = count ?? (int)Math.Ceiling(ContentTypesOptions.Defaults.BATCH_SIZE * 2.5);

            var nonSupportUsers = new List<UsersResponse.UserType>();
            var supportUsers = new List<UsersResponse.UserType>();
            for (int i = 0; i < numSourceUsers; i++)
            {
                var user = Create<UsersResponse.UserType>();
                user.Domain = i % 2 == 0 ? new UsersResponse.UserType.DomainType { Name = "local" } : Create<UsersResponse.UserType.DomainType>();

                // Wrong - Work item in in backlog
                // This is not how the response should be built. The domain does not go into the name for UsersResponse.UserType
                // Replace domain name in the user name
                if (user.Name != null)
                {
                    var currentDomainName = TableauData.GetUserDomain(user)?.Name;
                    if (currentDomainName != null)
                    {
                        user.Name = user.Name.Replace(currentDomainName, user.Domain.Name);
                    }
                }
                user.SiteRole = allSiteRoles[i % allSiteRoles.Count];

                if (user.SiteRole == SiteRoles.SupportUser)
                {
                    supportUsers.Add(user);
                }
                else
                {
                    nonSupportUsers.Add(user);
                }

                SourceApi.Data.Users.Add(user);
            }

            return (nonSupportUsers, supportUsers);
        }

        #endregion - Prepare Source Data (Users) -

        #region - Prepare Source Data (Groups) -

        protected List<GroupsResponse.GroupType> PrepareSourceGroupsData(int? count = null)
        {
            var groups = new List<GroupsResponse.GroupType>();
            var allSiteRoles = SiteRoles.GetAll();
            var numSourceGroups = count ?? (int)Math.Ceiling(ContentTypesOptions.Defaults.BATCH_SIZE * 1.5);

            for (int i = 0; i < numSourceGroups; i++)
            {
                var group = Create<GroupsResponse.GroupType>();
                group.Domain = i % 2 == 0 ? new GroupsResponse.GroupType.DomainType { Name = "local" } : Create<GroupsResponse.GroupType.DomainType>();
                group.Import!.SiteRole = allSiteRoles[i % allSiteRoles.Count];
                groups.Add(group);
                SourceApi.Data.Groups.Add(group);
            }

            return groups;
        }

        #endregion - Prepare Source Data (Groups) -

        #region - Prepare Source Data (Projects) -

        protected List<ProjectsResponse.ProjectType> PrepareSourceProjectsData()
        {
            var projects = new List<ProjectsResponse.ProjectType>();

            // This will create 5 projects
            var numSourceProjects = 5;

            // Get all users that are not support users, and all groups
            var users = SourceApi.Data.Users.Where(u => u.SiteRole != SiteRoles.SupportUser).ToImmutableArray();
            var groups = SourceApi.Data.Groups;

            // Create the projects in a hierarchy. Project 0 is the only root project. 
            // Every next project is a child of the previous one
            for (var i = 0; i < numSourceProjects; i++)
            {
                var project = Create<ProjectsResponse.ProjectType>();

                if (i >= 1)
                    project.ParentProjectId = projects[^1].Id.ToString();
                else
                    project.ParentProjectId = null;

                project.Owner = new() { Id = users[i % users.Length].Id };

                projects.Add(project);
                SourceApi.Data.AddProject(project);
            }

            foreach (var project in projects)
            { // loop over all the projects created
                foreach (var contentType in DefaultPermissionsContentTypeUrlSegments.GetAll()) // loop over all the known content types and get their default permissions
                { // loop over all the known content types and get their default permissions

                    // Add the default permissions for the given content type to the project
                    SourceApi.Data.AddDefaultProjectPermissions(project.Id, contentType, new PermissionsType
                    {
                        ContentItem = new PermissionsContentItemType
                        {
                            Id = project.Id,
                            Name = project.Name
                        },
                        GranteeCapabilities = users.Select(u => new GranteeCapabilityType
                        {
                            User = new GranteeCapabilityType.UserType
                            {
                                Id = u.Id
                            },
                            Capabilities = CreateMany<ICapability>(2).Select(c => new CapabilityType(c)).ToArray()
                        })
                        .Concat(groups.Select(g => new GranteeCapabilityType
                        {
                            Group = new GranteeCapabilityType.GroupType
                            {
                                Id = g.Id
                            },
                            Capabilities = CreateMany<ICapability>(2).Select(c => new CapabilityType(c)).ToArray()
                        }))
                        .ToArray()
                    });
                }

                SourceApi.Data.AddProjectPermissions(project, new PermissionsType
                {
                    GranteeCapabilities = users.Select(u => new GranteeCapabilityType
                    {
                        User = new GranteeCapabilityType.UserType
                        {
                            Id = u.Id
                        },
                        Capabilities = CreateMany<ICapability>(2).Select(c => new CapabilityType(c)).ToArray()
                    })
                    .Concat(groups.Select(g => new GranteeCapabilityType
                    {
                        Group = new GranteeCapabilityType.GroupType
                        {
                            Id = g.Id
                        },
                        Capabilities = CreateMany<ICapability>(2).Select(c => new CapabilityType(c)).ToArray()
                    }))
                    .ToArray()
                });
            }

            return projects;
        }

        #endregion - Prepare Source Data (Projects) -

        #region - Prepare Source Data (Data Sources) -

        protected List<DataSourceResponse.DataSourceType> PrepareSourceDataSourceData()
        {
            var dataSources = new List<DataSourceResponse.DataSourceType>();

            // Get all users that are not support users, and all groups
            var users = SourceApi.Data.Users.Where(u => u.SiteRole != SiteRoles.SupportUser).ToList();
            var groups = SourceApi.Data.Groups;

            int counter = 0;
            foreach (var project in SourceApi.Data.Projects)
            { // loop over all the projects and add a data source to each

                var dataSource = Create<DataSourceResponse.DataSourceType>();
                dataSource.Project = new DataSourceResponse.DataSourceType.ProjectType { Id = project.Id, Name = project.Name };

                var owner = users[counter % users.Count];
                dataSource.Owner = new DataSourceResponse.DataSourceType.OwnerType { Id = owner.Id };

                SourceApi.Data.AddDataSourcePermissions(dataSource, new PermissionsType
                {
                    GranteeCapabilities = users.Select(u => new GranteeCapabilityType
                    {
                        User = new GranteeCapabilityType.UserType
                        {
                            Id = u.Id
                        },
                        Capabilities = CreateMany<ICapability>(2).Select(c => new CapabilityType(c)).ToArray()
                    })
                    .Concat(groups.Select(g => new GranteeCapabilityType
                    {
                        Group = new GranteeCapabilityType.GroupType
                        {
                            Id = g.Id
                        },
                        Capabilities = CreateMany<ICapability>(2).Select(c => new CapabilityType(c)).ToArray()
                    }))
                    .ToArray()
                });

                // Assert infra
                Assert.NotNull(dataSource.Tags);
                Assert.NotEmpty(dataSource.Tags);

                // Our data source data will just be a guid as a string, encoded to a byte array

                byte[] dataSourceData = Constants.DefaultEncoding.GetBytes($"<data>{Guid.NewGuid()}</data>");
                SourceApi.Data.AddDataSource(dataSource, dataSourceData);
                dataSources.Add(dataSource);
                counter++;
            }

            return dataSources;
        }

        #endregion - Prepare Source Data (Data Sources) -

        #region - Prepare Source Data (Workbooks) -

        protected List<WorkbookResponse.WorkbookType> PrepareSourceWorkbooksData()
        {
            void CreateViewsForWorkbook(WorkbookResponse.WorkbookType workbook, SimulatedWorkbookData workbookData, int viewCount = 2)
            {
                var users = SourceApi.Data.Users.Where(u => u.SiteRole != SiteRoles.SupportUser).ToList();
                var groups = SourceApi.Data.Groups;

                // Infra verification
                Assert.NotNull(workbook.Views);

                for (int i = 0; i < viewCount; i++)
                {
                    var view = Create<ViewResponse.ViewType>();

                    // The view content url needs to be updated to include the workbook name.
                    // The "Get Workbook" Rest API returns only
                    //   the id, which will be recreated on the target
                    //   the contentUrl, which on a real server is the name of the workbook plus the name of the sheet/view
                    //   but not the name of the sheet/view
                    //   as a transformer may change the name of the workbook, we need to make a reasonable valid view content url
                    view.ContentUrl = $"{workbook.Name}{Constants.PathSeparator}{view.Name}";

                    // Give the view permissions
                    SourceApi.Data.AddViewPermissions(view.Id, new PermissionsType
                    {
                        GranteeCapabilities = users.Select(u => new GranteeCapabilityType
                        {
                            User = new GranteeCapabilityType.UserType
                            {
                                Id = u.Id
                            },
                            Capabilities = CreateMany<ICapability>(2).Select(c => new CapabilityType(c)).ToArray()
                        })
                        .Concat(groups.Select(g => new GranteeCapabilityType
                        {
                            Group = new GranteeCapabilityType.GroupType
                            {
                                Id = g.Id
                            },
                            Capabilities = CreateMany<ICapability>(2).Select(c => new CapabilityType(c)).ToArray()
                        }))
                        .ToArray()
                    });

                    // Every other view is hidden, first is not hidden
                    bool hidden = i % 2 != 0;

                    workbookData.Views.Add(new SimulatedWorkbookData.SimulatedViewType(view, hidden));

                    // Infra verification
                    Assert.NotNull(view.Tags);
                    Assert.NotEmpty(view.Tags);
                }
            }

            // Creates a number of connections and returns the simulated "workbook file data" as a byte array
            void CreateConnectionsForWorkbook(SimulatedWorkbookData workbookData, int connectionCount = 2)
            {
                for (int i = 0; i < connectionCount; i++)
                {
                    workbookData.Connections.Add(Create<SimulatedConnection>());
                }
            }

            /*
             * Build workbook metadata
             * Add owner to workbook
             * Add permissions to workbook
             * Add views to workbook
             * Add permissions to views
             * Build workbook file
               * Build workbook connections
               * Save workbook connections to workbook file
             */

            var workbooks = new List<WorkbookResponse.WorkbookType>();

            // Get all users that are not support users, and all groups
            var users = SourceApi.Data.Users.Where(u => u.SiteRole != SiteRoles.SupportUser).ToList();
            var groups = SourceApi.Data.Groups;

            int counter = 0;
            foreach (var project in SourceApi.Data.Projects)
            { // loop over all the projects and add a data source to each

                // Create workbook
                var workbook = AutoFixture.Build<WorkbookResponse.WorkbookType>()
                                // Views need to be saved in the workbook data, so created at a later step
                                .With(x => x.Views, Array.Empty<WorkbookResponse.WorkbookType.ViewReferenceType>())
                                .Create();

                var workbookFileData = new SimulatedWorkbookData();

                // Add it to the current project
                workbook.Project = new WorkbookResponse.WorkbookType.ProjectType { Id = project.Id, Name = project.Name };

                // Give the workbook an owner
                var owner = users[counter % users.Count];
                workbook.Owner = new WorkbookResponse.WorkbookType.OwnerType { Id = owner.Id };

                // Give the workbook permissions
                SourceApi.Data.AddWorkbookPermissions(workbook, new PermissionsType
                {
                    GranteeCapabilities = users.Select(u => new GranteeCapabilityType
                    {
                        User = new GranteeCapabilityType.UserType
                        {
                            Id = u.Id
                        },
                        Capabilities = CreateMany<ICapability>(2).Select(c => new CapabilityType(c)).ToArray()
                    })
                    .Concat(groups.Select(g => new GranteeCapabilityType
                    {
                        Group = new GranteeCapabilityType.GroupType
                        {
                            Id = g.Id
                        },
                        Capabilities = CreateMany<ICapability>(2).Select(c => new CapabilityType(c)).ToArray()
                    }))
                    .ToArray()
                });

                CreateViewsForWorkbook(workbook, workbookFileData);

                CreateConnectionsForWorkbook(workbookFileData);
                CreateViewsForWorkbook(workbook, workbookFileData);

                SourceApi.Data.AddWorkbook(workbook, Constants.DefaultEncoding.GetBytes(workbookFileData.ToXml()));
                workbooks.Add(workbook);
                counter++;
            }

            return workbooks;
        }

        #endregion - Prepare Source Data (Workbooks) -

        #region - Prepare Source Data (Schedules) -

        private List<Server.ScheduleResponse.ScheduleType> PrepareSchedulesData()
        {
            var hourlySchedule = new Server.ScheduleResponse.ScheduleType
            {
                Id = Guid.NewGuid(),
                Name = ScheduleFrequencies.Hourly,
                Type = ScheduleTypes.Extract,
                Frequency = ScheduleFrequencies.Hourly,
                State = "Active",
                Priority = 50,
                ExecutionOrder = "Parallel",
                FrequencyDetails = new Server.ScheduleResponse.ScheduleType.FrequencyDetailsType
                {
                    Start = "00:25:00",
                    End = "01:25:00",
                    Intervals = [
                        new Server.ScheduleResponse.ScheduleType.FrequencyDetailsType.IntervalType { Hours = "1" },
                        new Server.ScheduleResponse.ScheduleType.FrequencyDetailsType.IntervalType { WeekDay = WeekDays.Sunday },
                        new Server.ScheduleResponse.ScheduleType.FrequencyDetailsType.IntervalType { WeekDay = WeekDays.Saturday }
                    ]
                }
            };
            var dailySchedule = new Server.ScheduleResponse.ScheduleType
            {
                Id = Guid.NewGuid(),
                Name = ScheduleFrequencies.Daily,
                Type = ScheduleTypes.Extract,
                Frequency = ScheduleFrequencies.Daily,
                State = "Active",
                Priority = 50,
                ExecutionOrder = "Parallel",
                FrequencyDetails = new Server.ScheduleResponse.ScheduleType.FrequencyDetailsType
                {
                    Start = "00:15:00",
                    End = "12:15:00",
                    Intervals = [
                        new Server.ScheduleResponse.ScheduleType.FrequencyDetailsType.IntervalType { Hours = "12" },
                        new Server.ScheduleResponse.ScheduleType.FrequencyDetailsType.IntervalType { WeekDay = WeekDays.Tuesday },
                        new Server.ScheduleResponse.ScheduleType.FrequencyDetailsType.IntervalType { WeekDay = WeekDays.Thursday }
                    ]
                }
            };
            var weeklySchedule = new Server.ScheduleResponse.ScheduleType
            {
                Id = Guid.NewGuid(),
                Name = ScheduleFrequencies.Weekly,
                Type = ScheduleTypes.Extract,
                Frequency = ScheduleFrequencies.Weekly,
                State = "Active",
                Priority = 50,
                ExecutionOrder = "Parallel",
                FrequencyDetails = new Server.ScheduleResponse.ScheduleType.FrequencyDetailsType
                {
                    Start = "03:10:00",
                    Intervals = [new Server.ScheduleResponse.ScheduleType.FrequencyDetailsType.IntervalType { WeekDay = WeekDays.Sunday }]
                }
            };
            var monthlyMultipleDaysSchedule = new Server.ScheduleResponse.ScheduleType
            {
                Id = Guid.NewGuid(),
                Name = $"{ScheduleFrequencies.Monthly}_Multiple",
                Type = ScheduleTypes.Extract,
                Frequency = ScheduleFrequencies.Monthly,
                State = "Active",
                Priority = 50,
                ExecutionOrder = "Parallel",
                FrequencyDetails = new Server.ScheduleResponse.ScheduleType.FrequencyDetailsType
                {
                    Start = "03:45:00",
                    Intervals = [
                        new Server.ScheduleResponse.ScheduleType.FrequencyDetailsType.IntervalType { MonthDay = "1" },
                        new Server.ScheduleResponse.ScheduleType.FrequencyDetailsType.IntervalType { MonthDay = "10" },
                        new Server.ScheduleResponse.ScheduleType.FrequencyDetailsType.IntervalType { MonthDay = "20" }
                    ]
                }
            };
            var monthlyLastSundaySchedule = new Server.ScheduleResponse.ScheduleType
            {
                Id = Guid.NewGuid(),
                Name = $"{ScheduleFrequencies.Monthly}_LastSunday",
                Type = ScheduleTypes.Extract,
                Frequency = ScheduleFrequencies.Monthly,
                State = "Active",
                Priority = 50,
                ExecutionOrder = "Parallel",
                FrequencyDetails = new Server.ScheduleResponse.ScheduleType.FrequencyDetailsType
                {
                    Start = "01:35:00",
                    Intervals = [new Server.ScheduleResponse.ScheduleType.FrequencyDetailsType.IntervalType { WeekDay = WeekDays.Sunday, MonthDay = "LastDay" }]
                }
            };
            var schedules = new List<Server.ScheduleResponse.ScheduleType>
            {
                hourlySchedule,
                dailySchedule,
                weeklySchedule,
                monthlyMultipleDaysSchedule,
                monthlyLastSundaySchedule
            };

            foreach (var schedule in schedules)
            {
                SourceApi.Data.AddSchedule(schedule);
            }

            return schedules;
        }

        #endregion - Prepare Source Data (Schedules) -

        #region - Prepare Source Data (ExtractRefreshTasks) -

        protected List<Server.ExtractRefreshTasksResponse.TaskType> PrepareSourceExtractRefreshTasksData()
        {
            var extractRefreshTasks = new List<Server.ExtractRefreshTasksResponse.TaskType>();

            var schedules = PrepareSchedulesData();

            var count = 0;
            foreach (var datasource in SourceApi.Data.DataSources)
            {
                extractRefreshTasks.Add(
                    new Server.ExtractRefreshTasksResponse.TaskType
                    {
                        ExtractRefresh = new Server.ExtractRefreshTasksResponse.TaskType.ExtractRefreshType
                        {
                            Id = Guid.NewGuid(),
                            Priority = 50,
                            DataSource = new Server.ExtractRefreshTasksResponse.TaskType.ExtractRefreshType.DataSourceType
                            {
                                Id = datasource.Id
                            },
                            Schedule = new Server.ExtractRefreshTasksResponse.TaskType.ExtractRefreshType.ScheduleType
                            {
                                Id = schedules[count % schedules.Count].Id
                            }
                        }
                    });
                count++;
            }

            foreach (var workbook in SourceApi.Data.Workbooks)
            {
                extractRefreshTasks.Add(
                    new Server.ExtractRefreshTasksResponse.TaskType
                    {
                        ExtractRefresh = new Server.ExtractRefreshTasksResponse.TaskType.ExtractRefreshType
                        {
                            Id = Guid.NewGuid(),
                            Priority = 50,
                            Workbook = new Server.ExtractRefreshTasksResponse.TaskType.ExtractRefreshType.WorkbookType
                            {
                                Id = workbook.Id
                            },
                            Schedule = new Server.ExtractRefreshTasksResponse.TaskType.ExtractRefreshType.ScheduleType
                            {
                                Id = schedules[count % schedules.Count].Id
                            }
                        }
                    });
                count++;
            }

            foreach (var extractRefreshTask in extractRefreshTasks)
            {
                var schedule = schedules.First(sch => sch.Id == extractRefreshTask.ExtractRefresh!.Schedule!.Id);
                SourceApi.Data.ServerExtractRefreshTasks.Add(extractRefreshTask);
                SourceApi.Data.AddExtractToSchedule(
                    new Server.ScheduleExtractRefreshTasksResponse.ExtractType
                    {
                        Id = extractRefreshTask.ExtractRefresh!.Id,
                        Type = count % 2 == 0 ? ExtractRefreshType.FullRefresh : ExtractRefreshType.ServerIncrementalRefresh
                    },
                    schedule);
                count++;
            }

            return extractRefreshTasks;
        }

        #endregion - Prepare Source Data (ExtractRefreshTasks) -

        #region - Prepare Source Data (Custom Views) -
        protected List<CustomViewResponse.CustomViewType> PrepareSourceCustomViewsData()
        {
            var customViews = new List<CustomViewResponse.CustomViewType>();

            // Get all users that are not support users, and all groups
            var users = SourceApi.Data.Users.Where(u => u.SiteRole != SiteRoles.SupportUser).ToList();
            var groups = SourceApi.Data.Groups;
            var workbooks = SourceApi.Data.Workbooks;

            var rnd = new Random();

            foreach (var workbook in workbooks)
            {
                var workbookViewData = SourceApi.Data.GetWorkbookFileData(workbook.Id)?.Views;
                if (workbookViewData is null)
                {
                    continue;
                }

                foreach (var viewData in workbookViewData)
                {
                    var simulatedView = viewData?.View;

                    if (simulatedView is null)
                    {
                        continue;
                    }

                    // pick a random user to be the custom view owner

                    var owner = workbook.Owner!;

                    var newCustomView = CreateCustomView(
                        workbook,
                        new()
                        {
                            Id = simulatedView.Id,
                            Name = simulatedView.Name,
                            ContentUrl = simulatedView.ContentUrl
                        },
                        owner);

                    SourceApi.Data.CustomViewDefaultUsers.TryAdd(newCustomView.Id, [new() { Id = owner.Id }]);

                    var customViewFileData = new SimulatedCustomViewData();

                    SourceApi.Data.AddCustomView(newCustomView, Constants.DefaultEncoding.GetBytes(customViewFileData.ToJson()));
                    customViews.Add(newCustomView);
                }
            }

            return customViews;

            CustomViewResponse.CustomViewType CreateCustomView(
                WorkbookResponse.WorkbookType workbook,
                WorkbookResponse.WorkbookType.ViewReferenceType view,
                WorkbookResponse.WorkbookType.OwnerType owner)
            {
                return AutoFixture.Build<CustomViewResponse.CustomViewType>()
                    .With(x
                        => x.View,
                        new CustomViewResponse.CustomViewType.ViewType()
                        {
                            Id = view.Id,
                            Name = view.Name
                        })
                    .With(x
                        => x.Workbook,
                        new CustomViewResponse.CustomViewType.WorkbookType()
                        {
                            Id = workbook.Id,
                            Name = workbook.Name
                        })
                    .With(x
                        => x.Owner,
                        new CustomViewResponse.CustomViewType.OwnerType()
                        {
                            Id = owner.Id
                        })
                    .Create();
            }
        }
        #endregion
    }
}

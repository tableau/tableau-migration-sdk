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
using System.Text;
using AutoFixture;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Rest.Models.Responses.Server;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Api.Simulation;
using Tableau.Migration.Net;
using CloudResponse = Tableau.Migration.Api.Rest.Models.Responses.Cloud;

namespace Tableau.Migration.Tests.Simulation
{
    public static class TableauDataExtensions
    {
        public static DataSourceResponse.DataSourceType CreateDataSource(
            this TableauData data,
            IFixture autoFixture,
            ProjectsResponse.ProjectType project,
            UsersResponse.UserType user,
            byte[]? fileData = null)
        {
            var dataSource = autoFixture.Build<DataSourceResponse.DataSourceType>()
                .With(ds => ds.Project, new DataSourceResponse.DataSourceType.ProjectType(project))
                .With(ds => ds.Owner, new DataSourceResponse.DataSourceType.OwnerType()
                {
                    Id = user.Id
                })
                .Create();

            fileData ??= Constants.DefaultEncoding.GetBytes(new SimulatedDataSourceData().ToXml());

            data.AddDataSource(dataSource, fileData);

            return dataSource;
        }

        public static DataSourceResponse.DataSourceType CreateDataSource(
            this TableauData data,
            IFixture autoFixture,
            ProjectsResponse.ProjectType project,
            UsersResponse.UserType user,
            SimulatedDataSourceData simulatedData)
        => data.CreateDataSource(
            autoFixture,
            project,
            user,
            Constants.DefaultEncoding.GetBytes(simulatedData.ToXml()));

        public static DataSourceResponse.DataSourceType CreateDataSource(this TableauData data, IFixture autoFixture)
        {
            return data.CreateDataSource(autoFixture, data.CreateProject(autoFixture), data.CreateUser(autoFixture));
        }

        public static WorkbookResponse.WorkbookType CreateWorkbook(
            this TableauData data,
            IFixture autoFixture,
            ProjectsResponse.ProjectType project,
            UsersResponse.UserType user,
            byte[]? fileData = null)
        {
            var workbook = autoFixture.Build<WorkbookResponse.WorkbookType>()
                .With(wb => wb.Project, new WorkbookResponse.WorkbookType.ProjectType(project))
                .With(wb => wb.Owner, new WorkbookResponse.WorkbookType.OwnerType()
                {
                    Id = user.Id
                })
                .Create();

            fileData ??= Constants.DefaultEncoding.GetBytes(new SimulatedWorkbookData().ToXml());

            data.AddWorkbook(workbook, fileData);

            return workbook;
        }

        public static WorkbookResponse.WorkbookType CreateWorkbook(this TableauData data, IFixture autoFixture)
        {
            return data.CreateWorkbook(autoFixture, data.CreateProject(autoFixture), data.CreateUser(autoFixture));
        }

        public static IImmutableList<WorkbookResponse.WorkbookType> CreateWorkbooks(this TableauData data, IFixture autoFixture, int count)
        {
            var workbooks = new List<WorkbookResponse.WorkbookType>(count);

            for (var i = 0; i != count; i++)
            {
                var workbook = data.CreateWorkbook(autoFixture);

                workbooks.Add(workbook);
            }

            return workbooks.ToImmutableArray();
        }

        public static PermissionsType CreateDataSourcePermissions(
            this TableauData data,
            IFixture autoFixture,
            DataSourceResponse.DataSourceType dataSource,
            Guid dataSourceId,
            string? dataSourceName)
        {
            var permissions = CreatePermissions(autoFixture, dataSourceId, dataSourceName);

            data.AddDataSourcePermissions(dataSource, permissions);

            return permissions;
        }

        public static PermissionsType CreateWorkbookPermissions(
            this TableauData data,
            IFixture autoFixture,
            WorkbookResponse.WorkbookType workbook,
            Guid workbookId,
            string? workbookName)
        {
            var permissions = CreatePermissions(autoFixture, workbookId, workbookName);

            data.AddWorkbookPermissions(workbook, permissions);

            return permissions;
        }

        public static PermissionsType CreateViewPermissions(
            this TableauData data,
            IFixture autoFixture,
            WorkbookResponse.WorkbookType.ViewReferenceType view,
            Guid viewId,
            string? viewName)
        {
            var permissions = CreatePermissions(autoFixture, viewId, viewName);

            data.AddViewPermissions(view, permissions);

            return permissions;
        }

        private static PermissionsType CreatePermissions(IFixture autoFixture, Guid contentItemId, string? contentItemName)
        {
            return autoFixture.Build<PermissionsType>()
                .With(p => p.ContentItem, new PermissionsContentItemType()
                {
                    Id = contentItemId,
                    Name = contentItemName
                })
                .Create();
        }

        public static UsersResponse.UserType CreateUser(this TableauData data, IFixture autoFixture)
        {
            var userType = autoFixture.Create<UsersResponse.UserType>();

            data.AddUser(userType);

            return userType;
        }

        public static IImmutableList<UsersResponse.UserType> CreateUsers(this TableauData data, IFixture autoFixture, int count)
        {
            var users = new List<UsersResponse.UserType>();

            for (var i = 0; i != count; i++)
            {
                users.Add(data.CreateUser(autoFixture));
            }

            return users.ToImmutableArray();
        }

        public static ProjectsResponse.ProjectType CreateProject(this TableauData data, IFixture autoFixture)
        {
            var project = autoFixture.Create<ProjectsResponse.ProjectType>();

            project.Owner = new() { Id = data.Users.PickRandom().Id };

            data.AddProject(project, null);

            return project;
        }

        public static IImmutableList<ProjectsResponse.ProjectType> CreateProjects(this TableauData data, IFixture autoFixture, int projectCount)
        {
            var projects = new List<ProjectsResponse.ProjectType>();

            for (var i = 0; i != projectCount; i++)
            {
                projects.Add(data.CreateProject(autoFixture));
            }

            return projects.ToImmutableArray();
        }

        public static ScheduleResponse.ScheduleType CreateSchedule(
            this TableauData data,
            IFixture autoFixture)
        {
            var schedule = autoFixture.Build<ScheduleResponse.ScheduleType>()
                .Create();

            data.AddSchedule(schedule);

            return schedule;
        }

        public static ScheduleResponse.ScheduleType CreateScheduleExtractRefreshTask(
            this TableauData data,
            IFixture autoFixture,
            ScheduleResponse.ScheduleType? schedule = null,
            Guid? extractRefreshId = null,
            string? extractRefreshType = null)
        {
            if (schedule is null)
            {
                schedule = data.CreateSchedule(autoFixture);
            }

            var composer = autoFixture.Build<ScheduleExtractRefreshTasksResponse.ExtractType>();

            if (extractRefreshId.HasValue)
            {
                composer.With(sert => sert.Id, extractRefreshId);
            }

            if (!extractRefreshType.IsNullOrEmpty())
            {
                composer.With(sert => sert.Type, extractRefreshType);
            }

            var extractRefreshSchedule = composer.Create();

            data.AddExtractToSchedule(
                extractRefreshSchedule,
                schedule);

            return schedule;
        }

        public static ExtractRefreshTasksResponse.TaskType CreateServerExtractRefreshTask(
            this TableauData data,
            IFixture autoFixture,
            DataSourceResponse.DataSourceType? dataSource = null,
            WorkbookResponse.WorkbookType? workbook = null)
        {
            var schedule = data.CreateScheduleExtractRefreshTask(autoFixture);

            var extractRefreshComposer = autoFixture
                .Build<ExtractRefreshTasksResponse.TaskType.ExtractRefreshType>()
                .With(ert => ert.Schedule, () => new ExtractRefreshTasksResponse.TaskType.ExtractRefreshType.ScheduleType
                {
                    Id = schedule.Id
                });

            if (dataSource is not null)
            {
                extractRefreshComposer = extractRefreshComposer
                    .With(ert => ert.Workbook, () => new ExtractRefreshTasksResponse.TaskType.ExtractRefreshType.WorkbookType
                    {
                        Id = workbook!.Id
                    })
                    .Without(ert => ert.DataSource);
            }
            else if (workbook is not null)
            {
                extractRefreshComposer = extractRefreshComposer
                    .With(ert => ert.DataSource, () => new ExtractRefreshTasksResponse.TaskType.ExtractRefreshType.DataSourceType
                    {
                        Id = dataSource!.Id
                    })
                    .Without(ert => ert.Workbook);
            }
            else
            {
                if (autoFixture.Create<bool>())
                {
                    var generatedDatasource = data.CreateDataSource(autoFixture);
                    extractRefreshComposer = extractRefreshComposer
                        .With(ert => ert.DataSource, () => new ExtractRefreshTasksResponse.TaskType.ExtractRefreshType.DataSourceType
                        {
                            Id = generatedDatasource!.Id
                        })
                        .Without(ert => ert.Workbook);
                }
                else
                {
                    var generatedWorkbook = data.CreateWorkbook(autoFixture);
                    extractRefreshComposer = extractRefreshComposer
                        .With(ert => ert.Workbook, () => new ExtractRefreshTasksResponse.TaskType.ExtractRefreshType.WorkbookType
                        {
                            Id = generatedWorkbook!.Id
                        })
                        .Without(ert => ert.DataSource);
                }
            }

            var extractRefreshTask = autoFixture
                .Build<ExtractRefreshTasksResponse.TaskType>()
                .With(tt => tt.ExtractRefresh, () => extractRefreshComposer.Create())
                .Create();

            data.ServerExtractRefreshTasks.Add(extractRefreshTask);

            return extractRefreshTask;
        }

        public static IImmutableList<ExtractRefreshTasksResponse.TaskType> CreateServerExtractRefreshTasks(
            this TableauData data,
            IFixture autoFixture,
            int extractRefreshCount)
        {
            var extractRefreshTasks = new List<ExtractRefreshTasksResponse.TaskType>();

            for (var i = 0; i != extractRefreshCount; i++)
            {
                extractRefreshTasks.Add(data.CreateServerExtractRefreshTask(autoFixture));
            }

            return extractRefreshTasks.ToImmutableArray();
        }

        public static CloudResponse.ExtractRefreshTasksResponse.TaskType CreateCloudExtractRefreshTask(
            this TableauData data,
            IFixture autoFixture,
            DataSourceResponse.DataSourceType? dataSource = null,
            WorkbookResponse.WorkbookType? workbook = null)
        {
            var extractRefreshComposer = autoFixture
                .Build<CloudResponse.ExtractRefreshTasksResponse.TaskType.ExtractRefreshType>()
                .With(ert => ert.Schedule);

            if (dataSource is not null)
            {
                extractRefreshComposer = extractRefreshComposer
                    .With(ert => ert.Workbook, () => new CloudResponse.ExtractRefreshTasksResponse.TaskType.ExtractRefreshType.WorkbookType
                    {
                        Id = dataSource!.Id
                    })
                    .Without(ert => ert.DataSource);
            }
            else if (workbook is not null)
            {
                extractRefreshComposer = extractRefreshComposer
                    .With(ert => ert.DataSource, () => new CloudResponse.ExtractRefreshTasksResponse.TaskType.ExtractRefreshType.DataSourceType
                    {
                        Id = workbook!.Id
                    })
                    .Without(ert => ert.Workbook);
            }
            else
            {
                if (autoFixture.Create<bool>())
                {
                    var generatedDatasource = data.CreateDataSource(autoFixture);
                    extractRefreshComposer = extractRefreshComposer
                        .With(ert => ert.DataSource, () => new CloudResponse.ExtractRefreshTasksResponse.TaskType.ExtractRefreshType.DataSourceType
                        {
                            Id = generatedDatasource!.Id
                        })
                        .Without(ert => ert.Workbook);
                }
                else
                {
                    var generatedWorkbook = data.CreateWorkbook(autoFixture);
                    extractRefreshComposer = extractRefreshComposer
                        .With(ert => ert.Workbook, () => new CloudResponse.ExtractRefreshTasksResponse.TaskType.ExtractRefreshType.WorkbookType
                        {
                            Id = generatedWorkbook!.Id
                        })
                        .Without(ert => ert.DataSource);
                }
            }

            var extractRefreshTask = autoFixture
                .Build<CloudResponse.ExtractRefreshTasksResponse.TaskType>()
                .With(tt => tt.ExtractRefresh, () => extractRefreshComposer.Create())
                .Create();

            data.CloudExtractRefreshTasks.Add(extractRefreshTask);

            return extractRefreshTask;
        }

        public static IImmutableList<CloudResponse.ExtractRefreshTasksResponse.TaskType> CreateCloudExtractRefreshTasks(
            this TableauData data,
            IFixture autoFixture,
            int extractRefreshCount)
        {
            var extractRefreshTasks = new List<CloudResponse.ExtractRefreshTasksResponse.TaskType>();

            for (var i = 0; i != extractRefreshCount; i++)
            {
                extractRefreshTasks.Add(data.CreateCloudExtractRefreshTask(autoFixture));
            }

            return extractRefreshTasks.ToImmutableArray();
        }

        public static CustomViewResponse.CustomViewType CreateCustomView(
           this TableauData data,
           IFixture autoFixture,
            WorkbookResponse.WorkbookType workbook,
            UsersResponse.UserType user,
           byte[]? fileData = null)
        {
            var customView = autoFixture.Build<CustomViewResponse.CustomViewType>()
               .With(cv => cv.Workbook, new CustomViewResponse.CustomViewType.WorkbookType(workbook))
               .With(cv => cv.Owner, new CustomViewResponse.CustomViewType.OwnerType()
               {
                   Id = user.Id
               })
               .Create();

            fileData ??= Constants.DefaultEncoding.GetBytes(new SimulatedWorkbookData().ToXml());

            data.AddCustomView(customView, fileData);

            return customView;
        }

        public static CustomViewResponse.CustomViewType CreateCustomView(this TableauData data, IFixture autoFixture)
            => data.CreateCustomView(autoFixture, data.CreateWorkbook(autoFixture), data.CreateUser(autoFixture));

        public static IImmutableList<CustomViewResponse.CustomViewType> CreateCustomViews(
            this TableauData data,
            IFixture autoFixture,
            int count)
        {
            var customViews = new List<CustomViewResponse.CustomViewType>(count);

            for (var i = 0; i != count; i++)
            {
                var customView = CreateCustomView(data, autoFixture);

                customViews.Add(customView);
            }

            return customViews.ToImmutableArray();
        }

        public static List<UsersWithCustomViewAsDefaultViewResponse.UserType> CreateCustomViewDefaultUsers(
            this TableauData data,
            IFixture autoFixture,
            Guid customViewId)
        {
            var users = CreateUsers(data, autoFixture, 5);

            var customViewUsers = new List<UsersWithCustomViewAsDefaultViewResponse.UserType>();
            foreach (var user in users)
            {
                customViewUsers.Add(new()
                {
                    Id = user.Id
                });
            }

            if (data.CustomViewDefaultUsers.TryGetValue(
                customViewId,
                out List<UsersWithCustomViewAsDefaultViewResponse.UserType>? value))
            {
                value.AddRange(customViewUsers);
            }
            else
            {
                data.CustomViewDefaultUsers.TryAdd(customViewId, customViewUsers);
            }

            return data.CustomViewDefaultUsers[customViewId];
        }

        public static SimulatedWorkbookData? GetWorkbookFileData(this TableauData data, Guid workbookId)
        {
            var wbFile = data.WorkbookFiles[workbookId];
            var wbFileText = Encoding.Default.GetString(wbFile);

            if (string.IsNullOrEmpty(wbFileText))
            {
                return null;
            }

            var simulatedWorkbook = wbFileText.FromXml<SimulatedWorkbookData>();
            return simulatedWorkbook;
        }
    }
}

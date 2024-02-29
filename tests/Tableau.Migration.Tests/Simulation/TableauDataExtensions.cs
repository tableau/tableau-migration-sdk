// Copyright (c) 2023, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the ""License"") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an ""AS IS"" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using AutoFixture;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Api.Simulation;
using Tableau.Migration.Net;

namespace Tableau.Migration.Tests.Simulation
{
    public static class TableauDataExtensions
    {
        private static readonly Lazy<Random> _random = new();

        public static T PickRandom<T>(this ICollection<T> c)
        {
            var randomIndex = _random.Value.Next(0, c.Count - 1);
            return c.ElementAt(randomIndex);
        }

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
    }
}

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
using System.Linq;
using AutoFixture;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Api.Simulation;
using Tableau.Migration.Content.Permissions;

namespace Tableau.Migration.Tests.Simulation.DataPreparation
{
    /// <summary>
    /// Static class responsible for preparing projects data for migration tests.
    /// </summary>
    public static class ProjectsDataPreparation
    {
        /// <summary>
        /// Prepares the source data for migration tests.
        /// </summary>
        /// <param name="sourceApi">The source API simulator.</param>
        /// <param name="fixture">The fixture for creating test data.</param>
        /// <returns>The list of prepared projects.</returns>
        public static List<ProjectsResponse.ProjectType> PrepareServerSource(
            TableauApiSimulator sourceApi,
            IFixture fixture)
        {
            ArgumentNullException.ThrowIfNull(sourceApi);
            ArgumentNullException.ThrowIfNull(fixture);

            var projects = new List<ProjectsResponse.ProjectType>();

            // This will create 5 projects
            var numSourceProjects = 5;

            // Get all users that are not support users, and all groups
            var users = sourceApi.Data.Users.Where(u => u.SiteRole != SiteRoles.SupportUser).ToImmutableArray();
            var groups = sourceApi.Data.Groups;

            // Create the projects in a hierarchy. Project 0 is the only root project. 
            // Every next project is a child of the previous one
            for (var i = 0; i < numSourceProjects; i++)
            {
                var project = fixture.Create<ProjectsResponse.ProjectType>();

                if (i >= 1)
                    project.ParentProjectId = projects[^1].Id.ToString();
                else
                    project.ParentProjectId = null;

                project.Owner = new() { Id = users[i % users.Length].Id };

                projects.Add(project);
                sourceApi.Data.AddProject(project);
            }

            foreach (var project in projects)
            { // loop over all the projects created
                foreach (var contentType in DefaultPermissionsContentTypeUrlSegments.GetAll()) // loop over all the known content types and get their default permissions
                { // loop over all the known content types and get their default permissions

                    // Add the default permissions for the given content type to the project
                    sourceApi.Data.AddDefaultProjectPermissions(project.Id, contentType, new PermissionsType
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
                            Capabilities = fixture.CreateMany<ICapability>(2).Select(c => new CapabilityType(c)).ToArray()
                        })
                        .Concat(groups.Select(g => new GranteeCapabilityType
                        {
                            Group = new GranteeCapabilityType.GroupType
                            {
                                Id = g.Id
                            },
                            Capabilities = fixture.CreateMany<ICapability>(2).Select(c => new CapabilityType(c)).ToArray()
                        }))
                        .ToArray()
                    });
                }

                sourceApi.Data.AddProjectPermissions(project, new PermissionsType
                {
                    GranteeCapabilities = users.Select(u => new GranteeCapabilityType
                    {
                        User = new GranteeCapabilityType.UserType
                        {
                            Id = u.Id
                        },
                        Capabilities = fixture.CreateMany<ICapability>(2).Select(c => new CapabilityType(c)).ToArray()
                    })
                    .Concat(groups.Select(g => new GranteeCapabilityType
                    {
                        Group = new GranteeCapabilityType.GroupType
                        {
                            Id = g.Id
                        },
                        Capabilities = fixture.CreateMany<ICapability>(2).Select(c => new CapabilityType(c)).ToArray()
                    }))
                    .ToArray()
                });
            }

            return projects;
        }
    }
}
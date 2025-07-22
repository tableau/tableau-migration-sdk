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
using System.Linq;
using System.Text;
using AutoFixture;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Simulation;
using Tableau.Migration.Net;

namespace Tableau.Migration.Tests.Simulation.DataPreparation
{
    /// <summary>
    /// Static class responsible for preparing workbooks data for migration tests.
    /// </summary>
    public static class WorkbooksDataPreparation
    {
        /// <summary>
        /// Prepares the source data for migration tests.
        /// </summary>
        /// <param name="sourceApi">The source API simulator.</param>
        /// <param name="fixture">The fixture for creating test data.</param>
        /// <returns>The list of prepared workbooks.</returns>
        public static List<WorkbookResponse.WorkbookType> PrepareServerSource(
            TableauApiSimulator sourceApi,
            IFixture fixture)
        {
            var workbooks = new List<WorkbookResponse.WorkbookType>();

            // Get all users that are not support users, and all groups
            var users = CommonDataPreparation.GetNonSupportUsers(sourceApi);
            var groups = sourceApi.Data.Groups;

            int counter = 0;
            foreach (var project in sourceApi.Data.Projects)
            { // loop over all the projects and add a workbook to each
                var workbook = fixture.Build<WorkbookResponse.WorkbookType>()
                                // Views need to be saved in the workbook data, so created at a later step
                                .With(x => x.Views, Array.Empty<WorkbookResponse.WorkbookType.WorkbookViewReferenceType>())
                                .Create();
                workbook.Project = CommonDataPreparation.CreateProjectReference<WorkbookResponse.WorkbookType.ProjectType>(project);

                var workbookFileData = new SimulatedWorkbookData();

                var owner = users[counter % users.Count];
                workbook.Owner = CommonDataPreparation.CreateOwnerReference<WorkbookResponse.WorkbookType.OwnerType>(owner);

                sourceApi.Data.AddWorkbookPermissions(workbook, CommonDataPreparation.CreatePermissions(users, groups, fixture));

                CreateViewsForWorkbook(workbook, workbookFileData, sourceApi, fixture);

                var keychains = CommonDataPreparation.CreateEmbeddedCredentials(sourceApi.Data.WorkbookKeychains, users, workbook.Id, counter, fixture);
                CommonDataPreparation.CreateConnections(workbookFileData, embed: keychains.EncryptedKeychainList.Any(), fixture);

                // Our workbook data will just be a guid as a string, encoded to a byte array
                byte[] workbookData = Encoding.UTF8.GetBytes(workbookFileData.ToXml());
                sourceApi.Data.AddWorkbook(workbook, workbookData);
                workbooks.Add(workbook);
                counter++;
            }

            return workbooks;
        }

        private static void CreateViewsForWorkbook(
            WorkbookResponse.WorkbookType workbook,
            SimulatedWorkbookData workbookFileData,
            TableauApiSimulator sourceApi,
            IFixture fixture)
        {
            // Create some views for the workbook
            var views = new List<WorkbookResponse.WorkbookType.WorkbookViewReferenceType>();
            var viewCount = fixture.Create<int>() % 5 + 1; // 1-5 views per workbook

            for (int i = 0; i < viewCount; i++)
            {
                var view = fixture.Create<ViewResponse.ViewType>();
                view.ContentUrl = $"{workbook.Name}{Constants.PathSeparator}{view.Name}";
                view.Workbook = new ViewResponse.ViewType.WorkbookReferenceType { Id = workbook.Id };
                view.Project = new ViewResponse.ViewType.ProjectReferenceType { Id = workbook.Project!.Id };

                // Give the view permissions
                var viewPermissions = CommonDataPreparation.CreatePermissions(CommonDataPreparation.GetNonSupportUsers(sourceApi), sourceApi.Data.Groups, fixture);
                sourceApi.Data.AddViewPermissions(view.Id, viewPermissions);

                // Every other view is hidden, first is not hidden
                bool hidden = i % 2 != 0;

                workbookFileData.Views.Add(new SimulatedWorkbookData.SimulatedViewType(view, hidden, viewPermissions));
                views.Add(new WorkbookResponse.WorkbookType.WorkbookViewReferenceType(view));
                sourceApi.Data.AddView(view);
            }

            workbook.Views = views.ToArray();
        }
    }
}
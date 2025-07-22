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
using AutoFixture;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Simulation;
using Tableau.Migration.Net;

namespace Tableau.Migration.Tests.Simulation.DataPreparation
{
    /// <summary>
    /// Static class responsible for preparing custom views data for migration tests.
    /// </summary>
    public static class CustomViewsDataPreparation
    {
        /// <summary>
        /// Prepares the source data for migration tests.
        /// </summary>
        /// <param name="sourceApi">The source API simulator.</param>
        /// <param name="fixture">The fixture for creating test data.</param>
        /// <returns>The list of prepared custom views.</returns>
        public static List<CustomViewResponse.CustomViewType> PrepareServerSource(
            TableauApiSimulator sourceApi,
            IFixture fixture)
        {
            var customViews = new List<CustomViewResponse.CustomViewType>();

            // Get all users that are not support users, and all groups
            var users = sourceApi.Data.Users.Where(u => u.SiteRole != SiteRoles.SupportUser).ToList();
            var groups = sourceApi.Data.Groups;
            var workbooks = sourceApi.Data.Workbooks;

            var rnd = new Random();

            foreach (var workbook in workbooks)
            {
                var workbookViewData = sourceApi.Data.GetWorkbookFileData(workbook.Id)?.Views;
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
                        owner,
                        fixture);

                    sourceApi.Data.CustomViewDefaultUsers.TryAdd(newCustomView.Id, [new() { Id = owner.Id }]);

                    var customViewFileData = new SimulatedCustomViewData();

                    sourceApi.Data.AddCustomView(newCustomView, Constants.DefaultEncoding.GetBytes(customViewFileData.ToJson()));
                    customViews.Add(newCustomView);
                }
            }

            return customViews;
        }

        private static CustomViewResponse.CustomViewType CreateCustomView(
            WorkbookResponse.WorkbookType workbook,
            WorkbookResponse.WorkbookType.WorkbookViewReferenceType view,
            WorkbookResponse.WorkbookType.OwnerType owner,
            IFixture fixture)
        {
            return fixture.Build<CustomViewResponse.CustomViewType>()
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
}
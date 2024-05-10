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
using System.Linq;
using System.Text;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal class RestCommitWorkbookUploadResponseBuilder : RestCommitUploadResponseBuilder<WorkbookResponse, WorkbookResponse.WorkbookType, CommitWorkbookPublishRequest>
    {
        public RestCommitWorkbookUploadResponseBuilder(
            TableauData data,
            IHttpContentSerializer serializer)
            : base(
                  data,
                  serializer,
                  data => data.Workbooks,
                  data => data.WorkbookFiles,
                  (data, wb, file) => data.AddWorkbook(wb, file))
        { }

        protected override WorkbookResponse.WorkbookType? GetExistingContentItem(CommitWorkbookPublishRequest commitRequest)
        {
            var commitWorkbook = Guard.AgainstNull(commitRequest.Workbook, () => commitRequest.Workbook);

            var targetWorkbook = Data.Workbooks
                .SingleOrDefault(wb =>
                    string.Equals(wb.Name, commitWorkbook.Name, Workbook.NameComparison));

            return targetWorkbook;
        }

        protected override WorkbookResponse.WorkbookType BuildContent(
            CommitWorkbookPublishRequest commitRequest,
            ref byte[] commitFileData,
            WorkbookResponse.WorkbookType? existingContent,
            UsersResponse.UserType currentUser,
            bool overwrite)
        {
            var commitWorkbook = Guard.AgainstNull(commitRequest.Workbook, () => commitRequest.Workbook);

            SimulatedWorkbookData? simulatedFileData;
            try
            {
                simulatedFileData = Encoding.Default.GetString(commitFileData).FromXml<SimulatedWorkbookData>();
                if (simulatedFileData is null)
                {
                    throw new BuildResponseException(System.Net.HttpStatusCode.BadRequest, 8, "Unable to parse file", "");
                }
            }
            catch (Exception)
            {
                throw new BuildResponseException(System.Net.HttpStatusCode.BadRequest, 8, "Unable to parse file", "");
            }

            var targetWorkbook = existingContent ?? new WorkbookResponse.WorkbookType
            {
                Id = Guid.NewGuid(),
                Name = commitWorkbook.Name,
                Description = commitWorkbook.Description,
                ContentUrl = Guid.NewGuid().ToString(), // Auto-generated to simplify.
                CreatedAt = DateTime.UtcNow.ToString(),
                UpdatedAt = DateTime.UtcNow.ToString(),
                EncryptExtracts = false,
                ShowTabs = commitWorkbook.ShowTabs,
                Size = 0,
                WebpageUrl = String.Empty,
                Owner = new WorkbookResponse.WorkbookType.OwnerType
                {
                    Id = currentUser.Id
                },
                Project = new WorkbookResponse.WorkbookType.ProjectType
                {
                    Id = commitWorkbook.Project?.Id ?? Data.DefaultProject.Id
                },
                Tags = Array.Empty<WorkbookResponse.WorkbookType.TagType>(),
                Views = Array.Empty<WorkbookResponse.WorkbookType.ViewReferenceType>()
            };

            targetWorkbook.Name = commitWorkbook.Name;
            targetWorkbook.UpdatedAt = DateTime.UtcNow.ToString();
            targetWorkbook.ShowTabs = commitWorkbook.ShowTabs;

            targetWorkbook.Owner = new()
            {
                Id = currentUser.Id
            };

            targetWorkbook.Project = new()
            {
                Id = commitWorkbook.Project?.Id ?? Data.DefaultProject.Id
            };

            targetWorkbook.Tags = [];

            // Update connection data
            foreach (var connection in commitRequest.Workbook.Connections)
            {
                var simulatedConnection = simulatedFileData.Connections
                                            .Where(c => c.ServerAddress == connection.ServerAddress && c.ServerPort == connection.ServerPort)
                                            .FirstOrDefault();

                if (simulatedConnection is not null)
                {
                    simulatedConnection.Credentials!.Name = connection.Credentials!.Name;
                    simulatedConnection.Credentials.Password = connection.Credentials.Password;
                    simulatedConnection.Credentials.OAuth = connection.Credentials.OAuth;
                    simulatedConnection.Credentials.Embed = connection.Credentials.Embed;
                }
            }

            // Update view data
            foreach (var simulatedView in simulatedFileData.Views)
            {
                Guard.AgainstNull(simulatedView.View, nameof(simulatedView.View));

                // Update the file data to hide the views that were requested to be hidden
                var viewToHide = commitWorkbook.Views.Where(v => v.Name == simulatedView.View.Name).FirstOrDefault();
                if (viewToHide is not null)
                {
                    simulatedView.Hidden = viewToHide.Hidden;
                }

                simulatedView.View.ContentUrl = $"{targetWorkbook.Name}{Constants.PathSeparator}{simulatedView.View.Name}";
            }

            // Write our updated file back to the commitFileData reference
            commitFileData = Encoding.Default.GetBytes(simulatedFileData.ToXml());

            return targetWorkbook;
        }

        protected override WorkbookResponse.WorkbookType BuildResponse(WorkbookResponse.WorkbookType workbook)
        {
            WorkbookResponse.WorkbookType workbookType = workbook;

            if (workbook.Owner is not null)
            {
                workbookType.Owner = new()
                {
                    Id = workbook.Owner.Id
                };
            }

            if (workbook.Project is not null)
            {
                workbookType.Project ??= new()
                {
                    Id = workbook.Project.Id,
                    Name = workbook.Project.Name,
                };
            }

            var tags = new List<WorkbookResponse.WorkbookType.TagType>();

            if (workbook.Tags is not null)
            {
                foreach (var workbookTag in workbook.Tags)
                {
                    tags.Add(new()
                    {
                        Label = workbookTag.Label
                    });
                }
            }

            workbookType.Tags = tags.ToArray();

            return workbookType;
        }
    }
}

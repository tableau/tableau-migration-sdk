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
using System.Linq;
using System.Text;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal class RestCommitCustomViewUploadResponseBuilder
        : RestCommitUploadResponseBuilder<CustomViewResponse, CustomViewResponse.CustomViewType, CommitCustomViewPublishRequest>
    {
        public RestCommitCustomViewUploadResponseBuilder(
            TableauData data,
            IHttpContentSerializer serializer)
            : base(
                  data,
                  serializer,
                  data => data.CustomViews,
                  data => data.CustomViewFiles,
                  (data, cv, file) => data.AddCustomView(cv, file))
        { }

        protected override CustomViewResponse.CustomViewType? GetExistingContentItem(
            CommitCustomViewPublishRequest commitRequest)
        {
            var commitCustomView = Guard.AgainstNull(commitRequest.CustomView, () => commitRequest.CustomView);

            var targetCustomView = Data.CustomViews
                .SingleOrDefault(wb =>
                    string.Equals(wb.Name, commitCustomView.Name, CustomView.NameComparison));

            return targetCustomView;
        }

        protected override CustomViewResponse.CustomViewType BuildContent(
            CommitCustomViewPublishRequest commitRequest,
            ref byte[] commitFileData,
            CustomViewResponse.CustomViewType? existingContent,
            UsersResponse.UserType currentUser,
            bool overwrite)
        {
            var commitCustomView = Guard.AgainstNull(commitRequest.CustomView, () => commitRequest.CustomView);

            SimulatedCustomViewData? simulatedFileData;
            try
            {
                simulatedFileData = Encoding.Default.GetString(commitFileData).FromJson<SimulatedCustomViewData>();
                if (simulatedFileData is null)
                {
                    throw new BuildResponseException(System.Net.HttpStatusCode.BadRequest, 8, "Unable to parse file", "");
                }
            }
            catch (Exception)
            {
                throw new BuildResponseException(System.Net.HttpStatusCode.BadRequest, 8, "Unable to parse file", "");
            }

            var targetCustomView = existingContent ?? new CustomViewResponse.CustomViewType
            {
                Id = Guid.NewGuid(),
                Name = commitCustomView.Name,
                CreatedAt = DateTime.UtcNow.ToString(),
                UpdatedAt = DateTime.UtcNow.ToString(),
                LastAccessedAt = DateTime.UtcNow.ToString(),
                Shared = true,
                Owner = new CustomViewResponse.CustomViewType.OwnerType
                {
                    Id = Guid.NewGuid()                
                },
                Workbook = new CustomViewResponse.CustomViewType.WorkbookType
                {
                    Id = Guid.NewGuid()
                },
                View = new CustomViewResponse.CustomViewType.ViewType(Guid.NewGuid(), $"ViewName{Guid.NewGuid()}")
            };

            targetCustomView.Name = commitCustomView.Name;
            targetCustomView.UpdatedAt = DateTime.UtcNow.ToString();
            targetCustomView.Shared = commitCustomView.Shared;
            targetCustomView.Workbook = new CustomViewResponse.CustomViewType.WorkbookType
            {
                Id = commitCustomView.Workbook == null ? Guid.NewGuid() : commitCustomView.Workbook.Id
            };

            targetCustomView.Owner = new()
            {
                Id = currentUser.Id
            };

            targetCustomView.View = new CustomViewResponse.CustomViewType.ViewType(Guid.NewGuid(), $"ViewName{Guid.NewGuid()}");

            commitFileData = Encoding.Default.GetBytes(simulatedFileData.ToJson());

            return targetCustomView;
        }

        protected override CustomViewResponse.CustomViewType BuildResponse(CustomViewResponse.CustomViewType customView)
            => customView;
    }
}

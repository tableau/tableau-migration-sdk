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
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal class RestCommitFlowUploadResponseBuilder : RestCommitUploadResponseBuilder<FlowResponse, FlowResponse.FlowType, CommitFlowPublishRequest>
    {
        public RestCommitFlowUploadResponseBuilder(
            TableauData data,
            IHttpContentSerializer serializer)
            : base(
                  data,
                  serializer,
                  data => data.Flows,
                  data => data.FlowFiles,
                  (data, flow, file) => data.AddFlow(flow, file))
        { }

        protected override FlowResponse.FlowType? GetExistingContentItem(CommitFlowPublishRequest commitRequest)
        {
            var commitFlow = Guard.AgainstNull(commitRequest.Flow, () => commitRequest.Flow);

            var targetFlow = Data.Flows
                .SingleOrDefault(f =>
                    f.Project?.Id == commitFlow.Project?.Id &&
                    string.Equals(f.Name, commitFlow.Name, Flow.NameComparison));

            return targetFlow;
        }

        protected override FlowResponse.FlowType BuildContent(
            CommitFlowPublishRequest commitRequest,
            ref byte[] commitFileData,
            FlowResponse.FlowType? existingContent,
            UsersResponse.UserType currentUser,
            bool overwrite)
        {
            var commitFlow = Guard.AgainstNull(commitRequest.Flow, () => commitRequest.Flow);

            var targetFlow = existingContent ?? new FlowResponse.FlowType
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow.ToString(),
                FileType = "flow"
            };

            targetFlow.Name = commitFlow.Name;
            targetFlow.Description = commitFlow.Description;
            targetFlow.UpdatedAt = DateTime.UtcNow.ToString();

            targetFlow.Owner = new()
            {
                Id = currentUser.Id
            };

            targetFlow.Project = new()
            {
                Id = commitFlow.Project?.Id ?? Data.DefaultProject.Id
            };

            targetFlow.Tags = [];

            // Publishing resets embedded credentials.
            Data.FlowKeychains.AddOrUpdate(targetFlow.Id, new RetrieveKeychainResponse(), (_, _) => new RetrieveKeychainResponse());

            return targetFlow;
        }

        protected override FlowResponse.FlowType BuildResponse(FlowResponse.FlowType flow)
        {
            FlowResponse.FlowType flowType = flow;

            if (flow.Owner is not null)
            {
                flowType.Owner = new()
                {
                    Id = flow.Owner.Id
                };
            }

            if (flow.Project is not null)
            {
                flowType.Project = new()
                {
                    Id = flow.Project.Id,
                    Name = flow.Project.Name,
                };
            }

            var tags = new List<FlowResponse.FlowType.TagType>();

            if (flow.Tags is not null)
            {
                foreach (var flowTag in flow.Tags)
                {
                    tags.Add(new()
                    {
                        Label = flowTag.Label
                    });
                }
            }

            flowType.Tags = tags.ToArray();

            return flowType;
        }
    }
}


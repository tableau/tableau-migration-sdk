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
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal class RestUserImportResponseBuilder : RestApiResponseBuilderBase<ImportJobResponse>
    {
        public RestUserImportResponseBuilder(TableauData data, IHttpContentSerializer serializer)
            : base(data, serializer, requiresAuthentication: true)
        { }

        protected static void AddUsers(TableauData data, StreamContent csvStreamContent)
        {
            var result = new List<UsersResponse.UserType>();
            using var stream = new MemoryStream();
            csvStreamContent.CopyTo(stream, null, new System.Threading.CancellationToken());
            stream.Seek(0, SeekOrigin.Begin);

            using (var textFieldParser = new TextFieldParser(stream))
            {
                textFieldParser.TextFieldType = FieldType.Delimited;
                textFieldParser.SetDelimiters(",");
                while (!textFieldParser.EndOfData)
                {
                    var userRows = textFieldParser.ReadFields();

                    if (userRows is null)
                        continue;

                    result.Add(ParseUser(data, userRows));
                }
            }

            foreach(var user in result)
            {
                data.AddUser(user);
            }
        }

        private static UsersResponse.UserType ParseUser(TableauData data, string[] columnData)
        {
            var username = columnData[0];
            string licenseLevel = columnData[3];
            string adminLevel = columnData[4];
            string publishingCapability = columnData[5];
            
            if (!bool.TryParse(publishingCapability, out bool canPublish))
            {
                throw new ArgumentException(
                    $"Publishing Capability should be boolean. Current value is {publishingCapability}.",
                    nameof(columnData));
            }

            return new UsersResponse.UserType
            {
                Id = Guid.NewGuid(),
                Name = username,
                SiteRole = SiteRoleMapping.GetSiteRole(adminLevel, licenseLevel, canPublish),
                Domain = TableauData.GetUserDomain(username) ?? new() { Name = data.DefaultDomain }
            };
        }

        protected override ValueTask<(ImportJobResponse Response, HttpStatusCode ResponseCode)> BuildResponseAsync(HttpRequestMessage request, CancellationToken cancel)
        {
            if (request.Content is not MultipartFormDataContent dataContents)
            {
                return BuildEmptyErrorResponseAsync(
                    HttpStatusCode.BadRequest,
                    0,
                    $"Form data must be {nameof(MultipartFormDataContent)}",
                    "");
            }

            var dataContent = dataContents.FirstOrDefault(
                x => x != null &&
                string.Equals("tableau_user_import", x.Headers.ContentDisposition?.Name, StringComparison.OrdinalIgnoreCase));

            if (dataContent is null)
            {
                return BuildEmptyErrorResponseAsync(
                    HttpStatusCode.BadRequest,
                    0,
                    $"{nameof(MultipartFormDataContent)} must contain the part tableau_user_import",
                    "");
            }

            var name = dataContent.Headers.ContentDisposition?.Name;

            AddUsers(Data, (StreamContent)dataContent);

            var job = new ImportJobResponse
            {
                Item = new ImportJobResponse.ImportJobType
                {
                    Id = Guid.NewGuid(),
                    FinishCode = 0,
                    Mode = "Asynchronous",
                    Progress = 0,
                    Type = "UserImport",
                    CreatedAt = DateTime.Now.ToIso8601()
                }
            };

            Data.AddJob(new JobResponse.JobType
            {
                Id = job.Item.Id,
                FinishCode = job.Item.FinishCode,
                Mode = job.Item.Mode,
                Progress = 100,
                Type = job.Item.Type,
                CreatedAt = job.Item.CreatedAt,
                CompletedAt = DateTime.Now.ToIso8601(),
            });

            return ValueTask.FromResult((job, HttpStatusCode.OK));
        }
    }
}


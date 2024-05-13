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
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Models
{
    public class ImportJobTests
    {
        public class Ctor : AutoFixtureTestBase
        {
            [Fact]
            public void Throws_when_job_is_null()
            {
                var response = Create<ImportJobResponse>();
                response.Item = null;

                var exception = Assert.Throws<ArgumentNullException>(() => new ImportJob(response));

                Assert.Equal("response.Item", exception.ParamName);
            }

            [Fact]
            public void Throws_when_job_id_is_default()
            {
                var response = Create<ImportJobResponse>();
                response.Item!.Id = Guid.Empty;

                var exception = Assert.Throws<ArgumentException>(() => new ImportJob(response));

                Assert.Equal("response.Item.Id", exception.ParamName);
            }

            [Fact]
            public void Throws_when_job_type_is_null()
            {
                var response = Create<ImportJobResponse>();
                response.Item!.Type = null;

                var exception = Assert.Throws<ArgumentNullException>(() => new ImportJob(response));

                Assert.Equal("response.Item.Type", exception.ParamName);
            }

            [Theory]
            [NullEmptyWhiteSpaceData]
            public void Throws_when_job_created_timestamp_is_null(string? createdAt)
            {
                var response = Create<ImportJobResponse>();
                response.Item!.CreatedAt = createdAt;

                var exception = Assert.Throws<ArgumentException>(() => new ImportJob(response));

                Assert.Equal("response.Item.CreatedAt", exception.ParamName);
            }

            [Fact]
            public void Initializes()
            {
                var response = Create<ImportJobResponse>();
                Assert.NotNull(response.Item);

                var model = new ImportJob(response);

                Assert.Equal(model.Id, response.Item.Id);
                Assert.Equal(model.Type, response.Item.Type);
                Assert.Equal(model.CreatedAtUtc, response.Item.CreatedAt!.ParseFromIso8601());
                Assert.Equal(model.ProgressPercentage, response.Item.Progress);
            }
        }
    }
}

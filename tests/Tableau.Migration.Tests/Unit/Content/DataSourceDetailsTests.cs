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

using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content
{
    public class DataSourceDetailsTests
    {
        public abstract class DataSourceDetailsTest : AutoFixtureTestBase
        {
            public IDataSourceDetails CreateDataSource() => Create<IDataSourceDetails>();
            public IDataSourceDetailsType CreateResponse() => Create<IDataSourceDetailsType>();

            public IContentReference CreateProjectReference() => Create<IContentReference>();
            public IContentReference CreateOwnerReference() => Create<IContentReference>();
        }

        public class Ctor
        {
            public class FromResponse : DataSourceDetailsTest
            {
                [Fact]
                public void Initializes()
                {
                    var response = CreateResponse();
                    var project = CreateProjectReference();
                    var owner = CreateOwnerReference();

                    var result = new DataSourceDetails(response, project, owner);

                    result.Assert(response, project, owner, ds => Assert.Equal(response.CertificationNote, ds.CertificationNote));
                }
            }

            public class FromDataSource : DataSourceDetailsTest
            {
                [Fact]
                public void Initializes()
                {
                    var dataSource = CreateDataSource();

                    var result = new DataSourceDetails(dataSource);

                    result.Assert(dataSource, ds => Assert.Equal(dataSource.CertificationNote, ds.CertificationNote));
                }
            }
        }
    }
}

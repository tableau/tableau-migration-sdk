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
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content
{
    public class WorkbookDetailsTests
    {
        public abstract class WorkbookDetailsTest : AutoFixtureTestBase
        {
            public IWorkbookDetails CreateWorkbook() => Create<IWorkbookDetails>();
            public IWorkbookDetailsType CreateResponse() => Create<IWorkbookDetailsType>();

            public IContentReference CreateProjectReference() => Create<IContentReference>();
            public IContentReference CreateOwnerReference() => Create<IContentReference>();
        }

        public class Ctor
        {
            public class FromResponse : WorkbookDetailsTest
            {
                [Fact]
                public void Initializes()
                {
                    var response = CreateResponse();
                    var project = CreateProjectReference();
                    var owner = CreateOwnerReference();

                    var result = new WorkbookDetails(response, project, owner);

                    result.Assert(response, project, owner, wb =>
                    {
                        foreach (var responseView in response.Views)
                        {
                            Assert.Single(wb.Views, v =>
                                v.Id == responseView.Id &&
                                v.Name == responseView.Name &&
                                v.ContentUrl == responseView.ContentUrl &&
                                v.Location == new ContentLocation(project.Location, wb.Name).Append(responseView.Name) &&
                                v.Tags.Select(t => t.Label).SequenceEqual(responseView.Tags.Select(t => t.Label)));
                        }
                    });
                }
            }

            public class FromWorkbook : WorkbookDetailsTest
            {
                [Fact]
                public void Initializes()
                {
                    var workbook = CreateWorkbook();

                    var result = new WorkbookDetails(workbook);

                    result.Assert(workbook, wb => Assert.Same(workbook.Views, wb.Views));
                }
            }
        }
    }
}

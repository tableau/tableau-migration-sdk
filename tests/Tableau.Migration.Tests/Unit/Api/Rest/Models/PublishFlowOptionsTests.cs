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

using System.IO;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models
{
    public class PublishFlowOptionsTests
    {
        public class Ctor : AutoFixtureTestBase
        {
            [Fact]
            public void Initializes()
            {
                var flow = Create<IPublishableFlow>();
                using var file = new MemoryStream();
                var fileType = FlowFileTypes.Tfl;

                var opts = new PublishFlowOptions(flow, file, fileType);

                Assert.Equal(flow.Name, opts.Name);
                Assert.Equal(flow.Description, opts.Description);
                Assert.Equal(((IContainerContent)flow).Container.Id, opts.ProjectId);
                Assert.Equal(flow.File.OriginalFileName, opts.FileName);

                Assert.Same(file, opts.File);
                Assert.Equal(fileType, opts.FileType);
            }
        }
    }
}

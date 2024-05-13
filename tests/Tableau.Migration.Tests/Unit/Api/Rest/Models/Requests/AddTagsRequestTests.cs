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

using Moq;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Requests;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models.Requests
{
    public class AddTagsRequestTests
    {
        public class TagType
        {
            public class LabelQuoting
            {
                [Theory]
                [InlineData(null)]
                [InlineData("")]
                [InlineData("text")]
                [InlineData("a tag")]
                public void RoundtripsQuotedLabel(string? label)
                {
                    var mockTag = new Mock<ITagType>();
                    mockTag.Setup(x => x.Label).Returns(label);

                    var t = new AddTagsRequest.TagType(mockTag.Object);

                    Assert.Equal(label, t.Label);
                    if (label.IsNullOrEmpty())
                    {
                        Assert.Equal(label, t.QuotedLabel);
                    }
                    else
                    {
                        Assert.Equal($"\"{label}\"", t.QuotedLabel);
                    }
                }
            }
        }
    }
}

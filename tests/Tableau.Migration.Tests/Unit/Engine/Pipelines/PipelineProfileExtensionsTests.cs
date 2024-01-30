// Copyright (c) 2023, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the ""License"") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an ""AS IS"" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System;
using Tableau.Migration.Engine.Pipelines;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Pipelines
{
    public class PipelineProfileExtensionsTests
    {
        public class GetSupportedContentTyeps
        {
            [Fact]
            public void AllPipelineProfilesReturnResults()
            {
                foreach (var val in Enum.GetValues<PipelineProfile>())
                {
                    try
                    {
                        var contentTypes = val.GetSupportedContentTypes();

                        //Empty results are fine, just don't want to throw the default argument exception.
                    }
                    catch (Exception)
                    {
                        Assert.Fail($"Content type {val} does not have defined content type support. Add a case statement to {nameof(PipelineProfileExtensions.GetSupportedContentTypes)}.");
                    }
                }
            }
        }
    }
}

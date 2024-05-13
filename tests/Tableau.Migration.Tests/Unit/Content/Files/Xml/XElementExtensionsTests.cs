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

using System.Collections.Immutable;
using System.Xml.Linq;
using Tableau.Migration.Content.Files.Xml;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content.Files.Xml
{
    public class XElementExtensionsTests
    {
        public class GetFeatureFlaggedAttributes
        {
            [Fact]
            public void FFSAware()
            {
                var el = XElement.Parse("<el _.fcp.Flag.true...test='yes' _.fcp.Flag.false...test='no' test='maybe' />");

                var testAttrs = el.GetFeatureFlaggedAttributes("test")
                    .ToImmutableArray();

                Assert.Equal(3, testAttrs.Length);
            }
        }
    }
}

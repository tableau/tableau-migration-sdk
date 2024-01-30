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

using System.Threading.Tasks;
using System.Xml.Linq;
using Moq;
using Tableau.Migration.Content.Files;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Interop.Hooks.Transformers;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Interop.Hooks.Transformers
{
    public class ISyncXmlContentTransformerTests
    {
        public class ExecuteAsync : AutoFixtureTestBase
        {
            public class TestImplementation : ISyncXmlContentTransformer<TestFileContentType>
            {
                public virtual void Execute(TestFileContentType ctx, XDocument xml) { }

                public virtual bool NeedsXmlTransforming(TestFileContentType ctx) => true;
            }

            [Fact]
            public async Task CallsExecuteAsync()
            {
                var mockTransformer = new Mock<TestImplementation>()
                {
                    CallBase = true
                };

                var mockFile = Freeze<Mock<IContentFileHandle>>();
                var ctx = Create<TestFileContentType>();

                var mockXmlStream = Freeze<Mock<ITableauFileXmlStream>>();
                var xml = Freeze<XDocument>();

                var result = await ((IMigrationHook<TestFileContentType>)mockTransformer.Object).ExecuteAsync(ctx, Cancel);

                Assert.Same(ctx, result);

                mockFile.Verify(x => x.GetXmlStreamAsync(Cancel), Times.Once);
                mockXmlStream.Verify(x => x.GetXmlAsync(Cancel), Times.Once);

                mockTransformer.Verify(x => x.Execute(ctx, xml), Times.Once);
            }
        }
    }
}

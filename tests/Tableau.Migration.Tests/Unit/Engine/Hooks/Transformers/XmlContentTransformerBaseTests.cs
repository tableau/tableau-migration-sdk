﻿//
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
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Moq;
using Tableau.Migration.Content.Files;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Engine.Hooks.Transformers;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Transformers
{
    public class XmlContentTransformerBaseTests
    {
        public class XmlContentTransformerBaseTest : AutoFixtureTestBase
        {
            protected readonly TestXmlTransformer Transformer;

            public XmlContentTransformerBaseTest()
            {
                Transformer = new();
            }

            protected (TestFileContentType item, Mock<IContentFileHandle> mockFile) CreateTestItem()
            {
                var mockFile = Create<Mock<IContentFileHandle>>();

                return (new(mockFile.Object), mockFile);
            }

            protected async Task<TestFileContentType?> ExecuteAsync(TestFileContentType ctx)
            {
                return await ((IXmlContentTransformer<TestFileContentType>)Transformer).ExecuteAsync(ctx, Cancel);
            }
        }

        public class TestXmlTransformer : XmlContentTransformerBase<TestFileContentType>
        {
            public Func<TestFileContentType, bool>? NeedsXmlTransformingFilter { get; set; }

            public Action<TestFileContentType, XDocument>? TransformXml { get; set; }

            protected override bool NeedsXmlTransforming(TestFileContentType ctx)
                => NeedsXmlTransformingFilter?.Invoke(ctx) ?? base.NeedsXmlTransforming(ctx);

            public override Task TransformAsync(TestFileContentType ctx, XDocument xml, CancellationToken cancel)
            {
                TransformXml?.Invoke(ctx, xml);
                return Task.CompletedTask;
            }
        }

        #region - NeedsXmlTransforming -

        public class NeedsXmlTransforming : XmlContentTransformerBaseTest
        {
            [Fact]
            public async Task CanFilterItemsToTransformAsync()
            {
                var (ctx1, mockFile1) = CreateTestItem();
                var (ctx2, mockFile2) = CreateTestItem();

                Transformer.NeedsXmlTransformingFilter = ctx => Object.ReferenceEquals(ctx, ctx1);

                await ExecuteAsync(ctx1);
                await ExecuteAsync(ctx2);

                mockFile1.Verify(x => x.GetXmlStreamAsync(Cancel), Times.Once);
                mockFile2.Verify(x => x.GetXmlStreamAsync(Cancel), Times.Never);
            }
        }

        #endregion

        #region - TransformAsync -

        public class TransformAsync : XmlContentTransformerBaseTest
        {
            public class TestOverwriteExecuteXmlTransformer : XmlContentTransformerBase<TestFileContentType>,
                IMigrationHook<TestFileContentType>
            {
                protected override bool NeedsXmlTransforming(TestFileContentType ctx)
                {
                    throw new NotImplementedException();
                }

                public override Task TransformAsync(TestFileContentType ctx, XDocument xml, CancellationToken cancel)
                {
                    throw new NotImplementedException();
                }

                Task<TestFileContentType?> IMigrationHook<TestFileContentType>.ExecuteAsync(TestFileContentType ctx, CancellationToken cancel)
                {
                    return Task.FromResult((TestFileContentType?)ctx);
                }
            }

            [Fact]
            public async Task CanOverwriteInterfaceDefaultAsync()
            {
                var transformer = new TestOverwriteExecuteXmlTransformer();

                var ctx = Create<TestFileContentType>();

                var result = await ((IXmlContentTransformer<TestFileContentType>)transformer).ExecuteAsync(ctx, Cancel);

                Assert.Same(ctx, result);
            }
        }

        #endregion
    }
}

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

using Tableau.Migration.Engine.Pipelines;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Pipelines
{
    public class MigrationPipelineContentTypeExtensionsTests
    {
        public class WithContentTypeInterface
        {
            public class NonGeneric
            {
                [Fact]
                public void Finds_types()
                {
                    var contentTypes = new[]
                    {
                        new MigrationPipelineContentType(typeof(TestContentType), typeof(object)),
                        new MigrationPipelineContentType(typeof(object), typeof(TestContentType))
                    };

                    var results = contentTypes.WithContentTypeInterface(typeof(IContentReference));

                    var result = Assert.Single(results);

                    Assert.Equal(new[] { typeof(TestContentType) }, result);
                }
            }

            public class Generic
            {
                [Fact]
                public void Finds_types()
                {
                    var contentTypes = new[]
                    {
                        new MigrationPipelineContentType(typeof(TestContentType), typeof(object)),
                        new MigrationPipelineContentType(typeof(object), typeof(TestContentType))
                    };

                    var results = contentTypes.WithContentTypeInterface<IContentReference>();

                    var result = Assert.Single(results);

                    Assert.Equal(new[] { typeof(TestContentType) }, result);
                }
            }
        }

        public class WithPublishTypeInterface
        {
            public class NonGeneric
            {
                [Fact]
                public void Finds_types()
                {
                    var contentTypes = new[]
                    {
                        new MigrationPipelineContentType(typeof(TestContentType), typeof(object)),
                        new MigrationPipelineContentType(typeof(object), typeof(TestContentType))
                    };

                    var results = contentTypes.WithPublishTypeInterface(typeof(IContentReference));

                    var result = Assert.Single(results);

                    Assert.Equal(new[] { typeof(TestContentType) }, result);
                }
            }

            public class Generic
            {
                [Fact]
                public void Finds_types()
                {
                    var contentTypes = new[]
                    {
                        new MigrationPipelineContentType(typeof(TestContentType), typeof(object)),
                        new MigrationPipelineContentType(typeof(object), typeof(TestContentType))
                    };

                    var results = contentTypes.WithPublishTypeInterface<IContentReference>();

                    var result = Assert.Single(results);

                    Assert.Equal(new[] { typeof(TestContentType) }, result);
                }
            }
        }

        public class WithPostPublishTypeInterface
        {
            public class NonGeneric
            {
                [Fact]
                public void Finds_types()
                {
                    var contentTypes = new[]
                    {
                        new MigrationPipelineContentType(typeof(TestContentType), typeof(object), typeof(int)),
                        new MigrationPipelineContentType(typeof(object), typeof(TestContentType), typeof(float))
                    };

                    var results = contentTypes.WithPostPublishTypeInterface(typeof(IContentReference));

                    var result = Assert.Single(results);

                    Assert.Equal(new[] { typeof(TestContentType), typeof(float) }, result);
                }
            }

            public class Generic
            {
                [Fact]
                public void Finds_types()
                {
                    var contentTypes = new[]
                    {
                        new MigrationPipelineContentType(typeof(TestContentType), typeof(object), typeof(int)),
                        new MigrationPipelineContentType(typeof(object), typeof(TestContentType), typeof(float))
                    };

                    var results = contentTypes.WithPostPublishTypeInterface<IContentReference>();

                    var result = Assert.Single(results);

                    Assert.Equal(new[] { typeof(TestContentType), typeof(float) }, result);
                }
            }
        }
    }
}

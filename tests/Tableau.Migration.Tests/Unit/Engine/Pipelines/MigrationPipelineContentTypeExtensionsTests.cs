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

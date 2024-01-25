using Tableau.Migration.Engine.Pipelines;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Pipelines
{
    public class MigrationPipelineContentTypeTests
    {
        public class Ctor
        {
            [Fact]
            public void Different_types()
            {
                var type1 = typeof(object);
                var type2 = typeof(string);

                var t = new MigrationPipelineContentType(type1, type2);

                Assert.Same(type1, t.ContentType);
                Assert.Same(type2, t.PublishType);
            }

            [Fact]
            public void Different_content_and_publish_types()
            {
                var type1 = typeof(object);
                var type2 = typeof(string);

                var t = new MigrationPipelineContentType(type1, type2);

                Assert.Same(type1, t.ContentType);
                Assert.Same(type2, t.PublishType);
                Assert.Same(type1, t.ResultType);
            }

            [Fact]
            public void Same_content_and_publish_types()
            {
                var type = typeof(object);

                var t = new MigrationPipelineContentType(type);

                Assert.Same(type, t.ContentType);
                Assert.Same(type, t.PublishType);
                Assert.Same(type, t.ResultType);
            }
        }

        public class GetContentTypeForInterface
        {
            [Fact]
            public void Returns_null_when_not_found()
            {
                var t = new MigrationPipelineContentType(typeof(object), typeof(TestContentType));

                Assert.Null(t.GetContentTypeForInterface(typeof(IContentReference)));
            }

            [Fact]
            public void Returns_content_type_when_found()
            {
                var type = typeof(TestContentType);

                var t = new MigrationPipelineContentType(type, typeof(object));

                Assert.Equal(new[] { type }, t.GetContentTypeForInterface(typeof(IContentReference)));
            }
        }

        public class GetPublishTypeForInterface
        {
            [Fact]
            public void Returns_null_when_not_found()
            {
                var t = new MigrationPipelineContentType(typeof(TestContentType), typeof(object));

                Assert.Null(t.GetPublishTypeForInterface(typeof(IContentReference)));
            }

            [Fact]
            public void Returns_publish_type_when_found()
            {
                var type = typeof(TestContentType);

                var t = new MigrationPipelineContentType(typeof(object), type);

                Assert.Equal(new[] { type }, t.GetPublishTypeForInterface(typeof(IContentReference)));
            }
        }

        public class GetPostPublishTypesForInterface
        {
            [Fact]
            public void Returns_null_when_not_found()
            {
                var t = new MigrationPipelineContentType(typeof(TestContentType), typeof(object));

                Assert.Null(t.GetPostPublishTypesForInterface(typeof(IContentReference)));
            }

            [Fact]
            public void Returns_publish_type_when_found()
            {
                var type = typeof(TestContentType);

                var t = new MigrationPipelineContentType(typeof(object), type, typeof(int));

                Assert.Equal(new[] { type, typeof(int) }, t.GetPostPublishTypesForInterface(typeof(IContentReference)));
            }
        }
    }
}

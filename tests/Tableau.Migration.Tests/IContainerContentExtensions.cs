using Moq;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content;

namespace Tableau.Migration.Tests
{
    public static class IContainerContentExtensions
    {
        public static Mock<TItem> WithProject<TItem>(this Mock<TItem> mock, IProjectType project)
            where TItem : class, IContainerContent
        {
            mock.As<IContainerContent>().SetupGet(p => p.Container)
                .Returns(new ContentReferenceStub(project.Id, "", new ContentLocation(), project.Name!));

            return mock;
        }
    }
}

using System.Collections.Immutable;
using Tableau.Migration.Api.Rest.Models;

namespace Tableau.Migration.Content
{
    internal class ResultWorkbook : ViewsWorkbook, IResultWorkbook
    {
        public ResultWorkbook(IWorkbookType response, IContentReference project, IContentReference owner, IImmutableList<IView> views)
            : base(response, project, owner, views)
        { }
    }
}

using System.Collections.Immutable;

namespace Tableau.Migration.Paging
{
    /// <summary>
    /// <see cref="IResult"/> interface for a page of data.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    public interface IPagedResult<TItem> : IResult<IImmutableList<TItem>>, IPageInfo
    { }
}

using Tableau.Migration;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Paging;

namespace Csharp.ExampleApplication.Services.MigrationContentLoader
{
    #region class
    public class EmptyMigrationContentLoader<TContent> : IMigrationContentLoader<TContent>
        where TContent : IContentReference
    {
        public IPager<TContent> GetMigrationContentPager(int pageSize)
            => new MemoryPager<TContent>([], pageSize);
    }
    #endregion
}


namespace Tableau.Migration.Api
{
    internal sealed class TableauServerVersionProvider : ITableauServerVersionProvider
    {
        public TableauServerVersion? Version { get; private set; }

        public void Set(TableauServerVersion version) => Version = version;
    }
}

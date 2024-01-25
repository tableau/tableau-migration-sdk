namespace Tableau.Migration.Engine.Hooks.Filters.Default
{
    /// <summary>
    /// Options for <see cref="PreviouslyMigratedFilter{TContent}"/>.
    /// </summary>
    public class PreviouslyMigratedFilterOptions
    {
        /// <summary>
        /// Gets whether or not to disable the default previously migrated filter.
        /// </summary>
        public bool Disabled { get; init; }
    }
}

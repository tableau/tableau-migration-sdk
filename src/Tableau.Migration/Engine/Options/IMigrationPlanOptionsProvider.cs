namespace Tableau.Migration.Engine.Options
{
    /// <summary>
    /// Interface for an object that can provide per-migration options of a given type,
    /// potentially falling back to global options or default values.
    /// </summary>
    /// <typeparam name="TOptions">The options type.</typeparam>
    public interface IMigrationPlanOptionsProvider<TOptions>
        where TOptions : class, new()
    {
        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <returns>The options</returns>
        TOptions Get();
    }
}

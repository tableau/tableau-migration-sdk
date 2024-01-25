namespace Tableau.Migration.Engine
{
    /// <summary>
    /// Interface for an object that can initialize a <see cref="IMigrationInput"/> object.
    /// </summary>
    /// <remarks>
    /// This interface is internal because it is only used to build a <see cref="IMigrationInput"/> object, 
    /// which in turn is only used to build a <see cref="IMigration"/> object.
    /// End users are intended to inject the final <see cref="IMigration"/> result and not bootstrap objects.
    /// </remarks>
    internal interface IMigrationInputInitializer
    {
        /// <summary>
        /// Initializes the <see cref="IMigrationInput"/> object.
        /// </summary>
        /// <param name="plan">The migration plan to execute.</param>
        /// <param name="previousManifest">A manifest from a previous migration of the same plan to use to determine what progress has already been made.</param>
        void Initialize(IMigrationPlan plan, IMigrationManifest? previousManifest);
    }
}

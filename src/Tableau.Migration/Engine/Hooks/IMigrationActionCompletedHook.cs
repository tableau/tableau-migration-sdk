using Tableau.Migration.Engine.Actions;

namespace Tableau.Migration.Engine.Hooks
{
    /// <summary>
    /// Interface representing a hook called when a <see cref="IMigrationAction"/> completes.
    /// </summary>
    public interface IMigrationActionCompletedHook : IMigrationHook<IMigrationActionResult>
    { }
}

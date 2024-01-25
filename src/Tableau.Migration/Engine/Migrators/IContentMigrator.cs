using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Engine.Migrators
{
    /// <summary>
    /// Interface for an object that can migrate content for a specific content type.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public interface IContentMigrator<TContent>
        where TContent : class, IContentReference
    {
        /// <summary>
        /// Migrates content for the given content type.
        /// </summary>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>A task to await.</returns>
        Task<IResult> MigrateAsync(CancellationToken cancel);
    }
}

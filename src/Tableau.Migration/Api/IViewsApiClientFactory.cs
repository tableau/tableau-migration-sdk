namespace Tableau.Migration.Api
{
    /// <summary>
    /// Interface for an object that can create <see cref="IViewsApiClient"/> objects
    /// </summary>
    public interface IViewsApiClientFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IViewsApiClient Create();
    }
}
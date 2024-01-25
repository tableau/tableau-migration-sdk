using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Tableau.Migration.Resources
{
    /// <summary>
    /// Interface for an object that can provide localized string from shared Tableau Migration SDK resources.
    /// </summary>
    /// <remarks>
    /// This interface is used so that <see cref="IStringLocalizer"/>s can be configured to use library resource files,
    /// regardless of where resource paths are configured by the user application through 
    /// <see cref="LocalizationServiceCollectionExtensions.AddLocalization(IServiceCollection, System.Action{LocalizationOptions})"/>.
    /// 
    /// This interface is registered automatically through <see cref="IServiceCollectionExtensions.AddTableauMigrationSdk(IServiceCollection, Microsoft.Extensions.Configuration.IConfiguration?)"/>
    /// and is not intended for use outside of the Tableau Migration SDK.
    /// </remarks>
    public interface ISharedResourcesLocalizer : IStringLocalizer
    { }
}

using Microsoft.Extensions.Options;

namespace Tableau.Migration.Config
{
    /// <summary>
    /// Methods to read the current <see cref="MigrationSdkOptions"/>.
    /// </summary>
    public class ConfigReader : IConfigReader
    {
        private readonly IOptionsMonitor<MigrationSdkOptions> _optionsMonitor;

        /// <summary>
        /// Creates a new <see cref="ConfigReader"/> object.
        /// </summary>
        /// <param name="optionsMonitor">The object to monitor configuration with.</param>
        public ConfigReader(IOptionsMonitor<MigrationSdkOptions> optionsMonitor)
        {
            _optionsMonitor = optionsMonitor;
        }

        /// <summary>
        /// Get the current <see cref="MigrationSdkOptions"/>
        /// The configuration values are auto-reloaded from supplied configuration
        /// Ex: .json file.
        /// </summary>
        /// <returns>The <see cref="MigrationSdkOptions"/></returns>
        public MigrationSdkOptions Get()
        {
            return _optionsMonitor.Get(nameof(MigrationSdkOptions));
        }
    }
}

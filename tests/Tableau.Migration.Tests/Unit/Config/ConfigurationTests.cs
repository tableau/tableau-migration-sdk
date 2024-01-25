using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tableau.Migration.Config;
using Tableau.Migration.Content.Permissions;
using Xunit;
using Xunit.Abstractions;

namespace Tableau.Migration.Tests.Unit.Config
{
    [Category("Configuration")]
    public class ConfigurationTests
        : IDisposable
    {
        private static readonly string ASSEMBLY_PATH = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        private static readonly string TEST_DATA_PATH = Path.Combine(ASSEMBLY_PATH, TEST_DATA_DIR);
        private static readonly string TEST_DATA_FILE1_PATH = Path.Combine(TEST_DATA_PATH, TEST_DATA_FILE1);
        private static readonly string TEST_DATA_FILE2_PATH = Path.Combine(TEST_DATA_PATH, TEST_DATA_FILE2);
        private static readonly string TEST_DATA_FILE3_PATH = Path.Combine(TEST_DATA_PATH, TEST_DATA_FILE3);
        private const string TEST_DATA_DIR = "TestData";
        private const string TEST_DATA_FILE1 = "configuration_testdata1.json";
        private static readonly string TEST_DATA_FILE1_CONTENT =
$@"{{
  ""MigrationSdkOptions"": {{
    ""Network"": {{
      ""{nameof(MigrationSdkOptions.Network.FileChunkSizeKB)}"": 2034
    }}
  }}
}}";
        private const string TEST_DATA_FILE2 = "configuration_testdata2.json";
        private static readonly string TEST_DATA_FILE2_CONTENT =
$@"{{
  ""MigrationSdkOptions"": {{
    ""Network"": {{
      ""{nameof(MigrationSdkOptions.Network.FileChunkSizeKB)}"": 2034
    }}
  }}
}}";

        private const string TEST_DATA_FILE3 = "configuration_testdata3.json";
        private static readonly string TEST_DATA_FILE3_CONTENT =
$@"{{
  ""MigrationSdkOptions"": {{
    ""DefaultPermissionsContentTypes"": {{
      ""{nameof(MigrationSdkOptions.DefaultPermissionsContentTypes.UrlSegments)}"": [""test1"", ""test2""]
    }}
  }}
}}";

        private readonly ITestOutputHelper _output;

        private readonly bool skipGithubRunnerTests;

        public ConfigurationTests(ITestOutputHelper output)
        {
            _output = output;

            var config = Environment.GetEnvironmentVariable("MIGRATIONSDK_SKIP_FLAKY_TESTS");

            skipGithubRunnerTests = config?.Equals("yes", StringComparison.OrdinalIgnoreCase) ?? false;

            Directory.CreateDirectory(
                TEST_DATA_PATH);
            File.WriteAllText(
                TEST_DATA_FILE1_PATH,
                TEST_DATA_FILE1_CONTENT);
            File.WriteAllText(
                TEST_DATA_FILE2_PATH,
                TEST_DATA_FILE2_CONTENT);
            File.WriteAllText(
                TEST_DATA_FILE3_PATH,
                TEST_DATA_FILE3_CONTENT);
        }

        public void Dispose()
        {
            Directory.Delete(TEST_DATA_PATH, recursive: true);
            GC.SuppressFinalize(this);
        }

        #region - Helper Methods -

        private static ServiceProvider GetServiceProvider(string? testDataFile = null)
        {
            var serviceCollection = new ServiceCollection().AddLogging()
                                                           .AddSingleton(Mock.Of<ILoggerProvider>());

            if (string.IsNullOrEmpty(testDataFile))
                return serviceCollection.AddTableauMigrationSdk()
                                        .BuildServiceProvider();

            var assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Assert.NotNull(assemblyLocation);
            var configuration = new ConfigurationBuilder()
                                .SetBasePath(assemblyLocation)
                                .AddJsonFile(path: Path.Combine(assemblyLocation, TEST_DATA_DIR, testDataFile), optional: false, reloadOnChange: true)
                                .AddEnvironmentVariables()
                                .Build();

            return serviceCollection.AddTableauMigrationSdk(configuration.GetSection(nameof(MigrationSdkOptions)))
                                    .BuildServiceProvider();
        }

        private static async Task EditConfigFile(string path, string configKey, int value, int waitAfterSaveMilliseconds)
        {

            var jsonString = File.ReadAllText(path);
            Assert.NotNull(jsonString);

            var jObject = (JObject)JsonConvert.DeserializeObject(jsonString)!;
            var jToken = jObject.SelectToken(configKey);

            Assert.NotNull(jToken);
            // Update the value of the property: 
            jToken.Replace(value);

            // Convert the JObject back to a string:
            string updatedJsonString = jObject.ToString();
            await File.WriteAllTextAsync(path, updatedJsonString).ConfigureAwait(false);

            /// Artificially induced wait so the IOptionsMonitor.OnChange() event handler can pick up file changes
            await Task.Delay(waitAfterSaveMilliseconds).ConfigureAwait(false);
        }

        #endregion

        [Fact]
        public void LoadFromInitialConfiguration()
        {
            var serviceProvider = GetServiceProvider(TEST_DATA_FILE1);
            var configHelper = serviceProvider.GetRequiredService<IConfigReader>();
            var freshConfig = configHelper.Get();
            Assert.NotNull(freshConfig?.Network);
            Assert.Equal(2034, freshConfig.Network.FileChunkSizeKB);
        }

        /// <summary>
        /// This test checks if changes to a config file can be hot-reloaded.
        /// </summary>
        [Theory]
        [InlineData(400, 10)]
        public async Task FileConfigChangeReload(int readDelay, int maxRetries)
        {
            var retries = 0;

            do
            {
                try
                {
                    var serviceProvider = GetServiceProvider(TEST_DATA_FILE2);
                    var configHelper = serviceProvider.GetRequiredService<IConfigReader>();

                    var oldConfig = configHelper.Get();
                    Assert.NotNull(oldConfig?.Network);
                    Assert.Equal(2034, oldConfig.Network.FileChunkSizeKB);

                    await EditConfigFile(Path.Combine(Directory.GetCurrentDirectory(), TEST_DATA_DIR, TEST_DATA_FILE2),
                                   $"{nameof(MigrationSdkOptions)}.Network.{nameof(MigrationSdkOptions.Network.FileChunkSizeKB)}",
                                   55,
                                   readDelay);

                    var newConfig = configHelper.Get();
                    Assert.NotNull(newConfig?.Network);
                    Assert.Equal(55, newConfig.Network.FileChunkSizeKB);

                    break;
                }
                catch
                {
                    retries++;
                }
            }
            while (retries < maxRetries);

            if (retries < maxRetries)
            {
                _output.WriteLine($"[INFO FileConfigChangeReload Tests]: Retries count: {retries}.");
                return;
            }

            if (skipGithubRunnerTests)
            {
                _output.WriteLine($"[WARN FileConfigChangeReload Tests]: Max retries ({maxRetries}) reached.");
            }
            else
            {
                Assert.Fail($"[WARN FileConfigChangeReload Tests]: Max retries ({maxRetries}) reached.");
            }
        }

        [Fact]
        public void DefaultsApplied()
        {
            var serviceProvider = GetServiceProvider();
            var configHelper = serviceProvider.GetRequiredService<IConfigReader>();

            var freshConfig = configHelper.Get();
            Assert.NotNull(freshConfig?.Network);
            Assert.Equal(512, freshConfig.Network.FileChunkSizeKB);

            Assert.NotNull(freshConfig?.DefaultPermissionsContentTypes);
        }

        [Fact]
        public void Reads_DefaultPermissionsContentTypes_config()
        {
            var serviceProvider = GetServiceProvider(TEST_DATA_FILE3);
            var configReader = serviceProvider.GetRequiredService<IConfigReader>();
            var config = configReader.Get();
            Assert.NotNull(config?.DefaultPermissionsContentTypes);

            var defaultUrlSegments = DefaultPermissionsContentTypeUrlSegments.GetAll();
            var configSegments = config.DefaultPermissionsContentTypes.UrlSegments;

            Assert.True(configSegments.SequenceEqual(defaultUrlSegments.Concat(new[] { "test1", "test2" })));
        }
    }
}

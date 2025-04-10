//
//  Copyright (c) 2025, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the "License") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Engine.Pipelines;

namespace Tableau.Migration.ManifestAnalyzer
{
    internal class ManifestAnalyzer : IHostedService
    {
        private const string MANIFEST_TOP_LEVEL_KEY = "Manifest Top Level";
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly MigrationManifestSerializer _manifestSerializer;
        private readonly ManifestAnalyzerOptions _options;

        public ManifestAnalyzer(
            IHostApplicationLifetime appLifetime,
            MigrationManifestSerializer manifestSerializer,
            IOptions<ManifestAnalyzerOptions> options)
        {
            _appLifetime = appLifetime;
            _manifestSerializer = manifestSerializer;
            _options = options.Value;
        }

        public async Task StartAsync(CancellationToken cancel)
        {
            var manifestPath = _options.ManifestPath;
            var errorFilePath = _options.ErrorFilePath;

            MigrationManifest? manifest;
            try
            {
                manifest = await _manifestSerializer.LoadAsync(manifestPath, cancel).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine("Manifest could not be loaded.");
                Console.Error.WriteLine(e);
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
                _appLifetime.StopApplication();
                return;
            }

            if (manifest is null)
            {
                Console.WriteLine("Manifest could not be loaded.");
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
                _appLifetime.StopApplication();
                return;
            }


            var errors = new Dictionary<string, Dictionary<Exception, int>>();
            var totalErrors = new Dictionary<string, int>();

            AnalyzeManifestTopLevel(manifest, errors, totalErrors);

            AnalyzeContentTypeEntries(manifest, errors, totalErrors);

            var summary = GenerateSummary(manifest);

            await WriteErrorsToFileAsync(errors, totalErrors, errorFilePath, manifestPath, summary).ConfigureAwait(false);

            _appLifetime.StopApplication();
        }

        private static void AnalyzeContentTypeEntries(MigrationManifest manifest, Dictionary<string, Dictionary<Exception, int>> errors, Dictionary<string, int> totalErrors)
        {
            // Go through each content type and count the unique errors and total errors
            foreach (var pipelineContentType in MigrationPipelineContentType.GetMigrationPipelineContentTypes(manifest.PipelineProfile))
            {
                var errorCounts = new Dictionary<Exception, int>(new ExceptionComparer());
                var totalErrorCount = 0;

                foreach (var entry in manifest.Entries.ForContentType(pipelineContentType.ContentType))
                {
                    totalErrorCount = CalculateErrorCounts(entry.Errors, errorCounts, totalErrorCount);
                }
                var friendlyName = pipelineContentType.GetConfigKey();
                AddErrors(friendlyName, errors, totalErrors, errorCounts, totalErrorCount);
                WriteErrorCountSummary(friendlyName, errorCounts.Count, totalErrorCount);
            }
        }

        private static void AnalyzeManifestTopLevel(MigrationManifest manifest, Dictionary<string, Dictionary<Exception, int>> errors, Dictionary<string, int> totalErrors)
        {
            var manifestErrorCounts = new Dictionary<Exception, int>(new ExceptionComparer());
            var totalManifestErrorCount = CalculateErrorCounts(manifest.Errors, manifestErrorCounts);
            AddErrors(MANIFEST_TOP_LEVEL_KEY, errors, totalErrors, manifestErrorCounts, totalManifestErrorCount);
            WriteErrorCountSummary(MANIFEST_TOP_LEVEL_KEY, manifestErrorCounts.Count, totalManifestErrorCount);
        }

        private static int CalculateErrorCounts(IReadOnlyList<Exception> errorList, Dictionary<Exception, int> errorCounts, int totalErrorCount = 0)
        {
            foreach (var error in errorList)
            {
                totalErrorCount++;
                errorCounts[error] = errorCounts.TryGetValue(error, out int value) ? ++value : 1;
            }

            return totalErrorCount;
        }

        private static string GenerateSummary(MigrationManifest manifest)
        {
            var summaryBuilder = new StringBuilder();
            summaryBuilder.AppendLine("<table>");
            summaryBuilder.AppendLine("<tr><th>Content Type</th><th>Success Rate</th><th>Success <span class='info-icon'>i<span class='tooltip'>Success includes both migrated and skipped entries.</span></span></th><th>Errored</th><th>Pending</th><th>Canceled</th><th>Total</th></tr>");

            foreach (var contentType in MigrationPipelineContentType.GetMigrationPipelineContentTypes(manifest.PipelineProfile))
            {
                var entries = manifest.Entries.ForContentType(contentType.ContentType);
                var total = entries.Count();
                var migrated = entries.Count(e => e.Status == MigrationManifestEntryStatus.Migrated);
                var skipped = entries.Count(e => e.Status == MigrationManifestEntryStatus.Skipped);
                var errored = entries.Count(e => e.Status == MigrationManifestEntryStatus.Error);
                var pending = entries.Count(e => e.Status == MigrationManifestEntryStatus.Pending);
                var canceled = entries.Count(e => e.Status == MigrationManifestEntryStatus.Canceled);
                var success = migrated + skipped;
                var successRate = total > 0 ? (success / (double)total) * 100 : 0;

                summaryBuilder.AppendLine($"<tr><td>{contentType.GetConfigKey()}</td><td>{successRate:F2}%</td><td>{success}</td><td>{errored}</td><td>{pending}</td><td>{canceled}</td><td>{total}</td></tr>");
            }

            summaryBuilder.AppendLine("</table>");
            return summaryBuilder.ToString();
        }

        private static void AddErrors(string friendlyName, Dictionary<string, Dictionary<Exception, int>> errors, Dictionary<string, int> totalErrors, Dictionary<Exception, int> errorCounts, int totalErrorCount)
        {
            errors.Add(friendlyName, errorCounts);
            totalErrors.Add(friendlyName, totalErrorCount);
        }

        private static void WriteErrorCountSummary(string friendlyName, int errorCounts, int totalErrorCount)
            => Console.WriteLine($"{friendlyName} has {errorCounts} unique errors and {totalErrorCount} total errors.");

        private static async Task WriteErrorsToFileAsync(Dictionary<string, Dictionary<Exception, int>> errors, Dictionary<string, int> totalErrors, string filePath, string manifestPath, string summary)
        {
            var contentBuilder = new StringBuilder();

            foreach (var topLevel in errors.Keys.Where(k => k == MANIFEST_TOP_LEVEL_KEY))
            {
                AppendSection(contentBuilder, topLevel, errors, totalErrors);
            }

            foreach (var contentType in errors.Keys.Where(k => k != MANIFEST_TOP_LEVEL_KEY))
            {
                AppendSection(contentBuilder, contentType, errors, totalErrors);
            }

            var fileName = Path.GetFileName(manifestPath);
            var dateTime = GetDateTimeFromFileName(fileName);
            var finalHtml = HtmlTemplate.Template
                .Replace("{{datetime}}", dateTime)
                .Replace("{{summary}}", summary)
                .Replace("{{content}}", contentBuilder.ToString());

            await File.WriteAllTextAsync(filePath, finalHtml).ConfigureAwait(false);

            // Open the generated HTML file in the default application
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                }
            };
            process.Start();

            static void AppendSection(StringBuilder contentBuilder, string key, Dictionary<string, Dictionary<Exception, int>> errors, Dictionary<string, int> totalErrors)
            {
                var errorList = errors[key];

                contentBuilder.AppendLine($"<button class=\"collapsible\">{key} has {errorList.Count} unique errors ({totalErrors[key]} total errors).</button>");

                if (totalErrors[key] == 0)
                    return;

                contentBuilder.AppendLine("<div class=\"content\">");

                foreach (var error in errorList.OrderByDescending(e => e.Value))
                {
                    contentBuilder.AppendLine($"<p>Count: {error.Value}</p>");
                    contentBuilder.AppendLine($"<pre class=\"error-message\">{System.Net.WebUtility.HtmlEncode(error.Key.Message)}</pre>");
                }

                contentBuilder.AppendLine("</div>");
            }
        }

        private static string GetDateTimeFromFileName(string fileName)
        {
            var match = Regex.Match(fileName, @"^Manifest-(\d{4}-\d{2}-\d{2})-(\d{2}-\d{2}-\d{2})\.json$");
            if (match.Success)
            {
                var date = DateTime.ParseExact(match.Groups[1].Value, "yyyy-MM-dd", null);
                var time = match.Groups[2].Value.Replace("-", ":");
                return $"{date:dddd, MMMM dd, yyyy} {time}";
            }
            return string.Empty;
        }

        public Task StopAsync(CancellationToken cancel) => Task.CompletedTask;
    }
}

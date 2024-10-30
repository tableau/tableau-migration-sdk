//
//  Copyright (c) 2024, Salesforce, Inc.
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
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Engine.Pipelines;

namespace Tableau.Migration.TestApplication
{
    internal static class MigrationSummaryBuilder
    {
        private const char KEY_VALUE_PAIR_SEPARATOR = ',';
        private const char KEY_VALUE_SEPARATOR = ':';

        public static string Build(MigrationResult result, DateTime startTime, DateTime endTime, TimeSpan elapsed)
        {
            var summaryBuilder = new StringBuilder();
            return summaryBuilder
                .Append("Migration completed.")
                .AppendLine()
                .AppendLine()
                .AppendLine($"Result    {KEY_VALUE_SEPARATOR}{result.Status}")
                .AppendLine($"Start Time{KEY_VALUE_SEPARATOR}{startTime}")
                .AppendLine($"End Time  {KEY_VALUE_SEPARATOR}{endTime}")
                .AppendLine($"Duration  {KEY_VALUE_SEPARATOR}{elapsed}")
                .AppendLine()
                .AppendContentMigrationResult(result)
                .ToString();
        }

        private static StringBuilder AppendContentMigrationResult(this StringBuilder summaryBuilder, MigrationResult result)
        {
            summaryBuilder.AppendLine();

            var contentTypeList = GetContentTypes();

            foreach (var contentType in contentTypeList)
            {
                var typeResult = result.Manifest.Entries.ForContentType(contentType);

                if (typeResult is null)
                {
                    continue;
                }

                summaryBuilder.AppendResultRow(typeResult);
            }
            return summaryBuilder;
        }

        private static StringBuilder AppendResultRow(
            this StringBuilder summaryBuilder,
            IMigrationManifestContentTypePartition typeResult)
        {
            var total = typeResult.ExpectedTotalCount;

            if (total == 0)
            {
                return summaryBuilder;
            }

            var statusTotals = typeResult.GetStatusTotals().Where(i => i.Value > 0).ToImmutableDictionary();

            var successStatusTotals = statusTotals.Where(k => k.Key.IsSuccess()).ToImmutableDictionary();
            var nonSuccessStatusTotals = statusTotals.Except(successStatusTotals).ToImmutableDictionary();

            var successTotal = successStatusTotals.Sum(s => s.Value);

            var contentTypeName = MigrationPipelineContentType.GetConfigKeyForType(typeResult.ContentType);

            summaryBuilder
                .AppendLine()
                .Append(
                $"{contentTypeName}:: {GetPercentage(successTotal, total)}% successful")
                .Append(
                $" ({GetMetricString("Total", total)}{KEY_VALUE_PAIR_SEPARATOR}{successStatusTotals.ToMetricString()})")
                .AppendLine();

            if (successTotal != total)
            {
                summaryBuilder.AppendLine($"\tDetails {nonSuccessStatusTotals.ToMetricString()}");
            }

            return summaryBuilder;

            static double GetPercentage(int count, double total)
                => total != 0 ? Math.Round(count * 100 / total, 2) : 0;
        }

        private static string ToMetricString(this ImmutableDictionary<MigrationManifestEntryStatus, int> statusTotals)
        {
            if (statusTotals.IsEmpty)
            {
                return string.Empty;
            }

            var resultBuilder = new StringBuilder();
            var firstItem = statusTotals.First();

            resultBuilder.Append(firstItem.ToMetricString());

            foreach (var item in statusTotals.Skip(1))
            {
                resultBuilder.Append($"{KEY_VALUE_PAIR_SEPARATOR}{item.ToMetricString()}");
            }

            return resultBuilder.ToString();
        }

        private static bool IsSuccess(this MigrationManifestEntryStatus status)
            => status == MigrationManifestEntryStatus.Migrated || status == MigrationManifestEntryStatus.Skipped;

        private static string ToMetricString(this KeyValuePair<MigrationManifestEntryStatus, int> statusTotal)
            => GetMetricString(statusTotal.Key.ToString(), statusTotal.Value);

        private static string GetMetricString(string metricName, int metricValue)
            => $"{metricName}{KEY_VALUE_SEPARATOR}{metricValue}";

        private static List<Type> GetContentTypes()
            => ServerToCloudMigrationPipeline.ContentTypes.Select(c => c.ContentType).ToList();
    }
}

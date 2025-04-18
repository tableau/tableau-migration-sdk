﻿//
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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Tableau.Migration.ManifestAnalyzer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                PrintHelp();
                return;
            }

            var manifestPath = args[0];
            var remainingArgs = args.Skip(1).ToArray();

            var host = Host.CreateDefaultBuilder(remainingArgs)
                .ConfigureAppConfiguration(config =>
                {
                    config.AddCommandLine(remainingArgs);
                })
                .ConfigureServices((context, services) =>
                {
                    var configuration = context.Configuration;

                    services.Configure<ManifestAnalyzerOptions>(options =>
                    {
                        options.ManifestPath = manifestPath;

                        var errorFilePath = configuration["ErrorFilePath"];
                        if (string.IsNullOrEmpty(errorFilePath))
                        {
                            var manifestDirectory = Path.GetDirectoryName(manifestPath) ?? throw new Exception("Can't get directory from manifest path.");
                            var manifestFileName = Path.GetFileNameWithoutExtension(manifestPath);
                            errorFilePath = Path.Combine(manifestDirectory, $"{manifestFileName}_Analysis.html");
                        }
                        options.ErrorFilePath = errorFilePath;
                    });

                    services.AddTableauMigrationSdk(configuration.GetSection("tableau:migrationSdk"));
                    services.AddHostedService<ManifestAnalyzer>();
                })
                .Build();

            await host.RunAsync().ConfigureAwait(false);
        }

        private static void PrintHelp()
        {
            Console.WriteLine("Usage: ManifestAnalyzer <ManifestPath> [--ErrorFilePath <path>]");
        }
    }
}

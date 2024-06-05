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

using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tableau.Migration.PythonGenerator.Config;
using Tableau.Migration.PythonGenerator.Generators;
using Tableau.Migration.PythonGenerator.Writers;

namespace Tableau.Migration.PythonGenerator
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            using var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((ctx, services) =>
                {
                    services
                        .Configure<PythonGeneratorOptions>(ctx.Configuration)
                        .AddSingleton<IPythonGenerator, Generators.PythonGenerator>()
                        .AddSingleton<IPythonTypeGenerator, PythonTypeGenerator>()
                        .AddSingleton<IPythonDocstringGenerator, PythonDocstringGenerator>()
                        .AddSingleton<IPythonPropertyGenerator, PythonPropertyGenerator>()
                        .AddSingleton<IPythonMethodGenerator, PythonMethodGenerator>()
                        .AddSingleton<IPythonEnumValueGenerator, PythonEnumValueGenerator>()
                        .AddSingleton<IPythonWriter, PythonWriter>()
                        .AddSingleton<IPythonTypeWriter, PythonTypeWriter>()
                        .AddSingleton<IPythonDocstringWriter, PythonDocstringWriter>()
                        .AddSingleton<IPythonPropertyWriter, PythonPropertyWriter>()
                        .AddSingleton<IPythonMethodWriter, PythonMethodWriter>()
                        .AddSingleton<IPythonEnumValueWriter, PythonEnumValueWriter>()
                        .AddSingleton<IPythonTestWriter, PythonTestWriter>()
                        .AddSingleton<IPythonClassTestWriter, PythonClassTestWriter>()
                        .AddSingleton<IPythonPropertyTestWriter, PythonPropertyTestWriter>()
                        .AddSingleton<IPythonConstructorTestWriter, PythonConstructorTestWriter>()
                        .AddHostedService<PythonGeneratorService>();
                })
                .Build();

            await host.RunAsync();
        }
    }
}

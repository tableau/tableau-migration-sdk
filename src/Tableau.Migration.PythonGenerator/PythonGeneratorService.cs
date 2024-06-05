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
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Tableau.Migration.PythonGenerator.Config;
using Tableau.Migration.PythonGenerator.Generators;
using Tableau.Migration.PythonGenerator.Writers;

namespace Tableau.Migration.PythonGenerator
{
    internal sealed class PythonGeneratorService : IHostedService
    {
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly PythonGeneratorOptions _options;
        private readonly IPythonGenerator _pyGenerator;
        private readonly IPythonWriter _pyWriter;

        public PythonGeneratorService(IHostApplicationLifetime appLifeTime,
            IOptions<PythonGeneratorOptions> options,
            IPythonGenerator pyGenerator,
            IPythonWriter pyWriter)
        {
            _appLifetime = appLifeTime;
            _options = options.Value;
            _pyGenerator = pyGenerator;
            _pyWriter = pyWriter;
        }

        public async Task StartAsync(CancellationToken cancel)
        {
            Console.WriteLine("Generating Python Wrappers...");
            Console.WriteLine("Import Path:" + _options.ImportPath);
            Console.WriteLine("Output Path:" + _options.OutputPath);

            var compilation = CSharpCompilation.Create(typeof(ContentLocation).Assembly.GetName().Name)
                .AddReferences(MetadataReference.CreateFromFile(Path.Combine(_options.ImportPath, "Tableau.Migration.dll"),
                    documentation: XmlDocumentationProvider.CreateFromFile(Path.Combine(_options.ImportPath, "Tableau.Migration.xml"))));

            var module = compilation.SourceModule.ReferencedAssemblySymbols.Single().Modules.Single();
            var rootNamespace = module.GlobalNamespace.GetNamespaceMembers().Single().GetNamespaceMembers().Single();

            var dotNetTypes = PythonGenerationList.FindTypesToGenerate(rootNamespace);

            var pyTypes = _pyGenerator.Generate(dotNetTypes);

            await _pyWriter.WriteAsync(pyTypes, cancel);

            _appLifetime.StopApplication();
        }

        public Task StopAsync(CancellationToken cancel) => Task.CompletedTask;
    }
}

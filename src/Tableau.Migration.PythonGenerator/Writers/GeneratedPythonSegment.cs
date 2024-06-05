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
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.PythonGenerator.Writers
{
    internal sealed class GeneratedPythonSegment : IAsyncDisposable
    {
        private const string BEGIN_REGION = "# region _generated";
        private const string END_REGION = "# endregion";

        private readonly string _path;
        private readonly string? _tailContent;

        public IndentingStringBuilder StringBuilder { get; }

        private GeneratedPythonSegment(string path, IndentingStringBuilder stringBuilder, string? tailContent) 
        {
            _path = path;
            StringBuilder = stringBuilder;
            _tailContent = tailContent;
        }

        private static async ValueTask<(IndentingStringBuilder StringBuilder, string? TailContent)> InitializeAsync(Stream stream, CancellationToken cancel)
        {
            using var reader = new StreamReader(stream, leaveOpen: true);
            var text = await reader.ReadToEndAsync(cancel);

            var newLine = text.Contains("\r") ? "\r\n" : "\n";

            IndentingStringBuilder sb = new(new(), newLine);
            string? tailContent = null;

            var regionStartIndex = text.IndexOf(BEGIN_REGION);
            if(regionStartIndex == -1)
            {
                sb.NonIndentingBuilder.Append(text);
            }
            else
            {
                if(regionStartIndex > 0)
                {
                    sb.NonIndentingBuilder.Append(text.Substring(0, regionStartIndex));
                }

                var regionEndIndex = text.IndexOf(END_REGION, regionStartIndex);
                if(regionEndIndex != -1)
                {
                    var tailIndex = regionEndIndex + END_REGION.Length;
                    if(tailIndex < text.Length - 1)
                    {
                        tailContent = text.Substring(tailIndex).TrimStart();
                    }
                }
            }

            sb.AppendLine(BEGIN_REGION);
            sb.AppendLine();

            return (sb, tailContent);
        }

        public static async ValueTask<GeneratedPythonSegment> OpenAsync(string path, CancellationToken cancel)
        {
            await using var stream = File.Open(path, FileMode.OpenOrCreate, FileAccess.Read);

            var init = await InitializeAsync(stream, cancel);

            return new GeneratedPythonSegment(path, init.StringBuilder, init.TailContent);
        }

        public async ValueTask DisposeAsync()
        {
            StringBuilder.AppendLine();
            StringBuilder.AppendLine(END_REGION);

            if (_tailContent is not null)
            {
                StringBuilder.AppendLine();
                StringBuilder.NonIndentingBuilder.Append(_tailContent);
            }

            await File.WriteAllTextAsync(_path, StringBuilder.ToString());
        }
    }
}

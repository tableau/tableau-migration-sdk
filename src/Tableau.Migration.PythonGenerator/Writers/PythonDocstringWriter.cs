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

namespace Tableau.Migration.PythonGenerator.Writers
{
    internal sealed class PythonDocstringWriter : IPythonDocstringWriter
    {
        private const string DOCSTRING_QUOTE = "\"\"\"";

        public void Write(IndentingStringBuilder builder, PythonDocstring? documentation)
        {
            if (documentation is null)
            {
                return;
            }

            if (!documentation.HasExtraInfo)
            {
                builder.AppendLine($"{DOCSTRING_QUOTE}{documentation.Summary}{DOCSTRING_QUOTE}");
                return;
            }

            builder.AppendLine($"{DOCSTRING_QUOTE}{documentation.Summary}");

            if (documentation.HasArgs)
            {
                builder.AppendLine();
                using var argsBuilder = builder.AppendLineAndIndent("Args:");
                foreach (var arg in documentation.Args)
                {
                    argsBuilder.AppendLine($"{arg.Name}: {arg.Documentation}");
                }
            }

            if (documentation.HasReturns)
            {
                builder.AppendLine();
                builder.AppendLine($"Returns: {documentation.Returns}");
            }

            builder.AppendLine(DOCSTRING_QUOTE);
        }
    }
}

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
using System.Text;

namespace Tableau.Migration.PythonGenerator
{
    internal sealed class IndentingStringBuilder : IDisposable
    {
        private readonly string _newLine;
        private readonly uint _indentLevel;

        private const string INDENT = "    ";

        public StringBuilder NonIndentingBuilder { get; }

        public IndentingStringBuilder(StringBuilder builder, uint indentLevel = 0)
            : this(builder, Environment.NewLine, indentLevel)
        { }

        public IndentingStringBuilder(StringBuilder builder, string newLine, uint indentLevel = 0)
        {
            NonIndentingBuilder = builder;
            _newLine = newLine;
            _indentLevel = indentLevel;
        }

        private void Indent()
        {
            for(uint i = 0; i < _indentLevel; i++)
            {
                NonIndentingBuilder.Append(INDENT);
            }
        }

        public IndentingStringBuilder AppendLine(string? text = null)
        {
            Indent();
            NonIndentingBuilder.Append(text);
            NonIndentingBuilder.Append(_newLine);
            return this;
        }

        public IndentingStringBuilder AppendLineAndIndent(string text)
        {
            AppendLine(text);
            return new IndentingStringBuilder(NonIndentingBuilder, _indentLevel + 1);
        }

        public override string ToString() => NonIndentingBuilder.ToString();

        public void Dispose()
        { }
    }
}

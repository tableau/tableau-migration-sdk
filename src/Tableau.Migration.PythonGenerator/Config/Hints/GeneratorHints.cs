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
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Tableau.Migration.PythonGenerator.Config.Hints
{
    internal sealed class GeneratorHints
    {
        public NamespaceHints[] Namespaces { get; set; } = Array.Empty<NamespaceHints>();

        public TypeHints? ForType(ITypeSymbol type)
        {
            var ns = type.ContainingNamespace.ToDisplayString();
            var nsHint = Namespaces.FirstOrDefault(nh => string.Equals(nh.Namespace, ns, StringComparison.Ordinal));
            if(nsHint is null)
            {
                return null;
            }

            return nsHint.Types.FirstOrDefault(th => string.Equals(th.Type, type.Name, StringComparison.Ordinal));
        }
    }
}

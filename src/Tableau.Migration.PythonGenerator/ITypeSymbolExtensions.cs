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
using Microsoft.CodeAnalysis;

namespace Tableau.Migration.PythonGenerator
{
    internal static class ITypeSymbolExtensions
    {
        public static string ToPythonModuleName(this ITypeSymbol ts)
        {
            var containingNs = ts.ContainingNamespace;
            if (containingNs == null)
            {
                return ts.Name;
            }

            return GetPythonModuleName(containingNs.ToDisplayString());
        }

        public static string ToPythonModuleName(Type ts)
        {
            var containingNs = ts.Namespace;
            if (containingNs == null)
            {
                return ts.Name;
            }

            return GetPythonModuleName(containingNs);
        }

        private static string GetPythonModuleName(string dotNetNamespace)
        {
            return "tableau_migration." + dotNetNamespace
                .Replace("Tableau.", "")
                .Replace(".", "_")
                .ToLower();
        }
    }
}

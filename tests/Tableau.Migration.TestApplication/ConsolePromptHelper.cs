//
//  Copyright (c) 2026, Salesforce, Inc.
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

namespace Tableau.Migration.TestApplication
{
    internal static class ConsolePromptHelper
    {
        public static ConsoleKey ReadYesNo(string prompt, Action? interactivePromptRenderer = null)
        {
            if (!IsInteractive())
            {
                Console.WriteLine($"{prompt}N (auto-selected in non-interactive mode)");
                return ConsoleKey.N;
            }

            ConsoleKey key;
            do
            {
                if (interactivePromptRenderer is null)
                {
                    Console.Write(prompt);
                }
                else
                {
                    interactivePromptRenderer();
                }

                key = Console.ReadKey().Key;
                Console.WriteLine();
            } while (key is not ConsoleKey.Enter && key is not ConsoleKey.Y && key is not ConsoleKey.N);

            return key;
        }

        public static void WaitForExitIfInteractive()
        {
            if (!IsInteractive())
                return;

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        public static bool IsInteractive() => !Console.IsInputRedirected;


        public static bool IsYes(this ConsoleKey key) => key is ConsoleKey.Y;
    }
}

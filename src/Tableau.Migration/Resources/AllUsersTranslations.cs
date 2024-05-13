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

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Tableau.Migration.Resources
{
    internal static class AllUsersTranslations
    {
        internal const string English = "All Users";

        // This list comes from the "local_names.group.all_users.name" values in "shared_wg_[culture].properties" files at
        // https://sourcegraph.prod.tableautools.com/teams/near/-/tree/workgroup/src/shared/libraries/tab-strings/res/localization
        private static readonly IImmutableList<string> _translations = ImmutableArray.Create(
            English, // en, en_GB (This is first in the list since it's most likely to be matched.)
            "Alle Benutzer", // de
            "Todos los usuarios", // es
            "Tous les utilisateurs", // fr, fr_CA
            "Tutti gli utenti", // it
            "すべてのユーザー", // ja
            "모든 사용자", // ko
            "Todos os usuários", // pt
            "Alla användare", // sv
            "ผู้ใช้ทั้งหมด", // th
            "所有用户", // zh
            "所有使用者" // zh_TW
        );

        /// <summary>
        /// Gets the list of all "All Users" group translations.
        /// </summary>
        /// <param name="extra">Extra translations supplied by the user.</param>
        /// <returns>A list of "All Users" group translations.</returns>
        public static IImmutableList<string> GetAll(IEnumerable<string>? extra = null)
        {
            if (extra is not null)
                return _translations.Concat(extra).Distinct().ToImmutableArray();

            return _translations;
        }
    }
}

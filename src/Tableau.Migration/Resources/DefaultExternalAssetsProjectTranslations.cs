using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Tableau.Migration.Resources
{
    internal class DefaultExternalAssetsProjectTranslations
    {
        internal const string English = "External Assets Default Project";

        // This list comes from the "project.external_assets_default_project.name" values in "admin_specific_[culture].properties" files at
        // https://sourcegraph.prod.tableautools.com/teams/near/-/tree/workgroup/src/shared/libraries/tab-strings/res/localization
        private static readonly IImmutableList<string> _translations = ImmutableArray.Create(
            English, // en, en_GB (This is first in the list since it's most likely to be matched.)
            "Projet par défaut de ressources externes", // fr
            "Projet par défaut pour les ressources externes", // fr_CA
            "Standardprojekt für externe Assets", // de
            "Proyecto predeterminado de recursos externos.", // es
            "Progetto predefinito per risorse esterne", // it
            "外部アセットの既定のプロジェクト", // ja
            "외부 자산 기본 프로젝트", // ko
            "Projeto padrão de ativos externos", // pt
            "Standardprojekt för externa resurser", // sv
            "โปรเจกต์เริ่มต้นของแอสเซทภายนอก", // th
            "外部资产默认项目", // zh
            "外部資產預設專案" // zh_TW
        );

        /// <summary>
        /// Gets the list of all "External Assets Default Project" project translations.
        /// </summary>
        /// <param name="extra">Extra translations supplied by the user.</param>
        /// <returns>A list of "External Assets Default Project" project translations.</returns>
        public static IImmutableList<string> GetAll(IEnumerable<string>? extra = null)
        {
            if (extra is not null)
                return _translations.Concat(extra).Distinct().ToImmutableArray();

            return _translations;
        }
    }
}

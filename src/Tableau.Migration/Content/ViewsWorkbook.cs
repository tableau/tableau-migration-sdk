using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Tableau.Migration.Api.Rest.Models;

namespace Tableau.Migration.Content
{
    internal abstract class ViewsWorkbook : Workbook, IChildPermissionsContent
    {
        /// <inheritdoc />
        public IImmutableList<IView> Views { get; }

        public ViewsWorkbook(IWorkbookType response, IContentReference project, IContentReference owner, IImmutableList<IView> views)
            : base(response, project, owner)
        {
            Views = views;
        }

        #region - IChildPermissionsContent Implementation -

        Type IChildPermissionsContent.ChildType { get; } = typeof(IView);

        IEnumerable<IContentReference> IChildPermissionsContent.ChildPermissionContentItems => Views;

        bool IChildPermissionsContent.ShouldMigrateChildPermissions => !ShowTabs;

        #endregion
    }
}

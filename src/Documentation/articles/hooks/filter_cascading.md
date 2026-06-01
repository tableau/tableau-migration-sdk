# Filter Cascading
When applying [filters](~/samples/filters/index.md), not migrating a content item can potentially effect other items that reference it.
For example, if a user is not migrated, a workbook owned by that user would normally not be able to migrate without changing the owner or taking other steps to handle the reference.
In many cases when a content item is skipped, the intention is to also skip migrating the content items that reference it.
For example, if a project is not migrated, you might want to also skip the workbooks and data sources in that project.
_Cascading_ a filter allows it to apply to all content items that reference the item being filtered.

## Cascading Filters
To have a filter cascade to content items that reference the current item, assign a cascade skip status in a filter.
Content items that reference this item will then have the same cascade skip status applied by a default filter when those content types are migrated.

### [Python](#tab/python)

```Python
item.status = FilterStatus.CASCADE_SKIP
```

### [C#](#tab/csharp)

```C#
item.Status = FilterStatus.CascadeSkip;
```

---

See [Sample: Filter projects by name](~/samples/filters/filter_projects_by_name.md) for a full example of a cascading filter.

## Non-Cascading Filters
To have a filter only apply to the current item, assign a non-cascade skip status in a filter.
Content items that reference this item will need to be handled manually through additional filters, mappings, or transformers.

### [Python](#tab/python)

```Python
item.status = FilterStatus.SKIP
```

### [C#](#tab/csharp)

```C#
item.Status = FilterStatus.Skip;
```

---

See [Sample: Filter users by site role](~/samples/filters/filter_users_by_site_role.md) for a full example of a non-cascading filter.

## Filter Ordering and Overriding
Filters run in the order they are [registered](~/samples/index.md#hook-registration).
Each filter is run on all items of the content type available to filter, including those marked for exclusion by previous filters.
This means that setting the filter status of an item overrides any decisions made by previous filters, including cascading filters from previous content types.

## Updating Boolean Filters
In previous versions of Migration SDK filters never cascaded, and used a simple boolean return value to determine whether the filtering content item was migrated or skipped.
Filters using these boolean return values continue to function as before.
To update these filters to allow them to cascade:

### [Python](#tab/python)

  - Override the [`filter`](~/api-python/reference/tableau_migration.migration_engine_hooks_filters_interop.PyContentFilterBase.md#tableau_migration.migration_engine_hooks_filters_interop.PyContentFilterBase.filter) method instead of the `should_migrate` method.
  - Convert the logic of the filter that returns `true` to either return without modifying the input context, or set the context's [`status`](~/api-python/reference/tableau_migration.migration_engine_hooks_filters.PyContentFilterContextItem.md#tableau_migration.migration_engine_hooks_filters.PyContentFilterContextItem.status) property to [`FilterStatus.MIGRATE`](~/api-python/reference/tableau_migration.migration_engine_hooks_filters.PyFilterStatus.md#tableau_migration.migration_engine_hooks_filters.PyFilterStatus.MIGRATE) to overwrite previous filters.
  - Convert the logic of the filter that returns `false` to set the context's [`status`](~/api-python/reference/tableau_migration.migration_engine_hooks_filters.PyContentFilterContextItem.md#tableau_migration.migration_engine_hooks_filters.PyContentFilterContextItem.status) property either to [`FilterStatus.SKIP`](~/api-python/reference/tableau_migration.migration_engine_hooks_filters.PyFilterStatus.md#tableau_migration.migration_engine_hooks_filters.PyFilterStatus.SKIP) or [`FilterStatus.CASCADE_SKIP`](~/api-python/reference/tableau_migration.migration_engine_hooks_filters.PyFilterStatus.md#tableau_migration.migration_engine_hooks_filters.PyFilterStatus.CASCADE_SKIP).

### [C#](#tab/csharp)

  - Override the [`Filter`](xref:Tableau.Migration.Engine.Hooks.Filters.ContentFilterBase`1#Tableau_Migration_Engine_Hooks_Filters_ContentFilterBase_1_Filter_Tableau_Migration_Engine_Hooks_Filters_ContentFilterContextItem__0__) method instead of the `ShouldMigrate` method.
  - Convert the logic of the filter that returns `true` to either return without modifying the input context, or set the context's [`status`](xref:Tableau.Migration.Engine.Hooks.Filters.ContentFilterContextItem`1#Tableau_Migration_Engine_Hooks_Filters_ContentFilterContextItem_1_Status) property to [`FilterStatus.Migrate`](xref:Tableau.Migration.Engine.Hooks.Filters.FilterStatus) to overwrite previous filters.
  - Convert the logic of the filter that returns `false` to set the context's [`status`](xref:Tableau.Migration.Engine.Hooks.Filters.ContentFilterContextItem`1#Tableau_Migration_Engine_Hooks_Filters_ContentFilterContextItem_1_Status) property either to [`FilterStatus.Skip`](xref:Tableau.Migration.Engine.Hooks.Filters.FilterStatus) or [`FilterStatus.CascadeSkip`](xref:Tableau.Migration.Engine.Hooks.Filters.FilterStatus).

---
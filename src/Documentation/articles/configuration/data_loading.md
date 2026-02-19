# Data Loading

Migration SDK loads data during migration for purposes such as:

- Finding content items to migrate.
- Mapping references between content items (e.g. ownership, permissions, etc.) to valid destination items.
- Converting Tableau Server schedules to Tableau Cloud schedules.

To efficiently handle migrations of large sites, the default data loading behavior loads all items, using the [batch size](~/articles/configuration/index.md#advanced-configuration) for paging, and caches necessary information for future use.
In most cases this "bulk loading" reduces total API calls compared to loading items individually, which minimizes API throttling and overall migration time.

For migration of large sites where only a few items are necessary, however, the time spent loading all items may exceed the time savings of reduced API calls.
In this situation data loading can be configured through the [plan builder](~/articles/configuration/index.md#basic-configuration).

- If a content type with many items has already migrated but few are actually referenced, for example when user migration has already been completed, [skipping the content type](~/articles/configuration/skip_content_type.md) with pre-caching disabled is recommended.
- In advanced scenarios where custom logic is needed, [migration services](~/articles/dependency_injection/migration_services.md) can be registered to control data loading behavior.

## Migration Content Loader

A migration content loader is responsible for finding the content items from the source site that should be considered for migration.
Loaders produce batches of content items that are then [mapped and filtered](~/articles/hooks/index.md) before migration.
Items returned by the loader are added to the manifest and used when finding content references on the source site.

Custom migration content loaders can be used by overriding the `IMigrationContentLoader` migration service, either for all content types or a specific content type.
See the [Custom Migration Services](~/articles/dependency_injection/migration_services.md) topic for details on overriding migration services.

### [Python](#tab/python)

[!code-python[](../../../../examples/Python.ExampleApplication/services/migration_content_loader/empty_migration_content_loader.py)]

### [C#](#tab/csharp)

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/Services/MigrationContentLoader/EmptyMigrationContentLoader.cs#class)]

---

## Content Reference Loading

When updating content references (e.g. a projects's owner ID), data from both source and destination sites are loaded as necessary.
Content reference data found through the normal course of the migration are cached automatically for use in future content types, but when content items are referenced that were filtered out or otherwise excluded from migration, additional data load attempts are made by the cache before producing migration errors.

Custom content reference loading behavior can be used by overridding the `IContentReferenceCacheLoadStrategyProvider` migration service, either for all content types or a specific content type.
Built-in providers are are available for the default bulk loading and lazy loading are available.
See the [Custom Migration Services](~/articles/dependency_injection/migration_services.md) topic for details on overriding migration services.

### [Python](#tab/python)

[//]: <> (Adding this as code so we don't change example application default behavior.)
```Python
plan_builder.services.set(ContentReferenceCacheLoadStrategyProviderBase[IUser], LazyContentReferenceCacheLoadStrategyProvider[IUser])
```

### [C#](#tab/csharp)

[//]: <> (Adding this as code so we don't change example application default behavior.)
```C#
planBuilder.Services.Set<IContentReferenceCacheLoadStrategyProvider<IUser>>(MigrationServiceFactoryContext ctx => 
{
	return ctx.Services.GetRequiredService<LazyContentReferenceCacheLoadStrategyProvider<IUser>>();
});
```

---
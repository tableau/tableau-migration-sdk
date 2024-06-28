# Custom Hooks

## Important Classes

Here are some important things to know when writing custom hooks:

- **Interfaces:** These interfaces expose supported methods. You can implement these directly or inherit available base classes. This applies to C# only.
- **Base Classes:** These classes can be inherited, allowing you to write your implementation in the overridden methods. You do not need to implement the interface explicitly if you use these.
- **Code Samples:** We have provided some simple code samples. You can use these as a starting point for your hooks.

## [Python](#tab/Python)

The base classes can be used as they are linked in the API reference. However, for ease of use, all base classes have been imported into the `tableau_migration` namespace without the `Py` prefix.
For example: [`PyContentFilterBase`](~/api-python/reference/tableau_migration.migration_engine_hooks_filters_interop.PyContentFilterBase.md) has been imported as `tableau_migration.PyContentFilterBase`.

#### Pre-Migration

| Type                                                                                                    | Base Class                                                                                                                                             | Code Samples                                                 |
|---------------------------------------------------------------------------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------|--------------------------------------------------------------|
| [Filters](~/api-python/reference/tableau_migration.migration_engine_hooks_filters_interop.md)           | [`ContentFilterBase<TContent>`](~/api-python/reference/tableau_migration.migration_engine_hooks_filters_interop.PyContentFilterBase.md)                | [Code Samples/Filters](~/samples/filters/index.md)           |
| [Mappings](~/api-python/reference/tableau_migration.migration_engine_hooks_mappings_interop.md)         | [`ContentMappingBase<TContent>`](~/api-python/reference/tableau_migration.migration_engine_hooks_mappings_interop.PyContentMappingBase.md)             | [Code Samples/Mappings](~/samples/mappings/index.md)         |
| [Transformers](~/api-python/reference/tableau_migration.migration_engine_hooks_transformers_interop.md) | [`ContentTransformerBase<TPublish>`](~/api-python/reference/tableau_migration.migration_engine_hooks_transformers_interop.PyContentTransformerBase.md) | [Code Samples/Transformers](~/samples/transformers/index.md) |

#### Post-Migration

| Type                                                                                                        | Base Class                                                                                                                                                                | Code Samples                                                                   |
|-------------------------------------------------------------------------------------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------|--------------------------------------------------------------------------------|
| [Post-Publish](~/api-python/reference/tableau_migration.migration_engine_hooks_postpublish_interop.md)      | [`ContentItemPostPublishHookBase<TPublish, TResult>`](~/api-python/reference/tableau_migration.migration_engine_hooks_postpublish_interop.PyBulkPostPublishHookBase.md)   | [Code Samples/Post-Publish Hooks](~/samples/post-publish/index.md)             |
| [Bulk Post-Publish](~/api-python/reference/tableau_migration.migration_engine_hooks_postpublish_interop.md) | [`BulkPostPublishHookBase<TSource>`](~/api-python/reference/tableau_migration.migration_engine_hooks_postpublish_interop.PyBulkPostPublishHookBase.md)                    | [Code Samples/Bulk Post-Publish](~/samples/bulk-post-publish/index.md)         |
| [Batch Migration Completed](~/api-python/reference/tableau_migration.migration_engine_hooks_interop.md)     | [`ContentBatchMigrationCompletedHookBase<TContent>`](~/api-python/reference/tableau_migration.migration_engine_hooks_interop.PyContentBatchMigrationCompletedHookBase.md) | [Code Samples/Batch Completed](~/samples/batch-migration-completed/index.md)   |
| [Migration Action Completed](~/api-python/reference/tableau_migration.migration_engine_hooks_interop.md)    | [`MigrationActionCompletedHookBase`](~/api-python/reference/tableau_migration.migration_engine_hooks_interop.PyMigrationActionCompletedHookBase.md)                       | [Code Samples/Action Completed](~/samples/migration-action-completed/index.md) |

#### Registration

To register Python hooks, register the object with the appropriate hook type list in the plan builder.

## [C#](#tab/CSharp)

#### Pre-Migration

| Type											                                                          | Base Class		                                                                                                                                   | Interface 		                                                                                           | Code Samples		                                          |
|---------------------------------------------------------------------------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------|-----------------------------------------------------------------------------------------------------------|--------------------------------------------------------------|
| [Filters](xref:Tableau.Migration.Engine.Hooks.Filters)                                                  | [`ContentFilterBase<TContent>`](xref:Tableau.Migration.Engine.Hooks.Filters.ContentFilterBase`1)                                                   | [`IContentFilter<TContent>`](xref:Tableau.Migration.Engine.Hooks.Filters.IContentFilter`1)                | [Code Samples/Filters](~/samples/filters/index.md)           |
| [Mappings](xref:Tableau.Migration.Engine.Hooks.Mappings)                                                | [`ContentMappingBase<TContent>`](xref:Tableau.Migration.Engine.Hooks.Mappings.ContentMappingBase`1)                                                | [`IContentMapping<TContent>`](xref:Tableau.Migration.Engine.Hooks.Mappings.IContentMapping`1)             | [Code Samples/Mappings](~/samples/mappings/index.md)         |
| [Transformers](xref:Tableau.Migration.Engine.Hooks.Transformers)                                        | [`ContentTransformerBase<TPublish>`](xref:Tableau.Migration.Engine.Hooks.Transformers.ContentTransformerBase`1)                                    | [`IContentTransformer<TPublish>`](xref:Tableau.Migration.Engine.Hooks.Transformers.IContentTransformer`1) | [Code Samples/Transformers](~/samples/transformers/index.md) |

#### Post-Migration

| Type 		                                                                                               | Base Class 		                                                                                                                     | Interface 		                                                                                                            | Code Samples		                                                             |
|----------------------------------------------------------------------------------------------------------|-----------------------------------------------------------------------------------------------------------------------------------------|------------------------------------------------------------------------------------------------------------------------------|--------------------------------------------------------------------------------|
| [Post-Publish](xref:Tableau.Migration.Engine.Hooks.PostPublish)                                          | [`ContentItemPostPublishHookBase<TPublish, TResult>`](xref:Tableau.Migration.Engine.Hooks.PostPublish.ContentItemPostPublishHookBase`1) | [`IContentItemPostPublishHook<TContent>`](xref:Tableau.Migration.Engine.Hooks.PostPublish.IContentItemPostPublishHook`2)     | [Code Samples/Post-Publish Hooks](~/samples/post-publish/index.md)             |
| [Bulk Post-Publish](xref:Tableau.Migration.Engine.Hooks.PostPublish)                                     | [`BulkPostPublishHookBase<TSource>`](xref:Tableau.Migration.Engine.Hooks.PostPublish.BulkPostPublishHookBase`1)                         | [`IBulkPostPublishHook<TSource>`](xref:Tableau.Migration.Engine.Hooks.PostPublish.IBulkPostPublishHook`1)                    | [Code Samples/Bulk Post-Publish](~/samples/bulk-post-publish/index.md)         |
| [Batch Migration Completed](xref:Tableau.Migration.Engine.Hooks)                                         | None 		                                                                                                                             | [`IContentBatchMigrationCompletedHook<TContent>`](xref:Tableau.Migration.Engine.Hooks.IContentBatchMigrationCompletedHook`1) | [Code Samples/Batch Completed](~/samples/batch-migration-completed/index.md)   |
| [Migration Action Completed](xref:Tableau.Migration.Engine.Hooks)                                        | None 		                                                                                                                             | [`IMigrationActionCompletedHook`](xref:Tableau.Migration.Engine.Hooks.IMigrationActionCompletedHook)                         | [Code Samples/Action Completed](~/samples/migration-action-completed/index.md) |

#### Registration

You can implement, register, and call hooks in the following ways:

1. **Object:** The caller supplies an object that implements a suitable interface. This is the most straightforward way to register. However, the caller must manage the object’s lifecycle and dependencies.
2. **Factory:** The caller supplies a type, with or without a factory function to create an object, that implements a suitable interface. This allows for the injection of SDK dependencies such as the manifest, content finders, etc. The lifecycle is managed by the DI container in the SDK (more details on .NET service lifetimes are [here](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection#service-lifetimes)). Before adding the type, you must register the type on the DI container with the corresponding lifecycle.
3. **Callback:** The caller supplies a callback function that conforms to the `ExecuteAsync` method of the hook interface. This is essentially a functional version of #1. Internally, the SDK wraps the callback in a transient object to execute the function.

---
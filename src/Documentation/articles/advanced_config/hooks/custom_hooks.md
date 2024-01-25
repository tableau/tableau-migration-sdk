# Custom Hooks

## Important classes

Here are some important things you should be aware of when writing custom hooks.

- Interfaces: These are interfaces that expose supported methods. You can implement these directly or inherit available base classes.
- Base classes: These are the classes you can inherit and write in your implementation in the overridden methods. You do not need to implement the interface explicitly if you use them.
- Code Samples: We have provided some simple code samples. You can use these as a starting point for your hooks.

### Pre-migration

| Type | Base Class | Interface| Code Samples|
| --- | --- |--- |--|
| [Filters](xref:Tableau.Migration.Engine.Hooks.Filters) | [`ContentFilterBase<TContent>`](xref:Tableau.Migration.Engine.Hooks.Filters.ContentFilterBase`1) |[`IContentFilter<TContent>`](xref:Tableau.Migration.Engine.Hooks.Filters.IContentFilter`1) |[Samples/Hooks/Filters](~/samples/hooks/filters.md) |
|[Mappings](xref:Tableau.Migration.Engine.Hooks.Mappings)| [`ContentMappingBase<TContent>`](xref:Tableau.Migration.Engine.Hooks.Mappings.ContentMappingBase`1) |[`IContentMapping<TContent>`](xref:Tableau.Migration.Engine.Hooks.Mappings.IContentMapping`1)|[Samples/Hooks/Mappings](~/samples/hooks/mappings.md) |
|[Transformers](xref:Tableau.Migration.Engine.Hooks.Transformers) | [`ContentTransformerBase<TPublish>`](xref:Tableau.Migration.Engine.Hooks.Transformers.ContentTransformerBase`1) |[`IContentTransformer<TPublish>`](xref:Tableau.Migration.Engine.Hooks.Transformers.IContentTransformer`1)|[Samples/Hooks/Transformers](~/samples/hooks/transformers.md)|

### Post-migration

| Type | Base Class | Interface| Code Samples|
| --- | --- |--- |--|
|[Post-Publish](xref:Tableau.Migration.Engine.Hooks.PostPublish)|[`ContentItemPostPublishHookBase<TPublish, TResult>`](xref:Tableau.Migration.Engine.Hooks.PostPublish.ContentItemPostPublishHookBase`1) |[`IContentItemPostPublishHook<TContent>`](xref:Tableau.Migration.Engine.Hooks.PostPublish.IContentItemPostPublishHook`2) | [Samples/Hooks/Post-Publish Hooks](~/samples/hooks/post_publish.md)|
|[Bulk Post-Publish](xref:Tableau.Migration.Engine.Hooks.PostPublish)|[`BulkPostPublishHookBase<TSource>`](xref:Tableau.Migration.Engine.Hooks.PostPublish.BulkPostPublishHookBase`1)| [`IBulkPostPublishHook<TSource>`](xref:Tableau.Migration.Engine.Hooks.PostPublish.IBulkPostPublishHook`1)|[Samples/Hooks/Bulk Post-Publish](~/samples/hooks/bulk_post_publish.md)|
|[Batch Migration Completed](xref:Tableau.Migration.Engine.Hooks)| none| [`IContentBatchMigrationCompletedHook<TContent>`](xref:Tableau.Migration.Engine.Hooks.IContentBatchMigrationCompletedHook`1)|[Samples/Hooks/Batch Migration Completed](~/samples/hooks/batch_migration_completed.md)|
|[Migration Action Completed](xref:Tableau.Migration.Engine.Hooks)| none| [`IMigrationActionCompletedHook`](xref:Tableau.Migration.Engine.Hooks.IMigrationActionCompletedHook)|[Samples/Hooks/Batch Migration Completed](~/samples/hooks/migration_completed.md)|

## Registration

You can implement, register and call hooks in the following ways:

1. Object: The caller supplies an object that implements a suitable interface. This is the most straightforward way to register. However, the caller must manage the object’s lifecycle and dependencies.
2. Factory: The caller supplies a type, with or without a factory function to create an object, that implements a suitable interface. Importantly this allows for injection of SDK dependencies such as the manifest, content finders, etc. The lifecycle is managed by the DI container in the SDK (more details on DotNet Service lifetimes are [here](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection#service-lifetimes)). So, before you add the type you must register the type on the DI container with the corresponding lifecycle.
3. Callback: The caller supplies a callback function that conforms to the ExecuteAsync method of the hook interface. Essentially a functional version of #1 - internally the SDK wraps the callback in a transient object to execute the function.

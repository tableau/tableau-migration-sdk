# Sample: Bulk logging

In the following example, a bulk post-publish hook logs published items.

# [Python](#tab/Python)

#### Bulk Post-Publish Hook Class

[!code-python[](../../../../examples/Python.ExampleApplication/hooks/post_publish/bulk_logging_hook.py)]

#### Registration

[Learn more.](~/samples/index.md?tabs=Python#hook-registration)

[//]: <> (Adding this as code as regions are not supported in python snippets)
```Python
plan_builder.hooks.add(BulkLoggingHookForDataSources)
```

# [C#](#tab/CSharp)

#### Bulk Post-Publish Hook Class

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/Hooks/PostPublish/BulkLoggingHook.cs#class)]

#### Registration

[Learn more.](~/samples/index.md?tabs=CSharp#hook-registration)

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/MyMigrationApplication.cs#BulkLoggingHook-Registration)]

#### Dependency Injection

[Learn more.](~/articles/dependency_injection.md)

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/Program.cs#BulkLoggingHook-DI)]

---

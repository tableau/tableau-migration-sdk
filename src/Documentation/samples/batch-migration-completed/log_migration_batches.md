# Sample: Batch Migration Logging

This example demonstrates how to log migration batch item statuses using a batch migration completed hook.

# [Python](#tab/Python)

#### Batch Migration Completed Hook Class

[!code-python[](../../../../examples/Python.ExampleApplication/hooks/batch_migration_completed/log_migration_batches_hook.py)]

#### Registration

[Learn more.](~/samples/index.md?tabs=Python#hook-registration)

[//]: <> (Adding this as code as regions are not supported in Python snippets)
```Python
plan_builder.hooks.add(LogMigrationBatchesHookForUsers)
plan_builder.hooks.add(LogMigrationBatchesHookForGroups)
```

# [C#](#tab/CSharp)

#### Batch Migration Completed Hook Class

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/Hooks/BatchMigrationCompleted/LogMigrationBatchesHook.cs#class)]

#### Registration

[Learn more.](~/samples/index.md?tabs=CSharp#hook-registration)

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/MyMigrationApplication.cs#LogMigrationBatchesHook-Registration)]

#### Dependency Injection

[Learn more.](~/articles/dependency_injection.md)

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/Program.cs#LogMigrationBatchesHook-DI)]

---

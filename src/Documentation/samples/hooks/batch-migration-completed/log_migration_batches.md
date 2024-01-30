# Sample: Batch Migration logging

In the following example, a batch migration completed hook logs migration batch item statuses.

## [Batch Migration Completed Hook Class](#tab/class)

[!code-csharp[](../../../../../examples/Csharp.ExampleApplication/Hooks/BatchMigrationCompleted/LogMigrationBatchesHook.cs#class)]

## [Registration](#tab/registration)

[!code-csharp[](../../../../../examples/Csharp.ExampleApplication/MyMigrationApplication.cs#LogMigrationBatchesHook-Registration)]

## [Dependency Injection](#tab/di)

[!code-csharp[](../../../../../examples/Csharp.ExampleApplication/Program.cs#LogMigrationBatchesHook-DI)]

---

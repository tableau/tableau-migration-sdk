# Sample: Bulk logging

In the following example, a bulk post-publish hook logs published items.

## [Bulk Post-Publish Hook Class](#tab/class)

[!code-csharp[](../../../../../examples/Csharp.ExampleApplication/Hooks/PostPublish/BulkLoggingHook.cs#class)]

## [Registration](#tab/registration)

[!code-csharp[](../../../../../examples/Csharp.ExampleApplication/MyMigrationApplication.cs#BulkLoggingHook-Registration)]

## [Dependency Injection](#tab/di)

[!code-csharp[](../../../../../examples/Csharp.ExampleApplication/Program.cs#BulkLoggingHook-DI)]

---

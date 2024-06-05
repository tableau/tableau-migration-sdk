# Code Samples

Once you have started building your migration using the example code in [C#](~/api-csharp/index.md) or [Python](~/api-python/index.md), you may want to further customize your migration using hooks. This section provides code samples to assist you.

[Learn more about hooks here.](~/articles/hooks/index.md)

## Hook Registration

To use hooks, you need to register them with the [plan builder](~/articles/configuration.md#migration-plan).

The process of registering hooks differs slightly between C# and Python, as described below.

# [Python](#tab/Python)

In Python, injecting an object into the class constructor is not supported, and in most cases, it is unnecessary.

To register a Python hook object, simply create the object and add it to the appropriate hook type collection.

**Generic Form:** `plan_builder.[hook type collection].add([hook object])`

**Example:** 
```
plan_builder.filters.add(UnlicensedUsersFilter)
```

# [C#](#tab/CSharp)

In C#, the hook object should be registered with Dependency Injection (DI). This allows hooks to have any object injected into the constructor without concern for the source of the object.

Learn more about [dependency injection](~/articles/dependency_injection.md).

To register your hook object with the dependency injection service provider, simply add it.

**Generic Form:** `services.AddScoped<[Object type]>();`

**Example:**
```
services.AddScoped<UnlicensedUsersFilter>();
```

Once the hook is registered with DI, it must be added to the appropriate [hook collection](~/articles/hooks/custom_hooks.md).

**Generic Form:** `_planBuilder.[hook type collection].Add<[object name], [hook type]>();`

**Example:**
```
_planBuilder.Filters.Add<UnlicensedUsersFilter, IUser>();
```

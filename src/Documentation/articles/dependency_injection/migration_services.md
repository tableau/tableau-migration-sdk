# Custom Migration Services

Migration services provide an alternative path for [dependency injection](~/articles/dependency_injection/index.md) to customize Migration SDK behavior.
Replacing most DI services is controlled through the application's `IServiceProvider` container, which typically does not change after application startup and is not easily available in all contexts.
Migration services are those DI services that the Migration SDK obtains through the [migration plan](~/articles/configuration/index.md#basic-configuration), which may or may not ultimately come from the `IServiceProvider` container.
This allows migration services to be customized on a per-plan basis, and makes service customization easier in interoperability scenarios (e.g. Python).

## Supported Migration Services

The services available to override through the migration services feature are listed in the [plan builder's](~/articles/configuration/index.md#basic-configuration) service collection.
If the migration service is [generic](#generic-migration-services) the open generic type is listed in the supported services list.

### [Python](#tab/python)

Supported migration services are available through the plan builder's [`services`](~/api-python/reference/tableau_migration.migration_engine.PyMigrationPlanBuilder.md#tableau_migration.migration_engine.PyMigrationPlanBuilder.services) property.

```Python
plan_builder = MigrationPlanBuilder()
for service in plan_builder.services.supported_services:
	print(service.name) # Service name contains the 
```

### [C#](#tab/csharp)

Supported migration services are available through the plan builder's [`Services`](xref:Tableau.Migration.IMigrationPlanBuilder`1#Tableau_Migration_IMigrationPlanBuilder_1_Services) property.

```C#
var planBuilder = new MigrationPlanBuilder();
foreach(var service in planBuilder.Services.SupportedServices)
{
    Console.WriteLine(service.Name);
}
```

---

## Overriding Migration Services

All migration services have default implementations provided by the Migration SDK.
When a migration service is registered with the [plan builder](~/articles/configuration/index.md#basic-configuration) it will be used in place of the default implementation for that plan.

### [Python](#tab/python)

Override migration services on a per-plan basis through the plan builder's [`services`](~/api-python/reference/tableau_migration.migration_engine.PyMigrationPlanBuilder.md#tableau_migration.migration_engine.PyMigrationPlanBuilder.services) property.

#### Migration Service Class

Like hooks, migration services are created by inheriting from a service base class.

[!code-python[](../../../../examples/Python.ExampleApplication/services/migration_content_loader/empty_migration_content_loader.py)]

#### Registration

Migration services are then registered with the service builder for a given service type.

[//]: <> (Adding this as code so we don't change example application default behavior.)
```Python
plan_builder.services.set(MigrationContentLoaderBase[IUser], EmptyMigrationContentLoader[IUser])
```

### [C#](#tab/csharp)

Override migration services on a per-plan basis through the plan builder's [`Services`](xref:Tableau.Migration.IMigrationPlanBuilder`1#Tableau_Migration_IMigrationPlanBuilder_1_Services) property.

#### Migration Service Class

Like hooks, migration services are created by implementing a service interface.

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/Services/MigrationContentLoader/EmptyMigrationContentLoader.cs#class)]

#### Registration

A service factory is then registered with the service builder for a given service type.
The service factory context includes the scoped `IServiceProvider` container for types available through application DI.

[//]: <> (Adding this as code so we don't change example application default behavior.)
```C#
planBuilder.Services.Set<IMigrationContentLoader<IUser>>(MigrationServiceFactoryContext ctx => 
{
	return ctx.Services.GetRequiredService<EmptyMigrationContentLoader<IUser>>();
});
```

---

## Generic Migration Services

Many migration services are [generic](https://learn.microsoft.com/en-us/dotnet/standard/generics), meaning they have type arguments.
Normally these type arguments represent migrating content types, so that different migration service implementations can be used for different content types.

When a migration service overrides a _closed generic_ type, meaning all type arguments are specified, that override will only be used for those type arguments.

Alternatively, a migration service can override the _open generic_ type, meaning no type arguments are specified.
Migration service overrides for open generic types are used when no other override is registered for the specific type arguments involved.

### [Python](#tab/python)

[//]: <> (Adding this as code so we don't change example application default behavior.)
```Python
# Closed generic, only overrides IUser.
plan_builder.services.set(MigrationContentLoaderBase[IUser], EmptyMigrationContentLoader[IUser])

# Open generic, used for all types without a closed generic override.
plan_builder.services.set(MigrationContentLoaderBase, EmptyMigrationContentLoader) 
```

### [C#](#tab/csharp)

The service factory context includes the requested type arguments for service creation.

[//]: <> (Adding this as code so we don't change example application default behavior.)
```C#
// Closed generic, only overrides IUser.
planBuilder.Services.Set<IMigrationContentLoader<IUser>>(MigrationServiceFactoryContext ctx => 
{
	return ctx.Services.GetRequiredService<EmptyMigrationContentLoader<IUser>>();
});

// Open generic, used for all types without a closed generic override.
planBuilder.Services.Set(typeof(IMigrationContentLoader<>), MigrationServiceFactoryContext ctx => 
{
	return ctx.Services.GetRequiredService(typeof(EmptyMigrationContentLoader<>).MakeGenericType(ctx.Type.GetGenericArguments()));
});
```

---
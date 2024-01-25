# Dependency Injection

Migration SDK uses [.NET Dependency Injection](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection) (DI) to manage required service dependencies.

## Dependency Retrieval and Management

The entry point for DI is the [IServiceProvider](https://learn.microsoft.com/en-us/dotnet/api/system.iserviceprovider?view=net-6.0) interface. This acts as a container to retrieve registered services and manage their lifetimes. 

## Dependency Lifetimes

Service lifetimes are controlled by the [ServiceLifetime](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.servicelifetime?view=dotnet-plat-ext-6.0) enum. These are automatically applied using `IServiceCollection.AddSingleton/AddScoped/AddTransient` extension methods.

## Dependency Registration

Individual services are registered using an [IServiceCollection](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection?view=dotnet-plat-ext-6.0) instance. This instance is a collection of registered services that is built to create an [IServiceProvider](https://learn.microsoft.com/en-us/dotnet/api/system.iserviceprovider?view=net-6.0) instance for service retrieval and management.

## Migration SDK Dependency Injection

[IServiceCollectionExtensions.AddTableauMigrationSdk](xref:Tableau.Migration.IServiceCollectionExtensions.AddTableauMigrationSdk(Microsoft.Extensions.DependencyInjection.IServiceCollection,Microsoft.Extensions.Configuration.IConfiguration)) is the main method used to register default Migration SDK services. This call is required to set up the dependencies required by the SDK.

Scoped services are used to isolate services, for example for source and destination API clients.

## Sample Code

The sample below shows a console application that initializes the Migration SDK using dependency injection. The full code is available in the `DependencyInjection.ExampleApplication` project.

[!code-csharp[](../../../examples/DependencyInjection.ExampleApplication/Program.cs)]
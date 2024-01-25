# Logging

The Migration SDK has built-in support for logging. SDK consumers can configure logging levels<sub>[.NET](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loglevel), [Python](https://docs.python.org/3/howto/logging.html#logging-levels)</sub> and add more logging providers (called handlers in Python)<sub>[.NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/logging-providers), [Python](https://docs.python.org/3/library/logging.handlers.html)</sub>.

Internally, the SDK will log every successfully **Request**/**Response** as an **Information** message with the [**Http Request Method**](https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods), the **Http Request Uri** and the [**Http Response Status Code**](https://developer.mozilla.org/en-US/docs/Web/HTTP/Status):

```Shell
> Tableau.Migration.Net.NetworkTraceLogger: Information: HTTP GET "https://localhost/api/2.4/serverinfo" responded "OK".
```

It will also log every errored **Request**/**Response** as an **Error** message with the [**Http Request Method**](https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods), the **Http Request Uri** and the **Error Message**:

```Shell
> Tableau.Migration.Net.NetworkTraceLogger: Error: HTTP GET "https://localhost/api/2.4/serverinfo" failed. Error: "An error occurred while sending the request.".
```

As part of the included tracings, it is possible to [configure](configuration.md) the level of details for each log message by setting the following configuration parameters:

- [Network.HeadersLoggingEnabled](xref:Tableau.Migration.Config.NetworkOptions#Tableau_Migration_Config_NetworkOptions_HeadersLoggingEnabled): Indicates whether the SDK logs request/response headers. The default value is disabled.
- [Network.ContentLoggingEnabled](xref:Tableau.Migration.Config.NetworkOptions#Tableau_Migration_Config_NetworkOptions_ContentLoggingEnabled): Indicates whether the SDK logs request/response content. The default value is disabled.
- [Network.BinaryContentLoggingEnabled](xref:Tableau.Migration.Config.NetworkOptions#Tableau_Migration_Config_NetworkOptions_BinaryContentLoggingEnabled): Indicates whether the SDK logs request/response binary (not textual) content. The default value is disabled.
- [Network.ExceptionsLoggingEnabled](xref:Tableau.Migration.Config.NetworkOptions#Tableau_Migration_Config_NetworkOptions_ExceptionsLoggingEnabled): Indicates whether the SDK logs network exceptions. The default value is disabled.

## C# Support

The Migration SDK supports logging with built-in or third-party providers such as the ones described in [.NET Logging Providers](https://learn.microsoft.com/en-us/dotnet/core/extensions/logging-providers). Refer to that article for guidance in your use case. Some basic examples are below.

### Adding your logging provider

You can add logging when you add the Migration SDK to the service collection.

#### Adding a default provider without configuration

```C#
services        
    .AddTableauMigrationSdk()
    .AddLogging();
```

#### Adding a logging provider with configuration

This example adds NLog.

```C#
services
    .AddTableauMigrationSdk()
    .AddLogging(builder =>
    {
        builder.AddNLog();
    })
```

> [!Note]
> See [LoggingServiceCollectionExtensions.AddLogging Method](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.loggingservicecollectionextensions.addlogging) for guidance on how to configure your logging provider.

## Python Support

The Migration SDK supports logging with built-in providers like the one described in [Python Logging docs](https://docs.python.org/3/howto/logging.html).

### SDK default handler

The SDK adds a StreamHandler to the root logger by executing the following command:

```Python
logging.basicConfig(
    format = '%(asctime)s - %(name)s - %(levelname)s - %(message)s', 
    level = logging.INFO)
```

### Overriding default handler configuration

To override the default configuration, the parameter `force` must be set to `True`.

```Python
logging.basicConfig(
    force = True,
    format = '%(asctime)s|%(levelname)s|%(name)s -\t%(message)s',
    level = logging.WARNING)
```

> [!Note]
> See [Logging Configuration](https://docs.python.org/3/library/logging.config.html) for advanced configuration guidance.

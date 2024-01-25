# Examples to get started with the Migration SDK (.NET)

## [Program.cs](#tab/program-cs)

[!code-csharp[CS](../../../examples/Csharp.ExampleApplication/Program.cs#namespace)]

## [Startup code](#tab/startup-cde)

[!code-csharp[CS](../../../examples/Csharp.ExampleApplication/MyMigrationApplication.cs#namespace)]

## [Config classes](#tab/config-classes)

[!code-csharp[CS](../../../examples/Csharp.ExampleApplication/Config/MyMigrationApplicationOptions.cs#namespace)]

[!code-csharp[CS](../../../examples/Csharp.ExampleApplication/Config/EndpointOptions.cs#namespace)]

## [Config file](#tab/appsettings)

```json
{
  "source": {
    "serverUrl": "http://server",
    "siteContentUrl": "",
    "accessTokenName": "my server token name",
    "accessToken": "my-secret-server-pat"
  },
  "destination": {
    "serverUrl": "https://pod.online.tableau.com",
    "siteContentUrl": "site-name",
    "accessTokenName": "my cloud token name",
    "accessToken": "my-secret-cloud-pat"
  }  
}
```

---

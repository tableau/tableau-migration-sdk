### [Program.cs](#tab/program-cs)

[!code-csharp[](../../../examples/Csharp.ExampleApplication/Program.cs#namespace)]

### [Startup code](#tab/startup-cde)

[!code-csharp[](../../../examples/Csharp.ExampleApplication/MyMigrationApplication.cs#namespace)]

### [Config classes](#tab/config-classes)

[!code-csharp[](../../../examples/Csharp.ExampleApplication/Config/MyMigrationApplicationOptions.cs#namespace)]

[!code-csharp[](../../../examples/Csharp.ExampleApplication/Config/EndpointOptions.cs#namespace)]

### [Config file](#tab/appsettings)

```json
{
  "source": {
    "serverUrl": "http://server",
    "siteContentUrl": "",
    "accessTokenName": "my-server-token-name",
    "accessToken": "my-secret-server-pat"
  },
  "destination": {
    "serverUrl": "https://pod.online.tableau.com",
    "siteContentUrl": "site-name",
    "accessTokenName": "my-cloud-token-name",
    "accessToken": "my-secret-cloud-pat"
  }  
}
```

---

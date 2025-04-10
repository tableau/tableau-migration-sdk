# Configuration

The Migration SDK uses two sources of configuration

1. [Basic Configuration](#basic-configuration) that uses the [Migration Plan](~/articles/terms.md#migration-plan). It contains configuration for a specific migration run.
2. [Advanced Configuration](#advanced-configuration). This configuration that is unlikely to change between migration runs.

![Configuration Blocks](../images/configuration.svg){width=65%}

## Basic Configuration

The bare minimum Migration SDK configuration is done using a [Migration Plan](~/articles/terms.md#migration-plan). It defines the source, destination, and hooks executed during migration. The easiest way to generate a new Migration Plan is using [`MigrationPlanBuilder`](xref:Tableau.Migration.Engine.MigrationPlanBuilder)([`IMigrationPlanBuilder`](xref:Tableau.Migration.IMigrationPlanBuilder) implementation). Before you [build](#build) a new plan, you need to:

- Define [Source](#source).
- Define [Destination](#destination).
- Define the [Migration Type](#migration-type).
- Customize with [hooks](#add-hooks-optional) (optional).

> [!IMPORTANT]
> Personal access tokens (PATs) are long-lived authentication tokens that allow you to sign in to the Tableau REST API without requiring hard-coded credentials or interactive sign-in.
> Best practices
>
> - Revoke and generate a new PAT every day to keep your server secure.
> - Access tokens should not be stored in plain text in application configuration files. Instead, use secure alternatives, such as encryption or a secrets management system.
> - If the source and destination sites are on the same server, use separate PATs.

### Source

The method [`MigrationPlanBuilder.FromSourceTableauServer`](xref:Tableau.Migration.Engine.MigrationPlanBuilder#Tableau_Migration_Engine_MigrationPlanBuilder_FromSourceTableauServer_System_Uri_System_String_System_String_System_String_System_Boolean_) defines the source server by instantiating a new [`TableauSiteConnectionConfiguration`](xref:Tableau.Migration.Api.TableauSiteConnectionConfiguration) with the following parameters:

- serverUrl
- siteContentUrl (optional)
- accessTokenName
- accessToken

### Destination

The method [`MigrationPlanBuilder.ToDestinationTableauCloud`](xref:Tableau.Migration.Engine.MigrationPlanBuilder#Tableau_Migration_Engine_MigrationPlanBuilder_ToDestinationTableauCloud_System_Uri_System_String_System_String_System_String_System_Boolean_) defines the destination server by instantiating a new [`TableauSiteConnectionConfiguration`](xref:Tableau.Migration.Api.TableauSiteConnectionConfiguration) with the following parameters:

- podUrl
- siteContentUrl: This is the site name on Tableau Cloud.
- accessTokenName
- accessToken

### Migration Type

The method [`MigrationPlanBuilder.ForServerToCloud`](xref:Tableau.Migration.Engine.MigrationPlanBuilder#Tableau_Migration_Engine_MigrationPlanBuilder_ForServerToCloud) defines the migration type and load all default hooks for a **Server to Cloud** migration.

### Add Hooks (optional)

The Plan Builder exposes the properties [`MigrationPlanBuilder.Hooks`](xref:Tableau.Migration.Engine.MigrationPlanBuilder#Tableau_Migration_Engine_MigrationPlanBuilder_Hooks), [`MigrationPlanBuilder.Filters`](xref:Tableau.Migration.Engine.MigrationPlanBuilder#Tableau_Migration_Engine_MigrationPlanBuilder_Filters), [`MigrationPlanBuilder.Mappings`](xref:Tableau.Migration.Engine.MigrationPlanBuilder#Tableau_Migration_Engine_MigrationPlanBuilder_Mappings), and [`MigrationPlanBuilder.Transformers`](xref:Tableau.Migration.Engine.MigrationPlanBuilder#Tableau_Migration_Engine_MigrationPlanBuilder_Transformers). With these properties, you can customize your migration plan. See [Custom Hooks article](hooks/custom_hooks.md) for more details.

### Build

 The method [`MigrationPlanBuilder.Build`](xref:Tableau.Migration.Engine.MigrationPlanBuilder#Tableau_Migration_Engine_MigrationPlanBuilder_Build) generates a Migration Plan ready to be used as an input to a migration process.

## Advanced configuration

[`MigrationSdkOptions`](xref:Tableau.Migration.Config.MigrationSdkOptions) is the configuration class the Migration SDK uses internally to process a migration. It contains adjustable properties that change some engine behaviors. These properties are useful tools to troubleshoot and tune a migration process.

> [!NOTE]
> Unless specified otherwise, all configuration options are dynamically applied during the migration.

### [Python](#tab/Python)

Configuration values are set via environment variables. The `:` delimiter doesn't work with environment variable hierarchical keys on all platforms. For example, the `:` delimiter is not supported by Bash. The double underscore (`__`), which is supported on all platforms, automatically replaces any `:` delimiters in environment variables. All configuration environment variables start with `MigrationSDK__`.

### [C#](#tab/CSharp)

We recommend using a [.NET Generic Host](https://learn.microsoft.com/en-us/dotnet/core/extensions/generic-host?tabs=appbuilder) to initialize the application. This will enable setting configuration values via `appsettings.json` which can be passed into `userOptions` in [`.AddTableauMigrationSdk`](xref:Tableau.Migration.IServiceCollectionExtensions#Tableau_Migration_IServiceCollectionExtensions_AddTableauMigrationSdk_Microsoft_Extensions_DependencyInjection_IServiceCollection_Microsoft_Extensions_Configuration_IConfiguration_). See  [.NET getting started examples](~/api-csharp/index.md) for more info.

---

### [ContentTypes](xref:Tableau.Migration.Config.ContentTypesOptions)

This is an array of [`MigrationSdkOptions.ContentTypesOptions`](xref:Tableau.Migration.Config.ContentTypesOptions). Each array object corresponds to settings for a single content type.

> [!IMPORTANT]
> The [type](xref:Tableau.Migration.Config.ContentTypesOptions.Type) values are case-insensitive. Duplicate [type](xref:Tableau.Migration.Config.ContentTypesOptions.Type) key values will result in an exception.

### [Python](#tab/Python)

#### Python Environment Variables

- `MigrationSDK__ContentTypes__<array index>__<content type config key>__Type`.
- `MigrationSDK__ContentTypes__<array index>__<content type config key>__BatchSize`.

**Example:** To set the `User` `BatchSize` to `201` and `Project` BatchSize to `203`, you would set environment variables as follows. Note the array indexes. They tie the setting values together in the Migration SDK.

```bash
# User BatchSize is 201
MigrationSDK__ContentTypes__0__Type = User
MigrationSDK__ContentTypes__0__BatchSize = 201

# Project BatchSize is 203
MigrationSDK__ContentTypes__1__Type = Project
MigrationSDK__ContentTypes__1__BatchSize = 203
```

### [C#](#tab/CSharp)

In the following `json` example config file,

- A `BatchSize` of `201` is applied to the content type `User`.
- A `BatchSize` of `203` for `Project`.
- A `BatchSize` of `200` for `ServerExtractRefreshTask`.

```JSON
{
    "MigrationSdkOptions": {
        "contentType": [
        {
            "type":"User",
            "batchSize": 201            
        },        
        {
            "type":"Project",
            "batchSize": 203            
        },
        {
            "type":"ServerExtractRefreshTask",
            "batchSize": 200
        }
        ],
    }
}

```

---

The following table describes each setting. They should always be set per content type as described previously. If a setting below is not set for a content type, the Migration SDK falls back to the default value.

[!include[](~/includes/configuration/sdk_opts_content_types.html)]

### [MigrationParallelism](xref:Tableau.Migration.Config.MigrationSdkOptions#Tableau_Migration_Config_MigrationSdkOptions_MigrationParallelism)

This setting defines the number of parallel tasks migrating the same type of content simultaneously. You can tune the Migration SDK processing time with this configuration.

*Default:* 10
*Python Environment Variable:* `MigrationSDK__MigrationParallelism`

> [!WARNING]
> There are [concurrency limits in REST APIs on Tableau Cloud](https://kb.tableau.com/articles/issue/concurrency-limits-in-rest-apis-on-tableau-cloud). The current default configuration is the balance between performance without blocking too many resources to the migration process.

### [File](xref:Tableau.Migration.Config.FileOptions)

This section contains options related to file storage.

[!include[](~/includes/configuration/sdk_opts_files.html)]

### [Network](xref:Tableau.Migration.Config.NetworkOptions)

This configuration section contains network-related options.

> [!IMPORTANT]
> `NetworkOptions.UserAgentComment` is not dynamically applied. It takes effect when you restart your application.

[!include[](~/includes/configuration/sdk_opts_network.html)]

#### [Resilience](xref:Tableau.Migration.Config.ResilienceOptions)

The `Resilience` sub-section deals with the resilience and transient-fault layer. See [Microsoft.Extensions.Http.Resilience](https://learn.microsoft.com/en-us/dotnet/core/resilience) for more details.

[!include[](~/includes/configuration/sdk_opts_network_res.html)]

### [DefaultPermissionsContentType](xref:Tableau.Migration.Config.DefaultPermissionsContentTypeOptions)

[!include[](~/includes/configuration/sdk_opts_def_perm.html)]

### [Job](xref:Tableau.Migration.Config.JobOptions)

The Migration SDK uses [two methods](hooks/index.md#hook-execution-flow) to publish the content to a destination server:

1. 'Bulk process': A single REST API call for multiple items.
2. 'Individual process': One REST API call per item.

This configuration only applies to the 'Bulk process'. Each batch publish REST API call returns a Job ID (see the [Tableau REST API Query Job](https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_jobs_tasks_and_schedules.htm#query_job) for details). The SDK uses this ID to determine job status. The following table describes the related settings.

[!include[](~/includes/configuration/sdk_opts_jobs.html)]

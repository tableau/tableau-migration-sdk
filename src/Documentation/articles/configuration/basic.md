# Basic Configuration

## Migration Plan

The migration plan is a required input in the migration process. It defines the Source and Destination servers and the hooks executed during the migration. Consider the Migration Plan as the steps the Migration SDK follows to migrate the information from a given source to a given destination.

The [`IMigrationPlan`](xref:Tableau.Migration.IMigrationPlan) interface defines the Migration Plan structure. The easiest way to generate a new Migration Plan is using [`MigrationPlanBuilder`](xref:Tableau.Migration.Engine.MigrationPlanBuilder). Following are the steps needed before [building](#build) a new plan:

- [Define the required Source server](#source).
- [Define the required Destination server](#destination).
- [Define the required Migration Type](#migration-type).

### Source

 The method [`MigrationPlanBuilder.FromSourceTableauServer`](xref:Tableau.Migration.Engine.MigrationPlanBuilder.FromSourceTableauServer(System.Uri,System.String,System.String,System.String,System.Boolean))  defines the source server by instantiating a new [`TableauSiteConnectionConfiguration`](xref:Tableau.Migration.Api.TableauSiteConnectionConfiguration) with the following parameters:

- **serverUrl**
- **siteContentUrl:**(optional)
- **accessTokenName**
- **accessToken**

### Destination

The method [`MigrationPlanBuilder.ToDestinationTableauCloud`](xref:Tableau.Migration.Engine.MigrationPlanBuilder.ToDestinationTableauCloud(System.Uri,System.String,System.String,System.String,System.Boolean)) defines the destination server by instantiating a new [`TableauSiteConnectionConfiguration`](xref:Tableau.Migration.Api.TableauSiteConnectionConfiguration) with the following parameters:

- **podUrl**
- **siteContentUrl** The site name on Tableau Cloud.
- **accessTokenName**
- **accessToken**

> [!Important]
> Personal access tokens (PATs) are long-lived authentication tokens that allow you to sign in to the Tableau REST API without requiring hard-coded credentials or interactive sign-in. Revoke and generate a new PAT every day to keep your server secure. Access tokens should not be stored in plain text in application configuration files, and should instead use secure alternatives such as encryption or a secrets management system. If the source and destination sites are on the same server, separate PATs should be used.

### Migration Type

The method [`MigrationPlanBuilder.ForServerToCloud`](xref:Tableau.Migration.Engine.MigrationPlanBuilder.ForServerToCloud) will define the migration type and load all default hooks for a **Server to Cloud** migration.

### Build

The method [`MigrationPlanBuilder.Build`](xref:Tableau.Migration.Engine.MigrationPlanBuilder.Build) generates a Migration Plan ready to be used as an input to a migration process.
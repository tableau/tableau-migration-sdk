# Example Hook Use Cases

## Remove Unlicensed Users During Migration
First, create a [filter](~/samples/filters/index.md) for [IUser](xref:Tableau.Migration.Content.IUser) items.
The filter should check the [LicenseLevel](xref:Tableau.Migration.Content.IUser.LicenseLevel) property for the `Unlicensed` value, either as a string constant or with the [LicenseLevels.Unlicensed](xref:Tableau.Migration.Api.Rest.Models.LicenseLevels.Unlicensed) constant.
This filter will prevent users matching the license level from being created on the destination site.

Additional logic is needed to handle references to these users since they will not exist on the destination site, for example if an unlicensed user owns a workbook.
One method would be to create a [mapping](~/samples/mappings/index.md) that maps unlicensed users to one or more alternate users that will exist on the destination site.
This will allow default hooks to update user references to the alternate users.

The filter and mapping are then [registered as custom hooks](custom_hooks.md#registration) with the migration plan.

## Filter Out Content Under Certain Projects
To increase flexibility, filters do not cascade, so filtering projects does not automatically filter content within a project.
To filter this content, create a [filter](~/samples/filters/index.md) for data sources, workbooks, and other content types as desired.
The filters should check the [Location](xref:Tableau.Migration.IContentReference.Location) property to determine if the content is in a project that has been filtered.
This can be done by comparing the path of the location's [Parent](xref:Tableau.Migration.ContentLocation.Parent) method result to either a known set of project paths, or by looking up the project in the the [migration manifest](xref:Tableau.Migration.IMigrationManifest) to see if the project was migrated.

The filters are then [registered as a custom hook](custom_hooks.md#registration) with the migration plan.

## Rename a Project
Content items are renamed through mappings so that references to content items can also be mapped during migration.
To rename a project, create a [mapping](~/samples/mappings/index.md) for [IProject](xref:Tableau.Migration.Content.IProject) items.
The mapping should change the final path segment of the project's [ContentLocation](xref:Tableau.Migration.ContentLocation) as desired.
Manipulating the path's last segment can be easily done through the location's [Rename](xref:Tableau.Migration.ContentLocation.Rename(System.String)) method.

The mapping is then [registered as a custom hook](custom_hooks.md#registration) with the migration plan.

## Combine Projects
To combine multiple source projects to a single destination project, a single destination project must be determined.
The destination project can either be created before the migration, or a [filter](~/samples/filters/index.md) for [IProject](xref:Tableau.Migration.Content.IProject) items can be created to filter all but one of the source projects.
In the case of using a filter, a [transformer](~/samples/transformers/index.md) can be created to merge permissions or other properties of the combined project when it is migrated.

With a single destination project determined a [mapping](~/samples/mappings/index.md) for [IProject](xref:Tableau.Migration.Content.IProject) items should be created.
The mapping should map the source projects to the destination combined project's location.

The filters and mapping are then [registered as custom hooks](custom_hooks.md#registration) with the migration plan.

## Change a Published Data Source to Use Tableau Bridge
First, create a [transformer](~/samples/transformers/index.md) for [IPublishableDataSource](xref:Tableau.Migration.Content.IPublishableDataSource) items.
The transformer should determine if the data source needs to use Tableau Bridge, and set the [UseRemoteQueryAgent](xref:Tableau.Migration.Content.IDataSource.UseRemoteQueryAgent) property to `true` for those data sources.

The transformer is then [registered as a custom hook](custom_hooks.md#registration) with the migration plan.

## Tagging Content During Migration
First, create a [transformer](~/samples/transformers/index.md) for the desired content types that [support tags](xref:Tableau.Migration.Content.IWithTags).
The transformer can then add a new [Tag](xref:Tableau.Migration.Content.Tag) object to the [Tags](xref:Tableau.Migration.Content.IWithTags.Tags) collection of the content item.
A default post-publish hook will then ensure that the tag is added after the content item is published.

The transformer is then [registered as a custom hook](custom_hooks.md#registration) with the migration plan.

## Embed Connection Credentials
To embed credentials in connections, create a [post-publish hook](~/samples/post-publish/index.md) for the appropriate publish and result types.
The hook should use dependeny injection to obtain the migration's [destination endpoint](xref:Tableau.Migration.Engine.Endpoints.IDestinationEndpoint).
The APIs of the destination can be accessed through the [SiteApi](xref:Tableau.Migration.Engine.Endpoints.IMigrationApiEndpoint.SiteApi) property of the endpoint after casting to the [IMigrationApiEndpoint](xref:Tableau.Migration.Engine.Endpoints.IMigrationApiEndpoint) interface.
The APIs of the desired content type should then be used to [get the connection ID](xref:Tableau.Migration.Api.IConnectionsApiClient#Tableau_Migration_Api_IConnectionsApiClient_GetConnectionsAsync_System_Guid_System_Threading_CancellationToken_) of the connection in question, and [update the connection](xref:Tableau.Migration.Api.IConnectionsApiClient#Tableau_Migration_Api_IConnectionsApiClient_UpdateConnectionAsync_System_Guid_System_Guid_Tableau_Migration_Api_Models_IUpdateConnectionOptions_System_Threading_CancellationToken_) to embed the credentials.

The hook is then [registered as a custom hook](custom_hooks.md#registration) with the migration plan.

## Cancel Migration On Batch Failure
By default the migration SDK continues migration if a single content items fail to migrate.
This allows for partial migration, but if fundamental items such a user fails to migrate, downstream items that reference this item may fail to migrate as well.
Canceling the migration after failures are seen when a batch completes allows for faster intervention, while ensuring that items of the batch have completed and are not partially migrated.

First, create a [batch completed](~/samples/batch-migration-completed/index.md) hook for one or more content types.
To inspect whether the batch migration has succeeded, check the [ItemResults](xref:Tableau.Migration.Engine.Migrators.Batch.IContentBatchMigrationResult`1.ItemResults) property of the context object.
The [status](xref:Tableau.Migration.Engine.Manifest.MigrationManifestEntryStatus) should not be the [Error](xref:Tableau.Migration.Engine.Manifest.MigrationManifestEntryStatus.Error) enumeration value.
If an error is detected, the hook should return the value of the context object's [ForNextBatch](xref:Tableau.Migration.Engine.Migrators.Batch.IContentBatchMigrationResult`1.ForNextBatch(System.Boolean)) method with a `false` value for the `performNextBatch` argument.

The hook is then [registered as a custom hook](custom_hooks.md#registration) with the migration plan.

## Cancel Migration After Users and Groups
To prevent content types from migrating entirely a [filter](~/samples/filters/index.md) that filters out all items may be used, but this still lists the items on the source site.
To end the migration early, an [action completed](~/samples/migration-action-completed/index.md) hook can be created.
The hook can use dependency injection to obtain the migration's [manifest](xref:Tableau.Migration.IMigrationManifest), and can inspect if the expected content types are returned by the [GetPartitionTypes](xref:Tableau.Migration.Engine.Manifest.IMigrationManifestEntryCollection.GetPartitionTypes) method of the manifest's [Entries](xref:Tableau.Migration.IMigrationManifest.Entries) property.
To cancel the migration, return the value of the context object's [ForNextAction](xref:Tableau.Migration.Engine.Actions.IMigrationActionResult.ForNextAction(System.Boolean)) method with a `false` value for the `performNextAction` argument.

The hook is then [registered as a custom hook](custom_hooks.md#registration) with the migration plan.
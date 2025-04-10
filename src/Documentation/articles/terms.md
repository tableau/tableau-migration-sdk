
# SDK Terminology

## Content Type

Content types are the various types of content that reside on Tableau Server or Tableau Cloud. For details about which content types the Migration SDK supports, see [Supported Content Types](https://help.tableau.com/current/api/migration_sdk/en-us/docs/supported_content_types.html). For unsupported content types, see [Data not supported by the Migration SDK](https://help.tableau.com/current/api/migration_sdk/en-us/docs/planning.html#data-not-supported-by-the-migration-sdk).

## Content Item

 Item of a certain content type.

## Content Migration Action or Content Action

The action that migrates Content Items of a certain Content Type.

## Migration Plan

A migration plan describes how a migration should be done and what customizations must be done inflight. The [`IMigrationPlan`](xref:Tableau.Migration.IMigrationPlan) interface defines the Migration Plan structure. See [Basic Configuration](~/articles/configuration.md#basic-configuration) for more guidance on the Migration Plan and how to use it.

## Plan Builder

This is the best way to build a migration plan. Calling the [`Build()`](xref:Tableau.Migration.Engine.MigrationPlanBuilder.Build) method on the [`IMigrationPlanBuilder`](xref:Tableau.Migration.IMigrationPlanBuilder) gives you a [`MigrationPlan`](xref:Tableau.Migration.Engine.MigrationPlan).

## Manifest

The migration manifest describes the various Tableau data items found to migrate and their migration results. See [`IMigrationManifest`](xref:Tableau.Migration.IMigrationManifest) for details.

## Manifest Serializer

The Migration SDK ships with a helpful serializer in both [C#](xref:Tableau.Migration.Engine.Manifest.MigrationManifestSerializer) and [Python](~/api-python/reference/tableau_migration.migration_engine_manifest.PyMigrationManifestSerializer.md). It serializes and deserializes migration manifests in JSON format.

## Migration Status

This is simply the status of the migration. See [`MigrationCompletionStatus`](xref:Tableau.Migration.MigrationCompletionStatus) for a list of statuses.

## Migration Result

This is the [result](xref:Tableau.Migration.MigrationResult) generated after the migration has finished. It has two properties

  1. [`Manifest`](#manifest)
  2. [`Migration Status`](#migration-status)

## Hook

A [hook](~/articles/hooks/index.md) is a means of modifying the standard functionality of the Migration SDK. These modifications include filtering, mapping, transforming migration content and reacting to other SDK events.
  
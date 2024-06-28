# Hooks

A hook is a means of modifying the standard functionality of the Migration SDK. These modifications include filtering, mapping, transforming migration content and reacting to other SDK events.

The Migration SDK has [default hooks](#default-hooks) that run for every migration.
> [!NOTE]
> You can also write [custom hooks](custom_hooks.md) to fit your specific use cases.

## Terminology

- Content Type: The type of Tableau content. Examples are user, project, workbook.
- Content Item: Items of a certain content type.
- Content Migration Action/Content Action: The action that migrates Content Items of a certain Content Type.

## Types of Hooks

The Migration SDK has the following types of hooks, categorized broadly based on when they run.

### Content

These types of hooks run on content items.

- Filters: Used to exclude certain content items based on known criteria.

   > [!Note]
   > Filters do not have a cascading effect. You will need to write similar filters or mappings for the related content items as well.

- Mappings: Used to map a source item to something different at the destination. The original does not change.
- Transformers: Used to change certain properties within various content types. A good example is permissions where the source and destination have different identifiers.
  
    > [!Important]
    > Transformers that change properties like names and IDs erase references to the original. If that is not intended, you should use mappings instead.

### General purpose

These types of hooks run before or after certain migration events.

- Post-Publish: Run on the destination content after the items for the content type have been published.

- Bulk Post-Publish: Execute after publishing a batch of content, when bulk publishing is supported. You can make changes to the published set of items with this type of hook. You can write this type of hook for content types such as Users.

- Migration Action Completed: Execute after the migration of each content type.
  
- Batch Migration Completed: Execute after the completion of the migration of a batch of Tableau’s content.

## Hook execution flow

This diagram displays how each [hook](#types-of-hooks) is called as part of the migration process.

![Hooks flowchart](~/images/hooks.svg){width=70%}

## Default Hooks

The plan builder may have pre-loaded hooks, called "Default" hooks. These are necessary due to inherent differences between Tableau Server and Tableau Cloud.

1. [Default Filters](xref:Tableau.Migration.Engine.Hooks.Filters.Default)
2. [Default Mappings](xref:Tableau.Migration.Engine.Hooks.Mappings.Default)
3. [Default Post-Publish Hooks](xref:Tableau.Migration.Engine.Hooks.PostPublish.Default)
4. [Default Transformers](xref:Tableau.Migration.Engine.Hooks.Transformers.Default)

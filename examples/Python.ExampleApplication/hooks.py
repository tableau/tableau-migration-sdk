# This file contains example hooks of different categories.

from tableau_migration import (
    ContentFilterBase,
    ContentMappingBase,
    ContentMappingContext,
    ContentMigrationItem,
    ContentTransformerBase,
    IGroup,
    IProject,
    IUser,
    MigrationActionCompletedHookBase,
    MigrationActionResult
)

# Hooks using classes.

class LogActionHook(MigrationActionCompletedHookBase):

    def execute(self, ctx: MigrationActionResult) -> MigrationActionResult:
        print("ACTION COMPLETED")
        return ctx

class TestGroupFilter(ContentFilterBase[IGroup]):
    
    def should_migrate(self, item: ContentMigrationItem[IGroup]) -> bool:
        return "Test" not in item.source_item.name

class UsernameMapping(ContentMappingBase[IUser]):
    
    def map(self, ctx: ContentMappingContext[IUser]) -> ContentMappingContext[IUser]:
        domain = ctx.mapped_location.parent()
        return ctx.map_to(domain.append(ctx.content_item.name + "@salesforce.com"))

class ProjectTransformer(ContentTransformerBase[IProject]):
    
    def transform(self, item_to_transform: IProject) -> IProject:
        item_to_transform.description = "[From Server]\n" + item_to_transform.description
        return item_to_transform
    
# Hooks using functions.
    
def log_action(self, ctx: MigrationActionResult) -> MigrationActionResult:
    print("ACTION COMPLETED")
    return ctx

def filter_groups(self, item: ContentMigrationItem[IGroup]) -> bool:
    return "Test" not in item.source_item.name

def map_username(self, ctx: ContentMappingContext[IUser]) -> ContentMappingContext[IUser]:
    domain = ctx.mapped_location.parent()
    return ctx.map_to(domain.append(ctx.content_item.name + "@salesforce.com"))

def add_project_origin_desccription(self, item_to_transform: IProject) -> IProject:
    item_to_transform.description = "[From Server]\n" + item_to_transform.description
    return item_to_transform
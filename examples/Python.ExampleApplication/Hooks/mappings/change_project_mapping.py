from typing import TypeVar
from tableau_migration import(
    IWorkbook,
    IDataSource,
    ContentMappingContext,
    ContentMappingBase)

T = TypeVar("T")

class ChangeProjectMapping(ContentMappingBase[T]):
        
    def map(self, ctx: ContentMappingContext[T]) -> ContentMappingContext[T]:
        # Get the container (project) location for the content item.
        container_location = ctx.content_item.location.parent()

        # We only want to map content items whose project name is "Test".
        if not container_location.name.casefold() == "Test".casefold():
            return ctx
        
        # Build the new project location.
        new_container_location = container_location.rename("Production")
        
        # Build the new content item location.
        new_location = new_container_location.append(ctx.content_item.name)

        # Map the new content item location.
        ctx = ctx.map_to(new_location)
        
        return ctx


# Create the workbook version of the templated ChangeProjectMapping class
class ChangeProjectMappingForWorkbooks(ChangeProjectMapping[IWorkbook]):
    pass  

# Create the datasource version of the templated ChangeProjectMapping class
class ChangeProjectMappingForDataSources(ChangeProjectMapping[IDataSource]):
    pass
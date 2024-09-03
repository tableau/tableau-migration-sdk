from tableau_migration import(
    IProject,
    ContentMappingBase,
    ContentMappingContext)


class ProjectRenameMapping(ContentMappingBase[IProject]):
    def map(self, ctx: ContentMappingContext[IProject]) -> ContentMappingContext[IProject]:
        if not ctx.content_item.name.casefold() == "Test".casefold():
            return ctx
        
        new_location = ctx.content_item.location.rename("Production")
        
        ctx = ctx.map_to(new_location)
        
        return ctx
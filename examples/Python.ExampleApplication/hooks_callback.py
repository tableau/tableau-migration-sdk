from Tableau.Migration.Content import IGroup
from Tableau.Migration.Engine import ContentMigrationItem

def log_callback(ctx):
    print("ACTION COMPLETED")
    return ctx

def filter_group(ctx):
    filtered = List[ContentMigrationItem[IGroup]]()
    for item in ctx:
        if "Test" not in item.SourceItem.Name:
            filtered.add(item)
    return filtered

def map_test_user(ctx):
    domain = ctx.MappedLocation.Parent()
    return ctx.MapTo(domain.Append(ctx.ContentItem.Name + "@salesforce.com"))

def add_project_origin_desc(ctx):
    ctx.Description = "[From Server]\n" + ctx.Description
    return ctx
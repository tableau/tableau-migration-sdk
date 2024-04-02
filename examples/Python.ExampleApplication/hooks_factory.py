from System.Collections.Generic import List
from Tableau.Migration.Content import IGroup, IProject, IUser
from Tableau.Migration.Engine import ContentMigrationItem
from Tableau.Migration.Engine.Actions import IMigrationActionResult
from Tableau.Migration.Engine.Hooks import IMigrationActionCompletedHook
from Tableau.Migration.Interop.Hooks import ISyncMigrationHook
from Tableau.Migration.Interop.Hooks.Filters import ISyncContentFilter
from Tableau.Migration.Interop.Hooks.Mappings import ISyncContentMapping
from Tableau.Migration.Interop.Hooks.Transformers import ISyncContentTransformer

class PyLogActionHook(ISyncMigrationHook[IMigrationActionResult], IMigrationActionCompletedHook):
    __namespace__ = "MyNamespace"
    
    def Execute(self, ctx):
        print("ACTION COMPLETED")
        return ctx

class PyTestGroupFilter(ISyncContentFilter[IGroup]):
    __namespace__ = "MyNamespace"
    
    def Execute(self, ctx):
        filtered = List[ContentMigrationItem[IGroup]]()
        for item in ctx:
            if "Test" not in item.SourceItem.Name:
                filtered.add(item)
        return filtered

class PyTestUsernameMapping(ISyncContentMapping[IUser]):
    __namespace__ = "MyNamespace"

    def Execute(self, ctx):
        domain = ctx.MappedLocation.Parent()
        return ctx.MapTo(domain.Append(ctx.ContentItem.Name + "@salesforce.com"))

class PyTestProjectTransformer(ISyncContentTransformer[IProject]):
    __namespace__ = "MyNamespace"
    
    def Execute(self, ctx):
        ctx.Description = "[From Server]\n" + ctx.Description
        return ctx
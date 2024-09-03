# Updating Python Hooks from v3 to v4+

Python hooks received a major update in version 4 of the Tableau Migration SDK. In version 3, all Python hooks were thin wrappers around C# code, and the actual object manipulated by the hook was a C# object. In version 4 and beyond, everything is still a wrapper around C#, but the class itself and the context object passed to be manipulated are now fully in Python with no more C# required. This change brings full autocomplete support in your favorite Python IDE.

This is a breaking change and will require an update to existing v3 hooks. On this page, we'll go over how to update your v3 hooks to v4+.

See [Custom Hooks](~/articles/hooks/custom_hooks.md) to learn how to build v4 hooks.

## Filters

There are a few changes that must be made to existing filters for them to work in version 4+.

The imports have changed. All imports are now from the Python `tableau_migration` namespaces instead of the C# `Tableau.Migration` namespaces.

The `__namespace__` and `_dotnet_base` variables are no longer required.

The `ShouldMigration` function is now `should_migrate`.

For registration, the type is no longer required. Simply add the object to the filter list.

### Version 3 -> Version 4+ Filter Class Diff
```diff
-from Tableau.Migration.Content import IUser
-from Tableau.Migration.Engine.Hooks.Filters import ContentFilterBase
+from tableau_migration import (
+    ContentFilterBase,
+    ContentMigrationItem,
+    IUser
+)


class FilterBob(ContentFilterBase[IUser]):
    """A class to filter out all users named Bob."""

-    __namespace__ = "MyNamespace"
-    _dotnet_base = ContentFilterBase[IUser]
    
    def __init__(self):
        """Default init to set up logging."""
        self._logger = logging.getLogger(self.__class__.__name__)
        self._logger.setLevel(logging.DEBUG)

-    def ShouldMigrate(self,item):
+    def should_migrate(self, item: ContentMigrationItem[IUser]) -> bool:
        """Implements ShouldMigrate from base."""
-        if item.SourceItem.Name.casefold() == "Bob".casefold():
+        if item.source_item.name.casefold() == "Bob".casefold():
            self._logger.debug('%s filtered Bob', self.__class__.__name__)
            return False
        
        return True
```

### Version 3 -> Version 4+ Registration Diff

```diff
-   plan_builder.filters.add(IUser, FilterBob)
+   plan_builder.filters.add(FilterBob)
```

## Mappings

There are a few changes that must be made to existing mappings for them to work in version 4+.

The imports have changed. All imports are now from the Python `tableau_migration` namespaces instead of the C# `Tableau.Migration` namespaces.

The `__namespace__` and `_dotnet_base` variables are no longer required.

The `Execute` function is now `map` with the parameter being of type `ContentMappingContext[T]`.

For registration, the type is no longer required. Simply add the object to the mapping list.

### Version 3 -> Version 4+ Mapping Class Diff
```diff
-from Tableau.Migration.Interop.Hooks.Mappings import ISyncContentMapping
-from Tableau.Migration.Content import IUser
-from Tableau.Migration import ContentLocation
+from tableau_migration import (
+    ContentLocation,
+    ContentMappingBase,
+    ContentMappingContext,
+    IUser
+)


-class SpecialUserMapping(ISyncContentMapping[IUser]):
+class SpecialUserMapping(ContentMappingBase[IUser]):
    """A class to map users to server admin."""

-    __namespace__ = "MyNamespace"
-    _dotnet_base = ISyncContentMapping[IUser]
-    _admin_username = ContentLocation.ForUsername("domain", "admin")
+    _admin_username = ContentLocation.for_username("domain", "admin")
        
    def __init__(self):
        """Default init to set up logging."""
        self._logger = logging.getLogger(self.__class__.__name__)
        self._logger.setLevel(logging.DEBUG)
    
-    def Execute(self,ctx):  # noqa: N802
+    def map(self, ctx: ContentMappingContext[IUser]) -> ContentMappingContext[IUser]:
-       if ctx.ContentItem.Email.casefold() == "bob@company.com".casefold():
+       if ctx.content_item.email.casefold() == "bob@company.com".casefold():
-            ctx=ctx.MapTo(self._admin_username)
+            ctx = ctx.map_to(self._admin_username)
-            self._logger.debug('Mapped %s to %s', ctx.ContentItem.Email, ctx.MappedLocation.ToString())
+            self._logger.debug('Mapped %s to %s', ctx.content_item.email, str(ctx.mapped_location))
        return ctx
```

### Version 3 -> Version 4+ Registration Diff

```diff
-   plan_builder.mappings.add(IUser, SpecialUserMapping())
+   plan_builder.mappings.add(SpecialUserMapping())
```

## Transformers

There are a few changes that must be made to existing transformers for them to work in version 4+.

The imports have changed. All imports are now from the Python `tableau_migration` namespaces instead of the C# `Tableau.Migration` namespaces.

The `__namespace__` and `_dotnet_base` variables are no longer required.

The `Execute` function is now `transform` with the parameter being of the publishable type to transform.

For registration, the type is no longer required. Simply add the object to the transformer list.

### Version 3 -> Version 4+ Transformer Class Diff

```diff
-from Tableau.Migration.Interop.Hooks.Transformers import ISyncContentTransformer
-from Tableau.Migration.Content import IPublishableDataSource
+from typing import TypeVar
+from tableau_migration import (
+    ContentTransformerBase,
+    IPublishableWorkbook,
+    IPublishableDataSource)

-class EncryptExtractsDataSourceTransformer(ISyncContentTransformer[IPublishableDataSource]):
+class EncryptExtractsDataSourceTransformer(ContentTransformerBase[IPublishableDataSource]):
-    __namespace__ = "MyNamespace"
    
-    def Execute(self, ctx):
+    def transform(self, itemToTransform: IPublishableDataSource) -> IPublishableDataSource:
-       ctx.EncryptExtracts = True
+       itemToTransform.encrypt_extracts = True
-       return ctx
+       return itemToTransform
   
```

### Version 3 -> Version 4+ Registration Diff

```diff
-   plan_builder.transformers.add(IPublishableDataSource, EncryptExtractsDataSourceTransformer())
+   plan_builder.transformers.add(EncryptExtractsDataSourceTransformer())
```
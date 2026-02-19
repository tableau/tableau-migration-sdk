from typing import TypeVar
from tableau_migration import (
    ContentTransformerBase,
    IPublishableDataSource,
    IPublishableWorkbook)
from tableau_migration.migration_content import PyTag

T = TypeVar("T")


class MigratedTagTransformer(ContentTransformerBase[T]):
    def transform(self, itemToTransform: T) -> T:
        tag: str = "Migrated"
        
        new_tag = PyTag.create(tag)
        
        current_tags = list(itemToTransform.tags)
        current_tags.append(new_tag)
        
        itemToTransform.tags = current_tags
        
        return itemToTransform
    
class MigratedTagTransformerForDataSources(MigratedTagTransformer[IPublishableDataSource]):
    pass

class MigratedTagTransformerForWorkbooks(MigratedTagTransformer[IPublishableWorkbook]):
    pass
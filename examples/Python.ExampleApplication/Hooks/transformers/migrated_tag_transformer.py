from typing import TypeVar
from tableau_migration import (
    ContentTransformerBase,
    IDataSource,
    ITag,
    IWorkbook)

T = TypeVar("T")


class MigratedTagTransformer(ContentTransformerBase[T]):
    def transform(self, itemToTransform: T) -> T:
        tag: str = "Migrated"
        
        itemToTransform.tags.append(ITag(tag))
        
        return itemToTransform
    
class MigratedTagTransformerForDataSources(MigratedTagTransformer[IDataSource]):
    pass

class MigratedTagTransformerForWorkbooks(MigratedTagTransformer[IWorkbook]):
    pass
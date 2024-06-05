from typing import TypeVar
from tableau_migration import (
    ContentTransformerBase,
    IPublishableWorkbook,
    IPublishableDataSource)

T = TypeVar("T")

class EncryptExtractTransformer(ContentTransformerBase[T]):
    def transform(self, itemToTransform: T) -> T:
        itemToTransform.encrypt_extracts = True
        
        return itemToTransform
    
class EncryptExtractTransformerForDataSources(EncryptExtractTransformer[IPublishableDataSource]):
    pass

class EncryptExtractTransformerForWorkbooks(EncryptExtractTransformer[IPublishableWorkbook]):
    pass
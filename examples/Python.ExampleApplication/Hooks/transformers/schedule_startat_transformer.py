from datetime import time
from tableau_migration import (
    ContentTransformerBase,
    ICloudExtractRefreshTask
)

class SimpleScheduleStartAtTransformer(ContentTransformerBase[ICloudExtractRefreshTask]):
    def transform(self, itemToTransform: ICloudExtractRefreshTask) -> ICloudExtractRefreshTask:
        # In this example, the `Start At` time is in the UTC time zone.
        if itemToTransform.schedule.frequency_details.start_at:
            prev_start_at = itemToTransform.schedule.frequency_details.start_at
            # A simple conversion to the EDT time zone.
            itemToTransform.schedule.frequency_details.start_at = time(prev_start_at.hour - 4, prev_start_at.minute, prev_start_at.second, prev_start_at.microsecond);
        
        return itemToTransform
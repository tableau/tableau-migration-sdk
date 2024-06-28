using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Content.Schedules;
using Tableau.Migration.Content.Schedules.Cloud;
using Tableau.Migration.Engine.Hooks.Transformers;
using Tableau.Migration.Resources;

namespace Csharp.ExampleApplication.Hooks.Transformers
{
    #region class
    public class SimpleScheduleStartAtTransformer<T> 
        : ContentTransformerBase<T> 
        where T : IWithSchedule<ICloudSchedule>
    {
        private readonly ILogger<IContentTransformer<T>>? _logger;

        public SimpleScheduleStartAtTransformer(
            ISharedResourcesLocalizer localizer, 
            ILogger<IContentTransformer<T>> logger) 
            : base(
                  localizer, 
                  logger)
        {
            _logger = logger;
        }

        public override async Task<T?> TransformAsync(
            T itemToTransform, 
            CancellationToken cancel)
        {
            // In this example, the `Start At` time is in the UTC time zone.
            if (itemToTransform.Schedule.FrequencyDetails.StartAt is not null)
            {
                // A simple conversion to the EDT time zone.
                var updatedStartAt = itemToTransform.Schedule.FrequencyDetails.StartAt.Value.AddHours(-4);

                _logger?.LogInformation(
                    @"Adjusting the 'Start At' from {previousStartAt} to {updatedStartAt}.",
                    itemToTransform.Schedule.FrequencyDetails.StartAt.Value,
                    updatedStartAt);

                itemToTransform.Schedule.FrequencyDetails.StartAt = updatedStartAt;
            }

            return await Task.FromResult(itemToTransform);
        }
    }
    #endregion
}

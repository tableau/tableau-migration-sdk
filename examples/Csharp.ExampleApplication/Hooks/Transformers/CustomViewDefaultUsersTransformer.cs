using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks.Transformers;
using Tableau.Migration.Resources;

namespace Csharp.ExampleApplication.Hooks.Transformers
{
    #region class
    public class CustomViewExcludeDefaultUserTransformer(
        ISharedResourcesLocalizer localizer,
        ILogger<CustomViewExcludeDefaultUserTransformer> logger)
        : ContentTransformerBase<IPublishableCustomView>(localizer, logger)
    {
        public IList<string> ExcludeUsernames { get; } = new List<string>() {"User1", "User2"};
            

        private readonly ILogger<CustomViewExcludeDefaultUserTransformer>? _logger = logger;

        public override async Task<IPublishableCustomView?> TransformAsync(IPublishableCustomView itemToTransform, CancellationToken cancel)
        {
            var newDefaultUsers = itemToTransform.DefaultUsers.Where(user => !ExcludeUsernames.Contains(user.Name)).ToList();

            itemToTransform.DefaultUsers = newDefaultUsers;

            _logger?.LogInformation(
                @"Excluding default users {newDefaultUsers}",
                newDefaultUsers);

            return await Task.FromResult(itemToTransform);
        }
    }
    #endregion
}
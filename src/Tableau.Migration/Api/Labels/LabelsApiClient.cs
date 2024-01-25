using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Api.Labels
{
    ///<inheritdoc/>
    internal class LabelsApiClient<TContent> : ApiClientBase, ILabelsApiClient<TContent>
        where TContent : IContentReference, IWithLabels
    {
        private readonly string _contentItemType;
        public LabelsApiClient(
            IRestRequestBuilderFactory restRequestBuilderFactory,
            ILoggerFactory loggerFactory,
            ISharedResourcesLocalizer sharedResourcesLocalizer)
            : base(restRequestBuilderFactory, loggerFactory, sharedResourcesLocalizer)
        {
            _contentItemType = LabelContentTypes.FromContentType(typeof(TContent));
        }

        ///<inheritdoc/>
        public async Task<IResult<ImmutableList<ILabel>>> GetLabelsAsync(
            Guid contentItemId,
            CancellationToken cancel,
            IEnumerable<string>? categories = null)
        {
            var requestUrl = categories == null || !categories.Any()
                ? RestRequestBuilderFactory.CreateUri($"labels")
                : RestRequestBuilderFactory
                    .CreateUri($"labels")
                    .WithQuery("categories", GetCategoryListString(categories));

            var getResult = await requestUrl
                .ForPostRequest()
                .WithXmlContent(new GetLabelsRequest(contentItemId, _contentItemType))
                .SendAsync<LabelsResponse>(cancel)
                .ToResultAsync(
                (response) =>
                {
                    return response.Items.Select(i => (ILabel)new Label(i)).ToImmutableList();
                },
                SharedResourcesLocalizer)
                .ConfigureAwait(false);

            return getResult;
        }

        /// <summary>
        /// Generate a comma separated list of categories.
        /// </summary>
        /// <param name="categories"></param>
        /// <returns></returns>
        internal static string GetCategoryListString(IEnumerable<string> categories)
        {
            if (!categories.Any())
                return string.Empty;

            StringBuilder categoryList = new();

            foreach (var category in categories)
            {
                categoryList.Append(category);
                categoryList.Append(',');
            }
            // Trim trailing comma.
            categoryList.Remove(categoryList.Length - 1, 1);

            return categoryList.ToString();
        }

        ///<inheritdoc/>
        public async Task<IResult<ImmutableList<ILabel>>> UpdateLabelsAsync(
            Guid contentItemId,
            IEnumerable<ILabelUpdateOptions> labels,
            CancellationToken cancel)
        {
            var updateTasks = new List<Task<IResult<IEnumerable<ILabel>>>>();

            foreach (var label in labels)
            {
                var updateTask = UpdateLabelAsync(
                    contentItemId,
                    _contentItemType,
                    label,
                    cancel);

                updateTasks.Add(updateTask);
            }

            var updateResult = await Task.WhenAll(updateTasks).ConfigureAwait(false);

            if (updateResult == null)
            {
                return Result<ImmutableList<ILabel>>.Failed(
                    new Exception($"Failed to create/update one or more labels for {contentItemId}."));
            }

            if (!updateResult.All(r => r.Success))
            {
                return Result<ImmutableList<ILabel>>.Failed(
                    updateResult.SelectMany(r => r.Errors).ToArray());
            }


            var updatedLabels = new List<ILabel>();
            foreach (var val in updateResult)
            {
                if (val.Value == null)
                    continue;

                updatedLabels.AddRange(val.Value);
            }

            return Result<ImmutableList<ILabel>>.Succeeded(updatedLabels.Distinct().ToImmutableList());
        }

        ///<inheritdoc/>
        public async Task<IResult<IEnumerable<ILabel>>> UpdateLabelAsync(
            Guid contentItemId,
            string contentItemType,
            ILabelUpdateOptions label,
            CancellationToken cancel)
        {
            var updateResult = await RestRequestBuilderFactory
              .CreateUri($"labels")
              .ForPutRequest()
              .WithXmlContent(new UpdateLabelsRequest(contentItemId, contentItemType, label))
              .SendAsync<LabelsResponse>(cancel)
              .ToResultAsync(
                (response)
                => response.Items.Select(i => (ILabel)new Label(i)),
                SharedResourcesLocalizer)
              .ConfigureAwait(false);

            return updateResult;
        }
    }
}

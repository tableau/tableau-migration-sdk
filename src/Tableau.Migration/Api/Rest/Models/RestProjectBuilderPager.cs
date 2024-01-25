using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Paging;

namespace Tableau.Migration.Api.Rest.Models
{
    /// <summary>
    /// <see cref="IPager{IProject}"/> implementation that can build
    /// <see cref="IProject"/> objects by loading and evaluating the project hierarchy.
    /// </summary>
    internal sealed class RestProjectBuilderPager : IndexedPagerBase<IProject>
    {
        private readonly IProjectsResponseApiClient _projectsResponseClient;
        private readonly IContentReferenceFinder<IUser> _userFinder;

        private IResult<RestProjectBuilder>? _projectBuilderResult;

        public RestProjectBuilderPager(IProjectsResponseApiClient projectsResponseClient, IContentReferenceFinder<IUser> userFinder,
            int pageSize)
            : base(pageSize)
        {
            _projectsResponseClient = projectsResponseClient;
            _userFinder = userFinder;
        }

        protected override async Task<IPagedResult<IProject>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancel)
        {
            if (_projectBuilderResult is null)
            {
                var rawPager = _projectsResponseClient.GetPager(pageSize);
                _projectBuilderResult = await RestProjectBuilder.LoadFromPagerAsync(rawPager, cancel).ConfigureAwait(false);
            }

            if (!_projectBuilderResult.Success)
            {
                return PagedResult<IProject>.Failed(_projectBuilderResult.Errors);
            }

            var projectBuilder = _projectBuilderResult.Value;
            var restProjects = projectBuilder.RestProjects.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            var projects = ImmutableArray.CreateBuilder<IProject>(pageSize);
            foreach (var restProject in restProjects)
            {
                var newProject = await projectBuilder.BuildProjectAsync(restProject, _userFinder, cancel)
                    .ConfigureAwait(false);

                projects.Add(newProject);
            }

            return PagedResult<IProject>.Succeeded(projects.ToImmutable(), pageNumber, pageSize, projectBuilder.Count);
        }
    }
}

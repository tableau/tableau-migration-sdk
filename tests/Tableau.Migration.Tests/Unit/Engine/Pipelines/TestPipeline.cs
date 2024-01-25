using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Tableau.Migration.Engine.Actions;
using Tableau.Migration.Engine.Pipelines;

namespace Tableau.Migration.Tests.Unit.Engine.Pipelines
{
    public class TestPipeline : MigrationPipelineBase
    {
        public int BuildPipelineCalls { get; private set; }

        public ImmutableArray<TestAction> TestPipelineActions { get; private set; }

        public TestPipeline(IServiceProvider services)
            : base(services)
        {
            TestPipelineActions = new[]
            {
                    base.CreateAction<TestAction>(),
                    base.CreateAction<TestAction>(),
                    base.CreateAction<TestAction>()
            }.ToImmutableArray();
        }

        new public TAction CreateAction<TAction>()
            where TAction : IMigrationAction
            => base.CreateAction<TAction>();

        new public IMigrateContentAction<TContent> CreateMigrateContentAction<TContent>()
            where TContent : class, IContentReference
            => base.CreateMigrateContentAction<TContent>();

        protected override IEnumerable<IMigrationAction> BuildPipeline()
        {
            BuildPipelineCalls++;

            return TestPipelineActions;
        }
    }
}

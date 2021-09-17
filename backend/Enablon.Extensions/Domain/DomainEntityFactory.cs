using System.Threading.Tasks;
using Enablon.Extensions.Domain.WorkItemAggregate;
using Squidex.Domain.Apps.Entities.Contents;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Commands;

namespace Enablon.Extensions.Domain
{
    internal class DomainEntityFactory
    {
        private readonly IContentLoader contentLoader;
        private readonly ICommandBus commandBus;
        private readonly DomainContext context;

        public DomainEntityFactory(IContentLoader contentLoader, ICommandBus commandBus, DomainContext context)
        {
            this.contentLoader = contentLoader;
            this.commandBus = commandBus;
            this.context = context;
        }

        public async Task<WorkItem?> FindWorkItem(DomainId id, long expectedVersion)
        {
            var previousState = await contentLoader.GetAsync(context.Tenant.Id, id, expectedVersion);

            return previousState != null ? new WorkItem(commandBus, context, previousState) : null;
        }

        public WorkItem BuildWorkItemFrom(IContentEntity contentEntity)
        {
            return new WorkItem(commandBus, context, contentEntity);
        }

        public async Task<RiskAssessmentPart?> FindRiskAssessmentPart(DomainId id)
        {
            IContentEntity? content = await contentLoader.GetAsync(context.Tenant.Id, id);

            return content != null ? new RiskAssessmentPart(commandBus, context, content) : null;
        }
    }
}

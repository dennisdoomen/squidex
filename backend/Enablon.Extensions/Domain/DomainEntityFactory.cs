using System;
using System.Threading.Tasks;
using Enablon.Extensions.Domain.WorkItemAggregate;
using Squidex.Domain.Apps.Entities.Contents;
using Squidex.Domain.Apps.Entities.Contents.Commands;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Commands;

namespace Enablon.Extensions.Domain
{
    internal class DomainEntityFactory
    {
        private readonly IContentLoader contentLoader;
        private readonly ICommandBus commandBus;
        private readonly DomainContext context;

        public DomainEntityFactory(IContentLoader contentLoader, CommandContext commandContext)
        {
            this.contentLoader = contentLoader;
            commandBus = commandContext.CommandBus;

            if (commandContext.Command is ContentCommand contentCommand)
            {
                context = new DomainContext
                {
                    Identity = contentCommand.Actor,
                    Principal = contentCommand.User,
                    Tenant = contentCommand.AppId
                };
            }
            else
            {
                throw new ArgumentException("This constructor only works for content commands");
            }
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

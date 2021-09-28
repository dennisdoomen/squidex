using System.Security.Claims;
using System.Threading.Tasks;
using Enablon.Extensions.Domain.WorkItemAggregate;
using Squidex.Domain.Apps.Entities.Contents;
using Squidex.Domain.Apps.Entities.Contents.Commands;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Commands;

namespace Enablon.Extensions.Domain
{
    internal class EntityRepository
    {
        private readonly IContentLoader contentLoader;
        private readonly ICommandBus commandBus;

        public EntityRepository(IContentLoader contentLoader, ICommandBus commandBus)
        {
            this.contentLoader = contentLoader;
            this.commandBus = commandBus;
        }

        public async Task<WorkItem?> FindWorkItem(DomainId tenantId, DomainId id, long? expectedVersion = null)
        {
            var entity = await contentLoader.GetAsync(tenantId, id, expectedVersion ?? EtagVersion.Any);

            return entity != null ? new WorkItem(entity.Id, entity.Version, entity.Data) : null;
        }
        
        public async Task<RiskAssessmentPart?> FindRiskAssessmentPart(DomainId tenantId, DomainId id)
        {
            IContentEntity? entity = await contentLoader.GetAsync(tenantId, id);

            return entity != null ? new RiskAssessmentPart(entity.AppId, entity.SchemaId, entity.Id, entity.Data, entity.Version) : null;
        }

        public Task Save(RiskAssessmentPart riskAssessmentPart, RefToken identity, ClaimsPrincipal? user)
        {
            return commandBus.PublishAsync(new UpdateContent
            {
                // TODO: is this required?
                Actor = identity,
                // TODO: is this required?
                User = user,
                AppId = riskAssessmentPart.Tenant,
                SchemaId = riskAssessmentPart.Schema,
                ContentId = riskAssessmentPart.Id,
                Data = riskAssessmentPart.Data,
                ExpectedVersion = riskAssessmentPart.Version,
                DoNotScript = true,
                DoNotValidate = true
            });
        }
    }
}

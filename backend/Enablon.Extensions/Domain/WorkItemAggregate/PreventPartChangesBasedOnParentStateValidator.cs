using System.Threading.Tasks;
using Squidex.Domain.Apps.Core.Contents;
using Squidex.Domain.Apps.Core.ValidateContent;
using Squidex.Domain.Apps.Entities.Contents;
using Squidex.Infrastructure.Commands;

namespace Enablon.Extensions.Domain.WorkItemAggregate
{
    public class PreventPartChangesBasedOnParentStateValidator : IValidator
    {
        private readonly IContentLoader contentLoader;
        private readonly ICommandBus commandBus;

        public PreventPartChangesBasedOnParentStateValidator(IContentLoader contentLoader, ICommandBus commandBus)
        {
            this.contentLoader = contentLoader;
            this.commandBus = commandBus;
        }

        public async Task ValidateAsync(object? value, ValidationContext context, AddError addError)
        {
            if (value is ContentData data && RiskAssessmentPart.AppliesTo(context.SchemaId))
            {
                var part = new RiskAssessmentPart(context.AppId, context.SchemaId, context.ContentId, data);
                if (part.Owner != null)
                {
                    var repository = new EntityRepository(contentLoader, commandBus);
                    var owner = await repository.FindWorkItem(context.AppId.Id, part.Owner.Value);
                    if (owner == null)
                    {
                        addError(context.Path, "Risk assessment part no longer has an owner");
                    }
                    else
                    {
                        if (owner.State == "Active")
                        {
                            addError(context.Path,
                                "You can't change the risk assessment part if the work item is active");
                        }
                    }
                }
            }
        }
    }
}

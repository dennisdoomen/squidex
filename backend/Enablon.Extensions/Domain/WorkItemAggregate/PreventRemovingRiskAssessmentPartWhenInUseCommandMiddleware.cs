using System.Threading.Tasks;
using Squidex.Domain.Apps.Core.Contents;
using Squidex.Domain.Apps.Entities.Contents;
using Squidex.Domain.Apps.Entities.Contents.Commands;
using Squidex.Infrastructure.Commands;
using Squidex.Infrastructure.Validation;

namespace Enablon.Extensions.Domain.WorkItemAggregate
{
    public class PreventRemovingRiskAssessmentPartWhenInUseCommandMiddleware : ICustomCommandMiddleware
    {
        private readonly IContentLoader contentLoader;

        public PreventRemovingRiskAssessmentPartWhenInUseCommandMiddleware(IContentLoader contentLoader)
        {
            this.contentLoader = contentLoader;
        }

        public async Task HandleAsync(CommandContext context, NextDelegate next)
        {
            if (context.Command is ContentCommand command && (IsDeleteRequest(command) || IsArchiveRequest(command)))
            {
                var factory = new EntityRepository(contentLoader, context.CommandBus);

                var part = await factory.FindRiskAssessmentPart(command.AppId.Id, command.ContentId);
                if (part?.Owner != null)
                {
                    throw new ValidationException($"You cannot delete this item. It is part of the work item {part.Owner}");
                }
            }
            
            await next(context);
        }

        private static bool IsArchiveRequest(ContentCommand command)
        {
            return
                command is ChangeContentStatus changeContentStatus
                && RiskAssessmentPart.AppliesTo(changeContentStatus.SchemaId)
                && changeContentStatus.Status == Status.Archived;
        }

        private static bool IsDeleteRequest(ContentCommand command)
        {
            return command is DeleteContent && RiskAssessmentPart.AppliesTo(command.SchemaId);
        }
    }
}

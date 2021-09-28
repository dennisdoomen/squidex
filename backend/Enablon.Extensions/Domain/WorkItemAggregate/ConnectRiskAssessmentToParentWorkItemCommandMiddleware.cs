using System.Threading.Tasks;
using Squidex.Domain.Apps.Entities.Contents;
using Squidex.Domain.Apps.Entities.Contents.Commands;
using Squidex.Infrastructure.Commands;

namespace Enablon.Extensions.Domain.WorkItemAggregate
{
    /// <summary>
    /// Ensures that the underlying content that we use to represent <see cref="WorkItem"/> and <see cref="RiskAssessmentPart"/>
    /// have bidirectional references.  
    /// </summary>
    /// <summary>
    /// This is used so that this class can easily use the state of the work item to validate certain business rules. 
    /// </summary>
    internal class ConnectRiskAssessmentToParentWorkItemCommandMiddleware : ICustomCommandMiddleware
    {
        private readonly IContentLoader contentLoader;

        public ConnectRiskAssessmentToParentWorkItemCommandMiddleware(IContentLoader contentLoader)
        {
            this.contentLoader = contentLoader;
        }

        public async Task HandleAsync(CommandContext context, NextDelegate next)
        {
            // Wait until the entire pipeline has processed the command
            await next(context);

            if (context.IsCompleted &&
                context.Command is ContentDataCommand updateRequest &&
                WorkItem.AppliesTo(updateRequest))
            {
                await HandleWorkItemChange(context, updateRequest);
            }
        }

        private async Task HandleWorkItemChange(CommandContext context, ContentDataCommand updateRequest)
        {
            var repository = new EntityRepository(contentLoader, context.CommandBus);

            var entity = context.Result<IContentEntity>();
            var newState = new WorkItem(entity.Id, entity.Version, entity.Data);

            WorkItem? previousState = null;
            if (newState.Version > 1)
            {
                previousState = await repository.FindWorkItem(entity.AppId.Id,
                    updateRequest.ContentId,
                    updateRequest.ExpectedVersion);
            }

            if (previousState == null || newState.RiskAssessmentPart != previousState.RiskAssessmentPart)
            {
                if (previousState?.RiskAssessmentPart != null)
                {
                    var oldPart = await repository.FindRiskAssessmentPart(entity.AppId.Id,
                        previousState.RiskAssessmentPart.Value);

                    if (oldPart != null)
                    {
                        oldPart.Owner = null;
                        await repository.Save(oldPart, updateRequest.Actor, updateRequest.User);
                    }
                }

                if (newState.RiskAssessmentPart != null)
                {
                    var newPart = await repository.FindRiskAssessmentPart(entity.AppId.Id, 
                        newState.RiskAssessmentPart.Value);

                    if (newPart != null)
                    {
                        newPart.Owner = newState.Id;
                        await repository.Save(newPart, updateRequest.Actor, updateRequest.User);
                    }
                }
            }
        }
    }
}

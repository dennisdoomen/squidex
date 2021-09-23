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
            var factory = new DomainEntityFactory(contentLoader, context.CommandBus, new DomainContext
            {
                Identity = updateRequest.Actor,
                Principal = updateRequest.User,
                Tenant = updateRequest.AppId
            });

            var newState = factory.BuildWorkItemFrom(context.Result<IContentEntity>());

            WorkItem? previousState = null;
            if (newState.Version > 1)
            {
                previousState = await factory.FindWorkItem(
                    updateRequest.ContentId,
                    updateRequest.ExpectedVersion);
            }

            if (previousState == null || newState.RiskAssessmentPart != previousState.RiskAssessmentPart)
            {
                if (previousState?.RiskAssessmentPart != null)
                {
                    var oldPart = await factory.FindRiskAssessmentPart(
                        previousState.RiskAssessmentPart.Value);

                    if (oldPart != null)
                    {
                        oldPart.Owner = null;
                        await oldPart.Save();
                    }
                }

                if (newState.RiskAssessmentPart != null)
                {
                    var newPart = await factory.FindRiskAssessmentPart(
                        newState.RiskAssessmentPart.Value);

                    if (newPart != null)
                    {
                        newPart.Owner = newState.Id;
                        await newPart.Save();
                    }
                }
            }
        }
    }
}

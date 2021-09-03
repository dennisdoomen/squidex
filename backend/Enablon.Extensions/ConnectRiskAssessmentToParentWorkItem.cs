using System;
using System.Linq;
using System.Threading.Tasks;
using Squidex.Domain.Apps.Core.Contents;
using Squidex.Domain.Apps.Entities.Contents;
using Squidex.Domain.Apps.Entities.Contents.Commands;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Commands;
using Squidex.Infrastructure.Json.Objects;
using Squidex.Infrastructure.Validation;

namespace Enablon.Extensions
{
    internal class ConnectRiskAssessmentToParentWorkItem : ICustomCommandMiddleware
    {
        private readonly IContentLoader contentLoader;

        public ConnectRiskAssessmentToParentWorkItem(IContentLoader contentLoader)
        {
            this.contentLoader = contentLoader;
        }

        public async Task HandleAsync(CommandContext context, NextDelegate next)
        {
            await next(context);
            
            if (context.Command is UpdateContent updateContent && context.IsCompleted &&
                updateContent.SchemaId.Name == "workitem")
            {
                var previousState = await contentLoader.GetAsync(updateContent.AppId.Id, updateContent.ContentId,
                    updateContent.ExpectedVersion);

                string? oldReferenceId = null;
                if (previousState != null && previousState.Data.TryGetValue("riskAssessment", out ContentFieldData oldFieldData))
                {
                    oldReferenceId = oldFieldData.Values.OfType<JsonArray>().SelectMany(x => x).SingleOrDefault()?.ToString();
                }

                string? newReferenceId = null;
                if (updateContent.Data.TryGetValue("riskAssessment", out ContentFieldData? newFieldData))
                {
                    var referencedIds = newFieldData.Values.OfType<JsonArray>().SelectMany(x => x).ToArray();
                    if (referencedIds.Length > 1)
                    {
                        throw new ValidationException(
                            "Did not expect the work item to contain more than one risk assessment part");
                    }

                    newReferenceId = referencedIds.SingleOrDefault()?.ToString();
                }

                if (newReferenceId != oldReferenceId)
                {
                    if (oldReferenceId != null)
                    {
                        IContentEntity? oldRiskAssessment =
                            await contentLoader.GetAsync(updateContent.AppId.Id, DomainId.Create(oldReferenceId));
                        
                        if (oldRiskAssessment != null)
                        {
                            oldRiskAssessment.Data.Remove("Owner");

                            await context.CommandBus.PublishAsync(new UpdateContent
                            {
                                Actor  = updateContent.Actor,
                                User = updateContent.User,
                                AppId = oldRiskAssessment.AppId,
                                SchemaId = oldRiskAssessment.SchemaId,
                                ContentId = oldRiskAssessment.Id,
                                Data = oldRiskAssessment.Data,
                                ExpectedVersion = oldRiskAssessment.Version,
                            });
                        }
                    }
                    
                    if (newReferenceId != null)
                    {
                        IContentEntity? newRiskAssessment = await contentLoader.GetAsync(updateContent.AppId.Id, DomainId.Create(newReferenceId));
                        if (newRiskAssessment != null)
                        {
                            var array = new JsonArray();
                            array.Add(JsonValue.Create(updateContent.ContentId));
                        
                            newRiskAssessment.Data.AddField("Owner",
                                new ContentFieldData().AddInvariant(array));

                            await context.CommandBus.PublishAsync(new UpdateContent
                            {
                                Actor  = updateContent.Actor,
                                User = updateContent.User,
                                AppId = newRiskAssessment.AppId,
                                SchemaId = newRiskAssessment.SchemaId,
                                ContentId = newRiskAssessment.Id,
                                Data = newRiskAssessment.Data,
                                ExpectedVersion = newRiskAssessment.Version
                            });
                        }
                    }
                }
            }
        }
    }
}

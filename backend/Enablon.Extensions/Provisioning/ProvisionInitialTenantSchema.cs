using System;
using System.Threading.Tasks;
using Squidex.Domain.Apps.Core.Schemas;
using Squidex.Domain.Apps.Entities;
using Squidex.Domain.Apps.Entities.Apps.Commands;
using Squidex.Domain.Apps.Entities.Apps.Templates.Builders;
using Squidex.Domain.Apps.Entities.Schemas.Commands;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Collections;
using Squidex.Infrastructure.Commands;

namespace Enablon.Extensions.Provisioning
{
    public class ProvisionInitialTenantSchema : ICustomCommandMiddleware
    {
        public async Task HandleAsync(CommandContext context, NextDelegate next)
        {
            await next(context);

            if (context.Command is CreateApp createApp && context.IsCompleted)
            {
                var appId = NamedId.Of(createApp.AppId, createApp.Name);

                var publish = new Func<IAppCommand, Task>(command =>
                {
                    command.AppId = appId;

                    return context.CommandBus.PublishAsync(command);
                });

                var riskAssessmentPartSchema =
                    SchemaBuilder.Create("RiskAssessmentPart")
                        .AddString("Level", f => f
                            .Label("Level")
                            .AsDropDown("1", "2")
                            .Required())
                        .Build();

                var workItemSchema =
                    SchemaBuilder.Create("WorkItem")
                        .AddString("RegistrationNumber", f => f
                            .Label("Registration Number")
                            .Unique()
                            .Length(100)
                            .Required())
                        .AddString("Kind", f => f
                            .Label("Kind")
                            .Length(100)
                            .Required())
                        .AddString("Variant", f => f
                            .Label("Variant")
                            .Length(100)
                            .Required())
                        .AddReferences("RiskAssessmentPart", f => f
                            .WithSchemaId(riskAssessmentPartSchema.SchemaId)
                            )
                        .Build();
                
                await publish(riskAssessmentPartSchema);
                await publish(workItemSchema);

                var addOwnerField = new AddField
                {
                    Name = "owner",
                    SchemaId = new NamedId<DomainId>(riskAssessmentPartSchema.SchemaId, riskAssessmentPartSchema.Name),
                    Properties = new ReferencesFieldProperties() with
                    {
                        SchemaIds = ImmutableList.Create(workItemSchema.SchemaId),
                        MinItems = 0,
                        MaxItems = 1,
                    }
                };
                
                await publish(addOwnerField);
            }
        }
    }
}

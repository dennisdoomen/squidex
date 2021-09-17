using System.Threading.Tasks;
using Squidex.Domain.Apps.Entities.Contents.Commands;
using Squidex.Infrastructure.Commands;

namespace Enablon.Extensions.Domain
{
    internal abstract class ActiveRecord
    {
        private readonly ICommandBus commandBus;
        private readonly DomainContext context;

        protected ActiveRecord(ICommandBus commandBus, DomainContext context)
        {
            this.commandBus = commandBus;
            this.context = context;
        }

        protected Task<CommandContext> PublishAsync(ContentCommand command)
        {
            command.Actor = context.Identity;
            command.User = context.Principal;
            command.AppId = context.Tenant;

            return commandBus.PublishAsync(command);
        }
    }
}

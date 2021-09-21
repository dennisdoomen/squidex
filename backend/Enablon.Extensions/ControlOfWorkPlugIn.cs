using Enablon.Extensions.Domain.WorkItemAggregate;
using Enablon.Extensions.Provisioning;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Squidex.Infrastructure.Commands;
using Squidex.Infrastructure.EventSourcing;
using Squidex.Infrastructure.Plugins;

namespace Enablon.Extensions
{
    [UsedImplicitly]
    public class ControlOfWorkPlugIn : IPlugin
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton<ICustomCommandMiddleware, ConnectRiskAssessmentToParentWorkItemCommandMiddleware>();
            services.AddSingleton<ICustomCommandMiddleware, ProvisionInitialTenantSchema>();
        }
    }
}

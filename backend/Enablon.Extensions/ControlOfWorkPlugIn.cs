using Enablon.Extensions.Domain.WorkItemAggregate;
using Enablon.Extensions.Provisioning;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Squidex.Domain.Apps.Core.ValidateContent;
using Squidex.Infrastructure.Commands;
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
            services.AddSingleton<ICustomCommandMiddleware, PreventRemovingRiskAssessmentPartWhenInUseCommandMiddleware>();
            services.AddSingleton<IValidatorsFactory, ValidatorsFactory>();
        }
    }
}

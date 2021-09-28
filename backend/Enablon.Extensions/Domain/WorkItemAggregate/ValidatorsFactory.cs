using System.Collections.Generic;
using Squidex.Domain.Apps.Core.ValidateContent;
using Squidex.Domain.Apps.Entities.Contents;
using Squidex.Infrastructure.Commands;

namespace Enablon.Extensions.Domain.WorkItemAggregate
{
    internal class ValidatorsFactory : IValidatorsFactory
    {
        private readonly IContentLoader contentLoader;
        private readonly ICommandBus commandBus;

        public ValidatorsFactory(IContentLoader contentLoader, ICommandBus commandBus)
        {
            this.contentLoader = contentLoader;
            this.commandBus = commandBus;
        }

        public IEnumerable<IValidator> CreateContentValidators(ValidatorContext context, ValidatorFactory factory)
        {
            if (RiskAssessmentPart.AppliesTo(context.SchemaId))
            {
                yield return new PreventPartChangesBasedOnParentStateValidator(contentLoader, commandBus);
            }
        }
    }
}

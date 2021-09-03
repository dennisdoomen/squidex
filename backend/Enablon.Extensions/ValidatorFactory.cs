using System.Collections.Generic;
using Squidex.Domain.Apps.Core.Schemas;
using Squidex.Domain.Apps.Core.ValidateContent;

namespace Enablon.Extensions
{
    internal class ValidatorFactory : IValidatorsFactory
    {
        public IEnumerable<IValidator> CreateValueValidators(ValidatorContext context, IField field, Squidex.Domain.Apps.Core.ValidateContent.ValidatorFactory factory)
        {
            yield return new ParentStateValidator();
        }
    }
}

using System;
using System.Threading.Tasks;
using Squidex.Domain.Apps.Core.ValidateContent;

namespace Enablon.Extensions
{
    internal class ParentStateValidator : IValidator
    {
        public Task ValidateAsync(object? value, ValidationContext context, AddError addError)
        {
            return Task.CompletedTask;
        }
    }
}

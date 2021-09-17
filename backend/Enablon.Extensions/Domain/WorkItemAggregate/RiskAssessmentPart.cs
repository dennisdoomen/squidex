using System.Linq;
using System.Threading.Tasks;
using Enablon.Extensions.Common;
using Squidex.Domain.Apps.Core.Contents;
using Squidex.Domain.Apps.Entities.Contents;
using Squidex.Domain.Apps.Entities.Contents.Commands;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Commands;
using Squidex.Infrastructure.Validation;

namespace Enablon.Extensions.Domain.WorkItemAggregate
{
    internal class RiskAssessmentPart : ActiveRecord
    {
        private readonly IContentEntity entity;
        private readonly ContentData data;

        public RiskAssessmentPart(ICommandBus commandBus, DomainContext context, IContentEntity entity) 
            : base(commandBus, context)
        {
            this.entity = entity;
            data = entity.Data.Clone();
        }

        /// <summary>
        /// References the <see cref="WorkItem"/> that part is composed of.
        /// </summary>
        public DomainId? Owner
        {
            get
            {
                string[] ids = data.GetFromJsonArray("Owner");
                if (ids.Length > 1)
                {
                    throw new ValidationException(
                        "Did not expect the work item to contain more than one risk assessment part");
                }

                return ids.Any() ? DomainId.Create(ids.Single()) : null;
            }
            set
            {
                if (value == null)
                {
                    data.Remove("Owner");
                }
                else
                {
                    data.SetAsJsonArray("Owner", new[] { value.Value.ToString()});
                }
            }
        }

        public Task Save()
        {
            return PublishAsync(new UpdateContent
            {
                SchemaId = entity.SchemaId,
                ContentId = entity.Id,
                Data = data,
                ExpectedVersion = entity.Version,
                DoNotScript = true,
                DoNotValidate = true
            });
        }
    }
}

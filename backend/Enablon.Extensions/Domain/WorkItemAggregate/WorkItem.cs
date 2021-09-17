using System.Linq;
using Enablon.Extensions.Common;
using Squidex.Domain.Apps.Core.Contents;
using Squidex.Domain.Apps.Entities.Contents;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Commands;
using Squidex.Infrastructure.Validation;

namespace Enablon.Extensions.Domain.WorkItemAggregate
{
    /// <summary>
    /// Root entity representing a document that has a state, signatures and various Control-of-Work parts.
    /// </summary>
    internal class WorkItem : ActiveRecord
    {
        public const string SchemaName = "workitem";
        
        private readonly ContentData date;
        private readonly IContentEntity entity;

        public WorkItem(ICommandBus commandBus, DomainContext context, IContentEntity entity) 
            : base(commandBus, context)
        {
            date = entity.Data.Clone();
            this.entity = entity;
        }

        public DomainId Id => entity.Id;

        public DomainId? RiskAssessmentPart
        {
            get
            {
                string[] referencedIds = date.GetFromJsonArray("riskAssessment");
                if (referencedIds.Length > 1)
                {
                    throw new ValidationException(
                        "Did not expect the work item to contain more than one risk assessment part");
                }

                return referencedIds.Any() ? DomainId.Create(referencedIds.Single()) : null;
            }
        }
    }
}

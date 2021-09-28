using System.Linq;
using Enablon.Extensions.Common;
using Squidex.Domain.Apps.Core.Contents;
using Squidex.Domain.Apps.Entities.Contents.Commands;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Validation;

namespace Enablon.Extensions.Domain.WorkItemAggregate
{
    /// <summary>
    /// Root entity representing a document that has a state, signatures and various Control-of-Work parts.
    /// </summary>
    internal class WorkItem
    {
        private const string SchemaName = "workitem";
        private readonly ContentData data;

        public static bool AppliesTo(ContentCommand contentCommand)
        {
            return contentCommand.SchemaId.Name == SchemaName;
        }

        public WorkItem(DomainId id, long version, ContentData data)
        {
            Id = id;
            Version = version;
            this.data = data.Clone();
        }

        public DomainId Id { get; }

        public long Version { get; }

        public DomainId? RiskAssessmentPart
        {
            get
            {
                string[] referencedIds = data.GetFromJsonArray("riskAssessmentPart");
                if (referencedIds.Length > 1)
                {
                    throw new ValidationException(
                        "Did not expect the work item to contain more than one risk assessment part");
                }

                return referencedIds.Any() ? DomainId.Create(referencedIds.Single()) : null;
            }
        }

        public string State => data["state"]?["iv"]?.ToString() ?? "Requesting";
    }
}

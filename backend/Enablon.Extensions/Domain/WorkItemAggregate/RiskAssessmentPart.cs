using System.Linq;
using Enablon.Extensions.Common;
using Squidex.Domain.Apps.Core.Contents;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Validation;

namespace Enablon.Extensions.Domain.WorkItemAggregate
{
    internal class RiskAssessmentPart
    {
        private const string SchemaName = "riskassessmentpart";

        public RiskAssessmentPart(NamedId<DomainId> tenant, NamedId<DomainId> schema, DomainId id, ContentData data, long version = -1)
        {
            Data = data.Clone();
            Tenant = tenant;
            Schema = schema;
            Id = id;
            Version = version;
        }

        public ContentData Data { get; }

        public NamedId<DomainId> Tenant { get; }
        
        public NamedId<DomainId> Schema { get; }

        public DomainId Id { get; }

        public long Version { get; }

        /// <summary>
        /// References the <see cref="WorkItem"/> that part is composed of.
        /// </summary>
        public virtual DomainId? Owner
        {
            get
            {
                string[] ids = Data.GetFromJsonArray("owner");
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
                    Data.Remove("owner");
                }
                else
                {
                    Data.SetAsJsonArray("owner", new[] { value.Value.ToString()});
                }
            }
        }

        public static bool AppliesTo(NamedId<DomainId> schema)
        {
            return schema.Name == SchemaName;
        }
    }
}

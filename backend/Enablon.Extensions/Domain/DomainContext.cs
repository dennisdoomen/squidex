using System.Security.Claims;
using Squidex.Infrastructure;

namespace Enablon.Extensions.Domain
{
    /// <summary>
    /// Encapsulates contextual information that we while processing commands or executing
    /// validations.
    /// </summary>
    internal class DomainContext
    {
        /// <summary>
        /// Gets or sets the identity of the user executing an action to which this context applies. 
        /// </summary>
        public RefToken Identity { get; set; }

        /// <summary>
        /// Gets or sets the security context including claims of the current identity.
        /// </summary>
        public ClaimsPrincipal? Principal { get; set; }
        
        /// <summary>
        /// Identifies the tenant (an app in Squidex)
        /// </summary>
        public NamedId<DomainId> Tenant { get; set; }
    }
}

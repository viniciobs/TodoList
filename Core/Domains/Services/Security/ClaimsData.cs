using System;

namespace Domains.Services.Security
{
    public class ClaimsData
    {
        public Guid UserId { get; private set; }
        public UserRole UserRole { get; private set; }

        public ClaimsData(Guid userId, UserRole userRole)
        {
            UserId = userId;
            UserRole = userRole;
        }
    }
}
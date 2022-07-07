using Domains;
using System;

namespace Repository.DTOs.Accounts
{
    public class AuthenticationResult
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        public string Token { get; set; }
        public bool IsActive { get; set; }
        public UserRole Role { get; set; }
    }
}
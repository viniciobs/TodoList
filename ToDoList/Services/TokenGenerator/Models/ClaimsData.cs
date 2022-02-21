using Domains;
using Repository.DTOs.Accounts;
using System;

namespace ToDoList.API.Services.TokenGenerator.Models
{
    public class ClaimsData
    {
        public Guid UserId { get; private set; }
        public UserRole UserRole { get; private set; }

        private ClaimsData()
        { }

        public static ClaimsData Convert(AuthenticationResult authenticationResult)
        {
            if (authenticationResult == null) throw new ArgumentNullException(nameof(authenticationResult));

            return new ClaimsData()
            {
                UserId = authenticationResult.UserId,
                UserRole = authenticationResult.Role
            };
        }
    }
}
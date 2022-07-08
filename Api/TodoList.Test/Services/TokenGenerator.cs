using Microsoft.Extensions.Options;
using NUnit.Framework;
using Repository.DTOs.Accounts;
using System;
using ToDoList.API.Services.TokenGenerator.Interfaces;
using ToDoList.API.Services.TokenGenerator.Models;
using ToDoList.UI.Configurations;

namespace TodoList.Test.Services
{
    public class TokenGenerator
    {
        private ITokenGenerator tokenGenerator;

        [SetUp]
        public void Setup()
        {
            Authentication authentication = new Authentication() { Secret = Guid.NewGuid().ToString() };
            IOptions<Authentication> authOptions = Options.Create(authentication);

            tokenGenerator = new ToDoList.API.Services.TokenGenerator.TokenGenerator(authOptions);
        }

        [Test]
        public void TestTokenGenerator_CreateClaimsDataWithNullEntry_ThrowsNullArgumentException()
        {
            Assert.Throws<ArgumentNullException>(() => ClaimsData.Convert(null));
        }

        [Test]
        public void TestTokenGeneratorWithNullEntry_ThrowsNullArgumentException()
        {
            Assert.Throws<ArgumentNullException>(() => tokenGenerator.GenerateToken(null));
        }

        [Test]
        public void TestTokenGenerator_Ok()
        {
            AuthenticationResult authenticationResult = new AuthenticationResult()
            {
                UserId = Guid.NewGuid(),
                Role = Domains.UserRole.Admin
            };

            string token = tokenGenerator.GenerateToken(ClaimsData.Convert(authenticationResult));
            Assert.IsNotNull(token);
        }
    }
}
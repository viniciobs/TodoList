using NUnit.Framework;
using Repository.DTOs.Accounts;
using System.Net;
using System.Threading.Tasks;
using TodoList.API.Test.Base;
using TodoList.API.Test.Helpers;

namespace TodoList.API.Test.Controllers
{
    public class AccountsControllerTests : APIBaseTest
    {
        [Theory]
        [TestCase("", " ")]
        [TestCase("", null)]
        [TestCase(null, "bar")]
        [TestCase(" ", "bar")]
        public async Task TryAuthenticateWithEmptyData_ReturnBadRequest(string login, string password)
        {
            // Arange
            var emptyData = new AuthenticationData();
            emptyData.Login = login;
            emptyData.Password = password;

            // Act
            var response = await _client.PostAsync("accounts/authenticate", emptyData.AsStringContent());

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);
        }

        [Theory]
        [TestCase("", "", "")]
        [TestCase("", "   ", null)]
        [TestCase("foo", "bar", "")]
        [TestCase("foo", null, "bar")]
        public async Task TryCreateNewAccountWithEmptyData_ReturnBadRequest(string name, string login, string password)
        {
            // Arrange
            var emptyData = new CreateAccountData();
            emptyData.Name = name;
            emptyData.Login = login;
            emptyData.Password = password;

            // Act
            var response = await _client.PostAsync("accounts/new", emptyData.AsStringContent());

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);
        }
    }
}
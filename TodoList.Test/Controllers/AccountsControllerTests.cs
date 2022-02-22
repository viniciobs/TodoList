using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Repository.DTOs.Accounts;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using TodoList.API.Test.Helpers;
using ToDoList.UI;

namespace TodoList.API.Test.Controllers
{
    public class AccountsControllerTests
    {
        private readonly HttpClient _client;

        public AccountsControllerTests()
        {
            var currentDirectory = Directory.GetCurrentDirectory();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(currentDirectory)
                .AddJsonFile("appsettings.json")
                .AddUserSecrets("b8bf55d2-cabd-4731-9649-5e3ac0b548ca")
                .Build();

            var webHostBuilder = new WebHostBuilder()
                .UseEnvironment("Development")
                .UseConfiguration(configuration)
                .UseStartup<Startup>();

            var server = new TestServer(webHostBuilder);

            _client = server.CreateClient();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        [Theory]
        [TestCase("", " ")]
        [TestCase("", null)]
        [TestCase(null, null)]
        [TestCase(null, "")]
        [TestCase("foo", "")]
        [TestCase("foo", null)]
        [TestCase(null, "bar")]
        [TestCase(" ", "bar")]
        public async Task TryAuthenticateWithEmptyData_ReturnBadRequest(string login, string password)
        {
            //Arange
            var emptyData = new AuthenticationData();
            emptyData.Login = login;
            emptyData.Password = password;

            // Act
            var response = await _client.PostAsync("accounts/authenticate", emptyData.AsStringContent());

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);
        }
    }
}
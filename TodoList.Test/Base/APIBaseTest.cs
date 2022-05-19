using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using ToDoList.UI;

namespace TodoList.API.Test.Base
{
    public class APIBaseTest
    {
        protected readonly HttpClient _client;

        public APIBaseTest()
        {
            var currentDirectory = Directory.GetCurrentDirectory();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(currentDirectory)
                .AddJsonFile("appsettings.json")
                .AddUserSecrets("b8bf55d2-cabd-4731-9649-5e3ac0b548ca")
                .Build();

            var webHostBuilder = new WebHostBuilder()
                .UseEnvironment("Test")
                .UseConfiguration(configuration)
                .UseStartup<Startup>();

            var server = new TestServer(webHostBuilder);

            _client = server.CreateClient();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
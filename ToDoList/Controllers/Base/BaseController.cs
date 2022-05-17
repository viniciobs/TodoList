using System.Net;
using Domains.Logger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ToDoList.API.Controllers.Base
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected string ipAddress;

        public BaseController()
        {
            ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();
        }

        protected void LogRequest(ILogger logger)
        {
            logger.LogTrace(new LogContent(ipAddress, $"Request: {Request.Path}").Serialized());
        }
    }
}
using System;
using System.Text.Json;

namespace Domains.Logger
{
    public class LogContent
    {
        public string IpAddress { get; private set; }
        public Guid? UserId { get; private set; }
        public DateTime DateTime { get; private set; }
        public string Message { get; private set; }

        public LogContent(Guid userId, string ipAddress, string message)
        {
            DateTime = DateTime.Now;

            IpAddress = ipAddress;
            UserId = userId;
            Message = message;
        }

        public LogContent(string ipAddress, string message)
        {
            DateTime = DateTime.Now;

            IpAddress = ipAddress;
            Message = message;
        }

        public string Serialized()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
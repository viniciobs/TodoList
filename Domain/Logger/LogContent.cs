using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Domains.Logger
{
    public class LogContent
    {
        public string IpAddress { get; private set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Guid? UserId { get; private set; }

        public DateTime DateTime { get; private set; }

        public string Message { get; private set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public object AuxiliarData { get; private set; }

        public LogContent(Guid userId, string ipAddress, string message, object auxiliarData = null)
        {
            DateTime = DateTime.Now;

            IpAddress = ipAddress;
            UserId = userId;
            Message = message;
            AuxiliarData = auxiliarData;
        }

        public LogContent(string ipAddress, string message, object auxiliarData = null)
        {
            DateTime = DateTime.Now;

            IpAddress = ipAddress;
            Message = message;
            AuxiliarData = auxiliarData;
        }

        public string Serialized()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
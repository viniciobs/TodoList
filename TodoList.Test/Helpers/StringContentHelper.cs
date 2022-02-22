using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace TodoList.API.Test.Helpers
{
    public static class StringContentHelper
    {
        public static StringContent AsStringContent(this object obj)
        {
            if (obj == null) return null;

            string jsonContent = JsonSerializer.Serialize(obj);

            return new StringContent(jsonContent, Encoding.UTF8, "application/json");
        }
    }
}
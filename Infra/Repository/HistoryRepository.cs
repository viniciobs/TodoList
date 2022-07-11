using Domains;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;
using Repository.Interfaces;
using System;
using System.Data;
using System.Text.Json;
using System.Threading.Tasks;

namespace Repository
{
    public class HistoryRepository : IHistoryRepository
    {
        private readonly string _conn;

        public HistoryRepository(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException(nameof(_conn));
            _conn = connectionString;
        }

        public async Task AddHistoryAsync(string serializedData)
        {
            const string SQL = "INSERT INTO [History] (UserId, Action, DateTime, Content) VALUES (CONVERT(uniqueidentifier, @userId), @action, @datetime, @content)";

            var history = GetHistory(serializedData);

            using (var conn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(SQL, conn))
            {
                cmd.Parameters.AddWithValue("@userId", history.UserId);
                cmd.Parameters.AddWithValue("@action", (int)history.Action);
                cmd.Parameters.AddWithValue("@datetime", history.DateTime.ToString("yyyy-MM-dd hh:mm:ss"));

                if (history.Content == null)
                {
                    cmd.Parameters.AddWithValue("@content", DBNull.Value);
                }
                else
                {
                    var content = JsonSerializer.Serialize(history.Content);
                    cmd.Parameters.AddWithValue("@content", content);
                }

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        private History GetHistory(string serializedData)
        {
            if (string.IsNullOrEmpty(serializedData)) throw new ArgumentNullException(nameof(serializedData));

            var data = JsonDocument.Parse(serializedData);
            var action = (HistoryAction)data.RootElement.GetProperty("Action").GetInt16();
            var userId = new Guid(data.RootElement.GetProperty("UserId").GetString());
            var content = JsonSerializer.Serialize(data.RootElement.GetProperty("Content"));

            return new History(userId, action, content);
        }
    }
}
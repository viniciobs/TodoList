using DataAccess;
using Domains;
using Domains.Exceptions;
using Microsoft.Data.SqlClient;
using Repository.DTOs.History;
using Repository.Interfaces;
using System;
using System.Data;
using System.Text.Json;
using System.Threading.Tasks;

namespace Repository
{
	public class HistoryRepository : _Commom.Repository, IHistoryRepository
	{
		private readonly string _conn;

		public HistoryRepository(ApplicationContext context, string connectionString)
			: base(context)
		{
			_conn = connectionString;
		}

		public async Task AddHistory(AddHistoryData data)
		{
			if (data == null) throw new MissingArgumentsException(nameof(data));
			if (data.UserId == default) throw new MissingArgumentsException(nameof(data.UserId));
	
			string content = JsonSerializer.Serialize(data.Content);
			History history = new History(data.UserId, data.Action, content);

			const string SQL = "INSERT INTO [History] (UserId, Action, DateTime, Content) VALUES (CONVERT(uniqueidentifier, @userId), @action, @datetime, @content)";
					
			using (SqlConnection con = new SqlConnection(_conn))
			{
				using (SqlCommand cmd = new SqlCommand(SQL, con))
				{
					cmd.Parameters.AddWithValue("@userId", history.UserId);
					cmd.Parameters.AddWithValue("@action", (int)history.Action);
					cmd.Parameters.AddWithValue("@datetime", history.DateTime.ToString("yyyy-MM-dd hh:mm:ss"));

					if (data.Content == null)
					{
						cmd.Parameters.AddWithValue("@content", DBNull.Value);
					}
					else
					{
						cmd.Parameters.AddWithValue("@content", content);
					}

					await con.OpenAsync();
					int result = await cmd.ExecuteNonQueryAsync();

					if (con.State == ConnectionState.Open) 
						con.Close();	
				}
			}
		}
	}
}

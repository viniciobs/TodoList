using Domains;
using System;

namespace Repository.DTOs.History
{
	public class AddHistoryData
	{
		public Guid UserId { get; set; }
		public HistoryAction Action { get; set; }
		public object Content { get; set; }
	}
}

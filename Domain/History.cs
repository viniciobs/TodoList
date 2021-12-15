using System;

namespace Domains
{
	public class History
	{		
		public Guid UserId { get; private set; }
		public HistoryAction Action { get; private set; }
		public DateTime DateTime { get; private set; }
		public string Content { get; private set; }

		private History()
		{}

		public History(Guid userId, HistoryAction action, string content)
		{
			UserId = userId;
			Action = action;
			DateTime = DateTime.UtcNow;
			Content = content;
		}

	}
}

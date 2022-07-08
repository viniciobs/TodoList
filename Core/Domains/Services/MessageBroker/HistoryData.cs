using System;

namespace Domains.Services.MessageBroker
{
    public class HistoryData
    {
        public Guid UserId { get; private set; }
        public HistoryAction Action { get; private set; }
        public object Content { get; private set; }

        public HistoryData(Guid userId, HistoryAction action)
        {
            UserId = userId;
            Action = action;
        }

        public HistoryData(Guid userId, HistoryAction action, object content)
        {
            UserId = userId;
            Action = action;
            Content = content;
        }
    }
}
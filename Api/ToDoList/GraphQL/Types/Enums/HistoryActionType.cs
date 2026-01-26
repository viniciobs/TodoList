using GraphQL.Types;

namespace ToDoList.GraphQL.Types.Enums
{
    public class HistoryActionType : EnumerationGraphType<Domains.HistoryAction>
    {
        public HistoryActionType()
        {
            Name = "HistoryAction";
            Description = "The type of action performed in the history record.";
        }
    }
}
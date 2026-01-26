using GraphQL.Types;
using ToDoList.GraphQL.Types.Enums;

namespace ToDoList.GraphQL.Types;

public class HistoryType : ObjectGraphType<Domains.History>
{
    public HistoryType()
    {
        Field(x => x.UserId)
            .Description("The ID of the user associated with this history record.");

        Field<HistoryActionType>("Action")
            .Description("The action performed in this history record.");

        Field(x => x.DateTime)
            .Description("The date and time when the action was performed.");

        Field(x => x.Content)
            .Description("The content of the history record.");
    }
}
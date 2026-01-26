using GraphQL.Types;
using Repository.DTOs.Tasks;

namespace ToDoList.GraphQL.Types;

public class TaskType : ObjectGraphType<TaskResult>
{
    public TaskType()
    {
        Name = "Task";
        Description = "Represents a task in the to-do list.";

        Field(x => x.Id);
        Field(x => x.Description);
        Field<UserType>("TargetUser");
        Field(x => x.CompletedAt, nullable: true);            
    }
}
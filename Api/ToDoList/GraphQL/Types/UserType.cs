using GraphQL.Types;
using Repository.DTOs.Users;

namespace ToDoList.GraphQL.Types;

public class UserType : ObjectGraphType<UserResult>
{
    public UserType()
    {
        Name = "User";
        Description = "Represents a user in the to-do list application.";
        
        Field(x => x.Id)
            .Description("The ID of the user.");

        Field(x => x.Name)
            .Description("The name of the user.");

        Field(x => x.IsActive)
            .Description("Indicates whether the user is active.");

        Field(x => x.CreatedAt)
            .Description("The date and time when the user was created.");

        Field<UserRoleType>("Role")
            .Description("The role of the user.");
    }
}
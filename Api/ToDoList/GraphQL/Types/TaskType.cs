using Domains;
using HotChocolate.Types;

namespace ToDoList.GraphQL.Types;

public class TaskType : ObjectType<User.Task>
{
    protected override void Configure(IObjectTypeDescriptor<User.Task> descriptor)
    {
        descriptor.BindFieldsExplicitly();

        descriptor.Field(x => x.Id);
        descriptor.Field(x => x.Description);
        descriptor.Field(x => x.CreatedAt);
        descriptor.Field(x => x.CompletedAt);
        descriptor.Field(x => x.CreatorUser).Type<UserType>();
        descriptor.Field(x => x.TargetUser).Type<UserType>();
    }
}
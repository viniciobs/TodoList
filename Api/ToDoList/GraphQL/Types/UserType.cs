using Domains;
using HotChocolate.Types;

namespace ToDoList.GraphQL.Types;

public class UserType : ObjectType<User>
{
    protected override void Configure(IObjectTypeDescriptor<User> descriptor)
    {
        descriptor.BindFieldsExplicitly();

        descriptor.Field(x => x.Id);
        descriptor.Field(x => x.Name);
        descriptor.Field(x => x.Role);
        descriptor.Field(x => x.IsActive);
        descriptor.Field(x => x.CreatedAt);        
    }
}
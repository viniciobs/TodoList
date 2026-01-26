using GraphQL.Types;

public class UserRoleType : EnumerationGraphType<Domains.UserRole>
{
    public UserRoleType()
    {
        Name = "UserRole";
        Description = "The role of the user.";
    }
}
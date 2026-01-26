using System;
using GraphQL;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using Repository.DTOs.Users;
using Repository.Interfaces;

namespace ToDoList.GraphQL.Queries;

public class UserQuery : IQuery
{
    private static void SetGetSingleUser(IUserRepository repository, Query query)
    {
        query.Field<Types.UserType>("user")
            .Argument<NonNullGraphType<IdGraphType>>("id", "The ID of the user.")
            .ResolveAsync(async context =>
            {
                var id = context.GetArgument<Guid>("id");

                if (id == default)
                    return null;

                var result = await repository.FindAsync(id);

                return UserResult.Convert(result);
            });
    }

    private static void SetGetMultipleUsers(IUserRepository repository, Query query)
    {
        query.Field<ListGraphType<Types.UserType>>("users")
            .Arguments(
                new QueryArguments(
                    new QueryArgument<StringGraphType> { Name = "name", Description = "Filter users by name." },
                    new QueryArgument<BooleanGraphType> { Name = "isActive", Description = "Filter users by active status." },
                    new QueryArgument<IntGraphType> { Name = "page" },
                    new QueryArgument<IntGraphType> { Name = "pageSize"}
                )
            )
            .ResolveAsync(async context => 
            {   
                var filter = new UserFilter();

                if (context.HasArgument("name"))
                {
                    var name = context.GetArgument<string>("name");
                    filter.Name = string.IsNullOrEmpty(name) ? null : name;                    
                }

                if (context.HasArgument("isActive"))
                {
                    filter.IsActive = context.GetArgument<bool>("isActive");
                }

                if (context.HasArgument("page"))
                {
                    filter.Page = context.GetArgument<int>("page");
                }

                if (context.HasArgument("pageSize"))
                {
                    filter.ItemsPerPage = context.GetArgument<int>("pageSize");
                }

                var result = await repository.GetAsync(filter);

                return result.Data;
            });
    }

    public void SetFields(Query query, IServiceProvider serviceProvider)
    {
        var userRepository = serviceProvider.GetRequiredService<IUserRepository>();

        SetGetMultipleUsers(userRepository, query);
        SetGetSingleUser(userRepository, query);
    }
}
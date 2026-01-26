using System;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using ToDoList.GraphQL.Queries;

namespace ToDoList.GraphQL;

public class AppSchema : Schema
{
    public AppSchema(IServiceProvider provider) 
        : base(provider)
    {
        Query = provider.GetRequiredService<Query>();        
    }
}
using System.Linq;
using DataAccess;
using Domains;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using ToDoList.GraphQL.Types;

namespace ToDoList.GraphQL.Queries;

public class Query
{    
    [UseProjection]
    [UseSorting]
    [UseFiltering(typeof(TaskType))]
    [UsePaging(MaxPageSize = 20, DefaultPageSize = 5)]
    public IQueryable<User.Task> GetTasks([Service] ApplicationContext dbContext)
        => dbContext.Task;    
}
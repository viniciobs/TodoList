using System;
using System.Collections.Generic;
using GraphQL.Types;

namespace ToDoList.GraphQL.Queries;

public interface IQuery
{
    void SetFields(Query query, IServiceProvider serviceProvider);
}

public class Query : ObjectGraphType
{
    public Query(IServiceProvider serviceProvider, IEnumerable<IQuery> services)
    {        
        foreach (var service in services)
        {
            service.SetFields(this, serviceProvider);
        }
    }
}
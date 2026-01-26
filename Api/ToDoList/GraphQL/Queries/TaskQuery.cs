using System;
using GraphQL;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using Repository.DTOs.Tasks;
using Repository.Interfaces;

namespace ToDoList.GraphQL.Queries;

public class TaskQuery : IQuery
{
    private void SetGetMultipleTasks(ITaskRepository taskRepository, Query query)
    {
        query.Field<ListGraphType<Types.TaskType>>("tasks")
            .Arguments(
                new QueryArguments(                    
                    new QueryArgument<StringGraphType> { Name = "createdBy", Description = "Filter tasks by user creator." },
                    new QueryArgument<StringGraphType> { Name = "assignedTo", Description = "Filter tasks by user assigned." },
                    new QueryArgument<BooleanGraphType> { Name = "isCompleted", Description = "Filter tasks by completion status." }
                ))
            .ResolveAsync(async context =>
            {
                var filter = new TaskFilter();

                if (context.HasArgument("createdBy"))
                {
                    filter.CreatorUser = context.GetArgument<Guid>("createdBy");
                }   

                if (context.HasArgument("assignedTo"))
                {
                    filter.TargetUser = context.GetArgument<Guid>("assignedTo");
                }                                

                if (context.HasArgument("isCompleted"))
                {
                    filter.Completed = context.GetArgument<bool>("isCompleted");
                }

                var result = await taskRepository.GetAsync(filter);
                return result.Data;
            });
    }

    public void SetFields(Query query, IServiceProvider serviceProvider)
    {
        var taskRepository = serviceProvider.GetRequiredService<ITaskRepository>();
        
        SetGetMultipleTasks(taskRepository, query);
    }
}
using Domains;
using Repository.Pagination;
using Repository.Util;
using System;

namespace Repository.DTOs.Tasks
{
    public class TaskFilter : PaginationFilter
    {
        public Period CompletedBetween { get; set; }
        public bool? Completed { get; set; }
        public Guid? CreatorUser { get; set; }
        public Guid? TargetUser { get; set; }
        public FilterHelper UserFilter { get; set; }
    }
}
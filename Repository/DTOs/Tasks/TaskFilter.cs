using Repository.DTOs._Commom;
using Repository.DTOs._Commom.Pagination;
using System;

namespace Repository.DTOs.Tasks
{
	public class TaskFilter : PaginationFilter
	{
		public Period CompletedBetween { get; set; }
		public bool? Completed { get; set; }
		public Guid? CreatorUser { get; set; }
		public Guid? TargetUser { get; set; }
	}
}

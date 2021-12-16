using Repository.DTOs._Commom;
using Repository.DTOs._Commom.Pagination;
using System;

namespace Repository.DTOs.Tasks
{
	public class TaskCommentFilter : PaginationFilter
	{
		public Guid TaskId { get; set; }
		public Period CreatedBetween { get; set; }
	}
}

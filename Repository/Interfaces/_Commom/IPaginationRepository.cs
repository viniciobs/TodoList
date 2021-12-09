using Repository.DTOs._Commom.Pagination;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Repository.Interfaces_Commom
{
	public interface IPaginationRepository
	{
		public PaginationResult<T> Paginate<T, S>(S filter, int total, IEnumerable<T> data)
			where T : class
			where S : PaginationFilter;
	}
}

using DataAccess;
using Repository.DTOs._Commom.Pagination;
using Repository.Interfaces_Commom;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository._Commom
{
	public class PaginationRepository : IPaginationRepository
	{
		public PaginationResult<T> Paginate<T, S>(S filter, int total, IEnumerable<T> data) 
			where T : class
			where S : PaginationFilter
		{
			var result = new PaginationResult<T>(data);
			result.Pagination = new PaginationData(filter, total);

			return result;
		}		
	}
}

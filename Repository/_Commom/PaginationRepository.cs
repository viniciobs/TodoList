using DataAccess;
using Microsoft.EntityFrameworkCore;
using Repository.DTOs._Commom.Pagination;
using Repository.Interfaces._Commom;
using Repository.Interfaces_Commom;
using System.Linq;
using System.Threading.Tasks;

namespace Repository._Commom
{
	public class PaginationRepository : IPaginationRepository
	{
		private readonly ApplicationContext _db;

		public PaginationRepository(ApplicationContext dbContext)
		{
			_db = dbContext;
		}

		public async Task<PaginationResult<TResult>> Paginate<TResult, TSource, TFilter>(IFilterRepository<TResult, TSource, TFilter> filterRepository, TFilter filter)
			where TResult : class
			where TSource : class
			where TFilter : PaginationFilter
		{
			var dataSet = filterRepository.ApplyIncludes(_db.Set<TSource>().AsNoTracking());
			var filteredQuery = filterRepository.ApplyFilter(dataSet, filter);
			
			var total = await filteredQuery.CountAsync();
			var source = filterRepository.OrderBy(filteredQuery).Skip((filter.Page - 1) * filter.ItemsPerPage).Take(filter.ItemsPerPage);
			var items = await filterRepository.CastToDTO(source).ToArrayAsync();

			var result = new PaginationResult<TResult>(items);
			result.Pagination = new PaginationData(filter, total);

			return result;
		}
	}
}

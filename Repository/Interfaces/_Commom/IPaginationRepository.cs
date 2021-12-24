using Repository.DTOs._Commom.Pagination;
using Repository.Interfaces._Commom;
using System.Threading.Tasks;

namespace Repository.Interfaces_Commom
{
	public interface IPaginationRepository
	{
		public  Task<PaginationResult<TResult>> Paginate<TResult, TSource, TFilter>(IFilterRepository<TResult, TSource, TFilter> filterRepository, TFilter filter)
			where TResult : class
			where TSource : class
			where TFilter : PaginationFilter;
	}
}

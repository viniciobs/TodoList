using System.Linq;

namespace Repository.Interfaces._Commom
{
	public interface IFilterRepository<TResult, TSource, TFilter>
		where TResult : class
		where TSource : class
		where TFilter : class
	{
		public IQueryable<TSource> ApplyIncludes(IQueryable<TSource> source);
		public IQueryable<TSource> ApplyFilter(IQueryable<TSource> source, TFilter filter);
		public IQueryable<TSource> OrderBy(IQueryable<TSource> source);
		public IQueryable<TResult> CastToDTO(IQueryable<TSource> source);
	}
}

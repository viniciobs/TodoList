using Repository.DTOs;
using System.Linq;

namespace Repository.Util
{
	public static class QueryHelper
	{
		public static string Like(this string text) => $"%{text}%";

		public static IQueryable<T> Paginate<T>(this IQueryable<T> source, PaginationFilter filter)
		{
			return source.Skip((filter.Page - 1) * filter.ItemsPerPage).Take(filter.ItemsPerPage);
		}
	}
} 
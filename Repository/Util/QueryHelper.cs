using Domains;
using Microsoft.EntityFrameworkCore;
using Repository.DTOs._Commom;
using Repository.DTOs._Commom.Pagination;
using Repository.DTOs.Tasks;
using Repository.DTOs.Users;
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

		public static IQueryable<User> Filter(this IQueryable<User> source, UserFilter filter)
		{
			if (filter == null) return source;

			var filterByName = !string.IsNullOrEmpty(filter.Name);
			var filterByLogin = !string.IsNullOrEmpty(filter.Login);
			var filterByStatus = filter.IsActive.HasValue;

			if (filterByName)
				source = source.Where((x) => EF.Functions.Like(x.Name, filter.Name.Like()));

			if (filterByLogin)
				source = source.Where((x) => EF.Functions.Like(x.Login, filter.Login.Like()));

			if (filterByStatus)
				source = source.Where((x) => x.IsActive == (bool)filter.IsActive);

			return source;
		}

		public static IQueryable<User.Task> Filter(this IQueryable<User.Task> source, TaskFilter filter)
		{
			if (filter == null) return source;

			var filterByStatus = filter.Completed.HasValue;
			var filterByCompletedPeriod = filter.CompletedBetween.HasValue;
			var filterByCreatorUser = filter.CreatorUser.HasValue;
			var filterByTargetUser = filter.TargetUser.HasValue;

			var period = filter.CompletedBetween;
			
			if (filterByStatus)
				source = source.Where((x) => x.CompletedAt.HasValue == (bool)filter.Completed);

			if (filterByCompletedPeriod)
				source = source.Where(x => x.CompletedAt.HasValue && period.IsBetween(x.CompletedAt.Value));

			if (filterByCreatorUser && filterByTargetUser && filter.UserFilter == FilterHelper.OR)
			{
				source = source.Where(x => x.CreatorUserId == filter.CreatorUser || x.TargetUserId == filter.TargetUser);
			}
			else
			{
				if (filterByCreatorUser)
					source = source.Where(x => x.CreatorUserId == filter.CreatorUser);

				if (filterByTargetUser)
					source = source.Where(x => x.TargetUserId == filter.TargetUser);
			}
			

			return source;
		}
	}
} 
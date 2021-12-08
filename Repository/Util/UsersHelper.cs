using Domains;
using Microsoft.EntityFrameworkCore;
using Repository.DTOs.Users;
using System.Collections.Generic;
using System.Linq;

namespace Repository.Util
{
	public static class UsersHelper
	{		
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
	}
}

using Repository.DTOs._Commom.Pagination;

namespace Repository.DTOs.Users
{
	public class UserFilter : PaginationFilter
	{
		public string Name { get; set; }
		public string Login { get; set; }
		public bool? IsActive { get; set; }
	}
}

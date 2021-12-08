namespace Repository.DTOs
{
	public abstract class PaginationFilter
	{
		const int DEFAULT_PAGE = 1;
		const int DEFAULT_ITEMS_PER_PAGE = 5;

		private int _page;
		private int _itemsPerPage;

		public int Page 
		{
			get 
			{
				if (_page <= 0)
					_page = DEFAULT_PAGE;

				return _page;
			}
			set
			{
				_page = value;
			}
		} 

		public int ItemsPerPage
		{
			get
			{
				if (_itemsPerPage <= 0)
					_itemsPerPage = DEFAULT_ITEMS_PER_PAGE;

				return _itemsPerPage;
			}
			set
			{
				_itemsPerPage = value;
			}
		}		
	}
}

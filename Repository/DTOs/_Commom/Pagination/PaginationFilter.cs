namespace Repository.DTOs._Commom.Pagination
{
    public abstract class PaginationFilter
    {
        public static readonly int DefaultPage = 1;
        public static readonly int DefaultItemsPerPage = 5;

        private int _page;
        private int _itemsPerPage;

        public int Page
        {
            get
            {
                if (_page <= 0)
                    _page = DefaultPage;

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
                    _itemsPerPage = DefaultItemsPerPage;

                return _itemsPerPage;
            }
            set
            {
                _itemsPerPage = value;
            }
        }
    }
}
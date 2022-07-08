using System;

namespace Repository.Pagination
{
    public class PaginationData
    {
        /// <summary>
        /// Returns the current page.
        /// </summary>
        public int CurrentPage { get; private set; }

        /// <summary>
        /// Returns a boolean stating if it has a previous page.
        /// </summary>
        public bool HasPrev { get; private set; }

        /// <summary>
        /// Returns a boolean stating if it has one or more next pages.
        /// </summary>
        public bool HasNext { get; private set; }

        /// <summary>
        /// Returns the items quantity.
        /// </summary>
        public int TotalItems { get; private set; }

        /// <summary>
        /// Returns the last page.
        /// </summary>
        public int LastPage { get; private set; }

        public PaginationData(PaginationFilter paginationFilter, int total)
        {
            CurrentPage = paginationFilter?.Page ?? PaginationFilter.DefaultPage;
            HasPrev = CurrentPage > 1;
            TotalItems = total;
            LastPage = (int)Math.Ceiling((decimal)total / paginationFilter?.ItemsPerPage ?? PaginationFilter.DefaultItemsPerPage);
            HasNext = CurrentPage < LastPage;
        }
    }
}
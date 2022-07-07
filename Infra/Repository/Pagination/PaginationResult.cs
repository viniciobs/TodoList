using System.Collections.Generic;

namespace Repository.Pagination
{
    public sealed class PaginationResult<T> where T : class
    {
        public IEnumerable<T> Data { get; private set; }
        public PaginationData Pagination { get; set; }

        public PaginationResult(IEnumerable<T> data)
        {
            Data = data;
        }
    }
}

using Microsoft.EntityFrameworkCore;

namespace HospitalAPI.Features.Pagination
{
    public class PagedList<T>
    {
        public List<T> Items { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public bool HasNextPage => Page * PageSize < TotalItems;
        public bool HasPreviousPage => Page > 1;

        private PagedList(List<T> items, int page, int pageSize, int totalItems)
        {
            Items = items;
            Page = page;
            PageSize = pageSize;
            TotalItems = totalItems;
        }

        public static async Task<PagedList<T>> CreatePagedList(IQueryable<T> query, int page, int pageSize)
        {
            int totalItems = await query.CountAsync();
            var items = await query.Skip( (page - 1) * pageSize).Take(pageSize).ToListAsync();

            return new(items, page, pageSize, totalItems);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
namespace MovieShop.Core.Helpers
{
    public class PaginatedList<T> : List<T>
    {
        public PaginatedList(List<T> items, int count, int page, int pageSize)
        {
            PageIndex = page;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalCount = count;
            AddRange(items);
        }
        public int PageIndex { get; }
        public int TotalPages { get; }
        public int TotalCount { get; }
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
        // (1) (2) (3) (4)
        // Get Movies by Pagination and can also include search
        // http://localhost:54232/api/movies?pageIndex=1&pagesize=25&title=m 80
        // dbContext.Movies --> get movies 1-25 where title like'%m%' skip(0) take 25
        // Select * from Movies where title like '%m%' offset 0 fetch next rows 25 order by title; Page 1
        // Select * from Movies where title like '%m%' offset 25 fetch next rows 25 order by title; Pge 2
        public static async Task<PaginatedList<T>> GetPaged(IQueryable<T> source, int pageIndex, int pageSize,
                                                Func<IQueryable<T>, IOrderedQueryable<T>> orderedQuery = null,
                                                Expression<Func<T, bool>> filter = null)
        {
            var query = source;
            if (filter != null) query = query.Where(filter);
            if (orderedQuery != null) query = orderedQuery(query);
            var count = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
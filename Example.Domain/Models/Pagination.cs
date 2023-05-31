using Example.Domain.Exceptions;

namespace Example.Domain.Models
{
    public class Pagination<TModel>
    {
        public long TotalCount { get; private set; }
        public int TotalPages { get; private set; }
        public int CurrentPage { get; private set; }
        public int? NextPage { get; private set; }
        public int? PreviousPage { get; private set; }
        public IEnumerable<TModel> Items { get; private set; }

        public Pagination(IEnumerable<TModel> items, long totalCount, int page = 1, int limit = 10, bool applyPageAndLimitToItems = false)
        {
            if (page <= 0)
            {
                throw new BadRequestException("Page must be greater than 0");
            }

            var startIndex = (page - 1) * limit;
            var endIndex = page * limit;

            TotalCount = totalCount;
            CurrentPage = page;
            Items = items ?? Enumerable.Empty<TModel>();
            if (applyPageAndLimitToItems)
            {
                Items = Items.Skip(startIndex).Take(limit);
            }
            if (startIndex > 0)
            {
                PreviousPage = page - 1;
            }
            if (endIndex < totalCount)
            {
                NextPage = page + 1;
            }

            TotalPages = limit > 0 ? (int)Math.Ceiling((decimal)totalCount / (decimal)limit) : 0;
        }
    }
}
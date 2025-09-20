namespace UrlShortener.Models.DTOs.Paging
{
    public class RequestParameters
    {

        const int maxPageSize = 50;
        private int _pageSize = 5;

        public int PageSize
        {
            get => _pageSize;
            set { _pageSize = (value > maxPageSize) ? maxPageSize : value; }
        }

        public int PageNumber { get; set; } = 1;

    }

    public class MetaData
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages; // For Next and Prev to work, you must return Total of records you can fetch
    }

    public class PagedList<T> : List<T>
    {
        public MetaData MetaData { get; set; }

        public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize) : base(items)
        {
            MetaData = new MetaData
            {
                TotalCount = count,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize)
            };
        }

        public static PagedList<T> ToPagedList(IEnumerable<T> source, int pageNumber, int pageSize)
        {
            var count = source.Count();
            return new PagedList<T>(source, count, pageNumber, pageSize);
        }
    }
}


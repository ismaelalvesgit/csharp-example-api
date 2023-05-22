namespace Example.Domain.Models
{
    public class QueryData
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? OrderBy { get; set; }
        public string[]? FilterBy { get; set; }
        public bool? OrderByDescending { get; set; } = false;
    }
}

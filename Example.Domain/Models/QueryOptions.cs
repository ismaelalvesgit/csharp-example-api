namespace Example.Domain.Models;

public class QueryOptions
{
    public string[]? Includes { get; set; }
    public WhereOptions[]? Where { get; set; }
    public string? OrderBy { get; set; }
    public bool? OrderByDescending { get; set; } = false;
}

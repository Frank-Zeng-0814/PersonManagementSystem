namespace Backend.DTOs;

public class PersonQueryParameters
{
    public string? SearchTerm { get; set; }
    public string? SortBy { get; set; } = "firstName";
    public string? SortOrder { get; set; } = "asc";
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

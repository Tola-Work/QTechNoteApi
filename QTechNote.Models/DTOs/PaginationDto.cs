namespace QTechNote.Models.DTOs;

public class PaginationDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class PaginationResponseDto<T>
{
    public IEnumerable<T>? Data { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
}
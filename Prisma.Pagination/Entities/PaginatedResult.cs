namespace Prisma.Pagination.Entities;

public class PaginatedResult<TEntity>
{
    public List<TEntity> Items { get; set; } = new();
    public uint TotalCount { get; set; }
    public uint Page { get; set; }
    public uint PageSize { get; set; }
    public uint TotalPages => (uint)Math.Ceiling(TotalCount / (double)PageSize);

    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}
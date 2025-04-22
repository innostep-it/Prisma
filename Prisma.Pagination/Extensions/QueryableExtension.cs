using System.Linq.Expressions;
using Prisma.Pagination.Entities;

namespace Prisma.Pagination.Extensions;

public static class QueryableExtension
{
    public static PaginatedResult<TOut> AsPaginatedResult<TIn, TOut>(
        this IQueryable<TIn> query,
        PaginatedRequest request,
        Func<TIn, TOut> mapper)
    {
        var totalCount = (uint)query.Count();

        if (request.OrderColumnName is not null)
        {
            var parameter = Expression.Parameter(typeof(TIn), "p");
            var property = Expression.Property(parameter, request.OrderColumnName);
            var lambda = Expression.Lambda<Func<TIn, object>>(Expression.Convert(property, typeof(object)), parameter);

            query = !request.OrderDirectionDesc
                ? query.OrderBy(lambda)
                : query.OrderByDescending(lambda);
        }

        var items = query
            .Skip((int)((request.Page - 1) * request.PageSize))
            .Take((int)request.PageSize)
            .ToList();

        return new PaginatedResult<TOut>
        {
            Items = items.Select(mapper).ToList(),
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
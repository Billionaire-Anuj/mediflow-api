using System.Linq.Expressions;

namespace Mediflow.Helper;

public static class QueryableExtensionMethod
{
    // TODO: Use of the following function for ordering by dynamic property names.
    public static IOrderedQueryable<T> OrderByDynamic<T>(this IQueryable<T> query, string propertyName, bool descending, bool firstOrder)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var property = propertyName.Split('.').Aggregate<string?, Expression>(parameter, Expression.PropertyOrField!);

        var lambda = Expression.Lambda(property, parameter);

        string methodName;

        if (firstOrder)
        {
            methodName = descending ? "OrderByDescending" : "OrderBy";
        }
        else
        {
            methodName = descending ? "ThenByDescending" : "ThenBy";
        }

        var result = typeof(Queryable).GetMethods()
            .First(m => m.Name == methodName && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(T), property.Type)
            .Invoke(null, [query, lambda]);

        return (IOrderedQueryable<T>)result!;
    }
}
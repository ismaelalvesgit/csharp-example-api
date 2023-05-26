using Example.Domain.Enums;
using System.Linq.Expressions;
using System.Reflection;

namespace Example.Data.Helpers
{
    public static class QueryHelper
    {
        private static readonly MethodInfo OrderByMethod =
        typeof(Queryable).GetMethods().Single(method =>
        method.Name == "OrderBy" && method.GetParameters().Length == 2);

        private static readonly MethodInfo OrderByDescendingMethod =
            typeof(Queryable).GetMethods().Single(method =>
            method.Name == "OrderByDescending" && method.GetParameters().Length == 2);

        public static bool PropertyExists<T>(this IQueryable<T> source, string propertyName)
        {
            return typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase |
                BindingFlags.Public | BindingFlags.Instance) != null;
        }

        public static IQueryable<T>? OrderByProperty<T>(this IQueryable<T> source, string propertyName)
        {
            ParameterExpression paramterExpression = Expression.Parameter(typeof(T));
            Expression orderByProperty = Expression.Property(paramterExpression, propertyName);
            LambdaExpression lambda = Expression.Lambda(orderByProperty, paramterExpression);
            MethodInfo genericMethod = OrderByMethod.MakeGenericMethod(typeof(T), orderByProperty.Type);
            object? ret = genericMethod.Invoke(null, new object[] { source, lambda });
            return ret as IQueryable<T>;
        }

        public static IQueryable<T>? OrderByPropertyDescending<T>(this IQueryable<T> source, string propertyName)
        {
            ParameterExpression paramterExpression = Expression.Parameter(typeof(T));
            Expression orderByProperty = Expression.Property(paramterExpression, propertyName);
            LambdaExpression lambda = Expression.Lambda(orderByProperty, paramterExpression);
            MethodInfo genericMethod = OrderByDescendingMethod.MakeGenericMethod(typeof(T), orderByProperty.Type);
            object? ret = genericMethod.Invoke(null, new object[] { source, lambda });
            return ret as IQueryable<T>;
        }

        public static IQueryable<T>? WhereByOperation<T>(this IQueryable<T> source, string member, object value, WhereOperator? @operator)
        {
            var item = Expression.Parameter(typeof(T), "item");
            var memberValue = member.Split('.').Aggregate((Expression)item, Expression.PropertyOrField);
            var memberType = memberValue.Type;
            if (value != null && value.GetType() != memberType)
                value = Convert.ChangeType(value, memberType);

            var condition = Expression.Equal(memberValue, Expression.Constant(value, memberType));
   
            switch (@operator)
            {
                case WhereOperator.NotEqual:
                    condition = Expression.NotEqual(memberValue, Expression.Constant(value, memberType)); break;
                case WhereOperator.GreaterThan:
                    condition = Expression.GreaterThan(memberValue, Expression.Constant(value, memberType)); break;
                case WhereOperator.GreaterThanOrEqual:
                    condition = Expression.GreaterThanOrEqual(memberValue, Expression.Constant(value, memberType)); break;
                case WhereOperator.LessThan:
                    condition = Expression.LessThan(memberValue, Expression.Constant(value, memberType)); break;
                case WhereOperator.LessThanOrEqual:
                    condition = Expression.LessThanOrEqual(memberValue, Expression.Constant(value, memberType)); break;
            }

            var predicate = Expression.Lambda<Func<T, bool>>(condition, item);
            return source.Where(predicate);
        }
    }
}

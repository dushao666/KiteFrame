using System.Linq.Expressions;

namespace Infrastructure.Extensions;

/// <summary>
/// 表达式构建器，用于动态构建 LINQ 查询条件
/// </summary>
public static class PredicateBuilder
{
    /// <summary>
    /// 创建一个始终返回 true 的表达式
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <returns>表达式</returns>
    public static Expression<Func<T, bool>> True<T>()
    {
        return p => true;
    }

    /// <summary>
    /// 创建一个始终返回 false 的表达式
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <returns>表达式</returns>
    public static Expression<Func<T, bool>> False<T>()
    {
        return p => false;
    }

    /// <summary>
    /// 使用 AND 连接两个表达式
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="express">第一个表达式</param>
    /// <param name="predicate">第二个表达式</param>
    /// <returns>连接后的表达式</returns>
    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> express, Expression<Func<T, bool>> predicate)
    {
        return express.Compose(predicate, Expression.AndAlso);
    }

    /// <summary>
    /// 使用 OR 连接两个表达式
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="express">第一个表达式</param>
    /// <param name="predicate">第二个表达式</param>
    /// <returns>连接后的表达式</returns>
    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> express, Expression<Func<T, bool>> predicate)
    {
        return express.Compose(predicate, Expression.OrElse);
    }

    /// <summary>
    /// 根据条件使用 AND 连接表达式
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="express">第一个表达式</param>
    /// <param name="condition">连接条件</param>
    /// <param name="predicate">第二个表达式</param>
    /// <returns>连接后的表达式</returns>
    public static Expression<Func<T, bool>> AndIf<T>(this Expression<Func<T, bool>> express, bool condition, Expression<Func<T, bool>> predicate)
    {
        if (!condition) return express;
        return express.Compose(predicate, Expression.AndAlso);
    }

    /// <summary>
    /// 根据条件使用 OR 连接表达式
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="express">第一个表达式</param>
    /// <param name="condition">连接条件</param>
    /// <param name="predicate">第二个表达式</param>
    /// <returns>连接后的表达式</returns>
    public static Expression<Func<T, bool>> OrIf<T>(this Expression<Func<T, bool>> express, bool condition, Expression<Func<T, bool>> predicate)
    {
        if (!condition) return express;
        return express.Compose(predicate, Expression.OrElse);
    }

    /// <summary>
    /// 对表达式取反
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="express">表达式</param>
    /// <returns>取反后的表达式</returns>
    public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> express)
    {
        var negated = Expression.Not(express.Body);
        return Expression.Lambda<Func<T, bool>>(negated, express.Parameters);
    }

    /// <summary>
    /// 组合两个表达式
    /// </summary>
    /// <typeparam name="T">表达式类型</typeparam>
    /// <param name="first">第一个表达式</param>
    /// <param name="second">第二个表达式</param>
    /// <param name="merge">合并函数</param>
    /// <returns>组合后的表达式</returns>
    internal static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
    {
        var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);

        var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

        return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
    }
}

/// <summary>
/// 参数重新绑定器
/// </summary>
internal class ParameterRebinder : ExpressionVisitor
{
    private readonly Dictionary<ParameterExpression, ParameterExpression> _map;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="map">参数映射</param>
    public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
    {
        _map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
    }

    /// <summary>
    /// 替换参数
    /// </summary>
    /// <param name="map">参数映射</param>
    /// <param name="exp">表达式</param>
    /// <returns>替换后的表达式</returns>
    public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
    {
        return new ParameterRebinder(map).Visit(exp);
    }

    /// <summary>
    /// 访问参数表达式
    /// </summary>
    /// <param name="p">参数表达式</param>
    /// <returns>访问后的表达式</returns>
    protected override Expression VisitParameter(ParameterExpression p)
    {
        if (_map.TryGetValue(p, out ParameterExpression replacement))
        {
            p = replacement;
        }
        return base.VisitParameter(p);
    }
}

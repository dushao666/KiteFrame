using FluentValidation;
using ValidationException = Infrastructure.Exceptions.ValidationException;

namespace Application.Behaviors;

/// <summary>
/// MediatR 校验管道行为：在请求到达处理器之前执行所有已注册的 FluentValidation 校验器，
/// 校验失败时抛出 <see cref="ValidationException"/>，由全局异常处理器统一转为 ApiResult 响应
/// </summary>
/// <typeparam name="TRequest">请求类型</typeparam>
/// <typeparam name="TResponse">响应类型</typeparam>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="validators">当前请求类型的所有校验器</param>
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    /// <summary>
    /// 执行校验，通过后继续管道
    /// </summary>
    /// <param name="request">请求</param>
    /// <param name="next">管道委托</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>处理器响应</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count != 0)
            {
                // 按属性名分组汇总错误明细
                var errors = failures
                    .GroupBy(f => f.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(f => f.ErrorMessage).Distinct().ToArray());

                var message = string.Join("；", failures.Select(f => f.ErrorMessage).Distinct());
                throw new ValidationException(message, errors);
            }
        }

        return await next();
    }
}

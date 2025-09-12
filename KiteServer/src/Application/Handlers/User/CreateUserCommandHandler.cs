using Application.Commands.User;
using Domain.Entities;
using Repository;
using SqlSugar;

namespace Application.Handlers.User;

/// <summary>
/// 创建用户命令处理器
/// </summary>
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ApiResult<long>>
{
    private readonly ISugarUnitOfWork<DBContext> _unitOfWork;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    public CreateUserCommandHandler(ISugarUnitOfWork<DBContext> unitOfWork, ILogger<CreateUserCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ApiResult<long>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            using (var context = _unitOfWork.CreateContext())
            {
                // 检查用户名是否存在
                var existingUser = await context.Users
                    .AsQueryable()
                    .Where(x => x.UserName == request.UserName)
                    .FirstAsync();

                if (existingUser != null)
                {
                    return ApiResult<long>.Fail("用户名已存在");
                }

                // 使用Mapster进行对象映射
                var user = request.Adapt<Domain.Entities.User>();

                var result = await context.Users.InsertReturnEntityAsync(user);
                context.Commit();

                _logger.LogInformation("创建用户成功，用户ID: {UserId}, 用户名: {UserName}", result.Id, result.UserName);
                return ApiResult<long>.Ok(result.Id, "创建成功");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建用户失败，用户名: {UserName}", request.UserName);
            return ApiResult<long>.Fail("创建用户失败");
        }
    }
}

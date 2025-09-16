using Application.Commands.User;

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

                // 验证邮箱格式
                if (!string.IsNullOrWhiteSpace(request.Email))
                {
                    var emailRegex = new System.Text.RegularExpressions.Regex(@"^[^\s@]+@[^\s@]+\.[^\s@]+$");
                    if (!emailRegex.IsMatch(request.Email))
                    {
                        return ApiResult<long>.Fail("邮箱格式不正确");
                    }
                }

                // 验证手机号格式
                if (!string.IsNullOrWhiteSpace(request.Phone))
                {
                    var phoneRegex = new System.Text.RegularExpressions.Regex(@"^1[3-9]\d{9}$");
                    if (!phoneRegex.IsMatch(request.Phone))
                    {
                        return ApiResult<long>.Fail("手机号格式不正确");
                    }
                }

                // 检查邮箱是否存在
                if (!string.IsNullOrWhiteSpace(request.Email))
                {
                    var existingEmailUser = await context.Users
                        .AsQueryable()
                        .Where(x => x.Email == request.Email)
                        .FirstAsync();

                    if (existingEmailUser != null)
                    {
                        return ApiResult<long>.Fail("邮箱已存在");
                    }
                }

                // 检查手机号是否存在
                if (!string.IsNullOrWhiteSpace(request.Phone))
                {
                    var existingPhoneUser = await context.Users
                        .AsQueryable()
                        .Where(x => x.Phone == request.Phone)
                        .FirstAsync();

                    if (existingPhoneUser != null)
                    {
                        return ApiResult<long>.Fail("手机号已存在");
                    }
                }

                // 使用Mapster进行对象映射
                var user = request.Adapt<Domain.Entities.User>();

                // 处理可空字段：将空字符串转换为 null，避免唯一键约束冲突
                if (string.IsNullOrWhiteSpace(user.DingTalkId))
                    user.DingTalkId = null;
                if (string.IsNullOrWhiteSpace(user.Email))
                    user.Email = null;
                if (string.IsNullOrWhiteSpace(user.Phone))
                    user.Phone = null;

                // 加密密码 - 使用SHA512保持与登录一致
                user.Password = EncryptionHelper.Sha512(request.Password);

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

using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Utilities;

/// <summary>
/// 密码验证结果
/// </summary>
public enum PasswordVerificationResult
{
    /// <summary>
    /// 验证失败
    /// </summary>
    Failed = 0,

    /// <summary>
    /// 验证成功
    /// </summary>
    Success = 1,

    /// <summary>
    /// 验证成功，但存储的哈希使用旧算法（或更弱的参数），建议重新哈希后更新存储
    /// </summary>
    SuccessRehashNeeded = 2
}

/// <summary>
/// 密码哈希帮助类
/// 使用带随机盐的 PBKDF2-SHA256 算法，输出自描述字符串（算法标记 + 迭代次数 + 盐 + 哈希），
/// 存储时只需保存一个字符串字段；验证时兼容历史遗留的无盐 SHA512 哈希（验证成功后应重新哈希升级）
/// </summary>
public static class PasswordHasher
{

    /// <summary>
    /// 哈希格式标记，用于识别 PBKDF2 自描述哈希与历史遗留哈希
    /// </summary>
    public const string FormatMarker = "$KITE$";

    /// <summary>
    /// 算法名称（写入自描述哈希）
    /// </summary>
    public const string AlgorithmName = "PBKDF2-SHA256";

    /// <summary>
    /// 默认迭代次数（OWASP 对 PBKDF2-HMAC-SHA256 的推荐下限）
    /// </summary>
    public const int DefaultIterations = 600_000;

    /// <summary>
    /// 盐长度（字节）
    /// </summary>
    public const int SaltSize = 16;

    /// <summary>
    /// 哈希长度（字节）
    /// </summary>
    public const int HashSize = 32;

    /// <summary>
    /// 使用 PBKDF2-SHA256 对密码进行哈希，返回自描述字符串
    /// 格式：<c>$KITE$PBKDF2-SHA256${迭代次数}${盐Base64}${哈希Base64}</c>
    /// </summary>
    /// <param name="password">明文密码</param>
    /// <param name="iterations">迭代次数（默认取 OWASP 推荐值）</param>
    /// <returns>自描述哈希字符串</returns>
    public static string HashPassword(string password, int iterations = DefaultIterations)
    {
        ArgumentException.ThrowIfNullOrEmpty(password);
        if (iterations < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(iterations), "迭代次数必须大于 0");
        }

        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, HashSize);

        return $"{FormatMarker}{AlgorithmName}${iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    /// <summary>
    /// 验证密码是否与存储的哈希匹配
    /// 同时兼容历史遗留的无盐 SHA512 哈希（128 位十六进制字符串）：
    /// 验证通过时返回 <see cref="PasswordVerificationResult.SuccessRehashNeeded"/>，调用方应使用
    /// <see cref="HashPassword(string, int)"/> 重新哈希并更新存储
    /// </summary>
    /// <param name="password">明文密码</param>
    /// <param name="storedHash">存储的哈希值</param>
    /// <returns>验证结果</returns>
    public static PasswordVerificationResult VerifyPassword(string password, string storedHash)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedHash))
        {
            return PasswordVerificationResult.Failed;
        }

        // 自描述 PBKDF2 哈希
        if (storedHash.StartsWith(FormatMarker, StringComparison.Ordinal))
        {
            return VerifyPbkdf2(password, storedHash);
        }

        // 历史遗留：无盐 SHA512（128 位十六进制字符串）
        if (storedHash.Length == 128 && IsHexString(storedHash))
        {
            var computed = EncryptionHelper.Sha512(password);
            return FixedTimeEquals(computed, storedHash)
                ? PasswordVerificationResult.SuccessRehashNeeded
                : PasswordVerificationResult.Failed;
        }

        return PasswordVerificationResult.Failed;
    }

    /// <summary>
    /// 验证 PBKDF2 自描述哈希
    /// </summary>
    /// <param name="password">明文密码</param>
    /// <param name="storedHash">自描述哈希字符串</param>
    /// <returns>验证结果</returns>
    private static PasswordVerificationResult VerifyPbkdf2(string password, string storedHash)
    {
        var segments = storedHash.Split('$', StringSplitOptions.RemoveEmptyEntries);
        // 期望分段：KITE / 算法名 / 迭代次数 / 盐Base64 / 哈希Base64
        if (segments.Length != 5
            || !string.Equals(segments[1], AlgorithmName, StringComparison.Ordinal)
            || !int.TryParse(segments[2], out var iterations)
            || iterations < 1)
        {
            return PasswordVerificationResult.Failed;
        }

        byte[] salt;
        byte[] expectedHash;
        try
        {
            salt = Convert.FromBase64String(segments[3]);
            expectedHash = Convert.FromBase64String(segments[4]);
        }
        catch (FormatException)
        {
            return PasswordVerificationResult.Failed;
        }

        var computedHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expectedHash.Length);

        return CryptographicOperations.FixedTimeEquals(computedHash, expectedHash)
            ? PasswordVerificationResult.Success
            : PasswordVerificationResult.Failed;
    }

    /// <summary>
    /// 常量时间字符串比较，避免时序侧信道
    /// </summary>
    /// <param name="left">左侧字符串</param>
    /// <param name="right">右侧字符串</param>
    /// <returns>是否相等</returns>
    private static bool FixedTimeEquals(string left, string right)
    {
        var leftBytes = Encoding.UTF8.GetBytes(left);
        var rightBytes = Encoding.UTF8.GetBytes(right);
        return CryptographicOperations.FixedTimeEquals(leftBytes, rightBytes);
    }

    /// <summary>
    /// 判断字符串是否为十六进制字符串
    /// </summary>
    /// <param name="value">待判断字符串</param>
    /// <returns>是否为十六进制字符串</returns>
    private static bool IsHexString(string value)
    {
        foreach (var c in value)
        {
            if (!Uri.IsHexDigit(c))
            {
                return false;
            }
        }

        return true;
    }
}

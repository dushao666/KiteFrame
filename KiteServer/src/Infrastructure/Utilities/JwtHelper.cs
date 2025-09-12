using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Utilities;

/// <summary>
/// JWT帮助类
/// </summary>
public static class JwtHelper
{
    /// <summary>
    /// 生成JWT Token
    /// </summary>
    /// <param name="claims">声明集合</param>
    /// <param name="secretKey">密钥</param>
    /// <param name="issuer">颁发者</param>
    /// <param name="audience">受众</param>
    /// <param name="expires">过期时间</param>
    /// <returns>JWT Token</returns>
    public static string GenerateToken(
        IEnumerable<Claim> claims,
        string secretKey,
        string issuer,
        string audience,
        DateTime expires)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(secretKey))
                throw new ArgumentException("密钥不能为空", nameof(secretKey));

            if (secretKey.Length < 32)
                throw new ArgumentException("密钥长度不能少于32个字符", nameof(secretKey));

            // 创建签名密钥
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 创建JWT Token
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expires,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"生成JWT Token失败: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// 验证JWT Token
    /// </summary>
    /// <param name="token">JWT Token</param>
    /// <param name="secretKey">密钥</param>
    /// <param name="issuer">颁发者</param>
    /// <param name="audience">受众</param>
    /// <returns>验证结果</returns>
    public static ClaimsPrincipal? ValidateToken(
        string token,
        string secretKey,
        string issuer,
        string audience)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            if (string.IsNullOrWhiteSpace(secretKey))
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            return principal;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 从Token中获取声明
    /// </summary>
    /// <param name="token">JWT Token</param>
    /// <param name="claimType">声明类型</param>
    /// <returns>声明值</returns>
    public static string? GetClaimFromToken(string token, string claimType)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            return jwtToken.Claims.FirstOrDefault(x => x.Type == claimType)?.Value;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 检查Token是否过期
    /// </summary>
    /// <param name="token">JWT Token</param>
    /// <returns>是否过期</returns>
    public static bool IsTokenExpired(string token)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(token))
                return true;

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            return jwtToken.ValidTo < DateTime.UtcNow;
        }
        catch
        {
            return true;
        }
    }

    /// <summary>
    /// 获取Token的过期时间
    /// </summary>
    /// <param name="token">JWT Token</param>
    /// <returns>过期时间</returns>
    public static DateTime? GetTokenExpiration(string token)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            return jwtToken.ValidTo;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 生成刷新Token
    /// </summary>
    /// <returns>刷新Token</returns>
    public static string GenerateRefreshToken()
    {
        var randomBytes = new byte[32];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
}

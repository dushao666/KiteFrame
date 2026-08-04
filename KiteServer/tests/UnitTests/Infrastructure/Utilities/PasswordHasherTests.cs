using Infrastructure.Utilities;

namespace UnitTests.Infrastructure.Utilities;

/// <summary>
/// <see cref="PasswordHasher"/> 单元测试
/// </summary>
public class PasswordHasherTests
{
    /// <summary>
    /// 测试用低迭代次数（仅为加快测试速度；生产使用 <see cref="PasswordHasher.DefaultIterations"/>）
    /// </summary>
    private const int TestIterations = 1000;

    [Fact(DisplayName = "哈希后验证：正确密码应验证成功")]
    public void VerifyPassword_CorrectPassword_ReturnsSuccess()
    {
        // 准备
        var storedHash = PasswordHasher.HashPassword("P@ssw0rd", TestIterations);

        // 执行
        var result = PasswordHasher.VerifyPassword("P@ssw0rd", storedHash);

        // 断言
        Assert.Equal(PasswordVerificationResult.Success, result);
    }

    [Fact(DisplayName = "哈希后验证：错误密码应验证失败")]
    public void VerifyPassword_WrongPassword_ReturnsFailed()
    {
        // 准备
        var storedHash = PasswordHasher.HashPassword("P@ssw0rd", TestIterations);

        // 执行
        var result = PasswordHasher.VerifyPassword("WrongPassword", storedHash);

        // 断言
        Assert.Equal(PasswordVerificationResult.Failed, result);
    }

    [Fact(DisplayName = "哈希结果为自描述格式且包含算法标记")]
    public void HashPassword_Result_ContainsFormatMarkerAndIterations()
    {
        // 准备 & 执行
        var storedHash = PasswordHasher.HashPassword("P@ssw0rd", TestIterations);

        // 断言
        Assert.StartsWith(PasswordHasher.FormatMarker, storedHash);
        Assert.Contains(PasswordHasher.AlgorithmName, storedHash);
        Assert.Contains($"${TestIterations}$", storedHash);
    }

    [Fact(DisplayName = "相同密码两次哈希结果不同（盐随机）")]
    public void HashPassword_SamePasswordTwice_ProducesDifferentHashes()
    {
        // 准备 & 执行
        var first = PasswordHasher.HashPassword("P@ssw0rd", TestIterations);
        var second = PasswordHasher.HashPassword("P@ssw0rd", TestIterations);

        // 断言
        Assert.NotEqual(first, second);
    }

    [Fact(DisplayName = "兼容历史遗留 SHA512 哈希：正确密码验证成功且提示需要重新哈希")]
    public void VerifyPassword_LegacySha512Hash_ReturnsRehashNeeded()
    {
        // 准备："123456" 的无盐 SHA512 哈希（与种子数据结构一致）
        var legacyHash = EncryptionHelper.Sha512("123456");

        // 执行
        var result = PasswordHasher.VerifyPassword("123456", legacyHash);

        // 断言
        Assert.Equal(PasswordVerificationResult.SuccessRehashNeeded, result);
    }

    [Fact(DisplayName = "兼容历史遗留 SHA512 哈希：错误密码验证失败")]
    public void VerifyPassword_LegacySha512HashWithWrongPassword_ReturnsFailed()
    {
        // 准备
        var legacyHash = EncryptionHelper.Sha512("123456");

        // 执行
        var result = PasswordHasher.VerifyPassword("654321", legacyHash);

        // 断言
        Assert.Equal(PasswordVerificationResult.Failed, result);
    }

    [Theory(DisplayName = "非法输入应验证失败")]
    [InlineData("", "任意哈希")]
    [InlineData("P@ssw0rd", "")]
    [InlineData("P@ssw0rd", "非法格式的哈希")]
    [InlineData("P@ssw0rd", "$KITE$未知算法$1000$c2FsdA==$aGFzaA==")]
    [InlineData("P@ssw0rd", "$KITE$PBKDF2-SHA256$不是数字$c2FsdA==$aGFzaA==")]
    [InlineData("P@ssw0rd", "$KITE$PBKDF2-SHA256$1000$非法Base64!@#$%^&*()")]
    public void VerifyPassword_InvalidInput_ReturnsFailed(string password, string storedHash)
    {
        // 准备 & 执行
        var result = PasswordHasher.VerifyPassword(password, storedHash);

        // 断言
        Assert.Equal(PasswordVerificationResult.Failed, result);
    }

    [Fact(DisplayName = "空密码不允许哈希")]
    public void HashPassword_EmptyPassword_Throws()
    {
        // 准备 & 执行 & 断言
        Assert.Throws<ArgumentException>(() => PasswordHasher.HashPassword(string.Empty, TestIterations));
    }

    [Fact(DisplayName = "非正迭代次数不允许哈希")]
    public void HashPassword_ZeroIterations_Throws()
    {
        // 准备 & 执行 & 断言
        Assert.Throws<ArgumentOutOfRangeException>(() => PasswordHasher.HashPassword("P@ssw0rd", 0));
    }
}

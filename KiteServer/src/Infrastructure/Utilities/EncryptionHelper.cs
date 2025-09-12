using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Utilities;

/// <summary>
/// 加密帮助类
/// </summary>
public static class EncryptionHelper
{
    /// <summary>
    /// AES加密字符串
    /// </summary>
    /// <param name="data">要加密的数据</param>
    /// <param name="secretKey">密钥（32字节）</param>
    /// <returns>加密后的Base64字符串</returns>
    public static string EncryptBytes(string data, string secretKey)
    {
        try
        {
            byte[] secretBytes = Encoding.UTF8.GetBytes(secretKey);
            byte[] buffer = Encoding.UTF8.GetBytes(data);
            
            using (Aes aes = Aes.Create())
            {
                aes.Key = secretBytes;
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;
                
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                byte[] encryptedValue = encryptor.TransformFinalBlock(buffer, 0, buffer.Length);
                
                return Convert.ToBase64String(encryptedValue);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("加密失败", ex);
        }
    }

    /// <summary>
    /// AES解密字符串
    /// </summary>
    /// <param name="data">要解密的Base64字符串</param>
    /// <param name="secretKey">密钥（32字节）</param>
    /// <returns>解密后的字符串</returns>
    public static string DecryptBytes(string data, string secretKey)
    {
        try
        {
            byte[] secretBytes = Encoding.UTF8.GetBytes(secretKey);
            byte[] buffer = Convert.FromBase64String(data);
            
            using (Aes aes = Aes.Create())
            {
                aes.Key = secretBytes;
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;
                
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                byte[] decryptedValue = decryptor.TransformFinalBlock(buffer, 0, buffer.Length);
                
                return Encoding.UTF8.GetString(decryptedValue);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("解密失败", ex);
        }
    }

    /// <summary>
    /// 将输入的字符串使用SHA512方式进行哈希散列
    /// </summary>
    /// <param name="input">输入字符串</param>
    /// <returns>SHA512哈希值</returns>
    public static string Sha512(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        
        using (var sha = SHA512.Create())
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", "");
        }
    }

    /// <summary>
    /// 将输入的字符串使用SHA256方式进行哈希散列
    /// </summary>
    /// <param name="input">输入字符串</param>
    /// <returns>SHA256哈希值</returns>
    public static string Sha256(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        
        using (var sha = SHA256.Create())
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", "");
        }
    }

    /// <summary>
    /// 将输入的字符串使用MD5方式进行哈希散列
    /// </summary>
    /// <param name="input">输入字符串</param>
    /// <returns>MD5哈希值</returns>
    public static string Md5(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        
        using (var md5 = MD5.Create())
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = md5.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", "");
        }
    }

    /// <summary>
    /// 生成随机盐值
    /// </summary>
    /// <param name="size">盐值长度</param>
    /// <returns>Base64编码的盐值</returns>
    public static string GenerateSalt(int size = 32)
    {
        using (var rng = RandomNumberGenerator.Create())
        {
            byte[] salt = new byte[size];
            rng.GetBytes(salt);
            return Convert.ToBase64String(salt);
        }
    }

    /// <summary>
    /// 使用PBKDF2算法对密码进行哈希
    /// </summary>
    /// <param name="password">密码</param>
    /// <param name="salt">盐值</param>
    /// <param name="iterations">迭代次数</param>
    /// <param name="hashLength">哈希长度</param>
    /// <returns>哈希后的密码</returns>
    public static string HashPassword(string password, string salt, int iterations = 10000, int hashLength = 32)
    {
        using (var pbkdf2 = new Rfc2898DeriveBytes(password, Convert.FromBase64String(salt), iterations, HashAlgorithmName.SHA256))
        {
            byte[] hash = pbkdf2.GetBytes(hashLength);
            return Convert.ToBase64String(hash);
        }
    }

    /// <summary>
    /// 验证密码
    /// </summary>
    /// <param name="password">输入的密码</param>
    /// <param name="salt">盐值</param>
    /// <param name="hashedPassword">存储的哈希密码</param>
    /// <param name="iterations">迭代次数</param>
    /// <param name="hashLength">哈希长度</param>
    /// <returns>是否匹配</returns>
    public static bool VerifyPassword(string password, string salt, string hashedPassword, int iterations = 10000, int hashLength = 32)
    {
        string computedHash = HashPassword(password, salt, iterations, hashLength);
        return computedHash == hashedPassword;
    }

    /// <summary>
    /// 生成RSA密钥对
    /// </summary>
    /// <param name="keySize">密钥长度</param>
    /// <returns>公钥和私钥</returns>
    public static (string PublicKey, string PrivateKey) GenerateRsaKeyPair(int keySize = 2048)
    {
        using (var rsa = RSA.Create(keySize))
        {
            string publicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey());
            string privateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey());
            
            return (publicKey, privateKey);
        }
    }

    /// <summary>
    /// RSA加密
    /// </summary>
    /// <param name="data">要加密的数据</param>
    /// <param name="publicKey">公钥</param>
    /// <returns>加密后的数据</returns>
    public static string RsaEncrypt(string data, string publicKey)
    {
        using (var rsa = RSA.Create())
        {
            rsa.ImportRSAPublicKey(Convert.FromBase64String(publicKey), out _);
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            byte[] encryptedBytes = rsa.Encrypt(dataBytes, RSAEncryptionPadding.OaepSHA256);
            
            return Convert.ToBase64String(encryptedBytes);
        }
    }

    /// <summary>
    /// RSA解密
    /// </summary>
    /// <param name="encryptedData">加密的数据</param>
    /// <param name="privateKey">私钥</param>
    /// <returns>解密后的数据</returns>
    public static string RsaDecrypt(string encryptedData, string privateKey)
    {
        using (var rsa = RSA.Create())
        {
            rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);
            byte[] encryptedBytes = Convert.FromBase64String(encryptedData);
            byte[] decryptedBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA256);
            
            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }

    /// <summary>
    /// 生成HMAC-SHA256签名
    /// </summary>
    /// <param name="data">要签名的数据</param>
    /// <param name="key">密钥</param>
    /// <returns>签名</returns>
    public static string HmacSha256(string data, string key)
    {
        using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
        {
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return Convert.ToBase64String(hash);
        }
    }

    /// <summary>
    /// 验证HMAC-SHA256签名
    /// </summary>
    /// <param name="data">原始数据</param>
    /// <param name="key">密钥</param>
    /// <param name="signature">签名</param>
    /// <returns>是否验证通过</returns>
    public static bool VerifyHmacSha256(string data, string key, string signature)
    {
        string computedSignature = HmacSha256(data, key);
        return computedSignature == signature;
    }
}

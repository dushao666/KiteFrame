namespace Shared.Configuration;

/// <summary>
/// 文件上传配置设置
/// </summary>
public class FileUploadSettings
{
    /// <summary>
    /// 配置节名称
    /// </summary>
    public const string SectionName = "FileUploadSettings";

    /// <summary>
    /// 最大文件大小（字节）
    /// </summary>
    public long MaxFileSize { get; set; } = 10485760; // 10MB

    /// <summary>
    /// 允许的文件扩展名
    /// </summary>
    public string[] AllowedExtensions { get; set; } = Array.Empty<string>();

    /// <summary>
    /// 上传路径
    /// </summary>
    public string UploadPath { get; set; } = "uploads";
}

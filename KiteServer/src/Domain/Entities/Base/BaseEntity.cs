namespace Domain.Entities.Base;

/// <summary>
/// 基础实体类
/// </summary>
public abstract class BaseEntity : IDeleted
{
    /// <summary>
    /// 主键ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public virtual long Id { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnDescription = "创建时间")]
    public virtual DateTime CreateTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    [SugarColumn(ColumnDescription = "更新时间")]
    public virtual DateTime UpdateTime { get; set; }

    /// <summary>
    /// 创建用户ID
    /// </summary>
    [SugarColumn(ColumnDescription = "创建用户ID")]
    public virtual long? CreateUserId { get; set; }

    /// <summary>
    /// 更新用户ID
    /// </summary>
    [SugarColumn(ColumnDescription = "更新用户ID")]
    public virtual long? UpdateUserId { get; set; }

    /// <summary>
    /// 是否删除
    /// </summary>
    [SugarColumn(ColumnDescription = "是否删除")]
    public virtual bool IsDeleted { get; set; }

    /// <summary>
    /// 删除时间
    /// </summary>
    [SugarColumn(ColumnDescription = "删除时间")]
    public virtual DateTime? DeleteTime { get; set; }

    /// <summary>
    /// 删除用户ID
    /// </summary>
    [SugarColumn(ColumnDescription = "删除用户ID")]
    public virtual long? DeleteUserId { get; set; }
}

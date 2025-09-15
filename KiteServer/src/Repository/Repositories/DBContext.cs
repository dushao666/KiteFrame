using Domain.Entities;
using Domain.Entities.Base;
using Domain.Interfaces;
using System.Linq.Expressions;

namespace Repository;

/// <summary>
/// 数据库上下文
/// </summary>
public class DBContext : SugarUnitOfWork
{
    /// <summary>
    /// 用户
    /// </summary>
    public DbSet<User> Users { get; set; } = null!;

    /// <summary>
    /// 角色
    /// </summary>
    public DbSet<Role> Roles { get; set; } = null!;

    /// <summary>
    /// 用户角色关联
    /// </summary>
    public SimpleClient<UserRole> UserRoles { get; set; } = null!;

    /// <summary>
    /// 菜单
    /// </summary>
    public DbSet<Menu> Menus { get; set; } = null!;

    /// <summary>
    /// 角色菜单关联
    /// </summary>
    public SimpleClient<RoleMenu> RoleMenus { get; set; } = null!;
}

/// <summary>
/// 数据集合
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
public class DbSet<T> : SimpleClient<T> where T : class, IDeleted, new()
{
    /// <summary>
    /// 逻辑删除
    /// </summary>
    public override bool Delete(T deleteObj)
    {
        deleteObj.IsDeleted = true;
        if (deleteObj is BaseEntity baseEntity)
        {
            baseEntity.DeleteTime = DateTime.Now;
        }
        return Context.Updateable(deleteObj).ExecuteCommand() > 0;
    }

    /// <summary>
    /// 逻辑删除
    /// </summary>
    public override async Task<bool> DeleteAsync(T deleteObj)
    {
        deleteObj.IsDeleted = true;
        if (deleteObj is BaseEntity baseEntity)
        {
            baseEntity.DeleteTime = DateTime.Now;
        }
        return await Context.Updateable(deleteObj).ExecuteCommandAsync() > 0;
    }

    /// <summary>
    /// 逻辑删除
    /// </summary>
    public override bool Delete(Expression<Func<T, bool>> exp)
    {
        return Context.Updateable<T>().SetColumns(it => new T() { IsDeleted = true }, true).Where(exp).ExecuteCommand() > 0;
    }

    /// <summary>
    /// 逻辑删除
    /// </summary>
    public override async Task<bool> DeleteAsync(Expression<Func<T, bool>> exp)
    {
        return await Context.Updateable<T>().SetColumns(it => new T() { IsDeleted = true }, true).Where(exp).ExecuteCommandAsync() > 0;
    }
}

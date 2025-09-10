using Domain.Entities;
using Domain.Interfaces;
using System.Linq.Expressions;

namespace Repository;

public class DBContext : SugarUnitOfWork
{
    /// <summary>
    /// 用户
    /// </summary>
    public DbSet<User> Users { get; set; }
}

public class DbSet<T> : SimpleClient<T> where T : class, IDeleted, new()
{
    /// <summary>
    /// 逻辑删除
    /// </summary>
    public override bool Delete(T deleteObj)
    {
        deleteObj.IsDeleted = true;
        return Context.Updateable(deleteObj).ExecuteCommand() > 0;
    }
    
    /// <summary>
    /// 逻辑删除
    /// </summary>
    public override async Task<bool> DeleteAsync(T deleteObj)
    {
        deleteObj.IsDeleted = true;
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

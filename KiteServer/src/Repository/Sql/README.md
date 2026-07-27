-- =============================================
-- 创建视图和存储过程（可选）
-- =============================================

-- 用户权限视图（方便查询用户的所有权限）
CREATE OR REPLACE VIEW `v_user_permissions` AS
SELECT
u.Id AS UserId,
u.UserName,
u.RealName,
d.Id AS DepartmentId,
d.DepartmentName,
d.DepartmentCode,
r.Id AS RoleId,
r.RoleName,
r.RoleCode,
m.Id AS MenuId,
m.MenuName,
m.MenuCode,
m.MenuType,
m.Path,
m.Component,
m.Permissions
FROM sys_user u
LEFT JOIN sys_department d ON u.DepartmentId = d.Id
LEFT JOIN sys_user_role ur ON u.Id = ur.UserId
LEFT JOIN sys_role r ON ur.RoleId = r.Id
LEFT JOIN sys_role_menu rm ON r.Id = rm.RoleId
LEFT JOIN sys_menu m ON rm.MenuId = m.Id
WHERE u.Status = 1 AND u.IsDeleted = 0
AND (d.Status = 1 AND d.IsDeleted = 0 OR d.Id IS NULL)
AND r.Status = 1 AND r.IsDeleted = 0
AND m.Status = 1 AND m.IsDeleted = 0;

-- 部门树形结构视图
CREATE OR REPLACE VIEW `v_department_tree` AS
SELECT
d.Id,
d.ParentId,
d.DepartmentName,
d.DepartmentCode,
d.DepartmentType,
d.LeaderId,
u.RealName AS LeaderName,
d.Phone,
d.Email,
d.Sort,
d.Status,
d.Remark,
d.CreateTime,
(SELECT COUNT(*) FROM sys_user WHERE DepartmentId = d.Id AND IsDeleted = 0) AS UserCount
FROM sys_department d
LEFT JOIN sys_user u ON d.LeaderId = u.Id
WHERE d.IsDeleted = 0;

-- =============================================
-- 数据完整性检查
-- =============================================

-- 检查数据插入是否成功
SELECT
'用户数量' as 检查项, COUNT(*) as 数量 FROM sys_user WHERE IsDeleted = 0
UNION ALL
SELECT
'角色数量' as 检查项, COUNT(*) as 数量 FROM sys_role WHERE IsDeleted = 0
UNION ALL
SELECT
'部门数量' as 检查项, COUNT(*) as 数量 FROM sys_department WHERE IsDeleted = 0
UNION ALL
SELECT
'菜单数量' as 检查项, COUNT(*) as 数量 FROM sys_menu WHERE IsDeleted = 0
UNION ALL
SELECT
'用户角色关联数量' as 检查项, COUNT(*) as 数量 FROM sys_user_role
UNION ALL
SELECT
'角色菜单权限数量' as 检查项, COUNT(*) as 数量 FROM sys_role_menu;

-- =============================================
-- 使用说明
-- =============================================
/*
权限管理系统说明：

1. 用户管理：
    - 默认管理员账户：admin / 123456
    - 测试用户：zhangsan, lisi, wangwu, zhaoliu / 123456
    - 支持用户的增删改查、状态管理、密码重置、部门分配等功能

2. 角色管理：
    - 超级管理员：拥有所有系统权限
    - 系统管理员：拥有系统管理相关权限
    - 普通用户：只有基础查看权限

3. 部门管理：
    - 支持树形结构的部门管理
    - 部门类型：公司(1)、部门(2)、小组(3)
    - 支持设置部门负责人、联系方式等信息
    - 用户可以分配到具体部门

4. 菜单管理：
    - 支持三级菜单结构：目录(1) -> 菜单(2) -> 按钮(3)
    - 每个菜单可配置路由、组件、图标、权限标识等
    - 支持菜单的显示/隐藏、缓存控制、外链设置等

5. 权限控制：
    - 基于RBAC模型：用户 -> 角色 -> 菜单权限
    - 支持页面级权限和按钮级权限控制
    - 权限标识格式：模块:功能:操作（如：system:user:add）

6. 数据权限：
    - 全部数据权限(1)：可查看所有数据
    - 自定义数据权限(2)：可自定义数据范围
    - 本部门数据权限(3)：只能查看本部门数据
    - 本部门及以下数据权限(4)：可查看本部门及下级部门数据
    - 仅本人数据权限(5)：只能查看自己的数据

7. 系统监控：
    - 在线用户监控：查看当前在线用户，支持强制下线
    - 登录日志：记录用户登录行为
    - 操作日志：记录用户操作行为

8. 系统工具：
    - 代码生成：根据数据库表结构生成CRUD代码
    - 支持预览、下载生成的代码

9. 测试数据说明：
    - 公司：KiteFrame科技有限公司
    - 部门结构：技术部、产品部、运营部、人事部
    - 技术部下设：前端开发组、后端开发组、测试组、运维组
    - 产品部下设：UI设计组、产品策划组
    - 测试用户分配到不同部门进行测试

使用建议：
- 生产环境请修改默认管理员密码
- 根据实际业务需求调整菜单结构和权限配置
- 定期清理系统日志，避免数据过多影响性能
- 建议为不同业务模块创建对应的角色和权限
- 合理设置部门结构，便于数据权限控制
  */
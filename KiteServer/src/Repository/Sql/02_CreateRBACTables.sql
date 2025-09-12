-- =============================================
-- KiteServer RBAC权限管理表建表脚本
-- 创建时间: 2025-09-12
-- 描述: 创建基于角色的访问控制(RBAC)相关表结构
-- =============================================

-- 使用测试数据库
USE `kite_test`;

-- =============================================
-- 菜单表 (sys_menu)
-- =============================================
DROP TABLE IF EXISTS `sys_menu`;
CREATE TABLE `sys_menu` (
  `Id` bigint NOT NULL AUTO_INCREMENT COMMENT '主键ID',
  `ParentId` bigint DEFAULT 0 COMMENT '父菜单ID（0表示根菜单）',
  `MenuName` varchar(50) NOT NULL COMMENT '菜单名称',
  `MenuCode` varchar(100) NOT NULL COMMENT '菜单编码（唯一标识）',
  `MenuType` int NOT NULL DEFAULT 1 COMMENT '菜单类型（1：目录，2：菜单，3：按钮）',
  `Path` varchar(200) DEFAULT NULL COMMENT '路由路径',
  `Component` varchar(200) DEFAULT NULL COMMENT '组件路径',
  `Icon` varchar(100) DEFAULT NULL COMMENT '菜单图标',
  `Sort` int NOT NULL DEFAULT 0 COMMENT '排序号',
  `IsVisible` tinyint(1) NOT NULL DEFAULT 1 COMMENT '是否显示（0：隐藏，1：显示）',
  `IsCache` tinyint(1) NOT NULL DEFAULT 0 COMMENT '是否缓存（0：不缓存，1：缓存）',
  `IsFrame` tinyint(1) NOT NULL DEFAULT 0 COMMENT '是否外链（0：否，1：是）',
  `Status` int NOT NULL DEFAULT 1 COMMENT '状态（0：禁用，1：启用）',
  `Permissions` varchar(500) DEFAULT NULL COMMENT '权限标识（多个用逗号分隔）',
  `Remark` varchar(500) DEFAULT NULL COMMENT '备注',
  `CreateTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `UpdateTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `CreateUserId` bigint DEFAULT NULL COMMENT '创建用户ID',
  `UpdateUserId` bigint DEFAULT NULL COMMENT '更新用户ID',
  `IsDeleted` tinyint(1) NOT NULL DEFAULT 0 COMMENT '是否删除（逻辑删除）',
  `DeleteTime` datetime DEFAULT NULL COMMENT '删除时间',
  `DeleteUserId` bigint DEFAULT NULL COMMENT '删除用户ID',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_sys_menu_code` (`MenuCode`),
  KEY `IDX_sys_menu_parent` (`ParentId`),
  KEY `IDX_sys_menu_type` (`MenuType`),
  KEY `IDX_sys_menu_status` (`Status`),
  KEY `IDX_sys_menu_sort` (`Sort`),
  KEY `IDX_sys_menu_isdeleted` (`IsDeleted`),
  KEY `IDX_sys_menu_createtime` (`CreateTime`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='菜单表';

-- =============================================
-- 角色表 (sys_role)
-- =============================================
DROP TABLE IF EXISTS `sys_role`;
CREATE TABLE `sys_role` (
  `Id` bigint NOT NULL AUTO_INCREMENT COMMENT '主键ID',
  `RoleName` varchar(50) NOT NULL COMMENT '角色名称',
  `RoleCode` varchar(100) NOT NULL COMMENT '角色编码（唯一标识）',
  `Sort` int NOT NULL DEFAULT 0 COMMENT '排序号',
  `Status` int NOT NULL DEFAULT 1 COMMENT '状态（0：禁用，1：启用）',
  `DataScope` int NOT NULL DEFAULT 1 COMMENT '数据权限范围（1：全部，2：自定义，3：本部门，4：本部门及以下，5：仅本人）',
  `Remark` varchar(500) DEFAULT NULL COMMENT '备注',
  `CreateTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `UpdateTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `CreateUserId` bigint DEFAULT NULL COMMENT '创建用户ID',
  `UpdateUserId` bigint DEFAULT NULL COMMENT '更新用户ID',
  `IsDeleted` tinyint(1) NOT NULL DEFAULT 0 COMMENT '是否删除（逻辑删除）',
  `DeleteTime` datetime DEFAULT NULL COMMENT '删除时间',
  `DeleteUserId` bigint DEFAULT NULL COMMENT '删除用户ID',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_sys_role_code` (`RoleCode`),
  KEY `IDX_sys_role_name` (`RoleName`),
  KEY `IDX_sys_role_status` (`Status`),
  KEY `IDX_sys_role_sort` (`Sort`),
  KEY `IDX_sys_role_isdeleted` (`IsDeleted`),
  KEY `IDX_sys_role_createtime` (`CreateTime`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='角色表';

-- =============================================
-- 用户角色关联表 (sys_user_role)
-- =============================================
DROP TABLE IF EXISTS `sys_user_role`;
CREATE TABLE `sys_user_role` (
  `Id` bigint NOT NULL AUTO_INCREMENT COMMENT '主键ID',
  `UserId` bigint NOT NULL COMMENT '用户ID',
  `RoleId` bigint NOT NULL COMMENT '角色ID',
  `CreateTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `CreateUserId` bigint DEFAULT NULL COMMENT '创建用户ID',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_sys_user_role` (`UserId`, `RoleId`),
  KEY `IDX_sys_user_role_user` (`UserId`),
  KEY `IDX_sys_user_role_role` (`RoleId`),
  KEY `IDX_sys_user_role_createtime` (`CreateTime`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='用户角色关联表';

-- =============================================
-- 角色菜单权限表 (sys_role_menu)
-- =============================================
DROP TABLE IF EXISTS `sys_role_menu`;
CREATE TABLE `sys_role_menu` (
  `Id` bigint NOT NULL AUTO_INCREMENT COMMENT '主键ID',
  `RoleId` bigint NOT NULL COMMENT '角色ID',
  `MenuId` bigint NOT NULL COMMENT '菜单ID',
  `CreateTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `CreateUserId` bigint DEFAULT NULL COMMENT '创建用户ID',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_sys_role_menu` (`RoleId`, `MenuId`),
  KEY `IDX_sys_role_menu_role` (`RoleId`),
  KEY `IDX_sys_role_menu_menu` (`MenuId`),
  KEY `IDX_sys_role_menu_createtime` (`CreateTime`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='角色菜单权限表';

-- =============================================
-- 插入初始化数据
-- =============================================

-- 插入默认角色
INSERT INTO `sys_role` (`RoleName`, `RoleCode`, `Sort`, `Status`, `DataScope`, `Remark`, `CreateUserId`) VALUES
('超级管理员', 'admin', 1, 1, 1, '系统超级管理员，拥有所有权限', 1),
('普通用户', 'user', 2, 1, 5, '普通用户角色', 1);

-- 插入系统菜单
INSERT INTO `sys_menu` (`ParentId`, `MenuName`, `MenuCode`, `MenuType`, `Path`, `Component`, `Icon`, `Sort`, `IsVisible`, `IsCache`, `IsFrame`, `Status`, `Permissions`, `Remark`, `CreateUserId`) VALUES
-- 一级菜单
(0, '系统管理', 'system', 1, '/system', NULL, 'ep:setting', 1, 1, 0, 0, 1, NULL, '系统管理目录', 1),
(0, '首页', 'dashboard', 2, '/welcome', '/welcome/index', 'ep:home-filled', 0, 1, 1, 0, 1, 'dashboard:view', '系统首页', 1),

-- 系统管理子菜单
(1, '用户管理', 'system:user', 2, '/system/user', '/system/user/index', 'ep:user', 1, 1, 1, 0, 1, 'system:user:list', '用户管理页面', 1),
(1, '角色管理', 'system:role', 2, '/system/role', '/system/role/index', 'ep:user-filled', 2, 1, 1, 0, 1, 'system:role:list', '角色管理页面', 1),
(1, '菜单管理', 'system:menu', 2, '/system/menu', '/system/menu/index', 'ep:menu', 3, 1, 1, 0, 1, 'system:menu:list', '菜单管理页面', 1),

-- 用户管理按钮权限
(3, '新增用户', 'system:user:add', 3, NULL, NULL, NULL, 1, 0, 0, 0, 1, 'system:user:add', '新增用户按钮', 1),
(3, '编辑用户', 'system:user:edit', 3, NULL, NULL, NULL, 2, 0, 0, 0, 1, 'system:user:edit', '编辑用户按钮', 1),
(3, '删除用户', 'system:user:delete', 3, NULL, NULL, NULL, 3, 0, 0, 0, 1, 'system:user:delete', '删除用户按钮', 1),
(3, '重置密码', 'system:user:resetpwd', 3, NULL, NULL, NULL, 4, 0, 0, 0, 1, 'system:user:resetpwd', '重置密码按钮', 1),

-- 角色管理按钮权限
(4, '新增角色', 'system:role:add', 3, NULL, NULL, NULL, 1, 0, 0, 0, 1, 'system:role:add', '新增角色按钮', 1),
(4, '编辑角色', 'system:role:edit', 3, NULL, NULL, NULL, 2, 0, 0, 0, 1, 'system:role:edit', '编辑角色按钮', 1),
(4, '删除角色', 'system:role:delete', 3, NULL, NULL, NULL, 3, 0, 0, 0, 1, 'system:role:delete', '删除角色按钮', 1),
(4, '分配权限', 'system:role:auth', 3, NULL, NULL, NULL, 4, 0, 0, 0, 1, 'system:role:auth', '分配权限按钮', 1),

-- 菜单管理按钮权限
(5, '新增菜单', 'system:menu:add', 3, NULL, NULL, NULL, 1, 0, 0, 0, 1, 'system:menu:add', '新增菜单按钮', 1),
(5, '编辑菜单', 'system:menu:edit', 3, NULL, NULL, NULL, 2, 0, 0, 0, 1, 'system:menu:edit', '编辑菜单按钮', 1),
(5, '删除菜单', 'system:menu:delete', 3, NULL, NULL, NULL, 3, 0, 0, 0, 1, 'system:menu:delete', '删除菜单按钮', 1);

-- 给admin用户分配超级管理员角色
INSERT INTO `sys_user_role` (`UserId`, `RoleId`, `CreateUserId`) VALUES (1, 1, 1);

-- 给超级管理员角色分配所有菜单权限
INSERT INTO `sys_role_menu` (`RoleId`, `MenuId`, `CreateUserId`)
SELECT 1, Id, 1 FROM `sys_menu` WHERE `Status` = 1 AND `IsDeleted` = 0;

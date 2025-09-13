-- =============================================
-- KiteServer 权限管理系统数据库建表脚本
-- 创建时间: 2025-01-13
-- 描述: 创建完整的RBAC权限管理系统表结构
-- =============================================

-- 创建数据库（如果不存在）
CREATE DATABASE IF NOT EXISTS `kite_test` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE DATABASE IF NOT EXISTS `kite_prod` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE DATABASE IF NOT EXISTS `kite_log` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- 使用测试数据库
USE `kite_test`;

-- =============================================
-- 用户表 (sys_user)
-- =============================================
DROP TABLE IF EXISTS `sys_user`;
CREATE TABLE `sys_user` (
  `Id` bigint NOT NULL AUTO_INCREMENT COMMENT '主键ID',
  `UserName` varchar(50) NOT NULL COMMENT '用户名',
  `Password` varchar(200) NOT NULL COMMENT '密码',
  `Email` varchar(100) DEFAULT NULL COMMENT '邮箱',
  `Phone` varchar(20) DEFAULT NULL COMMENT '手机号',
  `RealName` varchar(50) DEFAULT NULL COMMENT '真实姓名',
  `DingTalkId` varchar(100) DEFAULT NULL COMMENT '钉钉用户ID',
  `Avatar` varchar(200) DEFAULT NULL COMMENT '头像',
  `Status` int NOT NULL DEFAULT '1' COMMENT '状态（0：禁用，1：启用）',
  `LastLoginTime` datetime DEFAULT NULL COMMENT '最后登录时间',
  `LastLoginIp` varchar(50) DEFAULT NULL COMMENT '最后登录IP',
  `Remark` varchar(500) DEFAULT NULL COMMENT '备注',
  `CreateTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `UpdateTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `CreateUserId` bigint DEFAULT NULL COMMENT '创建用户ID',
  `UpdateUserId` bigint DEFAULT NULL COMMENT '更新用户ID',
  `IsDeleted` tinyint(1) NOT NULL DEFAULT '0' COMMENT '是否删除',
  `DeleteTime` datetime DEFAULT NULL COMMENT '删除时间',
  `DeleteUserId` bigint DEFAULT NULL COMMENT '删除用户ID',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_sys_user_username` (`UserName`),
  UNIQUE KEY `UK_sys_user_dingtalkid` (`DingTalkId`),
  KEY `IDX_sys_user_email` (`Email`),
  KEY `IDX_sys_user_phone` (`Phone`),
  KEY `IDX_sys_user_status` (`Status`),
  KEY `IDX_sys_user_isdeleted` (`IsDeleted`),
  KEY `IDX_sys_user_createtime` (`CreateTime`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='用户表';

-- =============================================
-- 角色表 (sys_role)
-- =============================================
DROP TABLE IF EXISTS `sys_role`;
CREATE TABLE `sys_role` (
  `Id` bigint NOT NULL AUTO_INCREMENT COMMENT '主键ID',
  `RoleName` varchar(50) NOT NULL COMMENT '角色名称',
  `RoleCode` varchar(100) NOT NULL COMMENT '角色编码（唯一标识）',
  `Sort` int NOT NULL DEFAULT '0' COMMENT '排序号',
  `Status` int NOT NULL DEFAULT '1' COMMENT '状态（0：禁用，1：启用）',
  `DataScope` int NOT NULL DEFAULT '1' COMMENT '数据权限范围（1：全部，2：自定义，3：本部门，4：本部门及以下，5：仅本人）',
  `Remark` varchar(500) DEFAULT NULL COMMENT '备注',
  `CreateTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `UpdateTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `CreateUserId` bigint DEFAULT NULL COMMENT '创建用户ID',
  `UpdateUserId` bigint DEFAULT NULL COMMENT '更新用户ID',
  `IsDeleted` tinyint(1) NOT NULL DEFAULT '0' COMMENT '是否删除',
  `DeleteTime` datetime DEFAULT NULL COMMENT '删除时间',
  `DeleteUserId` bigint DEFAULT NULL COMMENT '删除用户ID',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_sys_role_rolecode` (`RoleCode`),
  KEY `IDX_sys_role_status` (`Status`),
  KEY `IDX_sys_role_sort` (`Sort`),
  KEY `IDX_sys_role_isdeleted` (`IsDeleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='角色表';

-- =============================================
-- 菜单表 (sys_menu)
-- =============================================
DROP TABLE IF EXISTS `sys_menu`;
CREATE TABLE `sys_menu` (
  `Id` bigint NOT NULL AUTO_INCREMENT COMMENT '主键ID',
  `ParentId` bigint NOT NULL DEFAULT '0' COMMENT '父菜单ID（0表示根菜单）',
  `MenuName` varchar(50) NOT NULL COMMENT '菜单名称',
  `MenuCode` varchar(100) NOT NULL COMMENT '菜单编码（唯一标识）',
  `MenuType` int NOT NULL DEFAULT '2' COMMENT '菜单类型（1：目录，2：菜单，3：按钮）',
  `Path` varchar(200) DEFAULT NULL COMMENT '路由路径',
  `Component` varchar(200) DEFAULT NULL COMMENT '组件路径',
  `Icon` varchar(100) DEFAULT NULL COMMENT '菜单图标',
  `Sort` int NOT NULL DEFAULT '0' COMMENT '排序号',
  `IsVisible` tinyint(1) NOT NULL DEFAULT '1' COMMENT '是否显示（0：隐藏，1：显示）',
  `IsCache` tinyint(1) NOT NULL DEFAULT '0' COMMENT '是否缓存（0：不缓存，1：缓存）',
  `IsFrame` tinyint(1) NOT NULL DEFAULT '0' COMMENT '是否外链（0：否，1：是）',
  `Status` int NOT NULL DEFAULT '1' COMMENT '状态（0：禁用，1：启用）',
  `Permissions` varchar(500) DEFAULT NULL COMMENT '权限标识（多个用逗号分隔）',
  `Remark` varchar(500) DEFAULT NULL COMMENT '备注',
  `CreateTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `UpdateTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `CreateUserId` bigint DEFAULT NULL COMMENT '创建用户ID',
  `UpdateUserId` bigint DEFAULT NULL COMMENT '更新用户ID',
  `IsDeleted` tinyint(1) NOT NULL DEFAULT '0' COMMENT '是否删除',
  `DeleteTime` datetime DEFAULT NULL COMMENT '删除时间',
  `DeleteUserId` bigint DEFAULT NULL COMMENT '删除用户ID',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_sys_menu_menucode` (`MenuCode`),
  KEY `IDX_sys_menu_parentid` (`ParentId`),
  KEY `IDX_sys_menu_menutype` (`MenuType`),
  KEY `IDX_sys_menu_status` (`Status`),
  KEY `IDX_sys_menu_sort` (`Sort`),
  KEY `IDX_sys_menu_isdeleted` (`IsDeleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='菜单表';

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
  KEY `IDX_sys_user_role_userid` (`UserId`),
  KEY `IDX_sys_user_role_roleid` (`RoleId`),
  CONSTRAINT `FK_sys_user_role_userid` FOREIGN KEY (`UserId`) REFERENCES `sys_user` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_sys_user_role_roleid` FOREIGN KEY (`RoleId`) REFERENCES `sys_role` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='用户角色关联表';

-- =============================================
-- 角色菜单权限关联表 (sys_role_menu)
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
  KEY `IDX_sys_role_menu_roleid` (`RoleId`),
  KEY `IDX_sys_role_menu_menuid` (`MenuId`),
  CONSTRAINT `FK_sys_role_menu_roleid` FOREIGN KEY (`RoleId`) REFERENCES `sys_role` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_sys_role_menu_menuid` FOREIGN KEY (`MenuId`) REFERENCES `sys_menu` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='角色菜单权限关联表';

-- =============================================
-- 初始化数据
-- =============================================

-- 插入默认管理员用户
INSERT INTO `sys_user` (
  `Id`,
  `UserName`,
  `Password`,
  `Email`,
  `RealName`,
  `Status`,
  `Remark`
) VALUES (
  1,
  'admin',
  'ba3253876aed6bc22d4a6ff53d8406c6ad864195ed144ab5c87621b6c233b548baeae6956df346ec8c17f5ea10f35ee3cbc514797ed7ddd3145464e2a0bab413', -- 密码: 123456 (SHA512加密)
  'admin@kiteserver.com',
  '系统管理员',
  1,
  '系统默认管理员账户'
);

-- 插入默认角色
INSERT INTO `sys_role` (
  `Id`,
  `RoleName`,
  `RoleCode`,
  `Sort`,
  `Status`,
  `DataScope`,
  `Remark`
) VALUES
(1, '超级管理员', 'admin', 1, 1, 1, '系统超级管理员，拥有所有权限'),
(2, '系统管理员', 'system', 2, 1, 1, '系统管理员，拥有系统管理权限'),
(3, '普通用户', 'user', 3, 1, 5, '普通用户，只能查看基础信息');

-- 插入系统菜单
INSERT INTO `sys_menu` (
  `Id`,
  `ParentId`,
  `MenuName`,
  `MenuCode`,
  `MenuType`,
  `Path`,
  `Component`,
  `Icon`,
  `Sort`,
  `IsVisible`,
  `IsCache`,
  `IsFrame`,
  `Status`,
  `Permissions`,
  `Remark`
) VALUES
-- 一级菜单：首页
(1, 0, '首页', 'dashboard', 2, '/welcome', '/src/views/welcome/index.vue', 'ep:home-filled', 1, 1, 1, 0, 1, 'dashboard:view', '系统首页'),

-- 一级菜单：系统管理
(2, 0, '系统管理', 'system', 1, '/system', NULL, 'ep:setting', 2, 1, 0, 0, 1, NULL, '系统管理目录'),

-- 二级菜单：用户管理
(3, 2, '用户管理', 'system-user', 2, '/system/user', '/src/views/system/user/index.vue', 'ep:user', 1, 1, 1, 0, 1, 'system:user:list', '用户管理页面'),
(4, 3, '新增用户', 'system-user-add', 3, NULL, NULL, NULL, 1, 1, 0, 0, 1, 'system:user:add', '新增用户按钮'),
(5, 3, '编辑用户', 'system-user-edit', 3, NULL, NULL, NULL, 2, 1, 0, 0, 1, 'system:user:edit', '编辑用户按钮'),
(6, 3, '删除用户', 'system-user-delete', 3, NULL, NULL, NULL, 3, 1, 0, 0, 1, 'system:user:delete', '删除用户按钮'),
(7, 3, '重置密码', 'system-user-reset', 3, NULL, NULL, NULL, 4, 1, 0, 0, 1, 'system:user:reset', '重置用户密码按钮'),
(8, 3, '分配角色', 'system-user-role', 3, NULL, NULL, NULL, 5, 1, 0, 0, 1, 'system:user:role', '分配用户角色按钮'),

-- 二级菜单：角色管理
(9, 2, '角色管理', 'system-role', 2, '/system/role', '/src/views/system/role/index.vue', 'ep:avatar', 2, 1, 1, 0, 1, 'system:role:list', '角色管理页面'),
(10, 9, '新增角色', 'system-role-add', 3, NULL, NULL, NULL, 1, 1, 0, 0, 1, 'system:role:add', '新增角色按钮'),
(11, 9, '编辑角色', 'system-role-edit', 3, NULL, NULL, NULL, 2, 1, 0, 0, 1, 'system:role:edit', '编辑角色按钮'),
(12, 9, '删除角色', 'system-role-delete', 3, NULL, NULL, NULL, 3, 1, 0, 0, 1, 'system:role:delete', '删除角色按钮'),
(13, 9, '分配权限', 'system-role-permission', 3, NULL, NULL, NULL, 4, 1, 0, 0, 1, 'system:role:permission', '分配角色权限按钮'),

-- 二级菜单：菜单管理
(14, 2, '菜单管理', 'system-menu', 2, '/system/menu', '/src/views/system/menu/index.vue', 'ep:menu', 3, 1, 1, 0, 1, 'system:menu:list', '菜单管理页面'),
(15, 14, '新增菜单', 'system-menu-add', 3, NULL, NULL, NULL, 1, 1, 0, 0, 1, 'system:menu:add', '新增菜单按钮'),
(16, 14, '编辑菜单', 'system-menu-edit', 3, NULL, NULL, NULL, 2, 1, 0, 0, 1, 'system:menu:edit', '编辑菜单按钮'),
(17, 14, '删除菜单', 'system-menu-delete', 3, NULL, NULL, NULL, 3, 1, 0, 0, 1, 'system:menu:delete', '删除菜单按钮'),

-- 一级菜单：权限管理
(18, 0, '权限管理', 'permission', 1, '/permission', NULL, 'ep:lock', 3, 1, 0, 0, 1, NULL, '权限管理目录'),

-- 二级菜单：页面权限
(19, 18, '页面权限', 'permission-page', 2, '/permission/page', '/src/views/permission/page/index.vue', 'ep:document', 1, 1, 1, 0, 1, 'permission:page:view', '页面权限演示'),

-- 二级菜单：按钮权限
(20, 18, '按钮权限', 'permission-button', 2, '/permission/button', '/src/views/permission/button/index.vue', 'ep:key', 2, 1, 1, 0, 1, 'permission:button:view', '按钮权限演示'),

-- 一级菜单：系统监控
(21, 0, '系统监控', 'monitor', 1, '/monitor', NULL, 'ep:monitor', 4, 1, 0, 0, 1, NULL, '系统监控目录'),

-- 二级菜单：在线用户
(22, 21, '在线用户', 'monitor-online', 2, '/monitor/online', '/src/views/monitor/online/index.vue', 'ep:user-filled', 1, 1, 1, 0, 1, 'monitor:online:list', '在线用户监控'),
(23, 22, '强制下线', 'monitor-online-logout', 3, NULL, NULL, NULL, 1, 1, 0, 0, 1, 'monitor:online:logout', '强制用户下线按钮'),

-- 二级菜单：登录日志
(24, 21, '登录日志', 'monitor-loginlog', 2, '/monitor/loginlog', '/src/views/monitor/loginlog/index.vue', 'ep:document-checked', 2, 1, 1, 0, 1, 'monitor:loginlog:list', '登录日志查看'),
(25, 24, '删除日志', 'monitor-loginlog-delete', 3, NULL, NULL, NULL, 1, 1, 0, 0, 1, 'monitor:loginlog:delete', '删除登录日志按钮'),
(26, 24, '清空日志', 'monitor-loginlog-clear', 3, NULL, NULL, NULL, 2, 1, 0, 0, 1, 'monitor:loginlog:clear', '清空登录日志按钮'),

-- 二级菜单：操作日志
(27, 21, '操作日志', 'monitor-operlog', 2, '/monitor/operlog', '/src/views/monitor/operlog/index.vue', 'ep:document', 3, 1, 1, 0, 1, 'monitor:operlog:list', '操作日志查看'),
(28, 27, '删除日志', 'monitor-operlog-delete', 3, NULL, NULL, NULL, 1, 1, 0, 0, 1, 'monitor:operlog:delete', '删除操作日志按钮'),
(29, 27, '清空日志', 'monitor-operlog-clear', 3, NULL, NULL, NULL, 2, 1, 0, 0, 1, 'monitor:operlog:clear', '清空操作日志按钮'),

-- 一级菜单：系统工具
(30, 0, '系统工具', 'tool', 1, '/tool', NULL, 'ep:tools', 5, 1, 0, 0, 1, NULL, '系统工具目录'),

-- 二级菜单：代码生成
(31, 30, '代码生成', 'tool-gen', 2, '/tool/gen', '/src/views/tool/gen/index.vue', 'ep:cpu', 1, 1, 1, 0, 1, 'tool:gen:list', '代码生成工具'),
(32, 31, '生成代码', 'tool-gen-code', 3, NULL, NULL, NULL, 1, 1, 0, 0, 1, 'tool:gen:code', '生成代码按钮'),
(33, 31, '预览代码', 'tool-gen-preview', 3, NULL, NULL, NULL, 2, 1, 0, 0, 1, 'tool:gen:preview', '预览代码按钮'),
(34, 31, '下载代码', 'tool-gen-download', 3, NULL, NULL, NULL, 3, 1, 0, 0, 1, 'tool:gen:download', '下载代码按钮');

-- 分配用户角色（管理员用户分配超级管理员角色）
INSERT INTO `sys_user_role` (`UserId`, `RoleId`, `CreateUserId`) VALUES
(1, 1, 1);

-- 分配角色菜单权限（超级管理员拥有所有菜单权限）
INSERT INTO `sys_role_menu` (`RoleId`, `MenuId`, `CreateUserId`) VALUES
-- 超级管理员拥有所有菜单权限
(1, 1, 1), (1, 2, 1), (1, 3, 1), (1, 4, 1), (1, 5, 1), (1, 6, 1), (1, 7, 1), (1, 8, 1),
(1, 9, 1), (1, 10, 1), (1, 11, 1), (1, 12, 1), (1, 13, 1), (1, 14, 1), (1, 15, 1), (1, 16, 1), (1, 17, 1),
(1, 18, 1), (1, 19, 1), (1, 20, 1), (1, 21, 1), (1, 22, 1), (1, 23, 1), (1, 24, 1), (1, 25, 1), (1, 26, 1),
(1, 27, 1), (1, 28, 1), (1, 29, 1), (1, 30, 1), (1, 31, 1), (1, 32, 1), (1, 33, 1), (1, 34, 1),

-- 系统管理员拥有系统管理相关权限
(2, 1, 1), (2, 2, 1), (2, 3, 1), (2, 4, 1), (2, 5, 1), (2, 6, 1), (2, 7, 1), (2, 8, 1),
(2, 9, 1), (2, 10, 1), (2, 11, 1), (2, 12, 1), (2, 13, 1), (2, 14, 1), (2, 15, 1), (2, 16, 1), (2, 17, 1),
(2, 21, 1), (2, 22, 1), (2, 24, 1), (2, 27, 1),

-- 普通用户只有基础查看权限
(3, 1, 1), (3, 18, 1), (3, 19, 1), (3, 20, 1);


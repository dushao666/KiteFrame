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

-- 设置字符集
SET NAMES utf8mb4;
SET CHARACTER SET utf8mb4;

-- =============================================
-- 用户表 (sys_user)
-- =============================================
DROP TABLE IF EXISTS `sys_user`;
CREATE TABLE `sys_user` (
  `Id` bigint NOT NULL AUTO_INCREMENT COMMENT 'Primary Key ID',
  `UserName` varchar(50) NOT NULL COMMENT 'Username',
  `Password` varchar(200) NOT NULL COMMENT 'Password',
  `Email` varchar(100) DEFAULT NULL COMMENT 'Email',
  `Phone` varchar(20) DEFAULT NULL COMMENT 'Phone Number',
  `RealName` varchar(100) DEFAULT NULL COMMENT 'Real Name',
  `DingTalkId` varchar(100) DEFAULT NULL COMMENT 'DingTalk User ID',
  `Avatar` varchar(200) DEFAULT NULL COMMENT 'Avatar',
  `DepartmentId` bigint DEFAULT NULL COMMENT 'Department ID',
  `Status` int NOT NULL DEFAULT '1' COMMENT 'Status (0:Disabled, 1:Enabled)',
  `LastLoginTime` datetime DEFAULT NULL COMMENT 'Last Login Time',
  `LastLoginIp` varchar(50) DEFAULT NULL COMMENT 'Last Login IP',
  `Remark` varchar(500) DEFAULT NULL COMMENT 'Remark',
  `CreateTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Create Time',
  `UpdateTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'Update Time',
  `CreateUserId` bigint DEFAULT NULL COMMENT 'Create User ID',
  `UpdateUserId` bigint DEFAULT NULL COMMENT 'Update User ID',
  `IsDeleted` tinyint(1) NOT NULL DEFAULT '0' COMMENT 'Is Deleted',
  `DeleteTime` datetime DEFAULT NULL COMMENT 'Delete Time',
  `DeleteUserId` bigint DEFAULT NULL COMMENT 'Delete User ID',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_sys_user_username` (`UserName`),
  UNIQUE KEY `UK_sys_user_dingtalkid` (`DingTalkId`),
  KEY `IDX_sys_user_email` (`Email`),
  KEY `IDX_sys_user_phone` (`Phone`),
  KEY `IDX_sys_user_departmentid` (`DepartmentId`),
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
  `RoleName` varchar(100) NOT NULL COMMENT '角色名称',
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
-- 部门表 (sys_department)
-- =============================================
DROP TABLE IF EXISTS `sys_department`;
CREATE TABLE `sys_department` (
  `Id` bigint NOT NULL AUTO_INCREMENT COMMENT '主键ID',
  `ParentId` bigint NOT NULL DEFAULT '0' COMMENT '父部门ID（0表示根部门）',
  `DepartmentName` varchar(100) NOT NULL COMMENT '部门名称',
  `DepartmentCode` varchar(100) NOT NULL COMMENT '部门编码（唯一标识）',
  `DepartmentType` int NOT NULL DEFAULT '2' COMMENT '部门类型（1：公司，2：部门，3：小组）',
  `LeaderId` bigint DEFAULT NULL COMMENT '负责人用户ID',
  `Phone` varchar(20) DEFAULT NULL COMMENT '联系电话',
  `Email` varchar(100) DEFAULT NULL COMMENT '邮箱',
  `Sort` int NOT NULL DEFAULT '0' COMMENT '排序号',
  `Status` int NOT NULL DEFAULT '1' COMMENT '状态（0：禁用，1：启用）',
  `Remark` varchar(500) DEFAULT NULL COMMENT '备注',
  `CreateTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `UpdateTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `CreateUserId` bigint DEFAULT NULL COMMENT '创建用户ID',
  `UpdateUserId` bigint DEFAULT NULL COMMENT '更新用户ID',
  `IsDeleted` tinyint(1) NOT NULL DEFAULT '0' COMMENT '是否删除',
  `DeleteTime` datetime DEFAULT NULL COMMENT '删除时间',
  `DeleteUserId` bigint DEFAULT NULL COMMENT '删除用户ID',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_sys_department_code` (`DepartmentCode`),
  KEY `IDX_sys_department_parentid` (`ParentId`),
  KEY `IDX_sys_department_leaderid` (`LeaderId`),
  KEY `IDX_sys_department_type` (`DepartmentType`),
  KEY `IDX_sys_department_status` (`Status`),
  KEY `IDX_sys_department_sort` (`Sort`),
  KEY `IDX_sys_department_isdeleted` (`IsDeleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='部门表';

-- =============================================
-- 菜单表 (sys_menu)
-- =============================================
DROP TABLE IF EXISTS `sys_menu`;
CREATE TABLE `sys_menu` (
  `Id` bigint NOT NULL AUTO_INCREMENT COMMENT '主键ID',
  `ParentId` bigint NOT NULL DEFAULT '0' COMMENT '父菜单ID（0表示根菜单）',
  `MenuName` varchar(100) NOT NULL COMMENT '菜单名称',
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
  KEY `IDX_sys_user_role_roleid` (`RoleId`)
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
  KEY `IDX_sys_role_menu_menuid` (`MenuId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE utf8mb4_unicode_ci COMMENT='角色菜单权限关联表';

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
  `DepartmentId`,
  `Status`,
  `Remark`
) VALUES (
  1,
  'admin',
  'ba3253876aed6bc22d4a6ff53d8406c6ad864195ed144ab5c87621b6c233b548baeae6956df346ec8c17f5ea10f35ee3cbc514797ed7ddd3145464e2a0bab413',
  'admin@kiteserver.com',
  '系统管理员',
  2,
  1,
  '系统默认管理员账户'
),
-- 添加测试用户
(2, 'zhangsan', 'ba3253876aed6bc22d4a6ff53d8406c6ad864195ed144ab5c87621b6c233b548baeae6956df346ec8c17f5ea10f35ee3cbc514797ed7ddd3145464e2a0bab413', 'zhangsan@kiteframe.com', '张三', 6, 1, '前端开发工程师'),
(3, 'lisi', 'ba3253876aed6bc22d4a6ff53d8406c6ad864195ed144ab5c87621b6c233b548baeae6956df346ec8c17f5ea10f35ee3cbc514797ed7ddd3145464e2a0bab413', 'lisi@kiteframe.com', '李四', 7, 1, '后端开发工程师'),
(4, 'wangwu', 'ba3253876aed6bc22d4a6ff53d8406c6ad864195ed144ab5c87621b6c233b548baeae6956df346ec8c17f5ea10f35ee3cbc514797ed7ddd3145464e2a0bab413', 'wangwu@kiteframe.com', '王五', 8, 1, '测试工程师'),
(5, 'zhaoliu', 'ba3253876aed6bc22d4a6ff53d8406c6ad864195ed144ab5c87621b6c233b548baeae6956df346ec8c17f5ea10f35ee3cbc514797ed7ddd3145464e2a0bab413', 'zhaoliu@kiteframe.com', '赵六', 10, 1, 'UI设计师');

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

-- 插入部门数据
INSERT INTO `sys_department` (
  `Id`,
  `ParentId`,
  `DepartmentName`,
  `DepartmentCode`,
  `DepartmentType`,
  `LeaderId`,
  `Phone`,
  `Email`,
  `Sort`,
  `Status`,
  `Remark`
) VALUES
-- 根部门：公司
(1, 0, 'KiteFrame科技有限公司', 'kiteframe', 1, 1, '400-888-8888', 'contact@kiteframe.com', 1, 1, '公司总部'),

-- 一级部门
(2, 1, '技术部', 'tech', 2, 1, '021-88888888', 'tech@kiteframe.com', 1, 1, '技术研发部门'),
(3, 1, '产品部', 'product', 2, NULL, '021-88888889', 'product@kiteframe.com', 2, 1, '产品设计部门'),
(4, 1, '运营部', 'operation', 2, NULL, '021-88888890', 'operation@kiteframe.com', 3, 1, '运营推广部门'),
(5, 1, '人事部', 'hr', 2, NULL, '021-88888891', 'hr@kiteframe.com', 4, 1, '人力资源部门'),

-- 二级部门：技术部下属
(6, 2, '前端开发组', 'frontend', 3, NULL, NULL, 'frontend@kiteframe.com', 1, 1, '前端开发小组'),
(7, 2, '后端开发组', 'backend', 3, NULL, NULL, 'backend@kiteframe.com', 2, 1, '后端开发小组'),
(8, 2, '测试组', 'test', 3, NULL, NULL, 'test@kiteframe.com', 3, 1, '软件测试小组'),
(9, 2, '运维组', 'devops', 3, NULL, NULL, 'devops@kiteframe.com', 4, 1, '运维部署小组'),

-- 二级部门：产品部下属
(10, 3, 'UI设计组', 'ui', 3, NULL, NULL, 'ui@kiteframe.com', 1, 1, 'UI设计小组'),
(11, 3, '产品策划组', 'planning', 3, NULL, NULL, 'planning@kiteframe.com', 2, 1, '产品策划小组');

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

-- 二级菜单：部门管理
(35, 2, '部门管理', 'system-department', 2, '/system/department', '/src/views/system/department/index.vue', 'ep:office-building', 4, 1, 1, 0, 1, 'system:department:list', '部门管理页面'),
(36, 35, '新增部门', 'system-department-add', 3, NULL, NULL, NULL, 1, 1, 0, 0, 1, 'system:department:add', '新增部门按钮'),
(37, 35, '编辑部门', 'system-department-edit', 3, NULL, NULL, NULL, 2, 1, 0, 0, 1, 'system:department:edit', '编辑部门按钮'),
(38, 35, '删除部门', 'system-department-delete', 3, NULL, NULL, NULL, 3, 1, 0, 0, 1, 'system:department:delete', '删除部门按钮'),

-- 一级菜单：权限管理
(39, 0, '权限管理', 'permission', 1, '/permission', NULL, 'ep:lock', 3, 1, 0, 0, 1, NULL, '权限管理目录'),

-- 二级菜单：页面权限
(40, 39, '页面权限', 'permission-page', 2, '/permission/page', '/src/views/permission/page/index.vue', 'ep:document', 1, 1, 1, 0, 1, 'permission:page:view', '页面权限演示'),

-- 二级菜单：按钮权限
(41, 39, '按钮权限', 'permission-button', 2, '/permission/button', '/src/views/permission/button/index.vue', 'ep:key', 2, 1, 1, 0, 1, 'permission:button:view', '按钮权限演示'),

-- 一级菜单：系统监控
(42, 0, '系统监控', 'monitor', 1, '/monitor', NULL, 'ep:monitor', 4, 1, 0, 0, 1, NULL, '系统监控目录'),

-- 二级菜单：在线用户
(43, 42, '在线用户', 'monitor-online', 2, '/monitor/online', '/src/views/monitor/online/index.vue', 'ep:user-filled', 1, 1, 1, 0, 1, 'monitor:online:list', '在线用户监控'),
(44, 43, '强制下线', 'monitor-online-logout', 3, NULL, NULL, NULL, 1, 1, 0, 0, 1, 'monitor:online:logout', '强制用户下线按钮'),

-- 二级菜单：登录日志
(45, 42, '登录日志', 'monitor-loginlog', 2, '/monitor/loginlog', '/src/views/monitor/loginlog/index.vue', 'ep:document-checked', 2, 1, 1, 0, 1, 'monitor:loginlog:list', '登录日志查看'),
(46, 45, '删除日志', 'monitor-loginlog-delete', 3, NULL, NULL, NULL, 1, 1, 0, 0, 1, 'monitor:loginlog:delete', '删除登录日志按钮'),
(47, 45, '清空日志', 'monitor-loginlog-clear', 3, NULL, NULL, NULL, 2, 1, 0, 0, 1, 'monitor:loginlog:clear', '清空登录日志按钮'),

-- 二级菜单：操作日志
(48, 42, '操作日志', 'monitor-operlog', 2, '/monitor/operlog', '/src/views/monitor/operlog/index.vue', 'ep:document', 3, 1, 1, 0, 1, 'monitor:operlog:list', '操作日志查看'),
(49, 48, '删除日志', 'monitor-operlog-delete', 3, NULL, NULL, NULL, 1, 1, 0, 0, 1, 'monitor:operlog:delete', '删除操作日志按钮'),
(50, 48, '清空日志', 'monitor-operlog-clear', 3, NULL, NULL, NULL, 2, 1, 0, 0, 1, 'monitor:operlog:clear', '清空操作日志按钮'),

-- 一级菜单：系统工具
(51, 0, '系统工具', 'tool', 1, '/tool', NULL, 'ep:tools', 5, 1, 0, 0, 1, NULL, '系统工具目录'),

-- 二级菜单：代码生成
(52, 51, '代码生成', 'tool-gen', 2, '/tool/gen', '/src/views/tool/gen/index.vue', 'ep:cpu', 1, 1, 1, 0, 1, 'tool:gen:list', '代码生成工具'),
(53, 52, '生成代码', 'tool-gen-code', 3, NULL, NULL, NULL, 1, 1, 0, 0, 1, 'tool:gen:code', '生成代码按钮'),
(54, 52, '预览代码', 'tool-gen-preview', 3, NULL, NULL, NULL, 2, 1, 0, 0, 1, 'tool:gen:preview', '预览代码按钮'),
(55, 52, '下载代码', 'tool-gen-download', 3, NULL, NULL, NULL, 3, 1, 0, 0, 1, 'tool:gen:download', '下载代码按钮');

-- 分配用户角色
INSERT INTO `sys_user_role` (`UserId`, `RoleId`, `CreateUserId`) VALUES
(1, 1, 1), -- admin -> 超级管理员
(2, 3, 1), -- zhangsan -> 普通用户
(3, 3, 1), -- lisi -> 普通用户
(4, 3, 1), -- wangwu -> 普通用户
(5, 3, 1); -- zhaoliu -> 普通用户

-- 分配角色菜单权限（超级管理员拥有所有菜单权限）
INSERT INTO `sys_role_menu` (`RoleId`, `MenuId`, `CreateUserId`) VALUES
-- 超级管理员拥有所有菜单权限
(1, 1, 1), (1, 2, 1), (1, 3, 1), (1, 4, 1), (1, 5, 1), (1, 6, 1), (1, 7, 1), (1, 8, 1),
(1, 9, 1), (1, 10, 1), (1, 11, 1), (1, 12, 1), (1, 13, 1), (1, 14, 1), (1, 15, 1), (1, 16, 1), (1, 17, 1),
(1, 35, 1), (1, 36, 1), (1, 37, 1), (1, 38, 1), (1, 39, 1), (1, 40, 1), (1, 41, 1), (1, 42, 1), (1, 43, 1), (1, 44, 1),
(1, 45, 1), (1, 46, 1), (1, 47, 1), (1, 48, 1), (1, 49, 1), (1, 50, 1), (1, 51, 1), (1, 52, 1), (1, 53, 1), (1, 54, 1), (1, 55, 1),

-- 系统管理员拥有系统管理相关权限
(2, 1, 1), (2, 2, 1), (2, 3, 1), (2, 4, 1), (2, 5, 1), (2, 6, 1), (2, 7, 1), (2, 8, 1),
(2, 9, 1), (2, 10, 1), (2, 11, 1), (2, 12, 1), (2, 13, 1), (2, 14, 1), (2, 15, 1), (2, 16, 1), (2, 17, 1),
(2, 35, 1), (2, 36, 1), (2, 37, 1), (2, 38, 1), (2, 42, 1), (2, 43, 1), (2, 45, 1), (2, 48, 1),

-- 普通用户只有基础查看权限
(3, 1, 1), (3, 39, 1), (3, 40, 1), (3, 41, 1);

-- ========================================
-- 系统监控相关表
-- ========================================

-- 在线用户表
CREATE TABLE `sys_online_user` (
  `Id` bigint NOT NULL AUTO_INCREMENT COMMENT '主键ID',
  `SessionId` varchar(200) NOT NULL COMMENT '用户会话ID',
  `UserId` bigint NOT NULL COMMENT '用户ID',
  `UserName` varchar(50) NOT NULL COMMENT '用户名',
  `RealName` varchar(50) DEFAULT NULL COMMENT '真实姓名',
  `DeptId` bigint DEFAULT NULL COMMENT '部门ID',
  `DeptName` varchar(50) DEFAULT NULL COMMENT '部门名称',
  `IpAddress` varchar(50) DEFAULT NULL COMMENT 'IP地址',
  `IpLocation` varchar(200) DEFAULT NULL COMMENT 'IP归属地',
  `Browser` varchar(50) DEFAULT NULL COMMENT '浏览器类型',
  `Os` varchar(50) DEFAULT NULL COMMENT '操作系统',
  `LoginTime` datetime NOT NULL COMMENT '登录时间',
  `LastAccessTime` datetime NOT NULL COMMENT '最后访问时间',
  `ExpireTime` datetime NOT NULL COMMENT '过期时间',
  `Status` int NOT NULL DEFAULT '1' COMMENT '状态（1：在线，0：离线）',
  `CreateTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `UpdateTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_SessionId` (`SessionId`),
  KEY `IDX_UserId` (`UserId`),
  KEY `IDX_LoginTime` (`LoginTime`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='在线用户表';

-- 登录日志表
CREATE TABLE `sys_login_log` (
  `Id` bigint NOT NULL AUTO_INCREMENT COMMENT '主键ID',
  `UserId` bigint DEFAULT NULL COMMENT '用户ID',
  `UserName` varchar(50) DEFAULT NULL COMMENT '用户名',
  `IpAddress` varchar(50) DEFAULT NULL COMMENT 'IP地址',
  `IpLocation` varchar(200) DEFAULT NULL COMMENT 'IP归属地',
  `Browser` varchar(50) DEFAULT NULL COMMENT '浏览器类型',
  `Os` varchar(50) DEFAULT NULL COMMENT '操作系统',
  `Status` int NOT NULL COMMENT '登录状态（1：成功，0：失败）',
  `Message` varchar(500) DEFAULT NULL COMMENT '提示消息',
  `LoginTime` datetime NOT NULL COMMENT '登录时间',
  `CreateTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  PRIMARY KEY (`Id`),
  KEY `IDX_UserId` (`UserId`),
  KEY `IDX_UserName` (`UserName`),
  KEY `IDX_Status` (`Status`),
  KEY `IDX_LoginTime` (`LoginTime`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='登录日志表';

-- 操作日志表
CREATE TABLE `sys_operation_log` (
  `Id` bigint NOT NULL AUTO_INCREMENT COMMENT '主键ID',
  `UserId` bigint DEFAULT NULL COMMENT '用户ID',
  `UserName` varchar(50) DEFAULT NULL COMMENT '用户名',
  `Module` varchar(50) DEFAULT NULL COMMENT '模块名称',
  `BusinessType` varchar(50) DEFAULT NULL COMMENT '业务类型',
  `Method` varchar(200) DEFAULT NULL COMMENT '方法名称',
  `RequestMethod` varchar(10) DEFAULT NULL COMMENT '请求方式',
  `OperatorType` int DEFAULT '1' COMMENT '操作类别（1：管理员，2：用户）',
  `OperUrl` varchar(500) DEFAULT NULL COMMENT '请求URL',
  `OperIp` varchar(50) DEFAULT NULL COMMENT '主机地址',
  `OperLocation` varchar(200) DEFAULT NULL COMMENT '操作地点',
  `OperParam` text COMMENT '请求参数',
  `JsonResult` text COMMENT '返回参数',
  `Status` int NOT NULL DEFAULT '1' COMMENT '操作状态（1：正常，0：异常）',
  `ErrorMsg` text COMMENT '错误消息',
  `OperTime` datetime NOT NULL COMMENT '操作时间',
  `CostTime` bigint DEFAULT NULL COMMENT '消耗时间（毫秒）',
  `CreateTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  PRIMARY KEY (`Id`),
  KEY `IDX_UserId` (`UserId`),
  KEY `IDX_UserName` (`UserName`),
  KEY `IDX_Module` (`Module`),
  KEY `IDX_BusinessType` (`BusinessType`),
  KEY `IDX_Status` (`Status`),
  KEY `IDX_OperTime` (`OperTime`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='操作日志表';


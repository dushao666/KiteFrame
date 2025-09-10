-- =============================================
-- KiteServer 数据库建表脚本
-- 创建时间: 2025-09-10
-- 描述: 创建系统基础表结构
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
  `Password` varchar(100) NOT NULL COMMENT '密码',
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
-- 插入默认管理员用户
-- =============================================
INSERT INTO `sys_user` (
  `UserName`, 
  `Password`, 
  `Email`, 
  `RealName`, 
  `Status`, 
  `Remark`,
  `CreateTime`,
  `UpdateTime`
) VALUES (
  'admin', 
  'e10adc3949ba59abbe56e057f20f883e', -- 密码: 123456 (MD5加密)
  'admin@kiteserver.com', 
  '系统管理员', 
  1, 
  '系统默认管理员账户',
  NOW(),
  NOW()
);

-- =============================================
-- 生产环境建表脚本
-- =============================================
-- 使用生产数据库
USE `kite_prod`;

-- 用户表 (sys_user) - 生产环境
DROP TABLE IF EXISTS `sys_user`;
CREATE TABLE `sys_user` (
  `Id` bigint NOT NULL AUTO_INCREMENT COMMENT '主键ID',
  `UserName` varchar(50) NOT NULL COMMENT '用户名',
  `Password` varchar(100) NOT NULL COMMENT '密码',
  `Email` varchar(100) DEFAULT NULL COMMENT '邮箱',
  `Phone` varchar(20) DEFAULT NULL COMMENT '手机号',
  `RealName` varchar(50) DEFAULT NULL COMMENT '真实姓名',
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
  KEY `IDX_sys_user_email` (`Email`),
  KEY `IDX_sys_user_phone` (`Phone`),
  KEY `IDX_sys_user_status` (`Status`),
  KEY `IDX_sys_user_isdeleted` (`IsDeleted`),
  KEY `IDX_sys_user_createtime` (`CreateTime`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='用户表';

-- 插入默认管理员用户 - 生产环境
INSERT INTO `sys_user` (
  `UserName`, 
  `Password`, 
  `Email`, 
  `RealName`, 
  `Status`, 
  `Remark`,
  `CreateTime`,
  `UpdateTime`
) VALUES (
  'admin', 
  'e10adc3949ba59abbe56e057f20f883e', -- 密码: 123456 (MD5加密)
  'admin@kiteserver.com', 
  '系统管理员', 
  1, 
  '系统默认管理员账户',
  NOW(),
  NOW()
);

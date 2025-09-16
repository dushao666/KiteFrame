-- 修复用户表中的空字符串数据，将其转换为 NULL
-- 这样可以避免唯一键约束冲突

-- 修复钉钉ID
UPDATE sys_user 
SET DingTalkId = NULL 
WHERE DingTalkId = '';

-- 修复邮箱
UPDATE sys_user 
SET Email = NULL 
WHERE Email = '';

-- 修复手机号
UPDATE sys_user 
SET Phone = NULL 
WHERE Phone = ''; 
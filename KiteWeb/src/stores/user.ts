import { defineStore } from "pinia";
import { ref } from "vue";

interface UserRole {
  id: number;
  roleName: string;
  roleCode: string;
}

interface UserInfo {
  id: number;
  userName: string;
  realName: string;
  email: string;
  phone: string;
  avatar?: string;
}

export const useUserStore = defineStore("user", () => {
  // 用户信息
  const userInfo = ref<UserInfo | null>(null);
  
  // 用户角色
  const roles = ref<UserRole[]>([]);
  
  // 用户权限（菜单编码和按钮权限标识）
  const permissions = ref<string[]>([]);
  
  // 访问令牌
  const token = ref<string>("");

  // 设置用户信息
  const setUserInfo = (info: UserInfo) => {
    userInfo.value = info;
  };

  // 设置用户角色
  const setRoles = (userRoles: UserRole[]) => {
    roles.value = userRoles;
  };

  // 设置用户权限
  const setPermissions = (userPermissions: string[]) => {
    permissions.value = userPermissions;
  };

  // 设置访问令牌
  const setToken = (accessToken: string) => {
    token.value = accessToken;
  };

  // 检查是否有特定权限
  const hasPermission = (permission: string): boolean => {
    return permissions.value.includes(permission);
  };

  // 检查是否有特定角色
  const hasRole = (roleCode: string): boolean => {
    return roles.value.some(role => role.roleCode === roleCode);
  };

  // 检查是否有任一权限
  const hasAnyPermission = (permissionList: string[]): boolean => {
    return permissionList.some(permission => permissions.value.includes(permission));
  };

  // 检查是否有任一角色
  const hasAnyRole = (roleList: string[]): boolean => {
    return roleList.some(roleCode => 
      roles.value.some(role => role.roleCode === roleCode)
    );
  };

  // 清除用户数据
  const clearUserData = () => {
    userInfo.value = null;
    roles.value = [];
    permissions.value = [];
    token.value = "";
  };

  // 获取用户权限（用于API调用）
  const fetchUserPermissions = async () => {
    try {
      // 这里应该调用API获取用户权限
      // const response = await getUserPermissions();
      // setPermissions(response.data);
      
      // 暂时使用模拟数据 - 管理员拥有所有权限
      const mockPermissions = [
        "system:user:view",
        "system:user:add", 
        "system:user:edit",
        "system:user:delete",
        "system:user:assign",
        "system:role:view",
        "system:role:add",
        "system:role:edit", 
        "system:role:delete",
        "system:role:assign",
        "system:menu:view",
        "system:menu:add",
        "system:menu:edit",
        "system:menu:delete"
      ];
      setPermissions(mockPermissions);
      
      // 设置管理员角色
      setRoles([
        { id: 1, roleName: "系统管理员", roleCode: "admin" }
      ]);
    } catch (error) {
      console.error("获取用户权限失败:", error);
    }
  };

  // 初始化权限数据（页面加载时调用）
  const initUserPermissions = () => {
    // 暂时直接设置管理员权限，实际应该从登录接口或本地存储获取
    const mockPermissions = [
      "system:user:view",
      "system:user:add", 
      "system:user:edit",
      "system:user:delete",
      "system:user:assign",
      "system:role:view",
      "system:role:add",
      "system:role:edit", 
      "system:role:delete",
      "system:role:assign",
      "system:menu:view",
      "system:menu:add",
      "system:menu:edit",
      "system:menu:delete"
    ];
    setPermissions(mockPermissions);
    
    // 设置管理员角色
    setRoles([
      { id: 1, roleName: "系统管理员", roleCode: "admin" }
    ]);
  };

  return {
    userInfo,
    roles,
    permissions,
    token,
    setUserInfo,
    setRoles,
    setPermissions,
    setToken,
    hasPermission,
    hasRole,
    hasAnyPermission,
    hasAnyRole,
    clearUserData,
    fetchUserPermissions,
    initUserPermissions
  };
});

import { defineStore } from "pinia";
import { ref, computed } from "vue";
import {
  store,
  router,
  resetRouter,
  routerArrays,
  storageLocal
} from "../utils";
import {
  type UserResult,
  type RefreshTokenResult,
  type LoginRequest,
  type RefreshTokenRequest,
  getLogin,
  refreshTokenApi,
  logout
} from "@/api/user";
import {
  getUserPermissions,
  getUserMenuTree,
  checkUserPermission,
  type UserPermissionData,
  type RoleData,
  type MenuData
} from "@/api/permission";
import { useMultiTagsStoreHook } from "./multiTags";
import { type DataInfo, setToken, removeToken, getToken, userKey } from "@/utils/auth";

// 用户基本信息接口
interface UserInfo {
  id: number;
  userName: string;
  realName: string;
  email: string;
  phone: string;
  avatar?: string;
}

// 兼容原有类型
interface UserType {
  avatar: string;
  username: string;
  nickname: string;
  roles: Array<string>;
  permissions: Array<string>;
  isRemembered: boolean;
  loginDay: number;
}

export const useUserStore = defineStore("user", () => {
  // ==================== 原有状态（Options API 转 Composition API） ====================
  
  // 基本用户状态
  const avatar = ref<string>(storageLocal().getItem<DataInfo<number>>(userKey)?.avatar ?? "");
  const username = ref<string>(storageLocal().getItem<DataInfo<number>>(userKey)?.username ?? "");
  const nickname = ref<string>(storageLocal().getItem<DataInfo<number>>(userKey)?.nickname ?? "");
  const roles = ref<Array<string>>(storageLocal().getItem<DataInfo<number>>(userKey)?.roles ?? []);
  const permissions = ref<Array<string>>(storageLocal().getItem<DataInfo<number>>(userKey)?.permissions ?? []);
  const isRemembered = ref<boolean>(false);
  const loginDay = ref<number>(7);

  // ==================== 新增权限管理状态 ====================
  
  // 详细用户信息
  const userInfo = ref<UserInfo | null>(null);
  
  // 详细角色信息
  const roleList = ref<RoleData[]>([]);
  
  // 菜单树
  const menuTree = ref<MenuData[]>([]);
  
  // 访问令牌
  const token = ref<string>("");

  // ==================== 计算属性 ====================
  
  // 当前用户ID
  const userId = computed(() => userInfo.value?.id || 0);
  
  // 是否已登录
  const isLoggedIn = computed(() => !!token.value && !!userInfo.value);

  // ==================== 原有方法（保持兼容性） ====================
  
  /** 存储头像 */
  const SET_AVATAR = (newAvatar: string) => {
    avatar.value = newAvatar;
  };

  /** 存储用户名 */
  const SET_USERNAME = (newUsername: string) => {
    username.value = newUsername;
  };

  /** 存储昵称 */
  const SET_NICKNAME = (newNickname: string) => {
    nickname.value = newNickname;
  };

  /** 存储角色 */
  const SET_ROLES = (newRoles: Array<string>) => {
    roles.value = newRoles;
  };

  /** 存储按钮级别权限 */
  const SET_PERMS = (newPermissions: Array<string>) => {
    permissions.value = newPermissions;
  };

  /** 存储是否勾选了登录页的免登录 */
  const SET_ISREMEMBERED = (bool: boolean) => {
    isRemembered.value = bool;
  };

  /** 设置登录页的免登录存储几天 */
  const SET_LOGINDAY = (value: number) => {
    loginDay.value = Number(value);
  };

  // ==================== 新增权限管理方法 ====================
  
  /** 设置用户详细信息 */
  const setUserInfo = (info: UserInfo) => {
    userInfo.value = info;
    // 同步到原有状态
    SET_USERNAME(info.userName);
    SET_NICKNAME(info.realName || info.userName);
    if (info.avatar) {
      SET_AVATAR(info.avatar);
    }
  };

  /** 设置用户角色 */
  const setRoles = (userRoles: RoleData[]) => {
    roleList.value = userRoles;
    // 同步到原有状态
    const roleCodes = userRoles.map(role => role.roleCode);
    SET_ROLES(roleCodes);
  };

  /** 设置用户权限 */
  const setPermissions = (userPermissions: string[]) => {
    // 同步到原有状态
    SET_PERMS(userPermissions);
  };

  /** 设置访问令牌 */
  const setTokenValue = (accessToken: string) => {
    token.value = accessToken;
  };

  /** 设置菜单树 */
  const setMenuTree = (menus: MenuData[]) => {
    menuTree.value = menus;
  };

  // ==================== 权限检查方法 ====================
  
  /** 检查是否有特定权限 */
  const hasPermission = (permission: string): boolean => {
    return permissions.value.includes(permission);
  };

  /** 检查是否有特定角色 */
  const hasRole = (roleCode: string): boolean => {
    return roleList.value.some(role => role.roleCode === roleCode) || 
           roles.value.includes(roleCode);
  };

  /** 检查是否有任一权限 */
  const hasAnyPermission = (permissionList: string[]): boolean => {
    return permissionList.some(permission => permissions.value.includes(permission));
  };

  /** 检查是否有任一角色 */
  const hasAnyRole = (roleCodeList: string[]): boolean => {
    return roleCodeList.some(roleCode => hasRole(roleCode));
  };

  // ==================== API调用方法 ====================
  
  /** 获取用户权限信息 */
  const fetchUserPermissions = async (targetUserId?: number) => {
    try {
      const id = targetUserId || userId.value;
      if (!id) {
        console.warn("用户ID不存在，无法获取权限信息");
        return;
      }

      const response = await getUserPermissions(id);
      if (response?.success && response.data) {
        const data = response.data;
        
        // 设置角色信息
        setRoles(data.roles);
        
        // 设置权限信息
        setPermissions(data.permissions);
        
        // 设置菜单树
        setMenuTree(data.menus);
        
        console.log("用户权限信息获取成功");
      }
    } catch (error) {
      console.error("获取用户权限失败:", error);
    }
  };

  /** 获取用户菜单树 */
  const fetchUserMenuTree = async (targetUserId?: number) => {
    try {
      const id = targetUserId || userId.value;
      if (!id) {
        console.warn("用户ID不存在，无法获取菜单树");
        return;
      }

      const response = await getUserMenuTree(id);
      if (response?.success && response.data) {
        setMenuTree(response.data);
        console.log("用户菜单树获取成功");
      }
    } catch (error) {
      console.error("获取用户菜单树失败:", error);
    }
  };

  /** 检查用户权限（API调用） */
  const checkPermission = async (permission: string, targetUserId?: number): Promise<boolean> => {
    try {
      const id = targetUserId || userId.value;
      if (!id) {
        console.warn("用户ID不存在，无法检查权限");
        return false;
      }

      const response = await checkUserPermission(id, permission);
      return response?.success && response.data === true;
    } catch (error) {
      console.error("检查用户权限失败:", error);
      return false;
    }
  };

  /** 初始化权限数据（页面加载时调用） */
  const initUserPermissions = async () => {
    try {
      // 从token中获取用户信息
      const tokenData = getToken();
      console.log("🔍 初始化权限 - token数据:", tokenData);
      
      if (tokenData && tokenData.username && tokenData.accessToken) {
        // 恢复基本的token和用户状态
        setTokenValue(tokenData.accessToken);
        
        // 设置基本的状态信息（从token中获取）
        SET_USERNAME(tokenData.username);
        SET_NICKNAME(tokenData.nickname || tokenData.username);
        SET_AVATAR(tokenData.avatar || "");
        
        // 如果token中有角色和权限信息，先设置这些信息
        if (tokenData.roles && tokenData.roles.length > 0) {
          SET_ROLES(tokenData.roles);
          console.log("✅ 恢复角色信息:", tokenData.roles);
        }
        if (tokenData.permissions && tokenData.permissions.length > 0) {
          SET_PERMS(tokenData.permissions);
          console.log("✅ 恢复权限信息:", tokenData.permissions);
        }
        
        console.log("✅ 用户权限初始化成功，用户:", tokenData.username);
      } else {
        console.log("⚠️ 未找到有效的token信息");
        console.log("  - tokenData存在:", !!tokenData);
        console.log("  - username存在:", !!(tokenData?.username));
        console.log("  - accessToken存在:", !!(tokenData?.accessToken));
        
        // 不调用clearUserData()，避免清空可能存在的用户状态
      }
    } catch (error) {
      console.error("❌ 初始化用户权限失败:", error);
      // 初始化失败时也不清空用户数据，让路由守卫来处理
    }
  };

  // ==================== 登录/登出方法 ====================
  
  /** 登入 */
  const loginByUsername = async (data: LoginRequest): Promise<UserResult> => {
    return new Promise<UserResult>((resolve, reject) => {
      getLogin(data)
        .then(async response => {
          if (response?.success && response.data) {
            // 提取角色和权限信息
            const roleCodes = response.data.roles?.map(role => role.roleCode) || [];
            const userPermissions = response.data.permissions || [];

            // 设置token到本地存储
            setToken({
              accessToken: response.data.accessToken,
              refreshToken: response.data.refreshToken,
              expires: new Date(response.data.expiresAt),
              username: response.data.userName,
              nickname: response.data.realName || response.data.userName,
              avatar: response.data.avatar || "",
              roles: roleCodes,
              permissions: userPermissions
            });

            // 更新store状态
            SET_USERNAME(response.data.userName);
            SET_NICKNAME(response.data.realName || response.data.userName);
            SET_AVATAR(response.data.avatar || "");
            SET_ROLES(roleCodes);
            SET_PERMS(userPermissions);
            
            // 设置详细用户信息
            const userInfoData: UserInfo = {
              id: response.data.userId || 1,
              userName: response.data.userName,
              realName: response.data.realName || response.data.userName,
              email: response.data.email || "",
              phone: response.data.phone || "",
              avatar: response.data.avatar || ""
            };
            setUserInfo(userInfoData);
            setTokenValue(response.data.accessToken);
            
            // 设置详细角色信息
            if (response.data.roles) {
              setRoles(response.data.roles);
            }
            
            // 初始化权限数据 - 使用明确的用户ID
            try {
              await fetchUserPermissions(userInfoData.id);
            } catch (error) {
              console.warn("获取详细权限信息失败，使用登录返回的权限数据:", error);
            }
          }
          resolve(response);
        })
        .catch(error => {
          reject(error);
        });
    });
  };

  /** 清除用户数据 */
  const clearUserData = () => {
    // 清除原有状态
    username.value = "";
    nickname.value = "";
    avatar.value = "";
    roles.value = [];
    permissions.value = [];
    
    // 清除新增状态
    userInfo.value = null;
    roleList.value = [];
    menuTree.value = [];
    token.value = "";
    
    // 清除本地存储
    removeToken();
  };

  /** 登出 */
  const logOut = async () => {
    try {
      // 调用后台登出接口
      await logout();
    } catch (error) {
      console.warn("登出接口调用失败:", error);
    } finally {
      // 清理本地状态
      clearUserData();
      useMultiTagsStoreHook().handleTags("equal", [...routerArrays]);
      resetRouter();
      router.push("/login");
    }
  };

  /** 刷新`token` */
  const handRefreshToken = async (data: RefreshTokenRequest): Promise<RefreshTokenResult> => {
    return new Promise<RefreshTokenResult>((resolve, reject) => {
      refreshTokenApi(data)
        .then(response => {
          if (response?.success && response.data) {
            // 更新token
            const currentToken = getToken();
            if (currentToken) {
              setToken({
                ...currentToken,
                accessToken: response.data.accessToken,
                refreshToken: response.data.refreshToken,
                expires: new Date(response.data.expiresAt)
              });
              setTokenValue(response.data.accessToken);
            }
            resolve(response);
          }
        })
        .catch(error => {
          reject(error);
        });
    });
  };

  // ==================== 返回状态和方法 ====================
  
  return {
    // 原有状态
    avatar,
    username,
    nickname,
    roles,
    permissions,
    isRemembered,
    loginDay,
    
    // 新增状态
    userInfo,
    roleList,
    menuTree,
    token,
    
    // 计算属性
    userId,
    isLoggedIn,
    
    // 原有方法（保持兼容性）
    SET_AVATAR,
    SET_USERNAME,
    SET_NICKNAME,
    SET_ROLES,
    SET_PERMS,
    SET_ISREMEMBERED,
    SET_LOGINDAY,
    loginByUsername,
    logOut,
    handRefreshToken,
    
    // 新增权限管理方法
    setUserInfo,
    setRoles,
    setPermissions,
    setToken: setTokenValue,
    setMenuTree,
    hasPermission,
    hasRole,
    hasAnyPermission,
    hasAnyRole,
    fetchUserPermissions,
    fetchUserMenuTree,
    checkPermission,
    initUserPermissions,
    clearUserData
  };
});

// 保持原有的Hook函数（兼容性）
export function useUserStoreHook() {
  return useUserStore(store);
}

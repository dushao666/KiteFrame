import { http } from "@/utils/http";

// API 返回结果类型
export type ApiResult<T = any> = {
  success: boolean;
  data?: T;
  message?: string;
  code?: number;
};

// 用户角色类型
export interface RoleData {
  id: number;
  roleName: string;
  roleCode: string;
  sort?: number;
  status?: number;
  dataScope?: number;
  remark?: string;
}

// 菜单类型
export interface MenuData {
  id: number;
  parentId: number;
  title: string;
  name: string;
  path: string;
  component: string;
  icon?: string;
  sort: number;
  status: number;
  isVisible: boolean;
  permissions?: string;
  children?: MenuData[];
}

// 用户权限信息
export interface UserPermissionData {
  userId: number;
  userName: string;
  roles: RoleData[];
  menus: MenuData[];
  permissions: string[];
}

// API 返回类型定义
export type UserPermissionResult = ApiResult<UserPermissionData>;
export type MenuTreeResult = ApiResult<MenuData[]>;
export type PermissionCheckResult = ApiResult<boolean>;
export type RoleMenuIdsResult = ApiResult<number[]>;

/**
 * 获取用户权限信息
 * @param userId 用户ID
 * @returns 用户权限信息
 */
export const getUserPermissions = (userId: number) => {
  return http.request<UserPermissionResult>("get", `/permission/user/${userId}`);
};

/**
 * 获取用户菜单树
 * @param userId 用户ID
 * @returns 用户菜单树
 */
export const getUserMenuTree = (userId: number) => {
  return http.request<MenuTreeResult>("get", `/permission/user/${userId}/menus`);
};

/**
 * 检查用户权限
 * @param userId 用户ID
 * @param permission 权限标识
 * @returns 是否有权限
 */
export const checkUserPermission = (userId: number, permission: string) => {
  return http.request<PermissionCheckResult>("get", `/permission/user/${userId}/check`, {
    params: { permission }
  });
};

/**
 * 获取所有菜单列表（用于权限配置）
 * @returns 菜单列表
 */
export const getAllMenus = () => {
  return http.request<MenuTreeResult>("get", "/permission/menus");
};

/**
 * 根据角色ID获取菜单权限
 * @param roleId 角色ID
 * @returns 菜单权限列表
 */
export const getRoleMenuIds = (roleId: number) => {
  return http.request<RoleMenuIdsResult>("get", `/permission/role/${roleId}/menus`);
}; 
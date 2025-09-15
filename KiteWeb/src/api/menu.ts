import { http } from "@/utils/http";

// 后台API返回的统一结果格式
export type ApiResult<T = any> = {
  success: boolean;
  message: string;
  data: T;
  code: number;
  timestamp: string;
};

// 菜单类型枚举
export enum MenuType {
  /** 目录 */
  Directory = 1,
  /** 菜单 */
  Menu = 2,
  /** 按钮 */
  Button = 3
}

// 菜单信息
export type MenuData = {
  /** 菜单ID */
  id: number;
  /** 父菜单ID */
  parentId: number;
  /** 菜单名称 */
  menuName: string;
  /** 菜单编码 */
  menuCode: string;
  /** 菜单类型 */
  menuType: MenuType;
  /** 路由路径 */
  path?: string;
  /** 组件路径 */
  component?: string;
  /** 菜单图标 */
  icon?: string;
  /** 排序号 */
  sort: number;
  /** 是否显示 */
  isVisible: boolean;
  /** 是否缓存 */
  isCache: boolean;
  /** 是否外链 */
  isFrame: boolean;
  /** 状态 */
  status: number;
  /** 权限标识 */
  permissions?: string;
  /** 备注 */
  remark?: string;
  /** 创建时间 */
  createTime: string;
  /** 更新时间 */
  updateTime: string;
  /** 子菜单列表 */
  children: MenuData[];
};

// 分页结果
export type PagedResult<T> = {
  /** 数据列表 */
  items: T[];
  /** 总数量 */
  totalCount: number;
  /** 页码 */
  pageIndex: number;
  /** 页大小 */
  pageSize: number;
};

// 获取菜单列表请求参数
export type GetMenusRequest = {
  /** 页码 */
  pageIndex?: number;
  /** 页大小 */
  pageSize?: number;
  /** 菜单名称 */
  menuName?: string;
  /** 菜单编码 */
  menuCode?: string;
  /** 菜单类型 */
  menuType?: MenuType;
  /** 状态 */
  status?: number;
};

// 创建菜单请求参数
export type CreateMenuRequest = {
  /** 父菜单ID */
  parentId: number;
  /** 菜单名称 */
  menuName: string;
  /** 菜单编码 */
  menuCode: string;
  /** 菜单类型 */
  menuType: MenuType;
  /** 路由路径 */
  path?: string;
  /** 组件路径 */
  component?: string;
  /** 菜单图标 */
  icon?: string;
  /** 排序号 */
  sort: number;
  /** 是否显示 */
  isVisible: boolean;
  /** 是否缓存 */
  isCache: boolean;
  /** 是否外链 */
  isFrame: boolean;
  /** 状态 */
  status: number;
  /** 权限标识 */
  permissions?: string;
  /** 备注 */
  remark?: string;
};

// 更新菜单请求参数
export type UpdateMenuRequest = {
  /** 父菜单ID */
  parentId: number;
  /** 菜单名称 */
  menuName: string;
  /** 菜单编码 */
  menuCode: string;
  /** 菜单类型 */
  menuType: MenuType;
  /** 路由路径 */
  path?: string;
  /** 组件路径 */
  component?: string;
  /** 菜单图标 */
  icon?: string;
  /** 排序号 */
  sort: number;
  /** 是否显示 */
  isVisible: boolean;
  /** 是否缓存 */
  isCache: boolean;
  /** 是否外链 */
  isFrame: boolean;
  /** 状态 */
  status: number;
  /** 权限标识 */
  permissions?: string;
  /** 备注 */
  remark?: string;
};

// API 返回类型定义
export type MenusResult = ApiResult<PagedResult<MenuData>>;
export type MenuResult = ApiResult<MenuData>;
export type MenuTreeResult = ApiResult<MenuData[]>;
export type CreateMenuResult = ApiResult<number>;
export type UpdateMenuResult = ApiResult<boolean>;
export type DeleteMenuResult = ApiResult<boolean>;

/** 获取菜单列表 */
export const getMenus = (params: GetMenusRequest) => {
  return http.request<MenusResult>("get", "/menu", { params });
};

/** 根据ID获取菜单 */
export const getMenuById = (id: number) => {
  return http.request<MenuResult>("get", `/menu/${id}`);
};

/** 获取菜单树 */
export const getMenuTree = () => {
  return http.request<MenuTreeResult>("get", "/menu/tree");
};

/** 获取用户菜单树 */
export const getUserMenuTree = (userId: number) => {
  return http.request<MenuTreeResult>("get", `/menu/user/${userId}`);
};

/** 创建菜单 */
export const createMenu = (data: CreateMenuRequest) => {
  return http.request<CreateMenuResult>("post", "/menu", { data });
};

/** 更新菜单 */
export const updateMenu = (id: number, data: UpdateMenuRequest) => {
  return http.request<UpdateMenuResult>("put", `/menu/${id}`, { data });
};

/** 删除菜单 */
export const deleteMenu = (id: number) => {
  return http.request<DeleteMenuResult>("delete", `/menu/${id}`);
};

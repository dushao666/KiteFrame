import { http } from "@/utils/http";

// 后台API返回的统一结果格式
export type ApiResult<T = any> = {
  success: boolean;
  message: string;
  data: T;
  code: number;
  timestamp: string;
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
  /** 总页数 */
  totalPages: number;
  /** 是否有上一页 */
  hasPreviousPage: boolean;
  /** 是否有下一页 */
  hasNextPage: boolean;
};

// 角色信息
export type RoleData = {
  /** 角色ID */
  id: number;
  /** 角色名称 */
  roleName: string;
  /** 角色编码 */
  roleCode: string;
  /** 排序号 */
  sort: number;
  /** 状态 */
  status: number;
  /** 数据权限范围 */
  dataScope: number;
  /** 备注 */
  remark?: string;
  /** 创建时间 */
  createTime?: string;
  /** 更新时间 */
  updateTime?: string;
};

// 获取角色列表请求参数
export type GetRolesRequest = {
  /** 页码 */
  pageIndex?: number;
  /** 页大小 */
  pageSize?: number;
  /** 关键词 */
  keyword?: string;
  /** 状态 */
  status?: number;
};

// 创建角色请求参数
export type CreateRoleRequest = {
  /** 角色名称 */
  roleName: string;
  /** 角色编码 */
  roleCode: string;
  /** 排序号 */
  sort: number;
  /** 状态 */
  status: number;
  /** 数据权限范围 */
  dataScope: number;
  /** 备注 */
  remark?: string;
};

// 更新角色请求参数
export type UpdateRoleRequest = {
  /** 角色名称 */
  roleName: string;
  /** 角色编码 */
  roleCode: string;
  /** 排序号 */
  sort: number;
  /** 状态 */
  status: number;
  /** 数据权限范围 */
  dataScope: number;
  /** 备注 */
  remark?: string;
};

// API 返回类型定义
export type RolesResult = ApiResult<PagedResult<RoleData>>;
export type RoleResult = ApiResult<RoleData>;
export type CreateRoleResult = ApiResult<number>;
export type UpdateRoleResult = ApiResult<boolean>;
export type DeleteRoleResult = ApiResult<boolean>;
export type CheckRoleCodeResult = ApiResult<boolean>;
export type EnabledRolesResult = ApiResult<RoleData[]>;

/** 获取角色列表 */
export const getRoles = (params: GetRolesRequest) => {
  return http.request<RolesResult>("get", "/role", { params });
};

/** 根据ID获取角色 */
export const getRoleById = (id: number) => {
  return http.request<RoleResult>("get", `/role/${id}`);
};

/** 创建角色 */
export const createRole = (data: CreateRoleRequest) => {
  return http.request<CreateRoleResult>("post", "/role", { data });
};

/** 更新角色 */
export const updateRole = (id: number, data: UpdateRoleRequest) => {
  return http.request<UpdateRoleResult>("put", `/role/${id}`, { data });
};

/** 删除角色 */
export const deleteRole = (id: number) => {
  return http.request<DeleteRoleResult>("delete", `/role/${id}`);
};

/** 获取所有启用的角色列表（用于下拉选择） */
export const getEnabledRoles = () => {
  return http.request<EnabledRolesResult>("get", "/role/enabled");
};

/** 检查角色编码是否存在 */
export const checkRoleCodeExists = (roleCode: string, excludeId?: number) => {
  const params: any = { roleCode };
  if (excludeId) {
    params.excludeId = excludeId;
  }
  return http.request<CheckRoleCodeResult>("get", "/role/check-code", {
    params
  });
};

// 数据权限范围枚举
export const DataScopeEnum = {
  ALL: 1, // 全部数据权限
  CUSTOM: 2, // 自定数据权限
  DEPT: 3, // 部门数据权限
  DEPT_AND_SUB: 4, // 部门及以下数据权限
  SELF: 5 // 仅本人数据权限
};

// 数据权限范围选项
export const dataScopeOptions = [
  { label: "全部数据权限", value: DataScopeEnum.ALL },
  { label: "自定数据权限", value: DataScopeEnum.CUSTOM },
  { label: "部门数据权限", value: DataScopeEnum.DEPT },
  { label: "部门及以下数据权限", value: DataScopeEnum.DEPT_AND_SUB },
  { label: "仅本人数据权限", value: DataScopeEnum.SELF }
];

// 状态枚举
export const StatusEnum = {
  ENABLED: 1, // 启用
  DISABLED: 0 // 禁用
};

// 状态选项
export const statusOptions = [
  { label: "启用", value: StatusEnum.ENABLED },
  { label: "禁用", value: StatusEnum.DISABLED }
];

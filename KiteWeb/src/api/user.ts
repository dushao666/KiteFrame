import { http } from "@/utils/http";

// 后台API返回的统一结果格式
export type ApiResult<T = any> = {
  success: boolean;
  message: string;
  data: T;
  code: number;
  timestamp: string;
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
};

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
  menuType: number;
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
  /** 子菜单列表 */
  children: MenuData[];
};

// 登录用户信息
export type LoginUserData = {
  /** 用户ID */
  userId: number;
  /** 用户名 */
  userName: string;
  /** 真实姓名 */
  realName?: string;
  /** 邮箱 */
  email?: string;
  /** 手机号 */
  phone?: string;
  /** 头像 */
  avatar?: string;
  /** 访问令牌 */
  accessToken: string;
  /** 刷新令牌 */
  refreshToken: string;
  /** 令牌过期时间 */
  expiresAt: string;
  /** 最后登录时间 */
  lastLoginTime?: string;
  /** 最后登录IP */
  lastLoginIp?: string;
  /** 用户角色列表 */
  roles?: RoleData[];
  /** 用户菜单权限 */
  menus?: MenuData[];
  /** 用户按钮权限列表 */
  permissions?: string[];
};

export type UserResult = ApiResult<LoginUserData>;

// 刷新令牌数据
export type RefreshTokenData = {
  /** 访问令牌 */
  accessToken: string;
  /** 刷新令牌 */
  refreshToken: string;
  /** 令牌过期时间 */
  expiresAt: string;
};

export type RefreshTokenResult = ApiResult<RefreshTokenData>;

// 登录请求参数
export type LoginRequest = {
  /** 登录类型 */
  type?: number;
  /** 用户名 */
  userName?: string;
  /** 密码 */
  password?: string;
  /** 手机号 */
  phone?: string;
  /** 短信验证码 */
  smsCode?: string;
  /** 记住我 */
  rememberMe?: boolean;
};

// 刷新令牌请求参数
export type RefreshTokenRequest = {
  refreshToken: string;
};

/** 登录 */
export const getLogin = (data: LoginRequest) => {
  return http.request<UserResult>("post", "/auth/signin", { data });
};

/** 刷新`token` */
export const refreshTokenApi = (data: RefreshTokenRequest) => {
  return http.request<RefreshTokenResult>("post", "/auth/refresh", { data });
};

/** 登出 */
export const logout = () => {
  return http.request<ApiResult>("post", "/auth/signout");
};

// ==================== 用户管理相关 API ====================

// 用户信息
export type UserData = {
  /** 用户ID */
  id: number;
  /** 用户名 */
  userName: string;
  /** 邮箱 */
  email?: string;
  /** 手机号 */
  phone?: string;
  /** 真实姓名 */
  realName?: string;
  /** 钉钉用户ID */
  dingTalkId?: string;
  /** 状态 */
  status: number;
  /** 备注 */
  remark?: string;
  /** 创建时间 */
  createTime: string;
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

// 获取用户列表请求参数
export type GetUsersRequest = {
  /** 页码 */
  pageIndex?: number;
  /** 页大小 */
  pageSize?: number;
  /** 关键词 */
  keyword?: string;
};

// 创建用户请求参数
export type CreateUserRequest = {
  /** 用户名 */
  userName: string;
  /** 密码 */
  password: string;
  /** 邮箱 */
  email?: string;
  /** 手机号 */
  phone?: string;
  /** 真实姓名 */
  realName?: string;
  /** 钉钉用户ID */
  dingTalkId?: string;
  /** 状态 */
  status?: number;
  /** 备注 */
  remark?: string;
};

// 更新用户请求参数
export type UpdateUserRequest = {
  /** 用户名 */
  userName: string;
  /** 邮箱 */
  email?: string;
  /** 手机号 */
  phone?: string;
  /** 真实姓名 */
  realName?: string;
  /** 钉钉用户ID */
  dingTalkId?: string;
  /** 状态 */
  status: number;
  /** 备注 */
  remark?: string;
};

// API 返回类型定义
export type UsersResult = ApiResult<PagedResult<UserData>>;
export type UserDetailResult = ApiResult<UserData>;
export type CreateUserResult = ApiResult<number>;
export type UpdateUserResult = ApiResult<boolean>;
export type DeleteUserResult = ApiResult<boolean>;

/** 获取用户列表 */
export const getUsers = (params: GetUsersRequest) => {
  return http.request<UsersResult>("get", "/user", { params });
};

/** 根据ID获取用户 */
export const getUserById = (id: number) => {
  return http.request<UserResult>("get", `/user/${id}`);
};

/** 创建用户 */
export const createUser = (data: CreateUserRequest) => {
  return http.request<CreateUserResult>("post", "/user", { data });
};

/** 更新用户 */
export const updateUser = (id: number, data: UpdateUserRequest) => {
  return http.request<UpdateUserResult>("put", `/user/${id}`, { data });
};

/** 删除用户 */
export const deleteUser = (id: number) => {
  return http.request<DeleteUserResult>("delete", `/user/${id}`);
};

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

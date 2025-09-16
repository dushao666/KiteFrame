import { http } from "@/utils/http";

// API 返回结果类型
export type ApiResult<T = any> = {
  success: boolean;
  data?: T;
  message?: string;
  code?: number;
  timestamp?: number;
};

// 分页结果类型
export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageIndex: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

// 在线用户类型
export interface OnlineUserData {
  id: number;
  sessionId: string;
  userId: number;
  userName: string;
  realName?: string;
  deptId?: number;
  deptName?: string;
  ipAddress?: string;
  ipLocation?: string;
  browser?: string;
  os?: string;
  loginTime: string;
  lastAccessTime: string;
  expireTime: string;
  status: number;
  statusText: string;
  onlineDuration: number;
}

// 登录日志类型
export interface LoginLogData {
  id: number;
  userId?: number;
  userName?: string;
  ipAddress?: string;
  ipLocation?: string;
  browser?: string;
  os?: string;
  status: number;
  statusText: string;
  message?: string;
  loginTime: string;
  createTime: string;
}

// 操作日志类型
export interface OperationLogData {
  id: number;
  userId?: number;
  userName?: string;
  module?: string;
  businessType?: string;
  method?: string;
  requestMethod?: string;
  operatorType: number;
  operatorTypeText: string;
  operUrl?: string;
  operIp?: string;
  operLocation?: string;
  operParam?: string;
  jsonResult?: string;
  status: number;
  statusText: string;
  errorMsg?: string;
  operTime: string;
  costTime?: number;
  createTime: string;
}

// 在线用户查询请求
export interface GetOnlineUsersRequest {
  pageIndex?: number;
  pageSize?: number;
  userName?: string;
  ipAddress?: string;
  status?: number;
}

// 登录日志查询请求
export interface GetLoginLogsRequest {
  pageIndex?: number;
  pageSize?: number;
  userName?: string;
  ipAddress?: string;
  status?: number;
  startTime?: string;
  endTime?: string;
}

// 操作日志查询请求
export interface GetOperationLogsRequest {
  pageIndex?: number;
  pageSize?: number;
  userName?: string;
  module?: string;
  businessType?: string;
  status?: number;
  startTime?: string;
  endTime?: string;
}

/**
 * 获取在线用户列表
 */
export const getOnlineUsers = (params: GetOnlineUsersRequest) => {
  return http.request<ApiResult<PagedResult<OnlineUserData>>>("get", "/monitor/online/users", {
    params
  });
};

/**
 * 强制用户下线
 */
export const forceLogout = (sessionId: string) => {
  return http.request<ApiResult<boolean>>("post", `/monitor/online/logout/${sessionId}`);
};

/**
 * 获取登录日志列表
 */
export const getLoginLogs = (params: GetLoginLogsRequest) => {
  return http.request<ApiResult<PagedResult<LoginLogData>>>("get", "/monitor/loginlogs", {
    params
  });
};

/**
 * 删除登录日志
 */
export const deleteLoginLogs = (ids: number[]) => {
  return http.request<ApiResult<boolean>>("delete", "/monitor/loginlogs", {
    data: ids
  });
};

/**
 * 清空登录日志
 */
export const clearLoginLogs = () => {
  return http.request<ApiResult<boolean>>("delete", "/monitor/loginlogs/clear");
};

/**
 * 获取操作日志列表
 */
export const getOperationLogs = (params: GetOperationLogsRequest) => {
  return http.request<ApiResult<PagedResult<OperationLogData>>>("get", "/monitor/operationlogs", {
    params
  });
};

/**
 * 删除操作日志
 */
export const deleteOperationLogs = (ids: number[]) => {
  return http.request<ApiResult<boolean>>("delete", "/monitor/operationlogs", {
    data: ids
  });
};

/**
 * 清空操作日志
 */
export const clearOperationLogs = () => {
  return http.request<ApiResult<boolean>>("delete", "/monitor/operationlogs/clear");
}; 
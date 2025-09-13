import { http } from "@/utils/http";
import type { ApiResult } from "./user";

// 菜单路由数据类型
export type MenuRoute = {
  id: number;
  parentId: number;
  menuName: string;
  menuCode: string;
  menuType: number;
  path?: string;
  component?: string;
  icon?: string;
  sort: number;
  isVisible: boolean;
  isCache: boolean;
  isFrame: boolean;
  status: number;
  permissions?: string;
  remark?: string;
  children: MenuRoute[];
};

type RouteResult = ApiResult<Array<MenuRoute>>;

export const getAsyncRoutes = () => {
  return http.request<RouteResult>("get", "/auth/routes");
};

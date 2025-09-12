import { http } from "@/utils/http";
import type { ApiResult } from "./user";

type RouteResult = ApiResult<Array<any>>;

export const getAsyncRoutes = () => {
  // 暂时返回静态路由，后续可以对接后台权限接口
  return Promise.resolve({
    success: true,
    message: "获取路由成功",
    data: [],
    code: 200,
    timestamp: new Date().toISOString()
  } as RouteResult);
};

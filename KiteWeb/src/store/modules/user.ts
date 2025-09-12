import { defineStore } from "pinia";
import {
  type userType,
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
import { useMultiTagsStoreHook } from "./multiTags";
import { type DataInfo, setToken, removeToken, getToken, userKey } from "@/utils/auth";

export const useUserStore = defineStore("pure-user", {
  state: (): userType => ({
    // 头像
    avatar: storageLocal().getItem<DataInfo<number>>(userKey)?.avatar ?? "",
    // 用户名
    username: storageLocal().getItem<DataInfo<number>>(userKey)?.username ?? "",
    // 昵称
    nickname: storageLocal().getItem<DataInfo<number>>(userKey)?.nickname ?? "",
    // 页面级别权限
    roles: storageLocal().getItem<DataInfo<number>>(userKey)?.roles ?? [],
    // 按钮级别权限
    permissions:
      storageLocal().getItem<DataInfo<number>>(userKey)?.permissions ?? [],
    // 是否勾选了登录页的免登录
    isRemembered: false,
    // 登录页的免登录存储几天，默认7天
    loginDay: 7
  }),
  actions: {
    /** 存储头像 */
    SET_AVATAR(avatar: string) {
      this.avatar = avatar;
    },
    /** 存储用户名 */
    SET_USERNAME(username: string) {
      this.username = username;
    },
    /** 存储昵称 */
    SET_NICKNAME(nickname: string) {
      this.nickname = nickname;
    },
    /** 存储角色 */
    SET_ROLES(roles: Array<string>) {
      this.roles = roles;
    },
    /** 存储按钮级别权限 */
    SET_PERMS(permissions: Array<string>) {
      this.permissions = permissions;
    },
    /** 存储是否勾选了登录页的免登录 */
    SET_ISREMEMBERED(bool: boolean) {
      this.isRemembered = bool;
    },
    /** 设置登录页的免登录存储几天 */
    SET_LOGINDAY(value: number) {
      this.loginDay = Number(value);
    },
    /** 登入 */
    async loginByUsername(data: LoginRequest) {
      return new Promise<UserResult>((resolve, reject) => {
        getLogin(data)
          .then(response => {
            if (response?.success && response.data) {
              // 设置token到本地存储
              setToken({
                accessToken: response.data.accessToken,
                refreshToken: response.data.refreshToken,
                expires: new Date(response.data.expiresAt).getTime().toString(),
                username: response.data.userName,
                nickname: response.data.realName || response.data.userName,
                avatar: response.data.avatar || "",
                roles: [], // 后续从权限接口获取
                permissions: [] // 后续从权限接口获取
              });

              // 更新store状态
              this.SET_USERNAME(response.data.userName);
              this.SET_NICKNAME(response.data.realName || response.data.userName);
              this.SET_AVATAR(response.data.avatar || "");
            }
            resolve(response);
          })
          .catch(error => {
            reject(error);
          });
      });
    },
    /** 登出 */
    async logOut() {
      try {
        // 调用后台登出接口
        await logout();
      } catch (error) {
        console.warn("登出接口调用失败:", error);
      } finally {
        // 清理本地状态
        this.username = "";
        this.nickname = "";
        this.avatar = "";
        this.roles = [];
        this.permissions = [];
        removeToken();
        useMultiTagsStoreHook().handleTags("equal", [...routerArrays]);
        resetRouter();
        router.push("/login");
      }
    },
    /** 刷新`token` */
    async handRefreshToken(data: RefreshTokenRequest) {
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
                  expires: new Date(response.data.expiresAt).getTime().toString()
                });
              }
              resolve(response);
            }
          })
          .catch(error => {
            reject(error);
          });
      });
    }
  }
});

export function useUserStoreHook() {
  return useUserStore(store);
}

import { $t } from "@/plugins/i18n";
import type { RouteConfigsTable } from "@/types/table";

const Layout = () => import("@/layout/index.vue");

export default {
  path: "/system",
  name: "System",
  component: Layout,
  redirect: "/system/user",
  meta: {
    icon: "ep/setting",
    title: $t("menus.pureSystem"),
    rank: 10
  },
  children: [
    {
      path: "/system/user",
      name: "SystemUser",
      component: () => import("@/views/system/user/index.vue"),
      meta: {
        title: $t("menus.pureUser"),
        auths: ["system:user:list"]
      }
    },
    {
      path: "/system/role",
      name: "SystemRole",
      component: () => import("@/views/system/role/index.vue"),
      meta: {
        title: $t("menus.pureRole"),
        auths: ["system:role:list"]
      }
    },
    {
      path: "/system/department",
      name: "SystemDepartment",
      component: () => import("@/views/system/department/index.vue"),
      meta: {
        title: $t("menus.pureDept"),
        auths: ["system:dept:list"]
      }
    },
    {
      path: "/system/menu",
      name: "SystemMenu",
      component: () => import("@/views/system/menu/index.vue"),
      meta: {
        title: $t("menus.pureMenu"),
        auths: ["system:menu:list"]
      }
    }
  ]
} satisfies RouteConfigsTable; 
import { $t } from "@/plugins/i18n";
import type { RouteConfigsTable } from "@/types/table";

const Layout = () => import("@/layout/index.vue");

export default {
  path: "/monitor",
  name: "Monitor",
  component: Layout,
  redirect: "/monitor/online",
  meta: {
    icon: "ep/monitor",
    title: $t("menus.pureMonitor"),
    rank: 20
  },
  children: [
    {
      path: "/monitor/online",
      name: "MonitorOnline",
      component: () => import("@/views/monitor/online/index.vue"),
      meta: {
        title: $t("menus.pureOnlineUser"),
        auths: ["monitor:online:list"]
      }
    },
    {
      path: "/monitor/loginlog",
      name: "MonitorLoginLog",
      component: () => import("@/views/monitor/loginlog/index.vue"),
      meta: {
        title: $t("menus.pureLoginLog"),
        auths: ["monitor:loginlog:list"]
      }
    },
    {
      path: "/monitor/operlog",
      name: "MonitorOperLog",
      component: () => import("@/views/monitor/operlog/index.vue"),
      meta: {
        title: $t("menus.pureOperLog"),
        auths: ["monitor:operlog:list"]
      }
    }
  ]
} satisfies RouteConfigsTable; 
<template>
  <div v-if="hasPermission">
    <slot />
  </div>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useUserStore } from "@/stores/user";

interface Props {
  // 权限标识，可以是菜单编码或按钮权限标识
  permission?: string;
  // 角色编码
  role?: string;
  // 多个权限标识（满足其中一个即可）
  permissions?: string[];
  // 多个角色编码（满足其中一个即可）
  roles?: string[];
  // 权限验证模式：and（同时满足）、or（满足其一）
  mode?: "and" | "or";
}

const props = withDefaults(defineProps<Props>(), {
  mode: "or"
});

const userStore = useUserStore();

// 检查是否有权限
const hasPermission = computed(() => {
  // 如果没有设置任何权限条件，默认显示
  if (!props.permission && !props.role && !props.permissions?.length && !props.roles?.length) {
    return true;
  }

  // 获取用户权限和角色信息
  const userPermissions = userStore.permissions || [];
  const userRoles = userStore.roles || [];

  let permissionChecks: boolean[] = [];
  let roleChecks: boolean[] = [];

  // 检查单个权限
  if (props.permission) {
    permissionChecks.push(userPermissions.includes(props.permission));
  }

  // 检查多个权限
  if (props.permissions?.length) {
    const hasAnyPermission = props.permissions.some(p => userPermissions.includes(p));
    permissionChecks.push(hasAnyPermission);
  }

  // 检查单个角色
  if (props.role) {
    roleChecks.push(userRoles.some(r => r.roleCode === props.role));
  }

  // 检查多个角色
  if (props.roles?.length) {
    const hasAnyRole = props.roles.some(role => 
      userRoles.some(r => r.roleCode === role)
    );
    roleChecks.push(hasAnyRole);
  }

  const allChecks = [...permissionChecks, ...roleChecks];

  if (allChecks.length === 0) {
    return true;
  }

  // 根据模式返回结果
  if (props.mode === "and") {
    return allChecks.every(check => check);
  } else {
    return allChecks.some(check => check);
  }
});
</script>

<style scoped>
/* 权限组件不需要额外样式 */
</style>
